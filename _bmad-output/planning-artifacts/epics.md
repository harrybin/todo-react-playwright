---
project_name: 'todo-react'
user_name: 'Harry'
date: '2026-05-03'
stepsCompleted: ['step-01-validate-prerequisites', 'step-02-design-epics']
inputDocuments: 
  - 'd:\harrybin\todo-react-playwright\_bmad-output\planning-artifacts\prd.md'
requirementsExtracted: true
extractionDate: '2026-05-03'
epicsApproved: true
approvalDate: '2026-05-03'
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
