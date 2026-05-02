---
project_name: 'todo-react'
user_name: 'Harry'
date: '2026-05-03'
stepsCompleted: ['step-01-validate-prerequisites', 'step-02-design-epics', 'step-03-create-stories']
inputDocuments: 
  - 'd:\harrybin\todo-react-playwright\_bmad-output\planning-artifacts\prd.md'
requirementsExtracted: true
extractionDate: '2026-05-03'
epicsApproved: true
approvalDate: '2026-05-03'
storiesCreated: true
storiesDate: '2026-05-03'
totalEpics: 8
totalStories: 50
---

# Epic & Story Requirements - todo-react

**Project:** todo-react  
**Domain:** B2B SaaS - Workforce Identity & Access Management  
**Complexity:** High  
**Project Context:** Brownfield (extending existing todo-react app)  
**Release Model:** Single-release MVP with all core enterprise authentication capabilities

---

## Extracted Functional Requirements (FRs)

### OIDC Authentication & IdP Integration (8 FRs)

- **FR1:** Admin can configure OIDC integration by providing IdP metadata URL, and the system auto-discovers endpoints and certificate information
- **FR2:** Admin can manually configure OIDC endpoints (authorization, token, userinfo) when automatic discovery fails
- **FR3:** System validates OIDC configuration and confirms protocol compliance before accepting configuration as active
- **FR4:** Employee can initiate login and be redirected to configured corporate IdP for authentication
- **FR5:** System receives authenticated user claims and group memberships from IdP via OIDC token
- **FR6:** System maps IdP group claims to todo-react application roles (Admin, User, Read-Only, Security Officer)
- **FR7:** System updates user role mapping on every login based on current IdP group membership
- **FR8:** Admin can configure multiple IdPs, and employee can select IdP by company domain or dropdown on login page

### Multi-Factor Authentication (7 FRs)

- **FR9:** Employee can enroll TOTP-based MFA by scanning QR code with authenticator app (Google Authenticator, Authy, Microsoft Authenticator)
- **FR10:** System enforces MFA on every login attempt — TOTP code must be provided and validated before session creation
- **FR11:** Employee can provide SMS-delivered one-time code as fallback when TOTP device is unavailable
- **FR12:** System validates TOTP codes with 30-second time window tolerance and prevents replay attacks
- **FR13:** System tracks MFA enrollment status for each user and blocks login if MFA not enrolled
- **FR14:** Employee can view MFA enrollment status and see registered MFA devices
- **FR15:** Admin can manually unenroll user's MFA device and force re-enrollment on next login

### Role-Based Access Control (8 FRs)

- **FR16:** Admin can assign application roles (Admin, User, Read-Only, Security Officer) to users
- **FR17:** Admin role can access admin console, configuration settings, user management, and audit logs
- **FR18:** User role can create, read, update, and delete tasks and resources they own or are assigned to
- **FR19:** Read-Only role can view tasks and resources but cannot create, update, or delete
- **FR20:** Security Officer role can access audit logs and compliance reports but cannot modify application settings or users
- **FR21:** System enforces role-based permissions on every API request — no permission-denied errors should reach UI
- **FR22:** Task/resource access is determined by: (1) resource tenant matches user tenant, (2) user role grants permission
- **FR23:** API returns HTTP 403 (Forbidden) when user lacks permission for requested action, with reason logged but not disclosed to user

### Multi-Tenant Isolation & Data Scoping (6 FRs)

- **FR24:** Employee from Tenant A cannot see, access, or modify data from Tenant B under any circumstances
- **FR25:** System scopes all database queries with tenant_id filter — no cross-tenant data accessible without explicit override
- **FR26:** Employee's tenant is bound to their user account based on IdP domain verification or admin assignment
- **FR27:** Admin API endpoint that returns users list returns only users from admin's own tenant
- **FR28:** System validates API request user's tenant against resource's tenant before returning data (prevents confused deputy attacks)
- **FR29:** Each tenant has isolated OIDC configuration — Tenant A's IdP settings do not affect Tenant B

### Admin Capabilities & Configuration (6 FRs)

- **FR30:** Admin can access step-by-step OIDC configuration wizard that guides through IdP selection, metadata configuration, and role mapping
- **FR31:** Admin can run validation test to confirm OIDC configuration is correct before activating for employees
- **FR32:** Admin can preview user roles and permissions that will result from current IdP claim-to-role mapping
- **FR33:** Admin can view admin dashboard showing: current authentication status, active sessions count, failed login attempts, MFA enrollment statistics
- **FR34:** Admin can view list of all users in tenant with current roles, last login time, and MFA enrollment status
- **FR35:** Admin can manually create user account without OIDC, assign role, and generate temporary password for manual onboarding

### Audit, Compliance & Reporting (5 FRs)

- **FR36:** System logs all authentication events: successful login, failed login (reason), MFA enrollment, MFA re-enrollment, MFA bypass, role change
- **FR37:** Audit log entry captures: timestamp, actor (user), action, resource affected, result (success/failure), IP address/device info
- **FR38:** Audit logs are immutable — once written, cannot be modified or deleted except by time-based automatic purge after 12 months
- **FR39:** Security Officer can search and filter audit logs by: date range, actor, action type, result, resource
- **FR40:** Security Officer can export audit logs as CSV or JSON for compliance investigations and audit evidence

### User Session & Account Management (7 FRs)

- **FR41:** System creates user session after successful OIDC + MFA authentication with session token and refresh token
- **FR42:** Session tokens expire after 1 hour; refresh tokens remain valid for 30 days
- **FR43:** System maintains refresh token validity across multiple devices — employee can authenticate on laptop and mobile without re-authenticating on original device
- **FR44:** Employee can manually log out, invalidating session and refresh tokens
- **FR45:** Employee can request permanent account and data deletion via self-service
- **FR46:** Admin can initiate account and data deletion for offboarded employees
- **FR47:** Data deletion is immutable and logged — cannot be reversed, recorded in audit log with full timestamp and approver

### Breakglass & Emergency Access (6 FRs)

- **FR48:** Help desk agent can generate temporary breakglass access code for user locked out of MFA
- **FR49:** Breakglass code grants 15-minute access window without MFA requirement, used once
- **FR50:** System logs every breakglass access use: who generated it, who used it, when, for how long
- **FR51:** User with active breakglass access can immediately re-enroll MFA with new device
- **FR52:** MFA re-enrollment accepts QR code scan and confirms new device in <2 minutes
- **FR53:** Admin can see breakglass access history and usage patterns to detect abuse

---

## Extracted Non-Functional Requirements (NFRs)

### Performance (5 NFRs)

- **NFR1:** OIDC authentication flow (login page → IdP → token validation → session creation) completes in <3 seconds for end-user perception
- **NFR2:** MFA code validation (submit TOTP code → validation → session update) completes in <500ms
- **NFR3:** Audit log search across 12 months of data returns results in <2 seconds for queries filtering by actor or action type
- **NFR4:** Admin dashboard loads and displays real-time statistics (active sessions, failed login count) in <1 second
- **NFR5:** API endpoints for task CRUD operations respond within 200ms (p95) under normal load

### Security (10 NFRs)

- **NFR6:** All authentication tokens (access, refresh, breakglass) are cryptographically signed with RS256 or stronger
- **NFR7:** Session tokens are stored in secure HTTP-only cookies with Secure flag; no access token in localStorage
- **NFR8:** All API communication uses HTTPS with TLS 1.2 or higher; no cleartext authentication data
- **NFR9:** TOTP/SMS secrets for MFA are encrypted at rest using AES-256; encryption keys managed via secure key management system
- **NFR10:** OIDC metadata endpoints are cached with signature verification; Man-in-the-Middle attacks on metadata discovery are prevented
- **NFR11:** Failed login attempts are rate-limited: max 5 failed attempts per user per 15 minutes, triggering temporary account lockout
- **NFR12:** Audit logs are cryptographically signed (HMAC-SHA256) to prevent tampering; any modification is detectable
- **NFR13:** Password reset flows use secure token mechanism (one-time use, 15-minute expiry, HTTPS only)
- **NFR14:** API endpoints validate tenant ownership for every request; no confused deputy attacks possible
- **NFR15:** Secrets (OIDC client secrets, SMS API keys, encryption keys) are stored in secure vault (not in code or config files)

### Scalability (5 NFRs)

- **NFR16:** System supports up to 10,000 concurrent MFA validations per second without exceeding 500ms latency (p95)
- **NFR17:** Database queries for tenant data retrieval scale linearly with tenant size; <100ms query time for 100K user tenants
- **NFR18:** Multi-tenant isolation enforced without requiring separate infrastructure per tenant; all tenants share database with tenant_id scoping
- **NFR19:** Authentication can scale from 10 users (small startup) to 100,000 users (large enterprise) without architectural changes
- **NFR20:** SMS MFA provider can scale to handle 50,000 SMS deliveries per day; automatic failover to backup provider if primary exceeds 90% capacity

### Reliability (6 NFRs)

- **NFR21:** Authentication service maintains >99.5% uptime (≤3.6 hours downtime per month) including planned maintenance
- **NFR22:** MFA SMS delivery succeeds within 2 minutes with 99% reliability; failed SMS triggers automatic retry with alternate provider
- **NFR23:** OIDC token endpoint handles temporary IdP outages gracefully; user session is not lost if IdP becomes temporarily unavailable during validation
- **NFR24:** Audit log writes are guaranteed (no silent failures); system returns HTTP 500 if audit log cannot be persisted
- **NFR25:** Database backups are taken hourly; Recovery Point Objective (RPO) ≤1 hour; Recovery Time Objective (RTO) ≤4 hours
- **NFR26:** Disaster recovery plan documents complete system restoration from backup; tested quarterly

### Accessibility (3 NFRs)

- **NFR27:** Admin console UI conforms to WCAG 2.1 Level AA standards; all interactive elements keyboard-accessible
- **NFR28:** MFA re-enrollment QR code includes text backup code option for users unable to scan QR code
- **NFR29:** Audit log dashboard supports screen reader navigation; data table rows are properly marked with ARIA labels

### Integration (3 NFRs)

- **NFR30:** OIDC integration with any OIDC 1.0-compliant IdP succeeds with <2 hours configuration time and zero code changes
- **NFR31:** IdP metadata updates (certificate rotation, endpoint changes) are detected and cached within 1 hour automatically
- **NFR32:** SMS MFA provider integration supports dual provider failover; automatic switch on 30-second timeout

---

## User Journeys & Personas

### Journey 1: IT Admin - OIDC Setup and First Day Operations
**Persona:** Sarah, IT Security Administrator

**Key Requirements Revealed:**
- OIDC configuration wizard with step-by-step validation
- Auto-discovery of standard IdP metadata
- Role mapping UI with permission preview
- Validation test before go-live
- Admin dashboard for monitoring live authentication

**Capabilities Enabled:** FR1, FR2, FR3, FR6, FR30, FR31, FR32, FR33

---

### Journey 2: Employee - First Login and Regular Usage
**Persona:** Marcus, Software Engineer

**Key Requirements Revealed:**
- Seamless OIDC redirect flow
- MFA enforcement on every session
- Automatic permission assignment based on IdP claims/groups
- Session management with refresh tokens
- Access control enforced on data retrieval (not just UI)

**Capabilities Enabled:** FR4, FR5, FR6, FR7, FR9, FR10, FR18, FR41, FR42, FR43

---

### Journey 3: Employee - MFA Device Loss and Recovery
**Persona:** Jessica, Product Manager

**Key Requirements Revealed:**
- Breakglass/emergency access for MFA recovery
- Secure temporary access tokens for help desk use
- Simple MFA re-enrollment flow
- Audit logging of breakglass usage for security review
- Help desk tools that don't require engineering involvement

**Capabilities Enabled:** FR48, FR49, FR50, FR51, FR52, FR36, FR37, FR38

---

### Journey 4: Security Officer - Audit and Compliance Review
**Persona:** David, IT Security and Compliance Officer

**Key Requirements Revealed:**
- Immutable audit logging of all auth and admin events
- Searchable, filterable audit dashboard
- Compliance report templates (SOC 2, etc.)
- Role-based audit visibility (security officers see full logs)
- Alert/detection for suspicious patterns (failed logins, privilege escalation)
- Audit log export and retention controls

**Capabilities Enabled:** FR20, FR36, FR37, FR38, FR39, FR40, FR50

---

## Epic List

### Epic 1: Multi-Tenant Isolation Foundation
Establish secure tenant boundaries at all layers so each enterprise customer's data is completely isolated and inaccessible to other tenants.  
**FRs covered:** FR24, FR25, FR26, FR27, FR28, FR29  
**User Value:** Enterprise customers have guaranteed data isolation and security  
**NFRs Supported:** NFR14, NFR18

---

### Epic 2: Admin OIDC Configuration & Validation
Enable IT admins to configure their corporate identity provider (Okta, Azure AD, etc.) with step-by-step setup, automatic IdP discovery, role mapping, and validation testing.  
**FRs covered:** FR1, FR2, FR3, FR6, FR8, FR30, FR31, FR32  
**User Value:** Admins can set up enterprise authentication in <2 hours without errors  
**NFRs Supported:** NFR30, NFR31, NFR1

---

### Epic 3: Employee OIDC Authentication & Session Management
Enable employees to authenticate through their corporate identity provider and maintain secure sessions across multiple devices with token expiry and refresh.  
**FRs covered:** FR4, FR5, FR7, FR41, FR42, FR43, FR44  
**User Value:** Employees securely access the system through SSO with seamless session management  
**NFRs Supported:** NFR1, NFR6, NFR7, NFR8, NFR23

---

### Epic 4: Mandatory Multi-Factor Authentication
Enforce TOTP-based MFA enrollment and validation on every login, with SMS fallback for device recovery.  
**FRs covered:** FR9, FR10, FR11, FR12, FR13, FR14, FR15  
**User Value:** System is hardened against account compromise; employees authenticate with 2-factor security  
**NFRs Supported:** NFR2, NFR9, NFR11, NFR16, NFR20, NFR21, NFR22, NFR28

---

### Epic 5: Role-Based Access Control & Admin Capabilities
Enable admins to assign roles (Admin, User, Read-Only, Security Officer), manage users, preview permissions, and system enforces role-based access on every API request.  
**FRs covered:** FR16, FR17, FR18, FR19, FR20, FR21, FR22, FR23, FR33, FR34, FR35  
**User Value:** Admins control access granularly; users only see resources they're authorized for  
**NFRs Supported:** NFR4, NFR5, NFR14, NFR21

---

### Epic 6: User Account Lifecycle & Data Deletion
Enable employees to request account deletion and admins to offboard users with immutable audit trails; ensure GDPR compliance.  
**FRs covered:** FR45, FR46, FR47  
**User Value:** Employees have control over their data; admins can manage team offboarding securely  
**NFRs Supported:** NFR12, NFR24, NFR25

---

### Epic 7: MFA Device Recovery & Emergency Access
Enable help desk to issue breakglass access codes for users locked out of MFA, allowing re-enrollment without security compromise.  
**FRs covered:** FR48, FR49, FR50, FR51, FR52, FR53  
**User Value:** Users locked out of MFA can recover quickly with help desk support  
**NFRs Supported:** NFR2, NFR6, NFR12, NFR13, NFR21

---

### Epic 8: Audit Logging & Compliance
System logs all authentication and admin events immutably; Security Officers can search, filter, and export logs for SOC 2 compliance investigations.  
**FRs covered:** FR36, FR37, FR38, FR39, FR40  
**User Value:** Enterprise compliance and security investigation; full audit trail for regulatory requirements  
**NFRs Supported:** NFR3, NFR12, NFR21, NFR24, NFR25, NFR27, NFR29

---

## Requirements Coverage Map

| FR | Epic | Description |
|---|---|---|
| FR1 | Epic 2 | Admin OIDC metadata URL configuration |
| FR2 | Epic 2 | Manual OIDC endpoint configuration |
| FR3 | Epic 2 | OIDC validation and protocol compliance |
| FR4 | Epic 3 | Employee OIDC login redirect |
| FR5 | Epic 3 | Receive authenticated user claims |
| FR6 | Epic 2 | Map IdP claims to application roles |
| FR7 | Epic 3 | Update role mapping on each login |
| FR8 | Epic 2 | Multiple IdP configuration and selection |
| FR9 | Epic 4 | TOTP MFA enrollment via QR code |
| FR10 | Epic 4 | Enforce MFA on every login |
| FR11 | Epic 4 | SMS fallback for MFA |
| FR12 | Epic 4 | TOTP validation with replay attack prevention |
| FR13 | Epic 4 | Track MFA enrollment status |
| FR14 | Epic 4 | View MFA devices |
| FR15 | Epic 4 | Admin unenroll MFA and force re-enrollment |
| FR16 | Epic 5 | Assign application roles |
| FR17 | Epic 5 | Admin role permissions |
| FR18 | Epic 5 | User role permissions |
| FR19 | Epic 5 | Read-Only role permissions |
| FR20 | Epic 5 | Security Officer role permissions |
| FR21 | Epic 5 | API-layer permission enforcement |
| FR22 | Epic 5 | Tenant + role-based access control |
| FR23 | Epic 5 | HTTP 403 error handling |
| FR24 | Epic 1 | Prevent cross-tenant data access |
| FR25 | Epic 1 | Database query tenant scoping |
| FR26 | Epic 1 | Tenant binding to user account |
| FR27 | Epic 1 | Tenant-scoped admin API endpoints |
| FR28 | Epic 1 | API validation of tenant ownership |
| FR29 | Epic 1 | Isolated OIDC config per tenant |
| FR30 | Epic 2 | OIDC configuration wizard |
| FR31 | Epic 2 | Configuration validation test |
| FR32 | Epic 2 | Role mapping preview |
| FR33 | Epic 5 | Admin dashboard with statistics |
| FR34 | Epic 5 | User list with roles and status |
| FR35 | Epic 5 | Manual user creation without OIDC |
| FR36 | Epic 8 | Log authentication events |
| FR37 | Epic 8 | Audit log entry structure (WHO, WHAT, WHEN, WHERE, OUTCOME) |
| FR38 | Epic 8 | Immutable audit logs with 12-month retention |
| FR39 | Epic 8 | Search and filter audit logs |
| FR40 | Epic 8 | Export audit logs (CSV/JSON) |
| FR41 | Epic 3 | Create user session after auth |
| FR42 | Epic 3 | Session and refresh token expiry |
| FR43 | Epic 3 | Cross-device session persistence |
| FR44 | Epic 3 | Manual logout |
| FR45 | Epic 6 | Employee self-service account deletion |
| FR46 | Epic 6 | Admin-initiated account deletion |
| FR47 | Epic 6 | Immutable deletion audit trail |
| FR48 | Epic 7 | Help desk generate breakglass code |
| FR49 | Epic 7 | Breakglass 15-minute access window |
| FR50 | Epic 7 | Log breakglass usage |
| FR51 | Epic 7 | Re-enroll MFA with breakglass access |
| FR52 | Epic 7 | MFA re-enrollment in <2 minutes |
| FR53 | Epic 7 | Admin view breakglass history |

---

## Success Criteria Alignment

### User Success
- **>95% MFA usage over 6 months:** Mandatory MFA enforcement (FR10, NFR21)
- **Sustained adoption without friction:** Smooth OIDC + MFA flows (FR4, FR9, NFR1, NFR2)

### Admin Success
- **Zero errors on first OIDC setup:** Step-by-step wizard with validation (FR30, FR31, FR32)
- **Sub-2-hour configuration:** Clear UI and auto-discovery (FR1, FR30, NFR30)

### Business Success
- **Support resolves 95% auth issues without escalation:** Breakglass access + audit tools (FR48-FR52, FR39, FR40)
- **Security readiness for SOC 2:** Immutable audit logging (FR36-FR40, NFR12)

### Technical Success
- **OIDC protocol compliance:** Full OIDC 1.0 support (FR1-FR8, NFR30, NFR31)
- **>99.5% MFA uptime:** Reliable MFA service (NFR21, NFR22)
- **Immutable audit logging:** Cryptographically signed logs (FR36-FR40, NFR12)

---

## Epics with Stories

---

## Epic 1: Multi-Tenant Isolation Foundation

**Goal:** Establish secure tenant boundaries at all layers so each enterprise customer's data is completely isolated and inaccessible to other tenants.

**FRs Covered:** FR24, FR25, FR26, FR27, FR28, FR29  
**NFRs Supported:** NFR14, NFR18

---

### Story 1.1: Database Schema with Tenant Scoping

As a **System Architect**,
I want **all user-scoped database entities to include a tenant_id field and enforce tenant filtering at the query layer**,
So that **data from different enterprise tenants cannot be accidentally accessed in the same query**.

**Acceptance Criteria:**

**Given** the database schema for users, sessions, roles, and tasks is being designed  
**When** I create the schema  
**Then** each table includes a `tenant_id` VARCHAR(255) NOT NULL field  
**And** a composite index is created on (tenant_id, id) for fast filtering  
**And** a unique constraint exists on (tenant_id, email) for the users table to allow same email across tenants

**Given** a developer queries the users table without filtering by tenant_id  
**When** the query is executed  
**Then** the query returns results only from a default safe tenant (e.g., none, or raises an error if no tenant context)  
**And** audit logs record the query context for security review

**Given** a query filters by `WHERE tenant_id = X`  
**When** the query is executed  
**Then** only users from tenant X are returned  
**And** no data from tenant Y is accessible

**Requirements Met:** FR24, FR25, NFR14, NFR18

---

### Story 1.2: Tenant Binding on User Account Creation

As an **Admin**,
I want **a user's tenant to be automatically determined and immutably bound to their account when they are first created**,
So that **users cannot access or be reassigned to other tenants**.

**Acceptance Criteria:**

**Given** an admin creates a new user account in the admin console  
**When** the user account is created  
**Then** the admin selects a tenant from a dropdown or is automatically assigned based on their own tenant context  
**And** the user's tenant_id is set and stored in the users table  
**And** a record is created in an audit log showing who created the user and which tenant they were assigned to

**Given** a user account has been created with tenant_id = "acme-corp"  
**When** an admin attempts to move that user to tenant_id = "widget-inc"  
**Then** the system rejects the change with an error message: "User tenant assignment cannot be changed after account creation"  
**And** the attempted change is logged in the audit system

**Given** an employee authenticates via OIDC  
**When** their user account is auto-created based on OIDC claims  
**Then** their tenant_id is set based on their email domain or explicit admin configuration  
**And** the tenant binding cannot be changed

**Requirements Met:** FR26, NFR14

---

### Story 1.3: API Tenant Validation Middleware

As a **Backend Developer**,
I want **every API request to validate that the requesting user's tenant matches the resource's tenant before returning data**,
So that **cross-tenant access is impossible even if the database filters fail**.

**Acceptance Criteria:**

**Given** a user from "acme-corp" tenant makes an API request to fetch a task with ID "task-123"  
**When** the request is processed  
**Then** the middleware extracts the user's tenant_id from their session token  
**And** queries the task resource to get its tenant_id  
**And** compares user tenant_id == resource tenant_id  
**And** if match: proceeds with normal response  
**And** if no match: returns HTTP 403 Forbidden with generic error message (no details about the resource)  
**And** logs the denied access attempt with actor, resource, and timestamp

**Given** a user attempts to bypass tenant validation via direct API call  
**When** the API endpoint is called  
**Then** the middleware validates tenant ownership BEFORE any business logic runs  
**And** no information about the resource is leaked in error messages  
**And** the attempt is recorded in security audit logs

**Given** an admin API endpoint returns a list of users  
**When** the endpoint is called  
**Then** the middleware validates that the requesting user is an Admin in their own tenant  
**And** the query filters results by the admin's tenant_id  
**And** users from other tenants are never included in the response

**Requirements Met:** FR28, NFR14, NFR5

---

### Story 1.4: Tenant-Scoped Admin Endpoints

As an **Admin**,
I want **admin-only endpoints (user list, user management, config) to only show data from my tenant**,
So that **I can never see or affect another tenant's users or configuration**.

**Acceptance Criteria:**

**Given** Admin A from tenant "acme-corp" calls the GET /admin/users endpoint  
**When** the request is processed  
**Then** only users from "acme-corp" are returned  
**And** users from "widget-inc" or any other tenant are not in the response  
**And** the response includes a count of total users (filtered to the admin's tenant only)

**Given** Admin A attempts to call GET /admin/users?tenant=widget-inc to bypass tenant filtering  
**When** the request is processed  
**Then** the middleware overrides any user-provided tenant parameter  
**And** uses the admin's own tenant_id from their session  
**And** returns only "acme-corp" users regardless of the query parameter

**Given** Admin B from "widget-inc" calls GET /admin/users  
**When** the request is processed  
**Then** only users from "widget-inc" are returned  
**And** "acme-corp" data is completely inaccessible to Admin B

**Given** an endpoint modifies a user (e.g., POST /admin/users/{userId}/role)  
**When** the endpoint is called  
**Then** the middleware validates that the target user belongs to the admin's tenant  
**And** if not: returns HTTP 403 Forbidden  
**And** if yes: proceeds with the modification  
**And** change is logged to audit trail with actor, resource, and tenant context

**Requirements Met:** FR27, FR29, NFR14

---

## Epic 2: Admin OIDC Configuration & Validation

**Goal:** Enable IT admins to configure their corporate identity provider (Okta, Azure AD, Google Workspace, Keycloak) with step-by-step setup, automatic IdP discovery, role mapping, and validation testing.

**FRs Covered:** FR1, FR2, FR3, FR6, FR8, FR30, FR31, FR32  
**NFRs Supported:** NFR30, NFR31, NFR1  
**Architectural Alignment:** Multi-tenant isolation (each tenant has isolated OIDC config), API-layer validation, audit logging of config changes

---

### Story 2.1: OIDC Configuration Data Model & Storage

As a **System Architect**,
I want **database schema to support storing OIDC configuration per tenant including discovery metadata, endpoints, and mappings**,
So that **each tenant can have isolated OIDC configuration with no cross-tenant access**.

**Acceptance Criteria:**

**Given** the OIDC configuration schema is being designed  
**When** I create the database schema  
**Then** a `oidc_configs` table is created with columns:
- `id` (UUID, primary key)
- `tenant_id` (VARCHAR, foreign key to tenants, NOT NULL)
- `name` (VARCHAR, e.g., "Okta Production")
- `idp_type` (VARCHAR, e.g., "okta", "azure_ad", "google_workspace", "keycloak")
- `metadata_url` (VARCHAR or NULL if manual config)
- `authorization_endpoint` (VARCHAR, discovered or manually provided)
- `token_endpoint` (VARCHAR, discovered or manually provided)
- `userinfo_endpoint` (VARCHAR, discovered or manually provided)
- `jwks_uri` (VARCHAR, for token verification)
- `client_id` (VARCHAR, encrypted at rest)
- `client_secret` (VARCHAR, encrypted at rest, stored in secure vault)
- `scope` (VARCHAR, e.g., "openid profile email groups")
- `is_active` (BOOLEAN, only one active config per tenant)
- `created_at` (TIMESTAMP)
- `updated_at` (TIMESTAMP)

**And** a composite unique constraint exists on (tenant_id, name) to prevent duplicate config names per tenant

**And** a composite index exists on (tenant_id, is_active) for quick lookup of active config

**And** a `role_mappings` table is created with columns:
- `id` (UUID, primary key)
- `oidc_config_id` (UUID, foreign key)
- `idp_group_claim` (VARCHAR, e.g., "okta_group", "groups")
- `idp_group_value` (VARCHAR, e.g., "everyone", "admin-team", "sales")
- `app_role` (VARCHAR, one of: "Admin", "User", "Read-Only", "Security Officer")
- `created_at` (TIMESTAMP)

**And** a composite unique constraint on (oidc_config_id, idp_group_value) to prevent duplicate mappings

**Given** an admin tenant attempts to query another tenant's OIDC config  
**When** the query is executed at the API layer  
**Then** the middleware validates tenant_id filter  
**And** only the requesting tenant's configs are returned  
**And** no cross-tenant config leakage occurs

**Requirements Met:** FR1, FR2, NFR14, NFR18

---

### Story 2.2: OIDC Metadata Discovery from URL

As an **Admin**,
I want **to provide an IdP metadata URL and have the system automatically discover OIDC endpoints and public keys**,
So that **I don't need to manually configure each endpoint**.

**Acceptance Criteria:**

**Given** an admin enters a metadata URL (e.g., `https://okta.com/.well-known/openid-configuration`)  
**When** the admin clicks "Discover Configuration"  
**Then** the system makes an HTTPS GET request to the metadata URL  
**And** parses the OpenID Configuration document  
**And** extracts endpoints: `authorization_endpoint`, `token_endpoint`, `userinfo_endpoint`, `jwks_uri`  
**And** displays the discovered endpoints to the admin for review  
**And** stores the values in the OIDC config when the admin confirms

**Given** the metadata URL is unreachable or returns non-200 status  
**When** the discovery attempt is made  
**Then** the system displays an error: "Could not reach metadata URL. Please verify the URL is correct and publicly accessible."  
**And** logs the discovery attempt failure with timestamp and error details

**Given** the metadata URL returns invalid JSON or missing required fields  
**When** the discovery attempt is made  
**Then** the system displays an error: "Metadata URL did not contain valid OpenID Configuration. Please verify the URL or configure endpoints manually."  
**And** no partial data is saved

**Given** an admin discovers metadata successfully  
**When** the system completes discovery  
**Then** a record is created in audit logs: action="oidc_discovery_initiated", status="success", tenant_id={tenant}, timestamp={now}

**Requirements Met:** FR1, FR30, NFR30, NFR1

---

### Story 2.3: OIDC Manual Endpoint Configuration

As an **Admin**,
I want **to manually configure OIDC endpoints when automatic discovery fails or is not available**,
So that **I can integrate with any OIDC-compliant IdP**.

**Acceptance Criteria:**

**Given** an admin accesses the OIDC configuration form with "Manual Configuration" option  
**When** I click "Manual Configuration"  
**Then** text input fields appear for:
- Authorization Endpoint URL
- Token Endpoint URL
- Userinfo Endpoint URL
- JWKS URI (public key endpoint)

**And** each field has input validation (must be valid HTTPS URL starting with https://)

**Given** an admin enters all required endpoints and clicks "Validate Endpoints"  
**When** validation is triggered  
**Then** the system makes test requests to each endpoint to verify they respond  
**And** for token and userinfo endpoints: makes unauthenticated OPTIONS request to verify CORS headers (if applicable)  
**And** for JWKS URI: fetches the keys and verifies they parse as valid JWK Set  
**And** displays success: "All endpoints are reachable and responding correctly"  
**And** saves the configuration only after successful validation

**Given** one or more endpoints are unreachable  
**When** validation is triggered  
**Then** the system displays error: "The following endpoints did not respond: [list]"  
**And** suggests: "Verify endpoints are publicly accessible and OIDC provider is running"  
**And** does not save the configuration

**Given** a valid configuration is saved  
**When** save completes  
**Then** an audit log entry is created: action="oidc_config_updated", config_name={name}, status="manual_configuration", tenant_id={tenant}

**Requirements Met:** FR2, FR3, NFR30, NFR1

---

### Story 2.4: OIDC Configuration Validation Before Activation

As an **Admin**,
I want **to run a validation test to confirm OIDC configuration is correct before activating it for employees**,
So that **I don't activate a broken configuration and lock employees out**.

**Acceptance Criteria:**

**Given** an admin has configured OIDC endpoints and client credentials  
**When** the admin clicks "Test Configuration"  
**Then** the system performs these validations:
1. Fetch OpenID Configuration from issuer (if metadata URL was provided)
2. Verify authorization_endpoint URL format and accessibility
3. Verify token_endpoint accepts POST requests with correct CORS headers
4. Verify userinfo_endpoint is accessible (unauthenticated)
5. Verify JWKS URI returns valid signing keys
6. Verify client_id and client_secret are valid (by testing a token exchange if possible, or verifying they match IdP format)

**And** displays results:
- ✅ Authorization Endpoint: Reachable
- ✅ Token Endpoint: Reachable, CORS OK
- ⚠️ JWKS URI: Unreachable, retry? (with retry option)
- ❌ Client Secret: Invalid format (with error details)

**Given** all validations pass  
**When** the test completes  
**Then** the admin sees green checkmarks for all items  
**And** a button appears: "Activate Configuration"  
**And** the system logs: action="oidc_validation_passed", config_id={id}, status="ready_for_activation"

**Given** one or more validations fail  
**When** the test completes  
**Then** the configuration cannot be activated  
**And** the admin sees red X marks with specific error messages  
**And** an "Activate" button does not appear  
**And** the system logs: action="oidc_validation_failed", config_id={id}, failed_checks=[list]

**Requirements Met:** FR3, NFR30

---

### Story 2.5: Role Mapping from IdP Claims to App Roles

As an **Admin**,
I want **to map IdP group claims to todo-react application roles (Admin, User, Read-Only, Security Officer)**,
So that **users are automatically assigned correct permissions based on their IdP group membership**.

**Acceptance Criteria:**

**Given** an admin is configuring role mappings  
**When** I access the Role Mapping UI  
**Then** a table appears with columns:
- IdP Group Claim Name (e.g., "groups", "roles", "okta_group")
- IdP Group Value (e.g., "admin-team", "everyone", "security-ops")
- Application Role (dropdown: Admin | User | Read-Only | Security Officer)
- Actions (Edit, Delete)

**And** a button appears to "Add New Mapping"

**Given** an admin clicks "Add New Mapping"  
**When** the mapping form opens  
**Then** fields appear for:
- Claim Name (text input, or dropdown of common claims: "groups", "roles")
- Claim Value (text input, e.g., "admin-team")
- Application Role (dropdown with 4 options)

**And** validation: Claim Name and Claim Value cannot both be empty

**Given** an admin adds mapping: IdP claim "groups" = "admin-team" → App Role "Admin"  
**When** the mapping is saved  
**Then** the mapping is stored in `role_mappings` table  
**And** the table is refreshed, showing the new mapping  
**And** audit log records: action="role_mapping_created", claim="groups/admin-team", app_role="Admin"

**Given** an admin edits a mapping (e.g., change "Admin" to "Read-Only")  
**When** the change is saved  
**Then** the mapping is updated  
**And** the change is logged: action="role_mapping_updated", claim={claim}, old_role="Admin", new_role="Read-Only"

**Given** an admin deletes a mapping  
**When** the delete is confirmed  
**Then** the mapping is removed  
**And** audit log records: action="role_mapping_deleted", claim={claim}, app_role={old_role}

**Requirements Met:** FR6, NFR14

---

### Story 2.6: Role Mapping Preview & Permission Consequences

As an **Admin**,
I want **to preview which users will be assigned which roles based on current IdP mappings**,
So that **I can catch misconfiguration before affecting real users**.

**Acceptance Criteria:**

**Given** an admin has configured one or more role mappings  
**When** the admin clicks "Preview Role Assignments"  
**Then** the system displays (if real IdP data available, or simulated examples):
- A table showing example users with their IdP groups and resulting app role
- Warning alerts if conflicts exist (e.g., user in multiple groups mapping to different roles)
- Count of users per resulting role (e.g., "5 users will be Admin, 45 users will be User")

**And** if using test/preview mode: clearly labeled "PREVIEW - Not Applied to Real Users"

**Given** a user "alice@acme.com" is in IdP groups ["everyone", "admin-team"]  
**When** mappings exist:
- "everyone" → "User"
- "admin-team" → "Admin"  

**Then** the preview displays Alice with: "IdP Groups: everyone, admin-team" → "Assigned Role: Admin (highest privilege group wins)"  
**And** explanation: "Role assignment follows priority: Admin > Security Officer > User > Read-Only"

**Given** the admin reviews the preview and it looks correct  
**When** the admin clicks "Apply Mappings"  
**Then** the mappings take effect immediately  
**And** audit log records: action="role_mappings_activated", total_users_affected={count}

**Requirements Met:** FR32, NFR1

---

### Story 2.7: Support for Multiple IdP Configurations Per Tenant

As an **Admin**,
I want **to configure and manage multiple OIDC identity providers in one tenant**,
So that **I can support employees from different corporate divisions or geographic regions using different IdPs**.

**Acceptance Criteria:**

**Given** an admin is on the OIDC Configuration page  
**When** I click "Add New Configuration"  
**Then** a form appears to create a new OIDC config (same as Story 2.2/2.3)  
**And** each config has a unique name (e.g., "Okta - US", "Azure AD - EMEA")  
**And** only ONE config can be "active" (is_active = true) per tenant at a time

**Given** an admin has two active configurations and attempts to activate a third  
**When** the third activation is attempted  
**Then** the system displays: "Only one configuration can be active. Switch from 'Okta - US' to 'Azure AD - EMEA'?"  
**And** button: "Yes, Deactivate 'Okta - US' and Activate 'Azure AD - EMEA'"

**Given** a user logs in and is redirected to OIDC  
**When** the OIDC endpoint is called  
**Then** the system uses the currently active configuration (is_active = true)  
**And** the user is redirected to the active IdP

**Given** an admin deactivates Config A and activates Config B  
**When** the switch occurs  
**Then** existing sessions using Config A continue to work (no forced logout)  
**And** NEW login attempts use Config B  
**And** audit log records: action="config_deactivated", config_name="Okta - US"  
**And** audit log records: action="config_activated", config_name="Azure AD - EMEA"

**Given** an admin wants to allow users to choose their IdP on the login page  
**When** multiple configs exist  
**Then** the login page displays IdP selection UI:
- Dropdown or buttons: "Login with Okta" | "Login with Azure AD"
- (This is implementation detail; Story 2.7 establishes the database/config structure)

**Requirements Met:** FR8, NFR14

---

### Story 2.8: Admin Dashboard - OIDC Configuration Status

As an **Admin**,
I want **to see a dashboard showing the status of OIDC configuration, last validation results, and active config details**,
So that **I can quickly verify the configuration is correct and detect issues**.

**Acceptance Criteria:**

**Given** an admin navigates to the OIDC Configuration Dashboard  
**When** the page loads  
**Then** the dashboard displays:

**Section 1: Active Configuration**
- Config Name (e.g., "Okta Production")
- IdP Type (e.g., "Okta")
- Status: ✅ Active (green) or ⚠️ Inactive
- Last Updated: {timestamp}
- Last Validation: {timestamp}

**Section 2: Configuration Details**
- Authorization Endpoint: https://okta.com/oauth2/v1/authorize
- Token Endpoint: https://okta.com/oauth2/v1/token
- Userinfo Endpoint: https://okta.com/oauth2/v1/userinfo
- JWKS URI: https://okta.com/oauth2/v1/keys
- Scope: openid profile email groups

**Section 3: Role Mappings**
- Table: IdP Group → App Role (read-only view)
- Count: "3 role mappings configured"

**Section 4: Recent Activity**
- Timestamp | Action | Status
- 2026-05-03 14:22 | Config Validation | ✅ Passed
- 2026-05-03 14:20 | Role Mapping Updated | ✅ Success
- 2026-05-02 09:15 | Config Activated | ✅ Success

**Section 5: Actions**
- Button: "Run Validation Test"
- Button: "Edit Configuration"
- Button: "View Audit Logs"

**Given** the last validation failed or was never run  
**When** the dashboard loads  
**Then** a warning banner displays: "⚠️ OIDC Configuration not validated. Click 'Run Validation Test' before employees log in."

**Requirements Met:** FR30, NFR1

---

## Epic 3: Employee OIDC Authentication & Session Management

**Goal:** Enable employees to authenticate through their corporate identity provider and maintain secure sessions across multiple devices with token expiry and refresh.

**FRs Covered:** FR4, FR5, FR7, FR41, FR42, FR43, FR44  
**NFRs Supported:** NFR1, NFR6, NFR7, NFR8, NFR23  
**Architectural Alignment:** JWT tokens (1-hour access, 30-day refresh), tenant_id bound to session, multi-device support, immutable audit logging

---

### Story 3.1: OIDC Login Redirect & Authorization Request

As an **Employee**,
I want **to click "Login with [IdP]" and be securely redirected to my corporate identity provider**,
So that **I can authenticate using my corporate credentials without entering passwords in the app**.

**Acceptance Criteria:**

**Given** an unauthenticated user visits the login page  
**When** the page loads  
**Then** a button or link appears: "Login with [Active IdP Name]" (e.g., "Login with Okta")

**And** if multiple OIDC configs are available: a dropdown or buttons for each IdP (e.g., "Login with Okta" | "Login with Azure AD")

**Given** the user clicks "Login with Okta"  
**When** the click is processed  
**Then** the frontend initiates OIDC Authorization Code Flow:
1. Generates a random `state` value (for CSRF protection)
2. Generates a random `nonce` value (for token validation)
3. Stores `state` and `nonce` in secure session storage (browser)
4. Constructs authorization URL: `https://okta.com/oauth2/v1/authorize?client_id={id}&redirect_uri={return_uri}&scope=openid+profile+email+groups&response_type=code&state={state}&nonce={nonce}`
5. Redirects browser to the authorization URL

**And** the redirect is an HTTP 302 (temporary redirect, standard OIDC pattern)

**Given** the authorization request is sent to the IdP  
**When** the request is received  
**Then** the IdP validates:
1. client_id is registered
2. redirect_uri matches registered redirect_uri
3. scope is valid

**And** the IdP redirects to its login/consent screens (outside this system)

**Given** the user successfully authenticates with the IdP  
**When** the IdP redirects back to the application  
**Then** the redirect URL includes: `https://app.example.com/auth/callback?code={authorization_code}&state={state}`

**And** the authorization code is single-use and expires in 10 minutes

**Requirements Met:** FR4, NFR1, NFR6

---

### Story 3.2: OIDC Token Exchange & User Claim Extraction

As a **Backend Service**,
I want **to exchange the authorization code for ID and access tokens, verify token signatures, and extract user claims**,
So that **I can confirm the user's identity and retrieve their IdP attributes (email, name, groups)**.

**Acceptance Criteria:**

**Given** the authorization callback is received with authorization_code and state  
**When** the backend receives the callback  
**Then** the backend validates:
1. `state` parameter matches the stored `state` (prevents CSRF)
2. If state mismatch: return HTTP 400, log security event, do not proceed

**And** the backend initiates token exchange:
1. Makes POST request to token_endpoint: `/oauth2/v1/token`
2. Request body includes:
   - grant_type=authorization_code
   - code={authorization_code}
   - client_id={app_client_id}
   - client_secret={app_client_secret} (sent in Authorization header for security)
   - redirect_uri={registered_redirect_uri}

**And** the backend receives response:
```json
{
  "access_token": "eyJ...",
  "id_token": "eyJ...",
  "token_type": "Bearer",
  "expires_in": 3600
}
```

**Given** tokens are received  
**When** the backend processes them  
**Then** the backend validates ID token:
1. Fetches the public key from JWKS URI
2. Verifies token signature using RS256 algorithm
3. Verifies `nonce` matches the stored `nonce` (prevents token replay)
4. Verifies `iss` (issuer) matches configured issuer
5. Verifies `aud` (audience) matches app's client_id
6. If any verification fails: return HTTP 401, log security event, do not create session

**And** the backend extracts ID token claims:
- `sub` (subject/user ID)
- `email` (user email)
- `name` (user display name)
- `groups` or `roles` (IdP group memberships for role mapping)

**And** logs the successful token exchange: action="oidc_token_exchanged", idp={idp_type}, user_email={email}, nonce_verified=true

**Requirements Met:** FR5, NFR1, NFR7, NFR23

---

### Story 3.3: User Record Creation or Lookup Based on OIDC Claims

As a **Backend Service**,
I want **to create a new user record or update an existing user based on OIDC claims**,
So that **employees can log in without manual pre-registration and their profile stays synchronized with the IdP**.

**Acceptance Criteria:**

**Given** OIDC claims are verified and extracted (email, name, groups)  
**When** the user lookup process runs  
**Then** the backend queries users table: SELECT * FROM users WHERE email = {email} AND tenant_id = {tenant_id}

**And** if user exists: proceed to token creation (Story 3.5)

**And** if user does NOT exist:
1. Create new user record with:
   - email = {extracted email}
   - name = {extracted name}
   - tenant_id = {determined from email domain or IdP metadata}
   - is_mfa_enrolled = false (MFA required on first login - Story 4.1)
   - created_at = now()
2. Log user creation: action="user_created_via_oidc", email={email}, tenant_id={tenant_id}
3. Proceed to token creation (Story 3.5)

**Given** a user exists but their name has changed in the IdP  
**When** the token exchange completes  
**Then** the backend updates the user record:
- name = {new name from IdP}
- idp_last_sync = now()
- Logs: action="user_profile_synced", email={email}

**Given** an employee logs in and their email domain doesn't match any tenant  
**When** the tenant lookup fails  
**Then** the backend assigns them to a default tenant (system configuration)  
**And** logs: action="user_assigned_default_tenant", email={email}, default_tenant={id}  
**Or** alternatively: displays error "Your organization is not configured. Contact admin@company.com" and does NOT create user

**Given** a user is created via OIDC login  
**When** the creation occurs  
**Then** the new user record is immutably tied to that tenant_id  
**And** cannot be moved to another tenant (enforced in API layer)

**Requirements Met:** FR26, NFR14, NFR18

---

### Story 3.4: Role Mapping from IdP Groups to Application Roles

As a **Backend Service**,
I want **to map the user's IdP group memberships to todo-react application roles based on configured mappings**,
So that **users are automatically assigned correct permissions based on their corporate organizational structure**.

**Acceptance Criteria:**

**Given** OIDC claims include groups (extracted from `groups` claim)  
**When** role mapping is performed  
**Then** the backend:
1. Retrieves active role_mappings for the tenant's OIDC config
2. For each IdP group the user is in: looks for matching mapping
3. If match found: assigns corresponding app role
4. If multiple groups map to different roles: assigns highest privilege role (Admin > Security Officer > User > Read-Only)

**Example:**
- User is in IdP groups: ["everyone", "admin-team"]
- Mappings exist:
  - "everyone" → "User"
  - "admin-team" → "Admin"
- Result: User is assigned "Admin" role

**Given** a user is assigned a role via mapping  
**When** the mapping is applied  
**Then** the backend:
1. Creates or updates user_role record: user_id={user_id}, role="Admin", assigned_via="oidc_mapping"
2. Logs: action="user_role_assigned_oidc", user_id={user_id}, role="Admin", idp_groups=["admin-team"]

**Given** a user has no IdP groups matching any mapping  
**When** role mapping is performed  
**Then** the user is NOT automatically assigned any role  
**And** they cannot log in (API denies access, returns HTTP 403)  
**And** admin must manually assign a role (Story 5.5)

**Given** on subsequent logins, the user's IdP groups have changed (e.g., removed from "admin-team", added to "sales-team")  
**When** role mapping is performed on next login  
**Then** the role mapping is recalculated based on current IdP groups  
**And** user role is updated: role changes from "Admin" to "User"  
**And** logs: action="user_role_updated_oidc", user_id={user_id}, old_role="Admin", new_role="User", idp_groups=["sales-team"]

**Requirements Met:** FR7, NFR14, NFR1

---

### Story 3.5: Access Token & Refresh Token Generation

As a **Backend Service**,
I want **to generate secure JWT access tokens and HttpOnly refresh tokens for authenticated users**,
So that **the user's browser and frontend can authenticate subsequent API requests**.

**Acceptance Criteria:**

**Given** user identity is verified and roles are assigned (after Stories 3.2, 3.3, 3.4)  
**When** token generation occurs  
**Then** the backend generates access token (JWT):

**Access Token Structure:**
```json
{
  "header": {
    "alg": "RS256",
    "kid": "{key_id}",
    "typ": "JWT"
  },
  "payload": {
    "sub": "{user_id}",
    "email": "{user_email}",
    "tenant_id": "{tenant_id}",
    "roles": ["{assigned_role}"],
    "iat": {current_timestamp},
    "exp": {current_timestamp + 3600},
    "jti": "{unique_token_id}"
  },
  "signature": "RS256({header}.{payload}, private_key)"
}
```

**And** signing is done with RS256 (RSA, SHA-256) using the backend's private key

**And** token includes:
- `tenant_id`: Immutably bound to user's tenant (prevents tenant escalation)
- `roles`: User's assigned roles from mapping (Admin, User, Read-Only, Security Officer)
- `exp`: 3600 seconds (1 hour) from now
- `jti`: Unique token identifier for revocation tracking

**And** the backend also generates a unique session_id (UUID):
1. Creates session record in database:
   - session_id (UUID)
   - user_id (UUID)
   - tenant_id (UUID)
   - device_id (derived from User-Agent hash)
   - refresh_token_hash (HMAC-SHA256 of refresh token, for logout invalidation)
   - created_at (now)
   - expires_at (now + 30 days)
   - is_active (true)

**And** the backend generates a refresh token:
- Random 256-bit value, base64 encoded
- Valid for 30 days
- Stored as hash in database (never store plaintext refresh tokens)

**And** sends HTTP response:
```
200 OK
Set-Cookie: refresh_token={refresh_token}; HttpOnly; Secure; SameSite=Strict; Path=/; Max-Age=2592000
Content-Type: application/json

{
  "access_token": "{jwt_access_token}",
  "token_type": "Bearer",
  "expires_in": 3600,
  "user_id": "{user_id}",
  "email": "{user_email}",
  "role": "{assigned_role}",
  "tenant_id": "{tenant_id}"
}
```

**And** logs: action="session_created", user_id={user_id}, session_id={session_id}, tenant_id={tenant_id}

**Given** the response is sent to the browser  
**When** the browser receives the response  
**Then** the refresh_token is automatically stored as HttpOnly cookie (cannot be accessed by JavaScript)  
**And** the access_token is returned in response body (frontend can store in memory or sessionStorage)

**Requirements Met:** FR41, FR42, NFR1, NFR6, NFR7, NFR8

---

### Story 3.6: Access Token Validation on Every API Request

As a **Backend Service**,
I want **to validate the access token on every API request and extract user identity from it**,
So that **I know who is making the request and can enforce authorization rules**.

**Acceptance Criteria:**

**Given** a user makes an API request (e.g., GET /api/tasks)  
**When** the request is received  
**Then** the backend middleware:
1. Extracts Authorization header: "Bearer {access_token}"
2. If header missing: return HTTP 401 Unauthorized
3. Parses JWT and validates:
   - Signature is valid (verify using issuer's public key)
   - `exp` (expiry) is in the future
   - If expired: return HTTP 401 Unauthorized, log "token_expired"
   - `iat` (issued at) is not in the future (prevents token created in future)
   - `jti` (token ID) is not in revocation list (optional optimization)

**And** if validation passes:
1. Extracts payload: user_id, email, tenant_id, roles
2. Stores in request context for use by endpoint handlers
3. Proceeds with request processing

**Given** the access token is valid but approaching expiry (e.g., <5 minutes remaining)  
**When** the response is sent  
**Then** the response includes header: `X-Token-Expires-Soon: true`  
**And** frontend can proactively refresh before it expires (Story 3.7)

**Given** an access token is invalid (signature mismatch, wrong format, tampered)  
**When** validation fails  
**Then** return HTTP 401 Unauthorized  
**And** log: action="token_validation_failed", reason="invalid_signature", ip={client_ip}  
**And** do NOT proceed with request processing

**Requirements Met:** FR41, NFR1, NFR23

---

### Story 3.7: Refresh Token Rotation & Session Persistence

As an **Employee**,
I want **to stay logged in across multiple devices for 30 days without re-authenticating**,
So that **I don't have to log in repeatedly on my phone and laptop every hour**.

**Acceptance Criteria:**

**Given** an access token has expired (after 1 hour)  
**When** the frontend receives HTTP 401 response  
**Then** the frontend automatically attempts token refresh:
1. Sends POST request to /auth/refresh
2. Refresh token is automatically included as HttpOnly cookie (browser handles this)
3. No user interaction required

**And** the backend receives the refresh request:
1. Validates refresh_token is present in HttpOnly cookie
2. Queries sessions table: SELECT * FROM sessions WHERE refresh_token_hash = HMAC-SHA256({token})
3. Validates session is still active (is_active = true, not expired, expires_at > now())
4. If session invalid: return HTTP 401, log "refresh_token_invalid"
5. If session valid: generates NEW access token (same structure as Story 3.5)

**And** sends HTTP response:
```
200 OK
Content-Type: application/json

{
  "access_token": "{new_jwt_token}",
  "expires_in": 3600
}
```

**And** logs: action="token_refreshed", session_id={session_id}, old_token_jti={old_jti}

**Given** a user is logged in on laptop and mobile (two separate sessions)  
**When** the laptop access token expires  
**Then** the laptop can refresh using its refresh_token independently  
**And** the mobile refresh_token remains valid and can be used separately  
**And** each device maintains its own session_id and refresh_token

**Given** 30 days have passed since session creation  
**When** the user attempts to refresh  
**Then** the refresh_token has expired (session.expires_at < now())  
**And** refresh request returns HTTP 401  
**And** user is redirected to login page  
**And** must re-authenticate via OIDC

**Requirements Met:** FR43, NFR6, NFR7, NFR8

---

### Story 3.8: Manual Logout & Token Invalidation

As an **Employee**,
I want **to manually log out and invalidate my session and refresh tokens**,
So that **I can securely end my session, especially on shared or public computers**.

**Acceptance Criteria:**

**Given** an authenticated user is logged in  
**When** the user clicks "Logout"  
**Then** the frontend initiates logout:
1. Sends POST request to /auth/logout
2. Access token is included in Authorization header
3. Refresh token is included as HttpOnly cookie

**And** the backend receives logout request:
1. Validates access_token is valid (as per Story 3.6)
2. Extracts user_id and session_id from token
3. Updates session record: is_active = false, invalidated_at = now()
4. Deletes or marks refresh_token as invalid
5. Logs: action="logout", session_id={session_id}, user_id={user_id}

**And** sends HTTP response:
```
200 OK
Set-Cookie: refresh_token=; HttpOnly; Secure; Path=/; Max-Age=0
Content-Type: application/json

{
  "message": "Logout successful"
}
```

**And** the frontend:
1. Clears access_token from memory
2. Clears refresh_token from cookies (Max-Age=0 instructs browser)
3. Redirects to login page

**Given** a user logs out  
**When** they attempt to use their old refresh_token  
**Then** the refresh request fails: session.is_active = false  
**And** returns HTTP 401 Unauthorized

**Given** an admin initiates logout for a user (Story 5.6)  
**When** admin logout is performed  
**Then** all sessions for that user are invalidated  
**And** the user is logged out from ALL devices  
**And** logs: action="admin_logout", admin_id={admin_id}, target_user_id={user_id}, all_sessions_invalidated=true

**Requirements Met:** FR44, NFR6, NFR23

---

## Epic 4: Mandatory Multi-Factor Authentication

**Goal:** Enforce TOTP-based MFA enrollment and validation on every login, with SMS fallback for device recovery.

**FRs Covered:** FR9, FR10, FR11, FR12, FR13, FR14, FR15  
**NFRs Supported:** NFR2, NFR9, NFR11, NFR16, NFR20, NFR21, NFR22, NFR28  
**Architectural Alignment:** TOTP + SMS fallback, replay prevention, immutable audit logging of all MFA events

---

### Story 4.1: MFA Enrollment - TOTP Secret Generation & QR Code

As an **Employee**,
I want **to enroll in MFA by scanning a QR code with my authenticator app (Google Authenticator, Authy, Microsoft Authenticator)**,
So that **I can secure my account with time-based one-time passwords**.

**Acceptance Criteria:**

**Given** a user has successfully logged in (OIDC + session created, but before MFA check)  
**When** the system detects MFA is not enrolled  
**Then** the user is redirected to MFA enrollment page (not past login)  
**And** a message displays: "Your organization requires multi-factor authentication. Please set up MFA to continue."

**And** the enrollment page displays:
1. Instructions: "Scan this QR code with your authenticator app (Google Authenticator, Authy, etc.)"
2. QR code image
3. "Can't scan? Enter this key manually:" + {base32_secret}
4. Text input: "Enter the 6-digit code from your app"
5. Button: "Verify & Enable MFA"

**Given** the user accesses MFA enrollment  
**When** enrollment is initiated  
**Then** the backend:
1. Generates a random 256-bit secret
2. Encodes secret as base32 for manual entry
3. Creates provisioning URI (RFC 6238 format):
   `otpauth://totp/app@example.com:user@example.com?secret={base32_secret}&issuer=app&algorithm=SHA1&digits=6&period=30`
4. Generates QR code from provisioning URI
5. Stores temporary MFA record in database (NOT YET ACTIVE):
   - user_id, tenant_id, mfa_secret (AES-256 encrypted), status="pending_verification", created_at
6. Returns QR code image to frontend (base64 or SVG)

**And** logs: action="mfa_enrollment_initiated", user_id={user_id}

**Given** the user scans the QR code with their authenticator app  
**When** they return to the app  
**Then** the authenticator app displays a 6-digit code  
**And** they enter this code in the text input and click "Verify & Enable MFA"

**Given** the user enters a code  
**When** verification is attempted  
**Then** the backend:
1. Retrieves the pending MFA secret for the user
2. Decrypts the AES-256 encrypted secret
3. Generates expected TOTP codes for current time window: now, now-30s, now+30s (±1 window for clock skew)
4. Checks if user-entered code matches ANY of the expected codes
5. If match: marks MFA as active (status="active"), stores timestamp of activation, logs success
6. If no match: returns error "Invalid code. Please check your code and try again." and does NOT activate MFA

**And** if activation succeeds:
- User record is updated: is_mfa_enrolled = true
- Logs: action="mfa_activated", user_id={user_id}
- Displays: "✅ MFA is now active. You'll be asked for a code on your next login."
- Generates backup codes (Story 4.2) or proceeds to next page

**And** if activation fails and code is invalid:
- Does NOT deactivate or change the pending MFA
- User can attempt again (up to X attempts before lockout)

**Requirements Met:** FR9, NFR2, NFR11, NFR20, NFR21

---

### Story 4.2: MFA Enrollment - Backup Codes for Account Recovery

As an **Employee**,
I want **to receive backup codes during MFA enrollment that I can use if I lose my authenticator device**,
So that **I can still access my account without waiting for admin help**.

**Acceptance Criteria:**

**Given** MFA has been successfully activated (Story 4.1 verification succeeds)  
**When** activation is complete  
**Then** the system generates 10 backup codes:
- Each code is 8 alphanumeric characters (e.g., "AB12CD34")
- Codes are randomly generated
- Codes are stored in database with metadata:
  - mfa_backup_codes table: id, user_id, code_hash (SHA-256), tenant_id, created_at, is_used (boolean), used_at
  - Only code_hash is stored (never plaintext)

**And** the page displays:
1. "Save these backup codes in a safe place"
2. List of 10 codes in a copyable format (e.g., monospace box)
3. "✅ Copy codes" button (copies to clipboard)
4. "📥 Download codes" button (downloads as .txt file)
5. Checkbox: "☐ I have saved my backup codes"
6. Button: "I Understand - Continue" (disabled until checkbox checked)

**And** the frontend displays a warning: "⚠️ If you lose your authenticator device and don't have these codes, you won't be able to log in. Save them NOW."

**Given** the user checks the checkbox and clicks "I Understand"  
**When** enrollment is complete  
**Then** MFA enrollment is fully finished  
**And** user is redirected to dashboard  
**And** logs: action="mfa_enrollment_complete", user_id={user_id}, backup_codes_generated=10

**Given** a user later uses a backup code (Story 4.5)  
**When** the code is used  
**Then** the mfa_backup_codes record is marked as used (is_used=true, used_at=now())  
**And** that backup code cannot be reused

**Given** all 10 backup codes have been used  
**When** the user attempts to log in without their authenticator device  
**Then** they cannot use a backup code (all spent)  
**And** must use SMS recovery (Story 4.4) or request breakglass (Epic 7)

**Requirements Met:** FR13, NFR13

---

### Story 4.3: MFA Enforcement on Every Login - TOTP Validation

As a **System**,
I want **to require a TOTP code on every login after OIDC authentication, and block access until MFA is validated**,
So that **all users are protected with 2-factor authentication regardless of role**.

**Acceptance Criteria:**

**Given** a user has completed OIDC authentication (access_token created, session created - Story 3.5)  
**When** user record shows is_mfa_enrolled = true  
**Then** the system:
1. Does NOT issue final session access_token yet
2. Creates temporary MFA session (short-lived, for MFA verification only)
3. Redirects to MFA verification page
4. Does NOT display any indication of which IdP was used (security)

**And** the MFA verification page displays:
1. "Enter the 6-digit code from your authenticator app"
2. Text input field (6 digits only)
3. Button: "Verify"
4. Link: "I don't have my device" (leads to backup code entry - Story 4.2)

**And** when user enters code and clicks "Verify":
1. Backend receives temporary MFA session ID + user-entered code
2. Validates temporary session is active and not expired (5-minute expiry)
3. Retrieves user's MFA secret
4. Generates expected TOTP codes for current window: now, now-30s, now+30s
5. Checks if user-entered code matches
6. Tracks which time windows have been used (replay prevention):
   - Stores window_timestamp of accepted code
   - If code is entered again within same 30-second window: REJECT as replay

**Given** code is valid and passes replay check:
1. Updates user record: last_mfa_verification = now()
2. Marks temporary MFA session as verified
3. Issues final access_token (same as Story 3.5)
4. Returns to frontend with credentials
5. Logs: action="mfa_verification_success", user_id={user_id}

**Given** code is invalid:
1. Increments failed_mfa_attempts counter
2. If attempts < 3: displays "Invalid code. Please try again. ({X} attempts remaining)"
3. If attempts >= 3: locks account for 10 minutes, returns HTTP 429 Too Many Requests
4. Logs: action="mfa_verification_failed", user_id={user_id}, attempt_number={X}

**Given** a user attempts to bypass MFA and directly call API endpoints with access_token from OIDC only:
1. If access_token does NOT have mfa_verified=true claim: API returns HTTP 403 Forbidden
2. All endpoints check this claim before processing

**Requirements Met:** FR10, FR12, NFR2, NFR9, NFR20, NFR21, NFR22

---

### Story 4.4: MFA Device Recovery - SMS Backup Code

As an **Employee**,
I want **to receive an SMS with a one-time code if I don't have my authenticator device**,
So that **I can still log in without calling support**.

**Acceptance Criteria:**

**Given** user is on MFA verification page (after OIDC, before access_token)  
**When** user clicks "I don't have my device"  
**Then** the page displays:
1. "Send recovery code to your phone"
2. Phone number display: "•••• •••• 7890" (last 4 digits visible)
3. Option: "Use this number" or "Enter a different number"
4. Button: "Send Recovery Code"

**And** when user clicks "Send Recovery Code":
1. Backend generates a 6-digit SMS code (different from TOTP)
2. Stores SMS code in database (not as plaintext):
   - mfa_sms_codes table: id, user_id, code_hash, phone_number (last 4 digits only), created_at, expires_at (10 min), is_used
3. Initiates SMS send via SMS provider:
   - Primary provider (e.g., Twilio): sends SMS to stored phone number
   - If send fails: attempts failover provider
   - If both fail: displays error "SMS could not be sent. Try again or contact support."
4. Logs: action="mfa_sms_code_sent", user_id={user_id}, phone_ending_in={last4}

**And** if SMS send succeeds:
- Displays: "✅ Recovery code sent to ••• ••• •••7890. Check your messages."
- Text input field appears: "Enter the 6-digit code from your SMS"
- Countdown timer: "Code expires in 10:00 minutes"
- After 10 minutes: code expires, user cannot use it

**And** page displays "No SMS?" with link to backup codes (Story 4.2) or breakglass (Epic 7)

**Given** user receives SMS and enters the 6-digit code:
1. Backend retrieves stored code_hash
2. Verifies code: hash(user_entered_code) == code_hash
3. If match: marks SMS code as used (is_used=true)
4. Proceeds as if TOTP was verified: issues access_token, logs success
5. Logs: action="mfa_sms_code_verified", user_id={user_id}

**Given** code is entered incorrectly or expired:
1. Displays error: "Invalid or expired code."
2. Does NOT increment failed_mfa_attempts (SMS recovery is last resort before breakglass)
3. Allows retry without lockout

**Requirements Met:** FR11, FR12, NFR16, NFR21, NFR28

---

### Story 4.5: MFA Device Recovery - Backup Code Usage

As an **Employee**,
I want **to use one of my saved backup codes to log in if I lose my authenticator device and can't receive SMS**,
So that **I can still access my account in emergency situations**.

**Acceptance Criteria:**

**Given** user is on MFA verification page  
**When** user clicks "I don't have my device" and then "Use a backup code"  
**Then** the page displays:
1. "Enter one of your backup codes (saved during MFA setup)"
2. Text input: "8-character code"
3. Button: "Verify"

**And** when user enters a code and clicks "Verify":
1. Backend queries mfa_backup_codes table: SELECT * WHERE user_id={user_id} AND code_hash=HASH({user_code}) AND is_used=false
2. If found and not used: marks code as used (is_used=true, used_at=now())
3. Proceeds as if TOTP was verified: issues access_token
4. Logs: action="mfa_backup_code_used", user_id={user_id}, remaining_backup_codes={remaining_count}

**And** if code is not found or already used:
1. Displays: "Invalid or already used backup code."
2. Allows user to:
   - Try another backup code, or
   - Use SMS recovery, or
   - Request breakglass access

**Given** user has used all 10 backup codes:
1. Backup code option is no longer available (greyed out or hidden)
2. User must use SMS recovery or request breakglass

**Requirements Met:** FR13, NFR13

---

### Story 4.6: MFA Device Management - View & Edit MFA Devices

As an **Employee**,
I want **to see which MFA device is currently active and when it was enrolled**,
So that **I can verify my security settings and detect unauthorized enrollment**.

**Acceptance Criteria:**

**Given** a user navigates to their account settings / MFA section  
**When** the page loads  
**Then** the page displays:

**Section 1: MFA Status**
- Status: ✅ Active
- Device Type: "TOTP (Time-based One-Time Password)"
- Enrolled: "May 3, 2026 at 2:15 PM"
- Last Used: "Today at 10:30 AM"

**Section 2: Backup Codes Status**
- Remaining Backup Codes: "3 of 10 used" (progress bar)
- "Generate new backup codes" (button - for emergency re-generation)

**Section 3: Actions**
- "Regenerate MFA Secret" (button - sets up new device)
- "Download Backup Codes Again" (button)
- "Disable MFA" (button - requires admin OR requires password confirmation)

**Given** user clicks "Regenerate MFA Secret"  
**When** clicked  
**Then** the system:
1. Initiates new MFA enrollment flow (Story 4.1)
2. Old MFA secret is marked as inactive but kept for audit
3. After new enrollment complete: old secret is deleted
4. Logs: action="mfa_secret_regenerated", user_id={user_id}

**Given** user clicks "Generate new backup codes"  
**When** clicked  
**Then** the system:
1. Generates 10 new backup codes
2. Marks old codes as obsolete
3. Displays new codes for download/save
4. Logs: action="mfa_backup_codes_regenerated", user_id={user_id}

**Requirements Met:** FR14, NFR2, NFR13

---

### Story 4.7: Admin MFA Management - View & Unenroll User MFA

As an **Admin**,
I want **to view all users' MFA enrollment status and manually unenroll a user's MFA if needed**,
So that **I can support offboarded employees or troubleshoot MFA issues**.

**Acceptance Criteria:**

**Given** an admin navigates to the user management page  
**When** the page loads  
**Then** the user list displays a column: "MFA Status"
- Values: "✅ Active" | "⏳ Pending" | "❌ Not Enrolled" | "🔒 Locked"

**And** each user row has an actions menu (three dots or dropdown)

**Given** admin clicks on a user row to view details  
**When** the user detail page loads  
**Then** a "MFA" section displays:
- Status: ✅ Active (Enrolled May 3, 2026)
- Last Verified: Today at 10:30 AM
- Button: "Unenroll MFA Device"
- (Button is red/dangerous action)

**Given** admin clicks "Unenroll MFA Device"  
**When** confirmation dialog appears  
**Then** dialog shows:
- "⚠️ Warning: This will remove MFA for this user"
- "They will need to set up MFA again on their next login"
- "Reason for unenrollment:" (dropdown: Device Lost | Troubleshooting | Offboarding | Other)
- Buttons: "Cancel" | "Unenroll"

**Given** admin confirms unenrollment  
**When** unenrollment is processed  
**Then** the system:
1. Sets user.is_mfa_enrolled = false
2. Marks MFA record as inactive / deleted
3. Next login: user will be redirected to MFA enrollment (Story 4.1)
4. Logs: action="mfa_unenrolled_by_admin", admin_id={admin_id}, user_id={user_id}, reason={reason}

**And** sends email to user: "Your MFA device was unenrolled. Set up a new device on your next login."

**Requirements Met:** FR15, NFR21

---

### Story 4.8: MFA Enforcement Verification - Non-Compliance Detection

As a **Security Officer**,
I want **to see a report of which users are not compliant with MFA enrollment**,
So that **I can identify and remediate policy violations**.

**Acceptance Criteria:**

**Given** a security officer accesses the "MFA Compliance Report"  
**When** the report loads  
**Then** it displays:
- Total Users: 150
- MFA Enrolled: 145 (96.7%) ✅
- MFA Not Enrolled: 5 (3.3%) ⚠️
- MFA Pending: 0
- Table: List of 5 users without MFA
  - Name, Email, Last Login, Days Without MFA, Actions

**And** each non-compliant user row has action: "Notify" (sends email reminder) or "Force Unenroll & Reenroll" (admin action)

**Given** report is generated  
**When** user clicks on a non-compliant user  
**Then** details show:
- Why not enrolled: Last login was 2 days ago (hasn't been prompted yet) / Declined enrollment / Failed enrollment attempt (X times)
- Option to send reminder: "Notify user to complete MFA enrollment"

**And** compliance report can be:
- Exported as PDF for audits
- Scheduled to run weekly and email to admin/security officer
- Filtered by department, last login date, etc.

**And** logs: action="mfa_compliance_report_viewed", security_officer_id={id}, report_date={date}

**Requirements Met:** FR13, FR21, NFR21

---

## Epic 5: Role-Based Access Control & Admin Capabilities

**Goal:** Enable admins to assign roles (Admin, User, Read-Only, Security Officer), manage users, preview permissions, and system enforces role-based access on every API request.

**FRs Covered:** FR16, FR17, FR18, FR19, FR20, FR21, FR22, FR23, FR33, FR34, FR35  
**NFRs Supported:** NFR4, NFR5, NFR14, NFR21  
**Architectural Alignment:** Permissions enforced at API layer (NestJS guards), tenant-scoped, immutable audit logging of role changes

---

### Story 5.1: Role Model & Permissions Definition

As a **System Architect**,
I want **to define the 4 application roles and their associated permissions**,
So that **role-based access control is consistent and auditable across the system**.

**Acceptance Criteria:**

**Given** the role-permission model is designed  
**When** I create the database schema  
**Then** a `roles` table is created with:
- role_id (UUID, primary key)
- role_name (VARCHAR: "Admin" | "User" | "Read-Only" | "Security Officer")
- description (TEXT)
- tenant_id (UUID, for future multi-tenant role customization)

**And** a `permissions` table is created with:
- permission_id (UUID)
- permission_code (VARCHAR: e.g., "task:create", "task:read", "task:delete", "user:manage", "audit:read", "config:write")
- description (TEXT)
- resource (VARCHAR: "task", "user", "config", "audit")
- action (VARCHAR: "create", "read", "update", "delete", "manage")

**And** a `role_permissions` table is created (junction):
- role_id (UUID, foreign key)
- permission_id (UUID, foreign key)
- Unique constraint: (role_id, permission_id)

**And** the role-permission mappings are:

**Admin Role Permissions:**
- task:create, task:read, task:update, task:delete (all tasks in tenant)
- user:manage (assign roles, view all users)
- config:write (OIDC config, role mappings)
- audit:read (search & export logs)

**User Role Permissions:**
- task:create (own tasks)
- task:read (own or assigned tasks)
- task:update (own or assigned tasks)
- task:delete (own tasks)
- audit:read (own logs only - limited view)

**Read-Only Role Permissions:**
- task:read (all tasks in tenant)
- NO write permissions

**Security Officer Role Permissions:**
- audit:read (full audit logs)
- user:read (user list, no modification)
- NO config or write permissions

**Given** the role model is defined  
**When** data is loaded at startup  
**Then** the system seeds these roles into the database (idempotent - if already exist, skip)

**Requirements Met:** FR16, FR17, FR18, FR19, FR20, NFR5

---

### Story 5.2: API Layer - Role-Based Permission Enforcement Middleware

As a **Backend Developer**,
I want **NestJS guards to validate user permissions on every API request before processing**,
So that **permission checks are not forgotten and are consistently enforced**.

**Acceptance Criteria:**

**Given** an API endpoint is defined (e.g., POST /api/tasks)  
**When** the endpoint is decorated with `@RequirePermission("task:create")`  
**Then** the NestJS middleware automatically:
1. Extracts user identity from access_token (tenant_id, roles)
2. Retrieves user's roles from JWT or database cache
3. Looks up all permissions for those roles
4. Checks if any of user's roles grant the required permission
5. If granted: proceeds with endpoint logic
6. If denied: returns HTTP 403 Forbidden with generic message
7. Logs: action="permission_check", endpoint={endpoint}, user_id={user_id}, permission={required}, granted={true/false}

**And** the decorator is used as:
```typescript
@Post('tasks')
@RequirePermission('task:create')
async createTask(@Body() dto: CreateTaskDto): Promise<TaskResponse> {
  // Logic here
}
```

**And** permission check pattern for multi-tenant:
1. Extract tenant_id from request user (immutable from session)
2. Extract tenant_id from resource being accessed (if exists)
3. Compare: request.user.tenant_id === resource.tenant_id
4. If mismatch: return HTTP 403, log denied access
5. Check permission: user.roles.any(role.permissions.includes(required_permission))
6. If false: return HTTP 403, log denied access

**Given** a user lacks permission  
**When** the guard rejects the request  
**Then** the response is:
```json
{
  "statusCode": 403,
  "message": "Forbidden",
  "error": "Access denied"
}
```

**And** NO indication of whether the resource exists or belongs to another tenant (prevents info leakage)

**And** audit log entry is created: action="permission_denied", user_id={user_id}, endpoint={endpoint}, resource_id={resource_id}, reason="insufficient_permissions"

**Requirements Met:** FR21, FR22, FR23, NFR5, NFR21

---

### Story 5.3: Manual User Creation Without OIDC (Pre-Onboarding)

As an **Admin**,
I want **to manually create user accounts without requiring OIDC, for use cases where employees haven't been provisioned in IdP yet**,
So that **I can onboard users quickly and generate temporary passwords**.

**Acceptance Criteria:**

**Given** an admin navigates to user management and clicks "Create User Manually"  
**When** the form opens  
**Then** form fields appear:
- Email address (required, must be unique in tenant)
- Full Name (required)
- Role (dropdown: Admin | User | Read-Only | Security Officer)
- Temporary Password: ☐ Generate Random | ○ Set Custom
- Buttons: "Create User" | "Cancel"

**And** when admin enters data and clicks "Create User":
1. Backend validates:
   - Email is valid format and not already in tenant
   - Role is one of 4 valid roles
   - Password (if custom) meets security requirements
2. If validation passes:
   - Creates user record with:
     - email
     - name
     - tenant_id (admin's own tenant)
     - role_id (selected role)
     - is_oidc_user = false
     - is_mfa_enrolled = false (must enroll on first login)
     - password_hash = bcrypt(generated_or_custom_password)
     - created_by = admin_id
   - Generates temporary password (if random generation selected)
   - Sends email to user: "Your account has been created. Use password [temp_pwd] on first login. You'll be prompted to change it."
   - Logs: action="user_created_manual", admin_id={admin_id}, user_id={new_user_id}, email={email}
   - Displays: "✅ User created. Temporary password sent to email."

**And** if validation fails:
- Displays error message with specific issue
- Form is not submitted

**Given** a user created manually logs in for the first time  
**When** they enter their email and temporary password  
**Then** the system:
1. Authenticates with password
2. Forces password change: "You must set a new password"
3. After password set: forces MFA enrollment (Story 4.1)
4. (No OIDC redirect; password-based login instead)

**Requirements Met:** FR35, NFR14, NFR1

---

### Story 5.4: Role Assignment & Modification

As an **Admin**,
I want **to assign or change a user's role with full audit visibility**,
So that **I can manage access control and track changes**.

**Acceptance Criteria:**

**Given** an admin views a user detail page  
**When** the page loads  
**Then** current role is displayed: "Current Role: Admin"  
**And** a dropdown appears to change role

**Given** admin changes role from "Admin" to "User"  
**When** admin clicks "Update Role"  
**Then** confirmation dialog appears:
- "⚠️ You are changing this user's role from Admin to User"
- "Reason for change:" (dropdown: Offboarding | Department Change | Permission Adjustment | Other)
- Buttons: "Cancel" | "Confirm Role Change"

**Given** admin confirms  
**When** the change is processed  
**Then** the system:
1. Updates user_roles table (or user.role_id)
2. Invalidates any cached permissions for that user
3. Creates audit log: action="user_role_changed", admin_id={admin_id}, user_id={user_id}, old_role="Admin", new_role="User", reason={reason}
4. If user is currently logged in: invalidates all refresh tokens (logs them out, forces re-login with new permissions)
5. Sends email to user: "Your role was changed from Admin to User on [date] by [admin_name]."
6. Displays: "✅ Role updated and audit logged."

**And** the audit log includes full trail:
- WHO (admin_id, admin_email)
- WHAT (old role, new role)
- WHEN (timestamp)
- WHY (reason from dropdown)
- OUTCOME (success)

**Requirements Met:** FR16, NFR14, NFR5, NFR21

---

### Story 5.5: User Management Dashboard - List & Filter

As an **Admin**,
I want **to view all users in my tenant with their roles, last login, and MFA status**,
So that **I can manage team access and identify inactive users**.

**Acceptance Criteria:**

**Given** an admin navigates to the user management page  
**When** the page loads  
**Then** a table displays all users in the tenant with columns:
- User Name (clickable to view details)
- Email
- Role (Admin | User | Read-Only | Security Officer)
- Last Login (timestamp or "Never")
- MFA Status (✅ Active | ⏳ Pending | ❌ Not Enrolled)
- Actions (View Details, Change Role, Disable, Delete)

**And** filtering options appear:
- Filter by Role (checkboxes: Admin, User, Read-Only, Security Officer)
- Filter by MFA Status (checkboxes)
- Filter by Last Login (Last 7 days | Last 30 days | Never)
- Search by Name or Email (text input)

**And** sorting: Click column header to sort (Name, Last Login, etc.)

**And** pagination: 25 users per page, with page selector

**Given** admin applies filters (e.g., "Role = Admin")  
**When** filters are applied  
**Then** table updates instantly showing only Admin-role users  
**And** result count updates: "5 of 150 users match your filters"

**Given** admin clicks on a user row  
**When** user detail page opens  
**Then** details display:
- Full Name, Email, Role
- Last Login: [timestamp]
- MFA Status: [status]
- Account Created: [timestamp]
- Created By: [admin_name] (if manual creation)
- Actions: Change Role, Disable User, Delete User, Generate Temporary Password, Logout User

**And** logs: action="user_list_viewed", admin_id={admin_id}, filters_applied={filters}, result_count={count}

**Requirements Met:** FR34, NFR5, NFR21

---

### Story 5.6: Admin Actions - User Disabling, Logout, & Force Password Reset

As an **Admin**,
I want **to disable users, force logout, or reset passwords for security or offboarding**,
So that **I can quickly respond to security incidents or manage offboarding**.

**Acceptance Criteria:**

**Given** admin clicks "Disable User" on a user detail page  
**When** confirmation dialog appears  
**Then** dialog shows:
- "⚠️ This will prevent the user from logging in"
- Checkbox: "☐ Also revoke existing sessions (log them out immediately)"
- Buttons: "Cancel" | "Disable User"

**Given** admin confirms  
**When** user is disabled  
**Then** the system:
1. Sets user.is_active = false
2. If checkbox selected: invalidates all refresh tokens for user (force logout)
3. Future login attempts with valid OIDC or password are rejected: HTTP 401 "User account is disabled"
4. Logs: action="user_disabled", admin_id={admin_id}, user_id={user_id}, revoke_sessions={true/false}
5. Sends email to user: "Your account has been disabled as of [date]."

**Given** admin clicks "Logout User" (on enabled user)  
**When** clicked  
**Then** the system:
1. Invalidates all refresh tokens for the user
2. User is logged out from all devices immediately
3. Next login attempt requires full OIDC or password re-authentication
4. Logs: action="admin_logout", admin_id={admin_id}, user_id={user_id}

**Given** admin clicks "Generate Temporary Password"  
**When** clicked  
**Then** the system:
1. Generates random 12-character password meeting complexity requirements
2. Sends email to user with temporary password
3. On next login: user must change password
4. Logs: action="temporary_password_generated", admin_id={admin_id}, user_id={user_id}

**Requirements Met:** FR23, NFR5, NFR21

---

### Story 5.7: Admin Dashboard - Authentication & Access Overview

As an **Admin**,
I want **to see a dashboard with real-time statistics on authentication status, active sessions, failed logins, and MFA compliance**,
So that **I can monitor system health and identify security issues**.

**Acceptance Criteria:**

**Given** admin navigates to the admin dashboard  
**When** the dashboard loads  
**Then** it displays:

**Section 1: Real-Time Statistics (auto-refreshes every 30s)**
- Active Sessions Right Now: 47 users
- Users Logged In Last 24h: 142 of 150
- Failed Login Attempts Last 24h: 3
- MFA Enrollment: 145/150 (96.7%) ✅

**Section 2: Charts**
- Login Activity (line chart): X-axis = time of day, Y-axis = login count
- User Roles Distribution (pie chart): Admin (3), User (120), Read-Only (15), Security Officer (12)

**Section 3: Recent Activity**
- Timestamp | Action | User | Status
- 2026-05-03 14:22 | Login | alice@acme.com | ✅ Success
- 2026-05-03 14:15 | MFA Verification | bob@acme.com | ✅ Success
- 2026-05-03 14:10 | Role Changed | charlie@acme.com | ✅ Admin → User

**Section 4: Alerts & Warnings**
- ⚠️ 5 users have not enrolled MFA (3.3%). [View] [Notify]
- ⚠️ 3 users have not logged in for 30 days (potential offboarding). [View]
- 🔒 Last security audit: 2 days ago

**And** all statistics are scoped to admin's tenant (multi-tenant isolation)

**And** logs: action="admin_dashboard_viewed", admin_id={admin_id}

**Requirements Met:** FR33, NFR4, NFR5, NFR21

---

### Story 5.8: Permission Preview & RBAC Testing

As an **Admin**,
I want **to preview what permissions a user will have with a selected role before assigning it**,
So that **I can understand permission implications and avoid misconfiguration**.

**Acceptance Criteria:**

**Given** admin is assigning a role to a user  
**When** admin clicks "Preview Permissions" button  
**Then** a modal displays:

**Modal: "Permissions for [Role Name]"**
- Description of the role
- Table of permissions:
  - Resource | Action | Permission Code
  - Task | Create, Read, Update | task:create, task:read, task:update
  - Task | Delete | task:delete
  - User | Manage | user:manage
  - Config | Write | config:write
  - Audit | Read | audit:read

**And** for each resource row:
- Green checkmark ✅ = User will have this permission
- Red X ❌ = User will NOT have this permission

**And** examples shown for each permission:
- "task:create" → User can create new tasks
- "user:manage" → User can assign roles and manage team
- "audit:read" → User can search and export audit logs

**And** warning if assigning Admin role: "⚠️ Admin role has full access including config and user management. Assign carefully."

**Given** admin clicks "I Understand, Assign Role"  
**When** clicked  
**Then** role is assigned as normal (Story 5.4)

**Requirements Met:** FR32, NFR5

---

## Epic 6: User Account Lifecycle & Data Deletion

**Goal:** Enable employees to request account deletion and admins to offboard users with immutable audit trails; ensure GDPR compliance.

**FRs Covered:** FR45, FR46, FR47  
**NFRs Supported:** NFR12, NFR24, NFR25  
**Architectural Alignment:** Immutable audit logging, GDPR compliance, no data recovery possible

---

### Story 6.1: Self-Service Account Deletion Request

As an **Employee**,
I want **to request permanent deletion of my account and all associated data via self-service**,
So that **I have control over my personal data in compliance with GDPR**.

**Acceptance Criteria:**

**Given** an authenticated employee navigates to account settings / "Delete Account"  
**When** the page loads  
**Then** a warning banner displays:
- "⚠️ WARNING: Account deletion is permanent and irreversible"
- "All your data will be deleted: tasks, documents, activity history"
- "You will lose immediate access to your account"

**And** a form appears:
- "Reason for deletion:" (dropdown: Privacy Concern | No Longer Needed | Other)
- Checkbox: "☐ I understand my data will be permanently deleted"
- Checkbox: "☐ I have downloaded/backed up any data I need"
- Text input: "Type 'DELETE' to confirm"
- Button: "Delete My Account Permanently"

**Given** user fills form correctly and clicks "Delete"  
**When** deletion is initiated  
**Then** the system:
1. Validates all checkboxes are checked and confirmation text = "DELETE"
2. Creates deletion_request record:
   - user_id, tenant_id, deletion_type="self_service", reason={selected}, requested_at=now(), status="pending"
3. Sends confirmation email: "Account deletion requested. Reply to confirm within 24 hours."
4. Logs: action="deletion_request_created", user_id={user_id}, type="self_service"
5. Displays: "✅ Deletion request submitted. Check your email to confirm."

**Given** user has 24-hour grace period  
**When** 24 hours pass without confirmation  
**Then** deletion_request expires and user can log in normally  
**And** logs: action="deletion_request_expired", user_id={user_id}

**Given** user clicks confirmation link in email within 24 hours  
**When** link is clicked  
**Then** the system:
1. Updates deletion_request status="confirmed"
2. Schedules deletion to occur within 24 hours (gives brief window for emergency cancellation)
3. Logs: action="deletion_request_confirmed", user_id={user_id}
4. Sends final email: "Your account will be permanently deleted in 24 hours. Reply immediately to cancel."

**Requirements Met:** FR45, NFR12, NFR24, NFR25

---

### Story 6.2: Admin-Initiated Account Deletion for Offboarding

As an **Admin**,
I want **to delete a user's account and data directly during offboarding**,
So that **I can immediately remove access for departing employees**.

**Acceptance Criteria:**

**Given** admin clicks "Delete User" on a user detail page  
**When** confirmation dialog appears  
**Then** dialog shows:
- "⚠️ PERMANENT DELETION: User account and all data will be deleted"
- "Reason:" (dropdown: Offboarding | Restructuring | Compliance | Other)
- "Effective Date:" (date picker, default = today)
- Checkbox: "☐ I confirm this user has been offboarded from the organization"
- Buttons: "Cancel" | "Delete User"

**Given** admin confirms  
**When** deletion is processed  
**Then** the system immediately:
1. Marks all user's sessions as inactive (forces logout)
2. Creates deletion record: admin_id={admin_id}, user_id={user_id}, deletion_type="admin_initiated", reason={reason}, created_at=now()
3. Queues data deletion task (asynchronous, occurs within 1 hour)
4. Logs: action="deletion_initiated_by_admin", admin_id={admin_id}, user_id={user_id}, reason={reason}
5. Displays: "✅ User deletion initiated. Data will be purged within 1 hour."

**And** during the 1-hour window: User cannot log in (account marked for deletion)

**And** deletion task executes:
1. Permanently deletes user record from users table
2. Deletes all user's tasks, documents, sessions
3. Deletes all personal data associated with user
4. Creates immutable deletion audit log entry (cannot be deleted)

**Requirements Met:** FR46, FR47, NFR12, NFR24, NFR25

---

### Story 6.3: Immutable Data Deletion Audit Trail

As a **Security Officer**,
I want **to verify that deleted users' data was actually deleted and see immutable proof**,
So that **I can attest to data deletion for GDPR compliance audits**.

**Acceptance Criteria:**

**Given** a security officer views the "Data Deletion Log"  
**When** the page loads  
**Then** a table displays all historical deletions:
- Deleted User Email (masked: ****@acme.com)
- Deletion Type (Self-Service | Admin-Initiated)
- Deletion Reason
- Initiated By (admin name if admin-initiated)
- Deletion Date (timestamp)
- Data Deletion Complete Date (timestamp)
- Audit Log Entry ID (reference to immutable audit record)

**And** deletion log entries are themselves immutable:
- Stored in deletion_audit_log table (INSERT-only, no UPDATE/DELETE)
- Signed with HMAC-SHA256 (tampering detection)
- Verified on display with signature validation

**Given** security officer clicks on a deletion entry  
**When** details page opens  
**Then** it displays:
- Full audit trail of deletion:
  - Deletion requested: [timestamp]
  - Deletion confirmed: [timestamp]
  - Data deletion completed: [timestamp]
- List of data types deleted: Users, Tasks, Sessions, Audit Logs (personal only)
- Signature verification: ✅ Verified (immutable)
- Cannot be edited or deleted (enforced by database constraints)

**And** security officer can export this report for GDPR compliance:
- PDF export button
- CSV export button
- Report includes: deletion dates, data types, responsible admin/user, audit signatures

**Requirements Met:** FR47, NFR12, NFR25

---

## Epic 7: MFA Device Recovery & Emergency Access

**Goal:** Enable help desk to issue breakglass access codes for users locked out of MFA, allowing re-enrollment without security compromise.

**FRs Covered:** FR48, FR49, FR50, FR51, FR52, FR53  
**NFRs Supported:** NFR2, NFR6, NFR12, NFR13, NFR21  
**Architectural Alignment:** Breakglass tokens (15-min validity), audit logging, no MFA required during recovery

---

### Story 7.1: Help Desk Breakglass Code Generation

As a **Help Desk Agent**,
I want **to generate temporary breakglass access codes for users locked out of MFA**,
So that **I can help users regain access without requiring them to contact an admin**.

**Acceptance Criteria:**

**Given** a help desk agent has "breakglass_admin" role  
**When** they navigate to "Emergency Access" tool  
**Then** a form appears:
- "User Email or ID:" (text input with autocomplete)
- "Reason for access:" (dropdown: Lost Device | Forgotten MFA | Device Malfunction | Account Recovery)
- "Max duration (minutes):" (default 15, max 60)
- Button: "Generate Breakglass Code"

**Given** help desk agent enters user email and clicks "Generate"  
**When** generation is triggered  
**Then** the system:
1. Validates user exists and is active
2. Generates breakglass code: 8-digit alphanumeric (e.g., "BG7K9Q2M")
3. Creates breakglass record:
   - breakglass_id (UUID)
   - user_id, tenant_id
   - code_hash (SHA-256, not plaintext)
   - generated_by (help_desk_agent_id)
   - reason={selected}
   - valid_from=now()
   - valid_until=now() + X minutes
   - is_used=false
   - used_at=null
4. Displays code to agent: "⚠️ SHARE WITH USER ONLY: BG7K9Q2M"
5. Message: "Code is valid for 15 minutes. One-time use."
6. Logs: action="breakglass_code_generated", agent_id={agent_id}, user_id={user_id}, reason={reason}, validity_minutes=15

**And** agent has copy button to share code

**And** if agent tries to generate multiple codes for same user:
- Only ONE active code per user at a time
- Generating new code invalidates previous one
- Logs: action="breakglass_code_superseded", previous_code_id={id}

**Requirements Met:** FR48, NFR6, NFR12, NFR21

---

### Story 7.2: Breakglass Code Usage & 15-Minute Access Window

As a **Locked-Out Employee**,
I want **to use a breakglass code to access my account for 15 minutes without MFA**,
So that **I can immediately re-enroll my MFA device**.

**Acceptance Criteria:**

**Given** an employee receives breakglass code from help desk  
**When** they navigate to login page with code parameter: `/login?breakglass={code}`  
**Then** a form appears:
- "Enter your email address:"
- "Enter your breakglass code:"
- Button: "Login with Breakglass"

**Given** user enters email and code  
**When** they click "Login with Breakglass"  
**Then** the system:
1. Validates code exists and is active (not expired, not used)
2. Validates email matches user_id associated with code
3. Validates code is within 15-minute window: valid_from <= now() <= valid_until
4. If validation passes:
   - Creates session with special flag: breakglass_session=true, breakglass_valid_until=now() + 15 minutes
   - Issues access_token with claim: "breakglass=true"
   - Logs: action="breakglass_session_created", user_id={user_id}, code_id={breakglass_id}
   - Displays: "✅ Breakglass access granted. Valid for 15 minutes. Please re-enroll MFA immediately."
   - Redirects user to MFA re-enrollment page (Story 7.3)
5. If validation fails: displays generic error "Invalid code or email"

**Given** user has breakglass_session=true  
**When** 15 minutes pass  
**Then** the access_token expires  
**And** user is logged out automatically  
**And** logs: action="breakglass_session_expired", user_id={user_id}

**Given** user attempts to access API endpoints with breakglass_session=true but invalid code  
**When** endpoint is called  
**Then** request is rejected if breakglass_valid_until < now()

**Requirements Met:** FR49, NFR6, NFR12, NFR21

---

### Story 7.3: MFA Re-Enrollment During Breakglass Access

As a **Locked-Out Employee**,
I want **to quickly set up a new MFA device while using breakglass access**,
So that **I can regain normal access before the 15-minute window closes**.

**Acceptance Criteria:**

**Given** user has breakglass_session active and is on MFA re-enrollment page  
**When** the page loads  
**Then** an urgent banner displays:
- "⚠️ URGENT: You have 14:32 minutes remaining to re-enroll MFA"
- (Countdown timer that updates every second)

**And** the MFA enrollment flow is presented (same as Story 4.1):
1. QR code with provisioning URI
2. Instructions to scan with authenticator app
3. Input field for 6-digit code
4. "Verify & Enable MFA" button

**Given** user scans QR code and enters code  
**When** code is verified  
**Then** the system:
1. Activates new MFA device (marks old as inactive)
2. Generates backup codes (Story 4.2)
3. Logs: action="mfa_reenrolled_via_breakglass", user_id={user_id}, breakglass_code_id={id}
4. Displays: "✅ MFA successfully re-enrolled. You can now log out and log in normally."
5. Creates new normal session (not breakglass) with mfa_verified=true
6. Marks breakglass_code as used (is_used=true, used_at=now())

**Given** 15-minute window expires before user finishes MFA re-enrollment  
**When** countdown reaches 0:00  
**Then** the page displays: "⏱️ Time expired. Log in again to continue."  
**And** breakglass_session is invalidated  
**And** user is logged out

**Requirements Met:** FR51, FR52, NFR13, NFR6

---

### Story 7.4: Breakglass Access Audit Logging & Compliance

As a **Security Officer**,
I want **to see complete audit trail of all breakglass code generation and usage**,
So that **I can detect misuse and ensure emergency access is used appropriately**.

**Acceptance Criteria:**

**Given** a security officer navigates to "Breakglass Audit Log"  
**When** the page loads  
**Then** a table displays all breakglass events:
- Timestamp (when code was generated or used)
- Event Type (Generated | Used | Expired)
- User Email (user who received access)
- Generated By (help desk agent name and ID)
- Code Hash (first 4 chars: "BG7K...")
- Reason
- Duration (e.g., "15 minutes")
- Status (✅ Used | ⏳ Active | ❌ Expired)
- Used By IP (if used)
- Used Duration (how long until actual use)

**And** searchable/filterable by:
- Date range
- User email
- Help desk agent
- Reason category

**Given** security officer clicks on a breakglass event  
**When** details page opens  
**Then** it displays complete timeline:
- Generated: 2026-05-03 14:22:15 by agent "john.support@acme.com"
- Valid window: 14:22:15 - 14:37:15 (15 minutes)
- Used: 2026-05-03 14:25:43 by user from IP 192.168.1.100
- MFA re-enrolled: 2026-05-03 14:33:01
- Status: ✅ Completed successfully

**And** can export report for compliance: "Show me all breakglass usage for last 30 days"

**And** warning if suspicious pattern detected (same user multiple codes in short time, etc.)

**And** logs: action="breakglass_audit_viewed", security_officer_id={id}

**Requirements Met:** FR50, NFR12, NFR21

---

## Epic 8: Audit Logging & Compliance

**Goal:** System logs all authentication and admin events immutably; Security Officers can search, filter, and export logs for SOC 2 compliance investigations.

**FRs Covered:** FR36, FR37, FR38, FR39, FR40  
**NFRs Supported:** NFR3, NFR12, NFR21, NFR24, NFR25, NFR27, NFR29  
**Architectural Alignment:** Immutable INSERT-only tables, HMAC-SHA256 signatures, 12-month retention with auto-purge

---

### Story 8.1: Immutable Audit Log Table & Event Schema

As a **System Architect**,
I want **an audit log table that is INSERT-only with cryptographic signatures to ensure immutability**,
So that **audit logs cannot be tampered with and will withstand compliance scrutiny**.

**Acceptance Criteria:**

**Given** the audit logging schema is designed  
**When** database schema is created  
**Then** an `audit_logs` table is created with:
- `id` (UUID, primary key)
- `tenant_id` (UUID, NOT NULL, indexed for fast query)
- `actor_id` (UUID, user who performed action)
- `action` (VARCHAR, e.g., "login", "mfa_enrolled", "role_assigned", "user_created", "config_changed")
- `resource_type` (VARCHAR, e.g., "user", "config", "session", "mfa_device")
- `resource_id` (UUID, affected resource)
- `outcome` (VARCHAR, "success" | "failed" | "denied")
- `details` (JSON, contextual data like old_value, new_value, ip_address, user_agent, device_fingerprint)
- `created_at` (TIMESTAMP NOT NULL, indexed)
- `signature` (VARCHAR, HMAC-SHA256 of entire record + secret_key)
- `signature_verified` (BOOLEAN, true = signature validated, false = tampering detected)

**And** database constraints:
- NO UPDATE trigger (table is INSERT-only)
- NO DELETE capability (cannot delete audit logs)
- Composite index on (tenant_id, created_at) for fast search
- Composite index on (tenant_id, actor_id, created_at) for user activity

**And** every action that occurs in the system logs an audit_log entry:
- Authentication: login_success, login_failed, login_via_breakglass, logout, session_refresh, mfa_verification
- Users: user_created, user_role_changed, user_disabled, user_deleted, user_profile_updated
- MFA: mfa_enrolled, mfa_device_changed, mfa_recovery_code_used, mfa_unenrolled
- Config: oidc_config_updated, role_mapping_changed, config_validated, config_activated
- Admin Actions: admin_logout_user, admin_user_delete_initiated, admin_role_assigned, breakglass_code_generated

**And** logs: action="audit_table_created", status="ready_for_logging"

**Requirements Met:** FR36, FR37, FR38, NFR12, NFR21, NFR24, NFR25, NFR27

---

### Story 8.2: Audit Event Capture - WHO, WHAT, WHEN, WHERE, OUTCOME

As a **Backend Service**,
I want **every significant event to be captured with full context (who, what, when, where, why, outcome)**,
So that **security officers can reconstruct any sequence of events**.

**Acceptance Criteria:**

**Given** a user logs in successfully  
**When** login completes  
**Then** an audit_log entry is created:
```json
{
  "tenant_id": "acme-corp",
  "actor_id": "user-123",
  "action": "login_success",
  "resource_type": "session",
  "resource_id": "session-456",
  "outcome": "success",
  "details": {
    "idp_type": "okta",
    "mfa_used": true,
    "ip_address": "192.168.1.100",
    "user_agent": "Mozilla/5.0...",
    "device_fingerprint": "abc123def456",
    "session_id": "session-456",
    "timestamp_iso": "2026-05-03T14:22:15Z"
  },
  "created_at": "2026-05-03T14:22:15Z",
  "signature": "sha256_hmac(...)"
}
```

**And** when a failed login is attempted:
```json
{
  "action": "login_failed",
  "outcome": "failed",
  "details": {
    "failure_reason": "invalid_mfa_code",
    "attempt_number": 2,
    "ip_address": "192.168.1.100",
    "user_email": "user@acme.com",  // Note: never store sensitive auth data
    "locked_until": "2026-05-03T14:27:15Z"  // If rate-limited
  }
}
```

**And** when a role is changed by admin:
```json
{
  "action": "user_role_changed",
  "outcome": "success",
  "details": {
    "admin_id": "admin-456",
    "user_id": "user-123",
    "old_role": "User",
    "new_role": "Admin",
    "change_reason": "Promotion",
    "ip_address": "10.0.0.50"
  }
}
```

**And** when permission is denied:
```json
{
  "action": "permission_denied",
  "outcome": "denied",
  "details": {
    "user_id": "user-123",
    "required_permission": "task:delete",
    "user_roles": ["User"],
    "requested_resource": "task-789",
    "ip_address": "192.168.1.100"
  }
}
```

**And** every field is consistently captured with WHO (actor_id), WHAT (action), WHEN (created_at), WHERE (ip_address, device_fingerprint), WHY (reason/details), OUTCOME (success/failed/denied)

**Requirements Met:** FR36, FR37, NFR3, NFR12, NFR21

---

### Story 8.3: HMAC-SHA256 Signature & Tampering Detection

As a **Security Officer**,
I want **to verify audit log signatures and detect any tampering**,
So that **I can prove logs have not been altered for compliance audits**.

**Acceptance Criteria:**

**Given** an audit log entry is created  
**When** the entry is written to database  
**Then** the system:
1. Serializes the log entry (deterministic JSON order)
2. Computes HMAC-SHA256: `signature = HMAC-SHA256(serialized_log + secret_key)`
3. Stores signature in the log entry
4. Logs the logging itself: action="audit_log_entry_created" (nested meta-audit)

**And** the secret_key is:
- Stored in secure vault (not in code or database)
- Rotated monthly
- Old keys kept for 12 months to verify historic logs

**Given** a security officer views audit logs  
**When** logs are displayed  
**Then** for each entry:
1. System recomputes HMAC-SHA256 using stored secret_key
2. Compares computed signature to stored signature
3. If match: displays ✅ "Verified - Log integrity confirmed"
4. If mismatch: displays 🚨 "TAMPERED - Log integrity compromised" in red
5. Logs verification: action="audit_log_signature_verified", log_id={id}, verified=true/false

**And** if ANY logs are detected as tampered:
- Email alert sent to security officer: "Audit log tampering detected: X entries compromised"
- Flag logs as suspicious in UI (red highlight)
- Retain logs for investigation (do not delete)

**And** when keys are rotated monthly:
1. New secret_key is generated
2. All existing logs are re-verified with old keys
3. Any verification failures trigger immediate security alert
4. Logs: action="audit_key_rotated", old_key_id={id}, new_key_id={id}, verification_status="{count_passed}/{count_total}"

**Requirements Met:** FR38, NFR12, NFR21, NFR27

---

### Story 8.4: Audit Log Search & Filtering Interface

As a **Security Officer**,
I want **to search and filter audit logs by date, user, action, resource, and outcome for investigation**,
So that **I can quickly find relevant events during security incidents**.

**Acceptance Criteria:**

**Given** a security officer navigates to the audit log viewer  
**When** the page loads  
**Then** a search interface appears with:

**Search Filters:**
- Date Range (From / To date pickers)
- Actor (User Email autocomplete dropdown)
- Action (Multi-select: login_success, login_failed, mfa_enrolled, role_changed, etc.)
- Resource Type (Multi-select: user, session, config, mfa_device)
- Outcome (Checkboxes: ✅ Success | ❌ Failed | ❌ Denied)
- IP Address (text input)
- Free Text Search (searches across all text fields)

**And** results display in table:
- Timestamp
- Actor (user email)
- Action (human-readable)
- Resource (type + ID)
- Outcome (✅/❌/❌)
- Details (expandable to view full JSON)
- Signature Status (✅ Verified | 🚨 Tampered)

**And** sorting by any column (Timestamp, Actor, Action, etc.)

**And** pagination: 50 entries per page

**Given** security officer enters date range "Last 7 days" and action "login_failed"  
**When** filters are applied  
**Then** results show only failed logins from the last 7 days  
**And** count shows: "47 failed login attempts"

**Given** security officer clicks an entry row  
**When** details panel opens  
**Then** it displays full JSON with all fields:
- Tenant, Actor, Action, Resource, Outcome
- Detailed metadata (IP, device fingerprint, user agent, reason)
- Timestamp with ISO format
- Signature verification status

**And** buttons to:
- Copy JSON
- View related events (same user, same resource)
- Export as CSV/JSON

**Requirements Met:** FR39, NFR3, NFR21

---

### Story 8.5: Audit Log Export for Compliance & Investigation

As a **Security Officer**,
I want **to export audit logs in CSV or JSON format for external compliance audits and forensic investigation**,
So that **I can provide audit evidence to external auditors and legal team**.

**Acceptance Criteria:**

**Given** a security officer has filtered audit logs (e.g., "Last 30 days")  
**When** they click "Export" button  
**Then** a dropdown appears:
- "Export as CSV"
- "Export as JSON"
- "Export as PDF Report"

**Given** user clicks "Export as CSV"  
**When** export is initiated  
**Then** the system:
1. Retrieves all matching entries (respecting current filters)
2. Converts to CSV format with columns:
   - Timestamp, Actor ID, Actor Email, Action, Resource Type, Resource ID, Outcome, IP Address, Device Fingerprint, Reason, Signature Verified
3. Streams file download: `audit_logs_2026-05-03.csv`
4. Logs: action="audit_logs_exported", security_officer_id={id}, export_format="csv", entry_count={count}

**Given** user clicks "Export as JSON"  
**When** export is initiated  
**Then** system exports full JSON array with all fields:
```json
[
  {
    "timestamp": "2026-05-03T14:22:15Z",
    "actor_id": "user-123",
    "action": "login_success",
    "details": {...},
    "signature_verified": true
  },
  ...
]
```

**And** file: `audit_logs_2026-05-03.json`

**Given** user clicks "Export as PDF Report"  
**When** export is initiated  
**Then** system generates formatted PDF:
- Report Title: "Audit Log Report"
- Date Range, Filters Applied
- Summary Statistics (X logins, Y role changes, Z failures)
- Table of entries (first 100, with note if more exist)
- Signature verification status
- Export timestamp and exported-by user

**And** File: `audit_log_report_2026-05-03.pdf`

**And** all exports include header: "Report generated: {timestamp}, Exported by: {user_email}, For: {tenant_name}"

**Requirements Met:** FR40, NFR3, NFR21, NFR27

---

### Story 8.6: Audit Log Retention & Auto-Purge After 12 Months

As a **System**,
I want **to automatically purge audit logs older than 12 months while creating immutable purge records**,
So that **we comply with retention policies and comply with GDPR "right to be forgotten" for old data**.

**Acceptance Criteria:**

**Given** automated purge process runs (daily at 2 AM UTC)  
**When** process executes  
**Then** the system:
1. Queries audit_logs WHERE created_at < (now() - 12 months)
2. Counts entries to be purged
3. Verifies signatures on all entries (last chance to detect tampering)
4. Creates immutable purge_audit_log record (separate table):
   - purge_timestamp = now()
   - entries_purged = {count}
   - date_range_purged = {start_date} to {end_date}
   - signature_verification_status = "{X passed}/{X total} verified"
   - purge_reason = "retention_policy_12_months"
5. Deletes matching entries from audit_logs (cascade delete)
6. Logs: action="audit_logs_purged", entries_deleted={count}, purge_record_id={id}

**And** purge_audit_log entries are themselves permanent and immutable (INSERT-only table)

**And** security officer can view purge history to verify retention compliance:
- "Latest purge: 2026-05-01, 1,247 entries deleted"
- Report shows audit log coverage: "Logs available: 2025-05-03 to 2026-05-03"

**And** if any log signature verification failed during purge:
- Purge is HALTED
- Alert sent to security officer: "Cannot purge audit logs - tampering detected in logs to be purged"
- Logs: action="audit_logs_purge_failed", reason="signature_verification_failed", tampering_detected=true

**Requirements Met:** FR38, NFR12, NFR24, NFR25

---

### Story 8.7: Audit Log Compliance Dashboard

As a **Security Officer**,
I want **to see a compliance dashboard showing audit log coverage, retention status, tampering detection**,
So that **I can verify readiness for SOC 2 or similar compliance audits**.

**Acceptance Criteria:**

**Given** a security officer navigates to "Compliance Dashboard"  
**When** the dashboard loads  
**Then** it displays:

**Section 1: Audit Log Coverage**
- Logs Available: "2025-05-03 to 2026-05-03 (365 days)"
- Total Entries: "1,247,582"
- Entries Per Day (avg): "3,419"

**Section 2: Retention Status**
- Retention Policy: "12 months"
- Last Purge: "2026-05-01, 1,247 old entries deleted"
- Next Scheduled Purge: "2026-06-01"
- Data Coverage: "100% (no gaps)"

**Section 3: Integrity Status**
- ✅ All signatures verified (1,247,582 / 1,247,582)
- Last verification: "Today at 2:15 AM"
- Tampering Detected: ❌ None
- Alert Level: ✅ All Clear

**Section 4: Event Distribution (Chart)**
- Pie chart: % of logs by action type
- Login events: 60%
- Admin actions: 15%
- MFA events: 15%
- Other: 10%

**Section 5: Compliance Readiness**
- ✅ SOC 2 Type II: Audit logs sufficient (365 days required, have 365)
- ✅ GDPR Right to Be Forgotten: Purge process automated
- ✅ Immutable Logging: All logs cryptographically signed
- ✅ Real-time Availability: Logs searchable and exportable

**And** buttons:
- "Export Compliance Report for External Auditors"
- "View Recent Events"
- "Verify Log Integrity Now"

**Requirements Met:** FR36-FR40, NFR3, NFR12, NFR21, NFR24, NFR25, NFR27, NFR29

---

**✅ Epic 1 Complete:** 4 stories created, all 6 FRs covered, stories are independent and non-blocking.

---

Now proceeding to **Epic 2: Admin OIDC Configuration & Validation**.

### **EPIC 2: Admin OIDC Configuration & Validation**

**Epic Goal:** Enable IT admins to configure their corporate identity provider (Okta, Azure AD, etc.) with step-by-step setup, automatic IdP discovery, role mapping, and validation testing.

**FRs Covered:** FR1, FR2, FR3, FR6, FR8, FR30, FR31, FR32  
**NFRs Supported:** NFR30, NFR31, NFR1

This epic creates the admin console experience for setting up OIDC, including the configuration wizard, automatic metadata discovery, and validation testing.

---

### **Proposed Story Breakdown for Epic 2:**

**Story 2.1: OIDC Configuration Data Model & Storage**
- Create database tables to store OIDC configurations (IdP metadata, client ID, secret, role mappings)
- Implement secure storage of secrets

**Story 2.2: OIDC Metadata Discovery & Validation**
- Implement automatic OIDC metadata discovery from standard well-known endpoint
- Validate OIDC configuration before saving

**Story 2.3: Admin OIDC Configuration UI Wizard (Step 1-2)**
- Build admin console page with step-by-step wizard
- Step 1: IdP Selection (Okta, Azure AD, Keycloak, etc.)
- Step 2: Metadata URL or manual endpoint entry

**Story 2.4: Admin OIDC Role Mapping UI Wizard (Step 3)**
- Build UI for mapping IdP groups/claims to todo-react roles
- Show preview of resulting permissions

**Story 2.5: OIDC Configuration Validation & Testing**
- Implement validation test that performs end-to-end OIDC flow
- Display test results (success/failure with diagnostic info)

**Story 2.6: Multiple IdP Support**
- Enable admins to configure multiple IdPs
- Allow employees to select IdP on login by domain or dropdown

---

**Harry, does this Epic 2 story structure work?**

- Are these stories appropriately sized?
- Should we split or combine any stories?
- Ready to proceed with detailed story creation?

Please respond to continue.
