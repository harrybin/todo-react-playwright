---
stepsCompleted: ['step-01-init', 'step-02-context', 'step-03-starter', 'step-04-decisions']
architecturalDecisionsApproved: true
inputDocuments: 
  - 'd:\harrybin\todo-react-playwright\_bmad-output\planning-artifacts\prd.md'
  - 'd:\harrybin\todo-react-playwright\docs\code-review\2026-05-02-enterprise-auth-review.md'
  - 'd:\harrybin\todo-react-playwright\_bmad-output\project-context.md'
workflowType: 'architecture'
project_name: 'todo-react'
user_name: 'Harry'
date: '2026-05-03'
contextAnalysisCompleted: true
starterSelected: 'NestJS'
---

# Architecture Decision Document - todo-react

_This document builds collaboratively through step-by-step discovery. Sections are appended as we work through each architectural decision together._

**Project:** todo-react  
**Domain:** B2B SaaS - Workforce Identity & Access Management  
**Complexity:** High  
**Project Context:** Brownfield (extending existing todo-react app)  
**Release Model:** Single-release MVP with all core enterprise authentication capabilities

---

## Input Documents Loaded

✅ **PRD:** Comprehensive product requirements (53 FRs, 32 NFRs)  
✅ **Enterprise Auth Review:** Code review and architectural considerations  
✅ **Project Context:** Critical implementation rules and technology stack  
✅ **Technology Stack:** React 19.1.0, TypeScript 5.8.3, Vite 6.3.5, Material-UI 7.1.0  

---

## Project Context Analysis

### Requirements Overview

**Functional Requirements Summary:**

Your project implements a comprehensive enterprise authentication platform with 53 functional requirements organized across 8 capability areas:

1. **OIDC Authentication & IdP Integration (8 FRs)** — Complete OpenID Connect 1.0 support with automatic metadata discovery, configuration validation, and integration with any OIDC-compliant identity provider (Okta, Azure AD, Google Workspace, Keycloak)

2. **Mandatory Multi-Factor Authentication (7 FRs)** — Platform-wide MFA enforcement using TOTP-based authenticator apps (Google Authenticator, Authy, Microsoft Authenticator) with SMS fallback for device recovery; MFA enrollment tracking and admin management capabilities

3. **Role-Based Access Control (8 FRs)** — Four application roles (Admin, User, Read-Only, Security Officer) with permissions enforced at the API layer (not just UI) for every request; role mapping from IdP claims/groups to application roles

4. **Multi-Tenant Isolation & Data Scoping (6 FRs)** — Enterprise-grade data isolation enforced at database (WHERE tenant_id filters), API (tenant validation before data access), and UI levels; tenant binding at account creation; no cross-tenant data access possible

5. **Admin Capabilities & Configuration (6 FRs)** — Step-by-step OIDC configuration wizard with validation testing, role mapping UI with permission preview, admin dashboards for monitoring authentication and user management

6. **Audit, Compliance & Reporting (5 FRs)** — Immutable audit logging of all authentication events (WHO, WHAT, WHEN, WHERE, OUTCOME); searchable/filterable logs; CSV/JSON export for compliance investigations

7. **User Session & Account Management (7 FRs)** — Secure session management with HTTP-only cookies, 1-hour access token expiry, 30-day refresh token validity, cross-device persistence, logout invalidation, self-service and admin-initiated account deletion with immutable audit trails

8. **Breakglass & Emergency Access (6 FRs)** — Help desk-issuable temporary breakglass codes for MFA-locked users; 15-minute access window, MFA re-enrollment flow, comprehensive audit logging of breakglass usage

**Non-Functional Requirements Summary:**

Your 32 NFRs establish demanding performance, security, scalability, and reliability targets:

- **Performance:** <3 second OIDC authentication flow, <500ms MFA validation, <2 second audit log search, <1 second admin dashboard load
- **Security:** RS256+ token signing, HTTP-only secure cookies, TLS 1.2+, AES-256 MFA secret encryption, HMAC-SHA256 audit log signing, rate-limited login attempts, tenant ownership validation, secure vault for secrets
- **Scalability:** 10K concurrent MFA validations/second, linear query scaling for 100K user tenants, shared multi-tenant infrastructure, 50K SMS deliveries/day with automatic failover
- **Reliability:** >99.5% uptime, 99% SMS delivery within 2 minutes, graceful IdP outage handling, guaranteed audit log persistence, hourly backups (1-hour RPO, 4-hour RTO), quarterly disaster recovery testing
- **Accessibility:** WCAG 2.1 Level AA compliance, QR code backup text codes, screen reader navigation for audit dashboards
- **Integration:** <2 hour OIDC configuration time, 1-hour IdP metadata update detection, dual SMS provider failover

### Scale & Complexity Assessment

**Project Scope:**
- Single-release MVP with all core enterprise authentication capabilities
- 3-5 engineers, 2-3 months estimated delivery
- 8 epics with 4 user journeys (IT Admin, Employee, Employee with MFA loss, Security Officer)

**Complexity Level:** **ENTERPRISE HIGH**

This is not a simple authentication add-on. It's a complete, security-first enterprise IAM platform that must:
- Enforce multi-tenant data isolation perfectly (no escape routes)
- Support complex authentication flows (OIDC → MFA → session → permissions)
- Provide production-grade audit logging with immutability guarantees
- Scale to enterprise customer needs (100K+ users per tenant)
- Maintain >99.5% reliability for a security-critical system

**Primary Technical Domain:** Backend API + Database Architecture + Frontend Admin Console
- **Backend:** Authentication service, MFA/TOTP validation, RBAC policy engine, audit logging, session management
- **Database:** Multi-tenant data model, tenant scoping, immutable audit tables, session storage, MFA device tracking
- **Frontend:** Admin console with OIDC wizard, role mapping UI, user management, audit dashboards

### Technical Constraints & Dependencies

**External Dependencies:**
- OIDC protocol compliance (OpenID Connect 1.0 spec)
- Integration with external identity providers (Okta, Azure AD, etc.) — must support metadata discovery
- SMS provider integration (primary + failover) for MFA recovery
- HTTP-only secure cookie support and TLS 1.2+ infrastructure

**Security Constraints:**
- All authentication tokens must be cryptographically signed (RS256 or stronger)
- Audit logs must be immutable once written (detectable tampering)
- Secrets must be vault-managed (not in code/config)
- Every API request must validate tenant ownership before data access
- Permission checks must be at API layer (not just UI)

**Performance Constraints:**
- OIDC flow must complete in <3 seconds (user perception)
- MFA validation <500ms (TOTP checking with replay prevention)
- Audit log search <2 seconds (across 12 months of data per tenant)
- Admin dashboard <1 second load time (real-time statistics)

**Data Constraints:**
- Audit logs retained 12 months with automatic purge
- Personal data deleted immediately upon request (GDPR compliance)
- 12-month compliance audit trail requirements

### Cross-Cutting Concerns (Architectural Impact)

These concerns touch multiple epics and must be designed consistently across the system:

**1. Multi-Tenant Data Isolation (Affects: All 8 Epics)**
- Every database query must filter by tenant_id
- Every API endpoint must validate tenant ownership before returning data
- UI must only show data from user's tenant
- Tenant context must flow through entire request lifecycle

**2. Security Enforcement at Multiple Layers (Affects: Epics 2-8)**
- Database-level filtering (WHERE tenant_id = X)
- API-level validation (middleware checking user.tenant_id == resource.tenant_id)
- UI-level filtering (users only see their tenant's data)
- Error handling must not leak information about other tenants

**3. Immutable Audit Logging (Affects: Epics 3-8)**
- Every authentication event logged: login, MFA enrollment, role changes, breakglass usage
- Logs cryptographically signed to detect tampering
- Must be available for real-time security investigation
- Must support compliance exports (CSV, JSON)
- Automatic purge after 12 months

**4. Session Management with Cross-Device Support (Affects: Epics 3-4, 7)**
- HTTP-only secure cookies for access tokens (1-hour expiry)
- Refresh tokens for cross-device persistence (30-day expiry)
- Logout must invalidate both tokens
- Multiple devices per user must maintain separate session refresh chains

**5. Authentication State Flow (Affects: Epics 2-8)**
- User's tenant_id must be bound to session after OIDC authentication
- Tenant context must be available to all downstream authorization decisions
- Breakglass access must include tenant context in audit logs
- Admin actions must be scoped to admin's own tenant

**6. Error Handling with Tenant Boundary Validation (Affects: Epics 3-8)**
- Return HTTP 403 (Forbidden) for permission denied
- Never disclose whether resource exists in other tenant
- Log all denied access attempts with actor, resource, and tenant context
- Rate-limit failed login attempts (5 attempts per 15 minutes)

### Architectural Implications Summary

From this analysis, several key architectural decisions emerge:

1. **Multi-Tenant Context Must Be Foundational** — Tenant scoping should be embedded in database query layer and API middleware, not bolted on per-endpoint
2. **Security Cannot Be UI-Only** — All permission enforcement must be at API layer; assume frontend can be bypassed
3. **Audit Logging Must Be Real-Time & Searchable** — Cannot be post-hoc reporting; must support live investigation and compliance queries
4. **Session Management Must Support Multi-Device Flows** — Single logout must invalidate all tokens while allowing cross-device re-authentication
5. **MFA Is Core, Not Optional** — Must be enforced at every login with zero exceptions (except breakglass, which is logged)
6. **Compliance Is Architectural, Not Bolted On** — GDPR deletion workflows, immutable audit logs, and SOC 2 controls must be in system design from start

---

## Starter Template Evaluation

### Primary Technology Domain

**Full-Stack B2B SaaS with Separate Concerns:**
- **Frontend:** React 19.1.0 + TypeScript + Vite (already established)
- **Backend:** Enterprise authentication service — OIDC integration, MFA, RBAC, audit logging
- **Database:** PostgreSQL with multi-tenant data model
- **Focus:** Backend authentication service architecture

### Selected Starter: NestJS

**Rationale:** Your enterprise auth platform requires:
- Multi-layer security enforcement (database → API → UI)
- Complex authentication flows (OIDC → MFA → RBAC → session)
- Production-grade immutable audit logging
- >99.5% reliability for security-critical system
- Tenant isolation across all components

NestJS provides the architectural foundation with:
- Guards & Interceptors for tenant validation middleware
- Decorators for auth-required and role-based endpoints
- Modules for organizing auth, MFA, RBAC, audit logging
- Dependency injection for clean separation of concerns
- Full TypeScript typing for security-critical code
- Built-in testing utilities for auth flows

**Initialization Command:**
```bash
npm install -g @nestjs/cli
nest new auth-service --package-manager npm
cd auth-service
npm install @nestjs/common @nestjs/core @nestjs/platform-express
npm install jsonwebtoken passport passport-jwt passport-openidconnect
npm install @prisma/client bcryptjs
npm install --save-dev @nestjs/testing jest @types/jest ts-jest
npx prisma init
```

### Architectural Decisions Provided by Starter

**Language & Runtime:**
- TypeScript 5.x with strict type checking
- Node.js LTS runtime
- Full type safety for auth-critical code

**Framework Architecture:**
- Controllers → Services → Repositories pattern
- Dependency injection for testability
- Request/Response interceptors for cross-cutting concerns
- Custom decorators for auth guards

**Code Organization:**
```
src/
├── auth/              # OIDC, JWT, session management
├── mfa/               # TOTP, SMS fallback, device management
├── rbac/              # Role definitions, permission checks
├── audit/             # Immutable audit logging
├── tenant/            # Tenant context, isolation
├── users/             # User management, deletion workflows
├── breakglass/        # Emergency access tokens
└── common/            # Shared utilities, decorators, filters
```

**Database Integration:**
- Prisma ORM with type-safe queries
- Multi-tenant schema from start
- Migrations for audit tables
- Transaction support for consistency

**Testing Framework:**
- Jest with ts-jest support
- Integration test utilities
- Test database seeding

**Development Experience:**
- Hot module reloading
- OpenAPI/Swagger auto-docs
- Environment-based configuration
- Request validation with class-validator

**Note:** Project initialization should be the first implementation story, establishing the authenticated service foundation.

---

## Architecture Decisions (APPROVED)

### Decision 1: Multi-Tenant Data Model
**Selected:** Tenant ID in Every Table + API Validation Middleware
- Database: WHERE tenant_id filters on every query (indexed for performance)
- API: NestJS guard middleware validates tenant ownership before data access
- Result: Defense in depth - both database and API layer prevent cross-tenant access
- Rationale: Matches NestJS patterns, easier to test and audit

### Decision 2: Authentication Flow Architecture
**Access Token:** JWT (RS256 signed, 1-hour expiry, includes tenant_id + roles)
**Refresh Token:** HttpOnly secure cookie (30-day expiry, database-backed for logout)
**Breakglass Token:** Short-lived single-use (15-minute expiry, database-backed)

**Session Binding:**
- Post-OIDC authentication: user.tenant_id bound to session (immutable)
- Post-MFA verification: session marked as MFA-verified
- All downstream RBAC checks use tenant_id from session

### Decision 3: MFA Implementation Details
**TOTP Algorithm:**
- HMAC-SHA1, 6-digit codes, 30-second time window
- Replay prevention: Check within ±1 window, mark window as used
- Secret storage: AES-256 encrypted at rest in database

**SMS Fallback Flow:**
- User can request SMS OTP when device unavailable
- Generate 6-digit OTP, send via SMS (primary + failover provider)
- Validity: 10 minutes from generation
- Audit: WHO requested, WHEN, FROM IP, OUTCOME

**Breakglass Codes:**
- Admin generates 1-time use codes for MFA-locked users
- Validity: 15 minutes from issuance
- Post-use: User must re-enroll MFA (prevents repeated breakglass use)
- Audit: WHO generated (admin), WHO used (user), WHEN, IP

### Decision 4: Session & Logout Strategy
**Multi-Device Persistence:**
- Each device: Unique session_id in database
- Per-device refresh tokens: Can logout single device or all
- User control: See active sessions and revoke individually

**Logout Mechanics:**
- Full logout: Invalidate ALL session_ids for user (within their tenant)
- Single device logout: Revoke specific session_id only
- Admin logout: Admin can invalidate user's sessions (audit logged)
- Token expiry: Access token checked on each request

### Decision 5: RBAC Permission Model
**Selected:** Hybrid - Permissions in JWT + In-Memory Cache (5-minute TTL)
- Permissions in JWT: No database lookup per request (fast path)
- Cache with TTL: Balances freshness vs performance
- Cache invalidation: Immediate flush on admin role changes

**Permission Check Pattern for Every API Endpoint:**
1. Extract tenant_id from session
2. Extract user roles from JWT
3. Check permission: role + required action
4. Validate: requesting user's tenant == resource's tenant
5. If permission denied: Log attempt + return HTTP 403 (never leak resource existence)

### Decision 6: Immutable Audit Logging Architecture
**Audit Log Schema:**
```json
{
  "id": "uuid",
  "tenant_id": "uuid",
  "actor_id": "uuid",
  "action": "login|mfa_enrolled|role_assigned|breakglass_used|session_revoked",
  "resource_id": "uuid",
  "outcome": "success|failed|denied",
  "metadata": { "ip": "x.x.x.x", "device_fingerprint": "...", "error_reason": "..." },
  "created_at": "timestamp",
  "signature": "hmac-sha256(payload + secret_key)"
}
```

**Immutability Guarantee:**
- Audit table: INSERT-only (no UPDATE/DELETE operations)
- Signature verification: HMAC-SHA256 detects any tampering
- Key rotation: Monthly, verify all historic signatures still valid
- Auto-purge: Delete after 12 months (purge event itself logged)

**Query Performance:**
- Index on (tenant_id, created_at) for fast searches
- Materialized view for admin dashboards (read-only, updated hourly)

### Decision 7: Error Handling & Information Leakage Prevention
**Error Response Strategy:**
- Permission Denied: HTTP 403 Forbidden (no detail in response)
- Resource Not Found: HTTP 404 (treat as 403 if might be permission issue)
- Invalid User: Never say "user doesn't exist" (could indicate other tenant)
- MFA Failure: Log internally, return generic error to user

**Rate Limiting:**
- Login endpoint: 5 failed attempts → 15-minute lockout
- MFA endpoint: 3 failed attempts → 10-minute lockout
- Every attempt: Audit logged with actor, IP, timestamp, outcome

---

## Implementation Ready
These 7 architectural decisions are locked and will guide all remaining stories (Epics 2-8). The first implementation story should initialize the NestJS auth-service with the multi-tenant schema foundation.

