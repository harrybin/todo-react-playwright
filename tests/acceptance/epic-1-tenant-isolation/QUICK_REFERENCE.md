# Epic 1 Quick Reference Guide
## Multi-Tenant Isolation - Test Artifacts & Development Guide

---

## Test Structure at a Glance

```
tests/acceptance/epic-1-tenant-isolation/
├── 01-critical-data-breach-scenarios.spec.ts    (10 tests - RED: data not isolated)
├── 02-critical-confused-deputy.spec.ts          (10 tests - CRITICAL: auth bypass)
├── 03-high-tenant-binding.spec.ts               (12 tests - HIGH: user binding)
├── 04-high-oidc-isolation.spec.ts               (12 tests - HIGH: auth isolation)
└── ACCEPTANCE_TESTS_SPEC.md                     (This file)
└── IMPLEMENTATION_CHECKLIST.md                  (Development tasks)

tests/fixtures/
└── multi-tenant-fixtures.ts                      (Shared test data & API helpers)
```

---

## Running Tests

```bash
# Run all tests (expect failures - ATDD RED phase)
npm test

# Run CRITICAL tests only
npm test -- 01-critical 02-critical

# Run HIGH priority tests
npm test -- 03-high 04-high

# Run specific test file
npm test -- 01-critical-data-breach-scenarios.spec.ts

# Run tests with UI (interactive)
npm run test:ui

# Run with coverage
npm test -- --coverage
```

---

## Test Data (Fixtures)

### Tenant A: ACME Corporation
| Item | Value |
|------|-------|
| Tenant ID | `tenant-001-acme` |
| Domain | `acme.example.com` |
| Admin | Alice (`alice@acme.example.com`, role: admin) |
| User | Bob (`bob@acme.example.com`, role: user) |
| Tasks | 2 (Q2 Planning, Revenue Report) |

### Tenant B: Globex Corporation
| Item | Value |
|------|-------|
| Tenant ID | `tenant-002-globex` |
| Domain | `globex.example.com` |
| Admin | Carol (`carol@globex.example.com`, role: admin) |
| User | Dave (`dave@globex.example.com`, role: user) |
| Tasks | 2 (Merger Plans, Customer Data) |

**Usage in tests:**
```typescript
await apiClient.authenticateUser(tenantA.users[0]); // Alice
await apiClient.authenticateUser(tenantB.users[1]); // Dave
```

---

## API Contract (What Backend Must Implement)

### Required Headers
- `Authorization: Bearer <JWT>` - All requests (except auth endpoints)
- `X-Tenant-ID: tenant-001-acme` - All data requests

### JWT Payload Requirements
```json
{
  "sub": "user-001-alice",
  "email": "alice@acme.example.com",
  "tenant_id": "tenant-001-acme",
  "role": "admin",
  "iat": 1234567890,
  "exp": 1234571490
}
```

### Required Endpoints

#### Task Management
| Endpoint | Method | Auth | Response | Tenant Scoped? |
|----------|--------|------|----------|---|
| /api/tasks | GET | User | Task[] | ✓ |
| /api/tasks/:id | GET | User | Task | ✓ (404 if wrong tenant) |
| /api/tasks | POST | User | Task | ✓ (auto-scoped) |
| /api/tasks/:id | PUT | User | Task | ✓ (404 if wrong tenant) |
| /api/tasks/:id | DELETE | User | 204 | ✓ (404 if wrong tenant) |

#### User Management
| Endpoint | Method | Auth | Response | Tenant Scoped? |
|----------|--------|------|----------|---|
| /api/users | GET | Admin | User[] | ✓ (403 if not admin) |
| /api/profile | GET | User | User | ✓ |
| /api/users/invite | POST | Admin | Invite | ✓ (domain validation) |

#### OpenID Connect Discovery
| Endpoint | Method | Response |
|----------|--------|----------|
| /api/.well-known/openid-configuration?tenant=X | GET | OIDC Config |
| /api/.well-known/jwks.json?tenant=X | GET | JWKS |

### Error Responses

| Scenario | Status | Body |
|----------|--------|------|
| Missing Authorization header | 401 | `{"error": "unauthorized"}` |
| Invalid JWT token | 401 | `{"error": "invalid_token"}` |
| Expired JWT token | 401 | `{"error": "token_expired"}` |
| Missing X-Tenant-ID header | 400 | `{"error": "missing_tenant_id"}` |
| Tenant mismatch (JWT vs header) | 403 | `{"error": "tenant_mismatch"}` |
| User not found (wrong tenant) | 404 | `{"error": "not_found"}` |
| Non-admin accessing admin endpoint | 403 | `{"error": "forbidden"}` |
| Cross-tenant data access | 404 | `{"error": "not_found"}` |

---

## Key Testing Patterns

### Pattern 1: Verify Tenant Isolation
```typescript
// GIVEN: Alice is authenticated to Tenant A
await apiClient.authenticateUser(tenantA.users[0]);

// WHEN: Alice fetches tasks
const tasks = await apiClient.getTasks();

// THEN: All tasks belong to Tenant A
expect(apiClient.verifyTasksAreFromTenant(tasks, tenantA.tenantId)).toBe(true);
expect(tasks.some(t => t.tenantId === tenantB.tenantId)).toBe(false);
```

### Pattern 2: Verify Cross-Tenant Access Blocked
```typescript
// GIVEN: Alice is authenticated to Tenant A
await apiClient.authenticateUser(tenantA.users[0]);

// WHEN: Alice tries to access Tenant B task by ID
const response = await fetch(`/api/tasks/${tenantB.tasks[0].id}`, {
  headers: {
    Authorization: `Bearer ${apiClient.authToken}`,
    'X-Tenant-ID': tenantA.tenantId
  }
});

// THEN: Access denied (404 to prevent info leak)
expect([403, 404]).toContain(response.status);
```

### Pattern 3: Verify Confused Deputy Prevention
```typescript
// GIVEN: Alice has JWT for Tenant A
await apiClient.authenticateUser(tenantA.users[0]);

// WHEN: Alice claims to be Tenant B in header (mismatch)
const response = await fetch('/api/tasks', {
  headers: {
    Authorization: `Bearer ${apiClient.authToken}`,
    'X-Tenant-ID': tenantB.tenantId  // Wrong tenant!
  }
});

// THEN: Backend validates JWT tenant_id != header tenant_id
expect([401, 403]).toContain(response.status);
```

---

## Critical Test Coverage Map

### Data Breach Prevention (FR24, FR25)
| Risk | Scenario | Test | Pass Criteria |
|------|----------|------|---|
| Direct ID access | User guesses task ID | CRITICAL-002 | 404 |
| Query bypass | `?tenant_id=B` parameter | CRITICAL-005 | Ignored |
| Unauthenticated access | No JWT token | CRITICAL-009 | 401 |
| Header mismatch | JWT≠X-Tenant-ID | CRITICAL-008 | 403 |

### Confused Deputy Prevention (FR28)
| Risk | Scenario | Test | Pass Criteria |
|------|----------|------|---|
| Admin impersonation | Admin A claims to be Admin B | CRITICAL-013 | 401/403 |
| Privilege escalation | User calls admin endpoint | CRITICAL-012 | 403 |
| Token forgery | JWT with wrong tenant_id | CRITICAL-015 | 401 |
| Session hijacking | Wrong tenant in header | CRITICAL-016 | 403 |

### Tenant Binding (FR26, FR27)
| Risk | Scenario | Test | Pass Criteria |
|------|----------|------|---|
| User list leak | Admin A lists Tenant B users | HIGH-003 | Only A users |
| Cross-tenant ops | Admin A modifies Tenant B user | HIGH-006 | 403/404 |
| Email domain bypass | Invite wrong domain | HIGH-008 | 400 |

### OIDC Isolation (FR29)
| Risk | Scenario | Test | Pass Criteria |
|------|----------|------|---|
| Token reuse | Use Tenant A token for Tenant B | HIGH-022 | 401 |
| Issuer mismatch | Wrong IdP signs token | HIGH-023 | 401 |
| Key mix-up | Use Tenant B JWKS to validate Tenant A token | HIGH-024 | 401 |

---

## Development Workflow (ATDD Cycle)

### 1. RED Phase (Tests Fail)
```bash
npm test
# Expected: 44 tests FAIL ❌
# Reason: Backend not implemented yet
```

### 2. GREEN Phase (Implement Backend)
```
Phase 1: JWT auth + query scoping
Phase 2: API endpoints
Phase 3: OIDC + middleware
Phase 4: Security hardening
```

### 3. VERIFY Phase (Tests Pass)
```bash
npm test
# Expected: 44 tests PASS ✓
# Result: Epic 1 DONE
```

### 4. Quality Gates
```bash
npm test -- --coverage
# Expected: Coverage > 85% for multi-tenant code
```

---

## Debugging Failed Tests

### If CRITICAL-002 fails (Cross-tenant task access)
**Issue:** Tenant A user CAN access Tenant B task  
**Check:**
- [ ] Query includes `WHERE tenant_id = ?`
- [ ] tenant_id parameter is user's tenant (from JWT)
- [ ] Database enforces NOT NULL on tenant_id
- [ ] No client-controlled tenant_id in query

### If CRITICAL-015 fails (Forged JWT accepted)
**Issue:** JWT signature validation not working  
**Check:**
- [ ] Using tenant-specific JWKS for validation
- [ ] Signature verification is mandatory
- [ ] Issuer claim matches tenant's IdP
- [ ] exp claim is checked

### If HIGH-003 fails (Admin sees wrong tenant users)
**Issue:** Admin endpoint returns cross-tenant users  
**Check:**
- [ ] /api/users filters by admin's tenant_id
- [ ] Query: `WHERE tenant_id = admin.tenant_id`
- [ ] Only admin role can call endpoint
- [ ] No way to override tenant via query params

### If HIGH-022 fails (Token from wrong IdP accepted)
**Issue:** OIDC validation not checking issuer  
**Check:**
- [ ] Load tenant's OIDC config (issuer URL)
- [ ] Validate token iss claim matches issuer
- [ ] Use tenant-specific JWKS for signature check
- [ ] Reject if issuer mismatch

---

## Common Mistakes to Avoid

❌ **WRONG: Client-controlled tenant scoping**
```typescript
// BAD - User can manipulate tenant_id
const tasks = await db.query(
  `SELECT * FROM tasks WHERE tenant_id = ?`,
  [req.body.tenant_id]  // VULNERABILITY!
);
```

✓ **RIGHT: Extract from JWT**
```typescript
// GOOD - Tenant from JWT (trusted source)
const userTenant = req.user.tenant_id;  // From JWT
const tasks = await db.query(
  `SELECT * FROM tasks WHERE tenant_id = ?`,
  [userTenant]
);
```

---

❌ **WRONG: Different error codes for different failures**
```typescript
// BAD - Reveals if task/tenant exists
if (task.tenant_id !== user.tenant_id) {
  return res.status(403).json({error: "forbidden"}); // Leaks that task exists!
}
return res.status(404).json({error: "not_found"});
```

✓ **RIGHT: Consistent 404 for all failures**
```typescript
// GOOD - No information leak
if (!task || task.tenant_id !== user.tenant_id) {
  return res.status(404).json({error: "not_found"}); // Could be either reason
}
```

---

❌ **WRONG: No tenant validation on write operations**
```typescript
// BAD - Doesn't verify ownership before update
await db.query(`UPDATE tasks SET title = ? WHERE id = ?`, [title, taskId]);
```

✓ **RIGHT: Validate tenant before write**
```typescript
// GOOD - Verifies tenant before updating
await db.query(
  `UPDATE tasks SET title = ? WHERE id = ? AND tenant_id = ?`,
  [title, taskId, userTenant]
);
```

---

❌ **WRONG: OIDC client not isolated per tenant**
```typescript
// BAD - Same OIDC client for all tenants
const client = new OIDCClient({
  issuer: "https://idp.example.com",
  client_id: "universal_client"
});
```

✓ **RIGHT: Separate OIDC client per tenant**
```typescript
// GOOD - Load tenant's specific config
const tenantConfig = await loadTenantOIDCConfig(tenant_id);
const client = new OIDCClient(tenantConfig);
```

---

## Performance Considerations

### Query Performance
- Ensure index on `(tenant_id, id)` for tasks table
- Ensure index on `(tenant_id, id)` for users table
- P95 latency target: < 500ms
- Monitor slow query log

### OIDC Performance
- Cache JWKS with 1-hour TTL per tenant
- Cache tenant config in memory (invalidate on admin update)
- Use connection pooling per tenant's IdP
- Lazy-load OIDC client on first use

### Database Connection Pooling
- One pool sufficient (query middleware handles tenant scoping)
- Set pool size based on concurrency (10-20 connections)
- Monitor pool utilization during load tests

---

## Security Checklist Before Merge

- [ ] All 44 tests passing
- [ ] No hardcoded tenant_ids in code
- [ ] No client-controlled tenant scoping
- [ ] All queries include tenant_id filter
- [ ] Database constraints enforce tenant_id NOT NULL
- [ ] Error responses don't leak tenant existence
- [ ] OIDC validation checks issuer claim
- [ ] JWT signature validation is mandatory
- [ ] Role-based access control enforced
- [ ] Admin endpoints require role='admin'
- [ ] Cross-tenant modification attempts return 404
- [ ] Unauthenticated requests return 401
- [ ] Tenant mismatch returns 403

---

## Deployment Checklist

- [ ] Feature flag implemented (multi_tenant_enabled)
- [ ] Database migration tested on staging
- [ ] Monitoring alerts configured
- [ ] Runbooks prepared
- [ ] On-call team trained
- [ ] Customer communication sent
- [ ] Rollback plan tested
- [ ] Gradual rollout planned (10% → 50% → 100%)

---

## Next Steps

1. **Backend team:** Review IMPLEMENTATION_CHECKLIST.md and start Phase 1
2. **QA team:** Familiarize with test structure and acceptance criteria
3. **DevOps team:** Set up monitoring and feature flag infrastructure
4. **Product/Docs team:** Update customer documentation for multi-tenant features

---

## Questions?

Refer to:
- **ACCEPTANCE_TESTS_SPEC.md** - Detailed test specifications
- **IMPLEMENTATION_CHECKLIST.md** - Development tasks and timeline
- **multi-tenant-fixtures.ts** - Test data and API helpers

**Contact:** Engineering lead for architecture clarifications
