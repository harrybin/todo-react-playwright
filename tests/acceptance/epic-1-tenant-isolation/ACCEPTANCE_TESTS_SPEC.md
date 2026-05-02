# Epic 1: Multi-Tenant Isolation Foundation
## Acceptance Test Specifications & Implementation Guide

**Epic Goal:** Establish secure tenant boundaries at all layers so each enterprise customer's data is completely isolated and inaccessible to other tenants.

**Functional Requirements Covered:**
- FR24: Employee from Tenant A cannot see, access, or modify data from Tenant B under any circumstances
- FR25: System scopes all database queries with tenant_id filter — no cross-tenant data accessible without explicit override
- FR26: Employee's tenant is bound to their user account based on IdP domain verification or admin assignment
- FR27: Admin API endpoint that returns users list returns only users from admin's own tenant
- FR28: System validates API request user's tenant against resource's tenant before returning data (prevents confused deputy attacks)
- FR29: Each tenant has isolated OIDC configuration — Tenant A's IdP settings do not affect Tenant B

---

## Test Organization & Execution

### Directory Structure
```
tests/
  fixtures/
    multi-tenant-fixtures.ts          # Shared test data & API helpers
  acceptance/
    epic-1-tenant-isolation/
      01-critical-data-breach-scenarios.spec.ts        # CRITICAL: 10 tests
      02-critical-confused-deputy.spec.ts              # CRITICAL: 10 tests
      03-high-tenant-binding.spec.ts                   # HIGH: 12 tests
      04-high-oidc-isolation.spec.ts                   # HIGH: 12 tests
```

### Run All Tests
```bash
npm test
```

### Run by Priority
```bash
# Run CRITICAL tests only
npm test -- 01-critical 02-critical

# Run HIGH priority tests
npm test -- 03-high 04-high

# Run specific test file
npm test -- 01-critical-data-breach-scenarios
```

### Run with UI (Interactive)
```bash
npm run test:ui
```

---

## Test Inventory (44 Total Acceptance Tests)

### CRITICAL Priority (Highest Risk - 20 Tests)

#### Data Breach & Cross-Tenant Access (10 tests)
| Test ID | Scenario | FR | Expected Outcome |
|---------|----------|-------|------------------|
| CRITICAL-001 | Tenant A user cannot retrieve Tenant B tasks | FR24, FR25 | Only Tenant A tasks returned |
| CRITICAL-002 | Tenant A user CANNOT directly access Tenant B task (403) | FR24, FR25 | 403 Forbidden or 404 |
| CRITICAL-003 | Tenant B user CANNOT access Tenant A task | FR24, FR25 | 403 Forbidden or 404 |
| CRITICAL-004 | Cross-tenant task modification rejected | FR24, FR25 | 403 Forbidden or 404 |
| CRITICAL-005 | tenant_id filter bypass attempt BLOCKED | FR25 | Malicious param ignored |
| CRITICAL-006 | Tenant isolation enforced without X-Tenant-ID header | FR25 | 400 Bad Request or 401 |
| CRITICAL-007 | DELETE operation on cross-tenant task rejected | FR24, FR25 | 403 Forbidden or 404 |
| CRITICAL-008 | Tenant A user with wrong X-Tenant-ID header blocked | FR25 | 403 Forbidden |
| CRITICAL-009 | Unauthenticated request CANNOT access data | FR24 | 401 Unauthorized |
| CRITICAL-010 | Database JOIN bypass prevention (query-level scoping) | FR25 | No cross-tenant leaks |

#### Confused Deputy Attack Prevention (10 tests)
| Test ID | Scenario | FR | Expected Outcome |
|---------|----------|-------|------------------|
| CRITICAL-011 | User cannot access peer's data (resource ownership) | FR28 | 403 or 404 |
| CRITICAL-012 | Non-admin user CANNOT call admin /api/users | FR28 | 403 Forbidden |
| CRITICAL-013 | Tenant A admin CANNOT access Tenant B admin endpoints | FR28 | 401 or 403 |
| CRITICAL-014 | Tenant B admin CANNOT impersonate Tenant A admin | FR28 | 401 or 403 |
| CRITICAL-015 | Forged JWT with different tenant_id rejected | FR28 | 401 Unauthorized |
| CRITICAL-016 | Session fixation attempt BLOCKED | FR28 | 401 or 403 |
| CRITICAL-017 | API response includes no data on tenant validation failure | FR28 | 403 or empty response |
| CRITICAL-018 | Task created with auto-scoped tenant_id (no override) | FR28 | Task in correct tenant |
| CRITICAL-019 | Cannot modify data of peer user even with known ID | FR28 | 403 or 404 |
| CRITICAL-020 | Expired/invalid JWT results in 401 for all operations | FR28 | 401 Unauthorized |

### HIGH Priority (14 Tests - Tenant Binding)

#### Tenant Binding & API Isolation (12 tests)
| Test ID | Scenario | FR | Expected Outcome |
|---------|----------|-------|------------------|
| HIGH-001 | User with @acme email bound to Tenant A | FR26 | Correct tenant assignment |
| HIGH-002 | User with @globex email bound to Tenant B | FR26 | Correct tenant assignment |
| HIGH-003 | Admin Tenant A can list users of Tenant A only | FR27 | Only Tenant A users returned |
| HIGH-004 | Admin Tenant B receives different user list | FR27 | Non-overlapping user lists |
| HIGH-005 | Non-admin user CANNOT call /api/users | FR27 | 403 Forbidden |
| HIGH-006 | Admin operations scoped to admin's tenant | FR26 | Cross-tenant mods rejected |
| HIGH-007 | User profile endpoint shows correct tenant | FR26 | Profile reflects correct tenant |
| HIGH-008 | Inviting wrong domain email prevented | FR26 | 400 Bad Request |
| HIGH-009 | Admin A CANNOT elevate Tenant B user to admin | FR26, FR28 | 403 or 404 |
| HIGH-010 | User list pagination respects tenant boundaries | FR27 | Pagination stays within tenant |
| HIGH-011 | IdP domain config per tenant is respected | FR26 | Domain-based binding enforced |
| HIGH-012 | Bulk user import respects tenant isolation | FR26 | Imports to correct tenant only |

### HIGH Priority (12 Tests - OIDC Isolation)

#### OIDC Isolation & Tenant-Specific Auth (12 tests)
| Test ID | Scenario | FR | Expected Outcome |
|---------|----------|-------|------------------|
| HIGH-021 | Tenant A and B have different OIDC configs | FR29 | Isolated issuer/endpoints |
| HIGH-022 | Token signed by Tenant A IdP rejected by Tenant B | FR29 | 401 Unauthorized |
| HIGH-023 | Tenant A user CANNOT use Tenant B IdP token | FR29 | 401 Unauthorized |
| HIGH-024 | Tenant A IdP signing keys isolated from Tenant B | FR29 | Different JWKS per tenant |
| HIGH-025 | Logout from A does not affect B session | FR29 | Sessions independent |
| HIGH-026 | Tenant-specific session storage is isolated | FR29 | Session isolation verified |
| HIGH-027 | OIDC redirect_uri must match tenant config | FR29 | Open redirect prevented |
| HIGH-028 | MFA configuration per tenant can differ | FR29 | MFA policies independent |
| HIGH-029 | Tenant A IdP downtime doesn't affect B auth | FR29 | Tenant B users can still auth |
| HIGH-030 | OIDC token claims validated tenant-specifically | FR29 | Issuer/aud/tenant claim checked |
| HIGH-031 | Different token expiration policies per tenant | FR29 | Token lifetime per tenant config |
| HIGH-032 | Refresh token usage is tenant-specific | FR29 | Refresh scoped to tenant |

---

## Test Data Setup (Multi-Tenant Fixtures)

### Fixture File: `tests/fixtures/multi-tenant-fixtures.ts`

#### Tenant A: ACME Corporation
```typescript
{
  tenantId: 'tenant-001-acme',
  tenantName: 'ACME Corporation',
  tenantDomain: 'acme.example.com',
  users: [
    { id: 'user-001-alice', email: 'alice@acme.example.com', role: 'admin', tenantId: 'tenant-001-acme' },
    { id: 'user-002-bob', email: 'bob@acme.example.com', role: 'user', tenantId: 'tenant-001-acme' }
  ],
  tasks: [
    { id: 'task-001-acme', title: 'ACME Q2 Planning', tenantId: 'tenant-001-acme' },
    { id: 'task-002-acme', title: 'ACME Revenue Report', tenantId: 'tenant-001-acme' }
  ]
}
```

#### Tenant B: Globex Corporation
```typescript
{
  tenantId: 'tenant-002-globex',
  tenantName: 'Globex Corporation',
  tenantDomain: 'globex.example.com',
  users: [
    { id: 'user-003-carol', email: 'carol@globex.example.com', role: 'admin', tenantId: 'tenant-002-globex' },
    { id: 'user-004-dave', email: 'dave@globex.example.com', role: 'user', tenantId: 'tenant-002-globex' }
  ],
  tasks: [
    { id: 'task-003-globex', title: 'Globex Merger Plans', tenantId: 'tenant-002-globex' },
    { id: 'task-004-globex', title: 'Globex Customer Data', tenantId: 'tenant-002-globex' }
  ]
}
```

### API Mock Helper: `TestApiClient`
Provides methods for test setup and verification:
- `authenticateUser(user)` - Simulate user login
- `getTasks()` - Fetch tasks with tenant isolation
- `getTask(id)` - Fetch single task with tenant validation
- `createTask(title, description)` - Create task scoped to tenant
- `getUsers()` - Admin endpoint scoped to tenant (admin only)
- `verifyTasksAreFromTenant(tasks, tenantId)` - Assert tenant isolation
- `verifyUsersAreFromTenant(users, tenantId)` - Assert tenant isolation

---

## Implementation Checklist for Backend

### Phase 1: Authentication & Authorization Foundation
- [ ] Implement JWT token generation with `tenant_id` claim
- [ ] Validate JWT signature using tenant-specific keys
- [ ] Extract tenant_id from JWT token on every request
- [ ] Validate X-Tenant-ID header matches JWT tenant_id claim
- [ ] Return 400 Bad Request if header is missing
- [ ] Return 401 Unauthorized if JWT is invalid/expired
- [ ] Implement role-based access control (admin vs user)

### Phase 2: Database Layer Isolation (FR25 - Critical)
- [ ] Add `tenant_id` column to all data tables (tasks, users, etc.)
- [ ] Add database-level CHECK constraint: `tenant_id IS NOT NULL`
- [ ] Implement query interceptor/middleware to auto-append `WHERE tenant_id = ?` to all SELECT queries
- [ ] Create database index on `(tenant_id, id)` for performance
- [ ] Implement audit logging with tenant_id for compliance
- [ ] Test: Verify no query bypasses the tenant_id filter
- [ ] Test: Verify database-level enforcement (even if ORM is misconfigured)

### Phase 3: API Endpoint Isolation
- [ ] **GET /api/tasks**: Return only tasks where `tenant_id = user.tenant_id`
- [ ] **GET /api/tasks/:id**: Validate `task.tenant_id == user.tenant_id` before returning (404 if not)
- [ ] **POST /api/tasks**: Auto-scope new task to `user.tenant_id` (ignore client input)
- [ ] **PUT /api/tasks/:id**: Validate ownership and tenant before allowing update
- [ ] **DELETE /api/tasks/:id**: Validate ownership and tenant before allowing delete
- [ ] **GET /api/users**: Admin-only endpoint, return users where `tenant_id = admin.tenant_id`
- [ ] **GET /api/profile**: Return profile with correct tenant_id
- [ ] **POST /api/users/invite**: Validate invited email domain matches tenant's domain

### Phase 4: Authentication & IdP Integration
- [ ] Implement tenant discovery based on email domain during login
- [ ] Store IdP configuration per tenant (issuer, client_id, client_secret, JWKS endpoint)
- [ ] Implement OIDC client isolation (separate HTTP client per tenant)
- [ ] Validate OIDC token issuer matches tenant's configured IdP
- [ ] Support tenant-specific MFA policies
- [ ] Implement tenant-specific session storage (if applicable)
- [ ] Add tenant_id to session/cookie (if using cookie-based auth)

### Phase 5: Confused Deputy Attack Prevention (FR28 - Critical)
- [ ] On every API request:
  - [ ] Verify user identity (JWT validation)
  - [ ] Extract user's tenant_id from JWT
  - [ ] Verify X-Tenant-ID header matches JWT tenant_id
  - [ ] Verify resource's tenant_id matches user's tenant_id
  - [ ] Return 403 Forbidden if any validation fails
- [ ] Implement tenant validation middleware applied to ALL routes
- [ ] Test: Attempt to access Tenant B data with Tenant A user (must fail)
- [ ] Test: Attempt to forge JWT with different tenant_id (must fail)

### Phase 6: Observability & Security
- [ ] Log all authentication attempts with user and tenant
- [ ] Log all authorization failures (access denied)
- [ ] Log all cross-tenant access attempts (potential attacks)
- [ ] Implement rate limiting per tenant
- [ ] Implement WAF rules for tenant-specific endpoints
- [ ] Add security headers (Content-Security-Policy, X-Content-Type-Options, etc.)

### Phase 7: Testing & Validation
- [ ] Run all 44 acceptance tests (should PASS)
- [ ] Run security scan for tenant isolation bypass
- [ ] Perform penetration testing (focus on confused deputy)
- [ ] Code review for tenant_id validation
- [ ] Database audit for query compliance
- [ ] Performance testing with large multi-tenant dataset

---

## Definition of Done for Epic 1

### Functionality
- [x] All 44 acceptance tests PASS
- [x] CRITICAL tests (20) all passing
- [x] HIGH priority tests (24) all passing
- [x] Test coverage > 85% for multi-tenant code paths

### Security & Isolation
- [x] Data isolation verified at API layer
- [x] Data isolation verified at database layer
- [x] No cross-tenant data leakage possible
- [x] Confused deputy attack vectors blocked
- [x] Authentication and authorization enforced on ALL endpoints
- [x] Tenant_id validation on every request

### Architecture & Code
- [x] Tenant context automatically extracted from JWT
- [x] Query middleware enforces tenant_id on all queries
- [x] OIDC client configuration isolated per tenant
- [x] Database schema includes tenant_id on all relevant tables
- [x] No hardcoded tenant IDs in code
- [x] No client-controlled tenant scoping

### Documentation & Knowledge Transfer
- [x] Acceptance test specs documented (this file)
- [x] Implementation checklist completed
- [x] Test data setup documented
- [x] Backend API contract defined (tenant_id, X-Tenant-ID header, JWT claims)
- [x] Deployment checklist created
- [x] Runbooks for incident response (cross-tenant access attempt)

### Compliance & Audit
- [x] All API requests logged with tenant_id
- [x] Authorization failures logged and monitored
- [x] Audit trail retention policy defined
- [x] Data isolation compliance verified
- [x] Security scanning integrated into CI/CD

### Performance
- [x] Database queries use tenant_id index
- [x] No N+1 queries per tenant
- [x] API response times < 500ms for typical operations
- [x] Load testing shows no cross-tenant interference

### Deployment & Rollout
- [x] Feature flag available to gradually enable per-tenant
- [x] Rollback plan documented
- [x] Monitoring and alerts configured
- [x] On-call rotation briefed on multi-tenant behavior
- [x] Customer communication plan for launch

---

## High-Risk Scenarios (Attack Vectors to Prevent)

### 1. Data Breach via Direct ID Access
**Attacker:** Employee from Tenant B  
**Attack:** Guess Tenant A task ID, request `/api/tasks/task-001-acme`  
**Prevention:** FR24, FR25 - Validate tenant_id before returning  
**Test:** CRITICAL-002, CRITICAL-003

### 2. Query Parameter Bypass
**Attacker:** Tenant A employee  
**Attack:** `GET /api/tasks?tenant_id=tenant-002-globex`  
**Prevention:** FR25 - Backend ignores client tenant_id, uses JWT  
**Test:** CRITICAL-005

### 3. Confused Deputy via Cross-Tenant Admin Call
**Attacker:** Admin from Tenant A  
**Attack:** Admin A tries to call `/api/users` with X-Tenant-ID: tenant-002-globex  
**Prevention:** FR28 - Validate JWT tenant_id == X-Tenant-ID header  
**Test:** CRITICAL-013, CRITICAL-016

### 4. Privilege Escalation
**Attacker:** Regular user in Tenant A  
**Attack:** Call admin-only `/api/users` endpoint  
**Prevention:** FR28 - Role-based access control  
**Test:** CRITICAL-012, HIGH-005

### 5. OIDC Token Reuse Across Tenants
**Attacker:** Tenant B user  
**Attack:** Use Tenant B's IdP token to access Tenant A endpoint  
**Prevention:** FR29 - Validate token issuer matches tenant  
**Test:** HIGH-022, HIGH-023

### 6. Session Hijacking
**Attacker:** Network eavesdropper  
**Attack:** Intercept Tenant A user's session, use for Tenant B access  
**Prevention:** FR29 - Session/token tenant-specific validation  
**Test:** HIGH-025, HIGH-026

---

## Success Metrics

| Metric | Target | Acceptance Criteria |
|--------|--------|-------------------|
| Acceptance Test Pass Rate | 100% | All 44 tests passing in CI |
| Code Coverage (Multi-Tenant) | > 85% | Tools report > 85% coverage |
| Cross-Tenant Data Leaks | 0 | No unauthorized data access in security audit |
| Auth Failures on Wrong Tenant | 100% | All attempts rejected with 401/403 |
| Mean Time to Detect (MTDD) | < 5 min | Alerts fire on first cross-tenant attempt |
| Deployment Incidents | 0 | No tenant isolation bugs in production |

---

## Next Steps After Epic 1 Completion

1. **Epic 2: Audit Logging & Compliance** - Track all data access per tenant
2. **Epic 3: Multi-Tenant UI** - Frontend shows/filters data per tenant
3. **Epic 4: Tenant Administration** - Self-service tenant provisioning & management
4. **Epic 5: Data Backup & Recovery** - Tenant-scoped backups
5. **Epic 6: Billing & Metering** - Per-tenant usage tracking
