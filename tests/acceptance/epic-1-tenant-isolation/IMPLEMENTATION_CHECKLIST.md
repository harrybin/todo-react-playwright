# Epic 1 Implementation Checklist
## Multi-Tenant Isolation Foundation Backend Tasks

**Epic Goal:** Establish secure tenant boundaries at all layers  
**Target:** Pass all 44 ATDD acceptance tests  
**Duration Estimate:** 2-3 sprints (80-120 hours)

---

## Phase 1: Project Setup & Architecture (1-2 days)

### Database Schema
- [ ] Add `tenant_id` UUID column to all data tables
  - [ ] tasks table: `tenant_id NOT NULL, CHECK (tenant_id IS NOT NULL)`
  - [ ] users table: `tenant_id NOT NULL, CHECK (tenant_id IS NOT NULL)`
  - [ ] user_sessions table: `tenant_id NOT NULL`
  - [ ] audit_logs table: `tenant_id NOT NULL`
- [ ] Create composite indexes for performance
  - [ ] CREATE INDEX idx_tasks_tenant_id ON tasks(tenant_id, id)
  - [ ] CREATE INDEX idx_users_tenant_id ON users(tenant_id, id)
  - [ ] CREATE INDEX idx_sessions_tenant_id ON user_sessions(tenant_id, user_id)
- [ ] Create foreign key constraints (tenant_id references tenants table)
- [ ] Run schema migration on dev environment
- [ ] Document schema changes

### Backend Architecture
- [ ] Design tenant context extraction middleware
- [ ] Design query middleware for automatic tenant_id filtering
- [ ] Plan OIDC client factory (per-tenant isolation)
- [ ] Create error handling for tenant mismatches
- [ ] Design logging strategy (include tenant_id in all logs)

### Testing Infrastructure
- [ ] Set up test database with 2 pre-populated tenants
- [ ] Create test data factory for fixtures
- [ ] Configure playwright.config.ts (✓ DONE)
- [ ] Set up multi-tenant test fixtures (✓ DONE)
- [ ] Run tests locally to verify setup

---

## Phase 2: Authentication & JWT Implementation (3-5 days)

### JWT Token Structure
- [ ] Design JWT payload with tenant_id claim
  ```json
  {
    "sub": "user-id",
    "email": "user@domain.com",
    "tenant_id": "tenant-001",
    "role": "admin",
    "iat": 1234567890,
    "exp": 1234571490
  }
  ```
- [ ] Implement JWT generation on login
- [ ] Implement JWT validation middleware
- [ ] Extract tenant_id from JWT on every request
- [ ] Store tenant_id in request context for middleware chain

### Tenant Discovery
- [ ] Implement email domain → tenant_id lookup table
- [ ] Create endpoint: `GET /api/.well-known/tenant-discovery?email=user@acme.example.com`
- [ ] Return tenant configuration (issuer, client_id, etc.)
- [ ] Cache tenant discovery results

### JWT Validation
- [ ] Create `validateJWT()` middleware
  - [ ] Verify signature
  - [ ] Check expiration
  - [ ] Extract tenant_id claim
  - [ ] Store in request context
- [ ] Create `validateTenantMatch()` middleware
  - [ ] Get user's tenant_id from JWT
  - [ ] Get X-Tenant-ID header
  - [ ] Ensure they match
  - [ ] Return 400 if header missing
  - [ ] Return 403 if mismatch
- [ ] Apply middlewares to ALL routes

### Tests to Pass
- [ ] CRITICAL-009: Unauthenticated request → 401
- [ ] CRITICAL-006: Missing X-Tenant-ID header handling
- [ ] CRITICAL-020: Expired JWT → 401
- [ ] CRITICAL-015: Forged JWT with wrong tenant_id → 401

---

## Phase 3: Database Query Isolation (3-5 days)

### Query Middleware Implementation
- [ ] Create query builder middleware
- [ ] Implement automatic `WHERE tenant_id = ?` injection for SELECT queries
- [ ] Implement tenant_id validation for UPDATE/DELETE queries
- [ ] Log all queries with tenant_id for audit
- [ ] Handle edge cases:
  - [ ] Aggregate queries (COUNT, SUM, etc.)
  - [ ] JOIN queries
  - [ ] Subqueries
  - [ ] Stored procedures

### Performance Optimization
- [ ] Verify indexes are used by query planner
- [ ] Run EXPLAIN ANALYZE on common queries
- [ ] Test performance with 10k+ tasks per tenant
- [ ] Monitor query execution time < 100ms p95

### Database-Level Enforcement
- [ ] Create READ-ONLY user role for API (can't execute unscoped queries)
- [ ] Add database audit trigger to log all data access
- [ ] Test: Attempt unscoped query → database rejects
- [ ] Test: Attempt query with wrong tenant_id → database rejects

### Tests to Pass
- [ ] CRITICAL-001: Only Tenant A tasks returned
- [ ] CRITICAL-010: Query-level scoping prevents JOIN bypass
- [ ] CRITICAL-005: Filter bypass blocked (malicious query params ignored)

---

## Phase 4: API Endpoint Implementation (5-7 days)

### GET /api/tasks (Task List)
- [ ] Implementation:
  ```
  SELECT * FROM tasks WHERE tenant_id = user.tenant_id
  ```
- [ ] Validation:
  - [ ] Verify user.tenant_id from JWT
  - [ ] Verify X-Tenant-ID header matches
  - [ ] Append tenant_id filter
- [ ] Response format:
  ```json
  {
    "tasks": [
      { "id": "task-001", "title": "...", "tenantId": "tenant-001-acme", ... }
    ]
  }
  ```
- [ ] Tests:
  - [ ] CRITICAL-001: Only Tenant A tasks
  - [ ] CRITICAL-005: Filter bypass blocked

### GET /api/tasks/:id (Single Task)
- [ ] Implementation:
  ```
  SELECT * FROM tasks 
  WHERE id = :id AND tenant_id = user.tenant_id
  ```
- [ ] Response:
  - [ ] If found: return task
  - [ ] If not found OR wrong tenant: return 404 (never 403 for security)
- [ ] Tests:
  - [ ] CRITICAL-002: Tenant A can't access Tenant B task (404)
  - [ ] CRITICAL-003: Tenant B can't access Tenant A task (404)

### POST /api/tasks (Create Task)
- [ ] Implementation:
  ```
  INSERT INTO tasks (title, description, tenant_id, user_id)
  VALUES (title, description, user.tenant_id, user.id)
  ```
- [ ] Validation:
  - [ ] Ignore any tenant_id in request body
  - [ ] Use user.tenant_id from JWT
- [ ] Response: Return created task with auto-scoped tenant_id
- [ ] Tests:
  - [ ] CRITICAL-018: Task created with correct tenant_id (no override)

### PUT /api/tasks/:id (Update Task)
- [ ] Implementation:
  ```
  UPDATE tasks 
  SET title = ?, description = ?
  WHERE id = :id AND tenant_id = user.tenant_id
  ```
- [ ] Validation:
  - [ ] Verify ownership (user_id == task.user_id or user is admin)
  - [ ] Verify tenant match (404 if not)
- [ ] Response: Return updated task
- [ ] Tests:
  - [ ] CRITICAL-004: Cross-tenant modification rejected (404)

### DELETE /api/tasks/:id (Delete Task)
- [ ] Implementation:
  ```
  DELETE FROM tasks
  WHERE id = :id AND tenant_id = user.tenant_id
  ```
- [ ] Response: 204 No Content on success, 404 if not found/wrong tenant
- [ ] Tests:
  - [ ] CRITICAL-007: Cross-tenant deletion rejected (404)
  - [ ] CRITICAL-019: Cannot delete peer's task (404 or 403)

### GET /api/users (List Users - ADMIN ONLY)
- [ ] Implementation:
  ```
  SELECT * FROM users 
  WHERE tenant_id = admin.tenant_id AND (admin.role = 'admin' OR error)
  ```
- [ ] Authorization:
  - [ ] Verify user is admin
  - [ ] Verify user's tenant_id from JWT
  - [ ] Return only users from that tenant
- [ ] Response:
  ```json
  {
    "users": [
      { "id": "user-001", "email": "...", "tenantId": "tenant-001", "role": "admin", ... }
    ]
  }
  ```
- [ ] Tests:
  - [ ] HIGH-003: Admin A lists only Tenant A users
  - [ ] CRITICAL-012: Non-admin → 403
  - [ ] CRITICAL-013: Admin A → 401/403 when accessing Tenant B

### GET /api/profile (Current User Profile)
- [ ] Implementation:
  ```
  SELECT * FROM users WHERE id = user.id AND tenant_id = user.tenant_id
  ```
- [ ] Response: User profile with correct tenant_id
- [ ] Tests:
  - [ ] HIGH-007: Profile shows correct tenant

### POST /api/users/invite (Invite User)
- [ ] Implementation:
  ```
  INSERT INTO user_invitations (email, tenant_id, role)
  VALUES (email, admin.tenant_id, role)
  ```
- [ ] Validation:
  - [ ] Verify admin.role = 'admin'
  - [ ] Verify email domain matches tenant's allowed domains
  - [ ] Return 400 if domain doesn't match
- [ ] Tests:
  - [ ] HIGH-008: Wrong domain email → 400
  - [ ] HIGH-009: Admin A can't invite to Tenant B

### Error Responses
- [ ] 400 Bad Request: Missing X-Tenant-ID header, invalid domain
- [ ] 401 Unauthorized: Invalid/expired JWT
- [ ] 403 Forbidden: Permission denied (non-admin accessing admin endpoint)
- [ ] 404 Not Found: Resource not found (includes cross-tenant attempts)
- [ ] Never return 403 for cross-tenant access (use 404 instead for security)

---

## Phase 5: OIDC Integration & IdP Isolation (5-7 days)

### OIDC Configuration Per Tenant
- [ ] Create `tenant_oidc_config` table:
  ```sql
  CREATE TABLE tenant_oidc_config (
    id UUID PRIMARY KEY,
    tenant_id UUID NOT NULL REFERENCES tenants(id),
    issuer VARCHAR NOT NULL,
    client_id VARCHAR NOT NULL,
    client_secret VARCHAR NOT NULL (encrypted),
    jwks_uri VARCHAR NOT NULL,
    authorization_endpoint VARCHAR NOT NULL,
    token_endpoint VARCHAR NOT NULL,
    userinfo_endpoint VARCHAR NOT NULL,
    redirect_uris TEXT[] NOT NULL,
    created_at TIMESTAMP,
    UNIQUE(tenant_id)
  )
  ```
- [ ] Populate test data for both tenants

### OIDC Client Factory
- [ ] Create `OIDCClientFactory` class
  - [ ] Cache clients per tenant
  - [ ] Lazy-load tenant config on first use
  - [ ] Validate issuer on each token validation
- [ ] Implement `getTenantOIDCClient(tenant_id)`
  - [ ] Load config from database
  - [ ] Create/reuse HTTP client
  - [ ] Return configured client

### OIDC Discovery Endpoint
- [ ] Implement `GET /api/.well-known/openid-configuration`
  - [ ] Query parameter: `?tenant=tenant-001`
  - [ ] Load tenant's OIDC config
  - [ ] Return OpenID Connect Discovery document
- [ ] Implement `GET /api/.well-known/jwks.json`
  - [ ] Query parameter: `?tenant=tenant-001`
  - [ ] Fetch tenant's IdP JWKS
  - [ ] Cache with TTL (1 hour)
  - [ ] Return JSON Web Key Set
- [ ] Tests:
  - [ ] HIGH-021: Different configs for Tenants A & B
  - [ ] HIGH-024: Different JWKS for each tenant

### Token Validation Per Tenant
- [ ] Create `validateTokenForTenant(token, tenant_id)` function
  - [ ] Load tenant's IdP config
  - [ ] Validate token signature using tenant's JWKS
  - [ ] Verify issuer matches tenant's configured issuer
  - [ ] Verify audience (aud) matches tenant's client_id
  - [ ] Verify tenant claim in token matches tenant_id
  - [ ] Extract user claims (sub, email, etc.)
  - [ ] Return 401 if any validation fails
- [ ] Tests:
  - [ ] HIGH-022: Token from Tenant A IdP rejected by Tenant B (401)
  - [ ] HIGH-023: User A can't use Tenant B IdP token (401)
  - [ ] HIGH-030: Token claims validated per tenant

### OIDC Callback Handler
- [ ] Implement `POST /api/auth/callback`
  - [ ] Receive authorization code
  - [ ] Discover tenant from state parameter (or email)
  - [ ] Get tenant's OIDCClient
  - [ ] Exchange code for token
  - [ ] Validate token for tenant
  - [ ] Create/update user record with correct tenant_id
  - [ ] Create session/return JWT
- [ ] Validation:
  - [ ] Verify state parameter
  - [ ] Verify redirect_uri matches registered URI
  - [ ] Verify code hasn't been used before (PKCE)
- [ ] Tests:
  - [ ] Can't reuse authorization code
  - [ ] Incorrect redirect_uri → rejected

### MFA Configuration Per Tenant
- [ ] Add `mfa_required` flag to tenant_oidc_config
- [ ] During token validation, check tenant's MFA requirement
- [ ] If MFA required and not present in token claims, reject
- [ ] Tests:
  - [ ] HIGH-028: Different MFA policies per tenant

### Tenant IdP Health
- [ ] Implement per-tenant IdP timeout handling
  - [ ] Each tenant's IdP client has independent timeout
  - [ ] Tenant A's IdP timeout doesn't affect Tenant B
- [ ] Implement per-tenant JWKS caching
  - [ ] Cache key includes tenant_id
  - [ ] Cache invalidation per tenant
- [ ] Tests:
  - [ ] HIGH-029: Tenant A IdP down doesn't affect Tenant B auth

---

## Phase 6: Middleware & Security (2-3 days)

### Tenant Context Middleware
- [ ] Create `extractTenantContext()` middleware
  - [ ] Parse JWT token
  - [ ] Extract tenant_id claim
  - [ ] Store in `req.tenant` object
  - [ ] Apply to ALL routes
- [ ] Error handling:
  - [ ] Missing Authorization header → 401
  - [ ] Invalid JWT → 401
  - [ ] Missing X-Tenant-ID header → 400
  - [ ] Tenant mismatch → 403

### Tenant Validation Middleware
- [ ] Create `validateTenantMatch()` middleware
  - [ ] Get tenant_id from JWT
  - [ ] Get tenant_id from X-Tenant-ID header
  - [ ] Verify they match
  - [ ] Return 403 if mismatch
- [ ] Apply to ALL data endpoints

### Role-Based Access Control (RBAC) Middleware
- [ ] Create `requireRole(role)` middleware
  - [ ] Check user.role from JWT
  - [ ] Verify role >= required role
  - [ ] Return 403 if insufficient role
- [ ] Supported roles:
  - [ ] 'user' - basic user
  - [ ] 'admin' - tenant admin
- [ ] Apply to admin endpoints:
  - [ ] GET /api/users (admin only)
  - [ ] POST /api/users/invite (admin only)
  - [ ] PUT /api/users/:id (admin only)

### Rate Limiting Per Tenant
- [ ] Implement rate limiter with tenant_id as key
- [ ] Configuration:
  - [ ] 100 requests/minute per user
  - [ ] 10,000 requests/minute per tenant
- [ ] Store in Redis with tenant prefix
- [ ] Return 429 Too Many Requests if exceeded

### Logging & Audit
- [ ] Add tenant_id to all logs
- [ ] Log format: `[tenant_id] [user_id] action: message`
- [ ] Log events:
  - [ ] Successful authentication
  - [ ] Failed authentication
  - [ ] Authorization failures
  - [ ] Cross-tenant access attempts
  - [ ] Data modifications
- [ ] Implement audit table:
  ```sql
  CREATE TABLE audit_logs (
    id UUID PRIMARY KEY,
    tenant_id UUID NOT NULL,
    user_id UUID,
    action VARCHAR NOT NULL,
    resource_type VARCHAR,
    resource_id UUID,
    result VARCHAR,
    ip_address INET,
    timestamp TIMESTAMP
  )
  ```

---

## Phase 7: Testing & Validation (2-3 days)

### Run All Acceptance Tests
- [ ] Run playwright tests: `npm test`
- [ ] Expected: All 44 tests PASS
  - [ ] 20 CRITICAL tests passing
  - [ ] 24 HIGH priority tests passing
- [ ] Run with coverage: `npm test -- --coverage`
- [ ] Expected: Multi-tenant code coverage > 85%

### Security Testing
- [ ] Attempt direct ID access cross-tenant → 404
- [ ] Attempt query parameter bypass → ignored
- [ ] Attempt JWT forging → 401
- [ ] Attempt role escalation → 403
- [ ] Attempt unscoped queries → database rejects
- [ ] Penetration testing:
  - [ ] Confused deputy attacks
  - [ ] Session hijacking
  - [ ] Token reuse

### Load Testing
- [ ] Load test with 1000 concurrent users across 10 tenants
- [ ] Expected: No cross-tenant data leakage
- [ ] Expected: P95 response time < 500ms
- [ ] Monitor tenant_id filtering overhead < 5%

### Database Validation
- [ ] Query audit: Ensure all queries include tenant_id
- [ ] Schema validation: Ensure tenant_id NOT NULL on all tables
- [ ] Foreign key check: All tenant_ids reference valid tenants
- [ ] No hardcoded tenant_ids in queries

### Code Review
- [ ] Security review: Tenant validation on all endpoints
- [ ] Architecture review: Query middleware effectiveness
- [ ] Database review: Index usage and query plans
- [ ] Documentation: API contract clear and tenant scoping documented

---

## Phase 8: Deployment & Monitoring (1-2 days)

### Pre-Deployment
- [ ] Database migration tested on staging
- [ ] All tests passing in CI/CD
- [ ] Code review approved
- [ ] Security scan passed
- [ ] Performance testing passed
- [ ] Rollback plan documented

### Deployment Strategy
- [ ] Feature flag: `multi_tenant_enabled` (default: false)
- [ ] Gradual rollout:
  - [ ] Phase 1: Staging only
  - [ ] Phase 2: 10% of production traffic
  - [ ] Phase 3: 50% of production traffic
  - [ ] Phase 4: 100% of production traffic
- [ ] Monitoring enabled during each phase
- [ ] Ability to instant rollback if issues detected

### Monitoring & Alerts
- [ ] Alert: Cross-tenant access attempt detected
- [ ] Alert: Unusual number of 403/404 responses
- [ ] Alert: OIDC token validation failures > 1%
- [ ] Alert: Query execution time > 1 second
- [ ] Dashboard:
  - [ ] Requests per tenant
  - [ ] Auth failures by reason
  - [ ] Cross-tenant access attempts
  - [ ] API latency by endpoint

### Incident Response
- [ ] Runbook: Cross-tenant data leak detected
- [ ] Runbook: OIDC IdP unavailable
- [ ] Runbook: Database query bypass detected
- [ ] On-call training: Multi-tenant architecture walkthrough

---

## Task Sizing & Sprint Planning

### Sprint 1 (Week 1): Database & Architecture (40 hours)
- Phase 1: Project Setup (8 hours)
- Phase 2: Authentication & JWT (20 hours)
- Phase 3: Query Middleware (12 hours)
- **Deliverable:** Working JWT auth with query scoping

### Sprint 2 (Week 2): API Endpoints (40 hours)
- Phase 4: API Endpoints (30 hours)
  - GET /api/tasks
  - GET /api/tasks/:id
  - POST /api/tasks
  - PUT /api/tasks/:id
  - DELETE /api/tasks/:id
  - GET /api/users (admin)
- Phase 5: OIDC Setup (10 hours)
- **Deliverable:** All task/user endpoints with tenant scoping

### Sprint 3 (Week 3): OIDC & Security (40 hours)
- Phase 5: OIDC Implementation (25 hours)
- Phase 6: Middleware & Security (10 hours)
- Phase 7: Testing (5 hours)
- **Deliverable:** Full OIDC isolation, security middleware, 40+ tests passing

### Sprint 4 (Week 4): Testing & Deployment (40 hours)
- Phase 7: Complete Testing (20 hours)
- Phase 8: Monitoring & Deployment (15 hours)
- Documentation & Knowledge Transfer (5 hours)
- **Deliverable:** All 44 tests passing, deployed to staging

---

## Definition of Done Checklist

### Code Quality
- [ ] All unit tests passing (if applicable)
- [ ] All 44 acceptance tests passing
- [ ] Code review approved by 2+ engineers
- [ ] Security scan passed (no vulnerabilities)
- [ ] Test coverage > 85%
- [ ] No hardcoded tenant_ids
- [ ] No client-controlled tenant scoping

### Security & Isolation
- [ ] No cross-tenant data accessible
- [ ] All endpoints require tenant validation
- [ ] OIDC properly isolated per tenant
- [ ] Database enforces tenant_id constraints
- [ ] Query middleware tested and validated
- [ ] Error handling doesn't leak tenant info

### Performance
- [ ] P95 response time < 500ms
- [ ] No N+1 queries
- [ ] Index usage verified
- [ ] Load tested with 1000+ concurrent users
- [ ] No cross-tenant interference under load

### Documentation
- [ ] API contract documented (tenant_id, X-Tenant-ID header, JWT claims)
- [ ] Database schema documented with tenant_id details
- [ ] OIDC configuration documented
- [ ] Architecture diagram updated
- [ ] Runbooks created for incident response
- [ ] Acceptance test specifications (this file) ✓ DONE
- [ ] Implementation checklist (this file) ✓ DONE

### Deployment
- [ ] Feature flag working (can disable if needed)
- [ ] Monitoring & alerts configured
- [ ] Rollback plan tested
- [ ] On-call trained on multi-tenant behavior
- [ ] Customer communication sent
- [ ] SLA updated for tenant data isolation guarantees

---

## Success Criteria

| Criteria | Target | Validation |
|----------|--------|-----------|
| Acceptance Tests | 100% (44/44) | All passing in CI |
| CRITICAL Tests | 100% (20/20) | All passing |
| HIGH Tests | 100% (24/24) | All passing |
| Code Coverage | > 85% | Coverage report |
| Cross-Tenant Leaks | 0 | Security audit |
| P95 Latency | < 500ms | Performance test |
| Deployment Incidents | 0 | Production monitoring |

---

## Questions & Edge Cases to Address

1. **What if user's email domain doesn't match any tenant?**
   - Action: Return 401 Unauthorized (unknown tenant)

2. **What if user's JWT tenant_id doesn't match X-Tenant-ID header?**
   - Action: Return 403 Forbidden (tenant mismatch)

3. **What if a Tenant A admin knows a Tenant B user's ID?**
   - Action: Queries return 404 (tenant isolation verified)

4. **What if OIDC token issuer is different than expected?**
   - Action: Return 401 Unauthorized (token rejected)

5. **What if database connection pooling gets confused by tenant_id?**
   - Action: Use connection pool per tenant (if needed)

6. **What if a Tenant A admin tries to elevate a Tenant B user?**
   - Action: Return 404 (user not found in Tenant A context)

7. **What if an API call is made without X-Tenant-ID header?**
   - Action: Return 400 Bad Request (header required)

8. **What if token expires mid-request?**
   - Action: Return 401 Unauthorized (token invalid)
