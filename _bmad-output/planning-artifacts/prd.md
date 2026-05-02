---
stepsCompleted:
  - step-01-init
  - step-01b-continue.md
  - step-02-discovery
  - step-02b-vision
  - step-02c-executive-summary
  - step-03-success
  - step-04-journeys
  - step-05-domain-skipped
  - step-06-innovation-skipped
  - step-07-project-type
  - step-08-scoping
  - step-09-functional
releaseMode: single-release
inputDocuments: []
documentCounts:
  briefCount: 0
  researchCount: 0
  brainstormingCount: 0
  projectDocsCount: 0
classification:
  projectType: SaaS B2B
  domain: B2B SaaS - Workforce Identity & Access Management
  complexity: High
  projectContext: Brownfield
vision:
  differentiator: "Security-first enterprise authentication with mandatory MFA — no compromises, no opt-outs"
  coreInsight: "Enterprises are tired of identity and access management tools that make security optional or complex. We make security the default and unavoidable."
  problemSolved: "Permission management and access control — ensuring only the right people have access to the right resources with no ambiguity or technical escape routes"
  keyTheme: "Security as non-negotiable default"
workflowType: 'prd'
---

# Product Requirements Document - todo-react

**Author:** Harry
**Date:** 2026-05-02

## Executive Summary

**todo-react** is positioning for enterprise customers by introducing mandatory enterprise authentication with security-first architecture. The product solves a critical challenge: enabling only authorized personnel to access resources through unambiguous permissions with platform-level enforcement. This positions todo-react as a trusted platform for enterprises where security is mandatory, not configurable.

### What Makes This Special

**Security as the operating model:** Enterprise authentication is built on mandatory MFA and security controls that cannot be disabled or misconfigured. This contrasts with traditional IAM platforms where security is treated as a feature to configure, allowing teams to accidentally weaken protection through convenience choices.

**Core insight:** Enterprises are overwhelmed managing platforms where security is optional. They need vendors where security is the operating model itself. By enforcing MFA and permission boundaries at the platform level, todo-react becomes a trusted solution for enterprises managing workforce access governance.

## Project Classification

| Attribute | Value |
|---|---|
| **Project Type** | SaaS B2B |
| **Domain** | Workforce Identity & Access Management |
| **Complexity** | High |
| **Project Context** | Brownfield (extending existing todo-react app) |

## Success Criteria

### User Success

**Sustained >95% MFA usage over 6 months:** Employees maintain consistent use of mandatory MFA authentication without seeking exemptions or workarounds. This metric validates that the MFA user experience is tolerable, security controls are trusted, and adoption is sustainable beyond initial rollout.

**Measurable outcome:** By month 6 post-launch, >95% of active employees continue to authenticate via MFA on every session, with <5% requesting exemptions or reporting persistent friction.

### Admin Success

**Zero errors on first OIDC setup attempt:** IT admins successfully configure enterprise OIDC authentication and role mapping without misconfiguration errors that require rework or engineering support. This validates setup UX quality and reduces time-to-value for enterprise customers.

**Measurable outcome:** When an admin completes OIDC IdP configuration and runs the validation test, configuration validation passes on the first attempt. First employee login via OIDC succeeds within the first 24 hours of admin setup completion.

### Business Success

**Support resolves 95% of auth-related questions without engineering escalation:** Support team can independently handle common auth issues (MFA device recovery, login failures, account linking) without escalating to engineering. This reduces operational burden and accelerates customer success.

**Measurable outcome:** Of all auth-related support tickets filed post-launch, ≥95% are resolved by support with zero engineering escalations. Engineering handles only protocol-level issues or security incidents, not operational support.

### Technical Success

**OIDC protocol compliance:** Authentication flows fully comply with OpenID Connect specification. All required OIDC endpoints (authorization, token, userinfo) function correctly with standard claims mapping.

**MFA reliability:** >99.5% uptime for MFA authentication service. Zero silent failures or lockouts due to MFA backend issues.

**Immutable audit logging:** All authentication and authorization events (successful login, failed MFA, role changes, permission grants) are logged with actor, timestamp, resource, and action. Logs are immutable and searchable by admin for security investigation.

## Product Scope

### MVP - Minimum Viable Product (Launch)

**OIDC Authentication:**
- OpenID Connect 1.0 protocol support
- Standard IdP metadata integration (any OIDC-compliant IdP: Okta, Azure AD, Google Workspace, etc.)
- Role claim mapping from IdP to todo-react application roles
- Session lifetime and refresh token management

**Mandatory MFA:**
- TOTP (Time-Based One-Time Password) via authenticator apps (Google Authenticator, Authy, Microsoft Authenticator)
- SMS fallback for MFA recovery
- MFA enforcement at platform level — no opt-out, no admin bypass

**Permission Model:**
- Role-based access control (RBAC) — admin, user, read-only roles
- Permission boundaries enforced on API layer and UI
- No cross-tenant access — tenant isolation at database query level

**Audit & Security:**
- Immutable auth event logging (login attempts, MFA events, role changes, access grants)
- Admin dashboard for viewing logs and investigating suspicious activity
- Breakglass/emergency access procedure for account recovery without bypassing security

**Admin Onboarding:**
- OIDC configuration wizard with step-by-step guidance
- Validation tests before go-live
- Role mapping UI for configuring IdP claims → app roles

### Growth Features (Post-MVP, Months 4-6)

**SAML 2.0 Support:**
- SAML 2.0 protocol for enterprises using legacy IdP systems
- Metadata-based and manual configuration
- Assertion encryption and signature validation

**SCIM Provisioning:**
- System for Cross-Domain Identity Management (SCIM 2.0)
- Automatic user and group provisioning from IdP
- Deprovisioning on user removal from IdP

**Advanced MFA:**
- WebAuthn/FIDO2 hardware key support
- Adaptive MFA (risk-based step-up authentication)
- SMS and email MFA options beyond TOTP

**Compliance & Reporting:**
- SOC 2 Type II readiness checklist
- Compliance-ready audit reports (access reviews, privilege escalation events)
- Data residency options (EU, US, custom)

### Vision (Future, 12+ months)

**Full IAM Platform:**
- Just-in-Time (JIT) provisioning for on-demand user creation
- Attribute-based access control (ABAC) for fine-grained permissions
- Policy engine for conditional access rules (IP-based, device-based, time-based)
- Single logout (SLO) across federated systems

**Advanced Security:**
- Zero-Trust architecture enforcement (device posture checks, network segmentation)
- Anomaly detection and automatic session termination
- Integration with threat intelligence feeds for suspicious activity detection

**Enterprise Features:**
- Multi-organization support (reseller/partner portals)
- Delegated admin roles (org admin without platform admin access)
- Tenant-specific branding for SSO login experience
- API-first architecture for partner integrations

## User Journeys

### Journey 1: IT Admin - OIDC Setup and First Day Operations

**Persona: Sarah, IT Security Administrator**

**Opening Scene:**
Sarah's company just signed up for todo-react's enterprise tier. She's been tasked with configuring OIDC authentication by end of day so the pilot team can test it tomorrow. She's done this with three other SaaS platforms—each time it was a three-hour ordeal with certificate errors, incorrect metadata, and frustrating support escalations. She's skeptical but hopeful.

**Rising Action:**
Sarah logs into the todo-react admin console and finds the OIDC configuration wizard. Step 1 asks for her IdP (she selects Okta). The system auto-populates the Okta metadata URL field and validates it immediately. Step 2 maps Okta groups to todo-react roles with a clear UI showing "Okta Group" → "todo-react Role." Sarah maps "engineering-team" to "admin" and "all-staff" to "user." The system shows a preview of what permissions each role will have.

She clicks "Validate Configuration" and gets an instant green checkmark: "✓ OIDC configuration is valid and ready."

**Climax:**
Sarah receives a test notification: "Test user successfully logged in via OIDC at 2:47 PM." No errors. No ambiguity. She didn't have to call support, debug certificate encoding, or iterate on role mappings. She did it right the first time.

**Resolution:**
By mid-afternoon, Sarah has configured OIDC, validated it with a test login, and set up role-based permissions. She's now monitoring the admin dashboard watching real employees from the pilot team successfully authenticate via SSO + MFA. For the first time with an IAM setup, she feels confident: security is enforced, roles are correct, and users aren't getting locked out. She messages her manager: "We're live. And I actually understood every step."

**Requirements Revealed:**
- OIDC configuration wizard with step-by-step validation
- Auto-discovery of standard IdP metadata
- Role mapping UI with permission preview
- Validation test before go-live
- Admin dashboard for monitoring live authentication

---

### Journey 2: Employee - First Login and Regular Usage

**Persona: Marcus, Software Engineer**

**Opening Scene:**
Marcus's company just enabled enterprise authentication. He received an email: "We've moved to SSO + MFA for security. Your first login will redirect you to our corporate identity provider." He's slightly annoyed—one more authentication step. He opens todo-react and clicks the "Log In" button.

**Rising Action:**
Marcus is redirected to Okta (his company's IdP). He logs in with his corporate username and password. Okta then asks for MFA: "Enter the code from your authenticator app." Marcus opens Google Authenticator, sees a 6-digit code, and enters it. The system validates instantly.

He's redirected back to todo-react. His session is created, and he's logged in. No friction, no confusion.

**Climax:**
Marcus opens his task list. All his existing todo-react data is still there, mapped to his corporate email. His permissions are automatically set based on his Okta group membership. He never sees a permission denied error because the system only shows him tasks he has access to. He can assign tasks to teammates, knowing that access control is enforced, not guessed at.

**Resolution:**
After his first login, Marcus continues to use todo-react as usual. Each time he logs in, the same flow happens—corporate SSO + MFA. After the first day, he doesn't think about the authentication layer anymore. What he notices is that his todo-react is now tied to his corporate identity, and only his team can see his tasks. Security feels automatic, not like a burden.

**Requirements Revealed:**
- Seamless OIDC redirect flow
- MFA enforcement on every session
- Automatic permission assignment based on IdP claims/groups
- Session management with refresh tokens
- Access control enforced on data retrieval (not just UI)

---

### Journey 3: Employee - MFA Device Loss and Recovery

**Persona: Jessica, Product Manager**

**Opening Scene:**
Jessica lost her phone yesterday. She has a new phone today and installed Google Authenticator, but her old TOTP codes won't work. She tries to log in to todo-react to finish a critical project update, and MFA blocks her: "Enter your authenticator code." She doesn't have it. She's stuck.

She calls the help desk, worried this will take hours to resolve.

**Rising Action:**
The help desk agent pulls up the admin dashboard, navigates to Jessica's account, and sees a "Breakglass Access" option. The help desk generates a temporary breakglass code and reads it to Jessica over the phone. Jessica enters the breakglass code and is logged in.

She's then guided to re-enroll in MFA with her new phone. The system gives her a QR code to scan with her authenticator app. Her new device is registered within 60 seconds.

**Climax:**
Jessica logs out and logs back in with her new authenticator device. Everything works. She's back in her todo-react, able to complete her project update. The recovery took 15 minutes, not the feared 2 hours.

**Resolution:**
Jessica experiences what security-first + operability-first looks like. The system didn't compromise security (breakglass access was temporary and logged), but it also didn't abandon her. She got help fast from support without needing engineering intervention. She's now more confident in the security—it's strict but not impossible to use.

**Requirements Revealed:**
- Breakglass/emergency access for MFA recovery
- Secure temporary access tokens for help desk use
- Simple MFA re-enrollment flow
- Audit logging of breakglass usage for security review
- Help desk tools that don't require engineering involvement

---

### Journey 4: Security Officer - Audit and Compliance Review

**Persona: David, IT Security and Compliance Officer**

**Opening Scene:**
David's company is pursuing SOC 2 Type II certification. The auditor is asking for identity and access logs: "Show us who accessed what, when, and from where. Prove that access controls are enforced and that admin actions are logged."

David needs to show comprehensive audit trails for todo-react. In many systems, he'd have to export CSVs and stitch together data from multiple places. He logs into todo-react's security dashboard.

**Rising Action:**
The dashboard shows an immutable audit log with:
- Timestamp of every login attempt (successful and failed)
- MFA events (enrolled, re-enrolled, bypass denied)
- Permission changes (role assignment, removal)
- Admin actions (configuration changes, user deprovisioning)
- Failed access attempts (unauthorized API calls)

Each log entry includes: WHO (actor), WHAT (action), WHEN (timestamp), WHERE (IP address/device), and OUTCOME (success/failure/reason).

David filters for "all admin role changes in the past 90 days" and exports the results as a searchable CSV. He also runs a compliance report template built into the system: "SOC 2 Control: Access Control." The report shows evidence that the system enforces RBAC, logs privilege escalation, and detects anomalous access patterns.

**Climax:**
David presents the audit logs and compliance report to the external auditor. The auditor immediately sees that access controls are enforced at the platform level, not configurable per-user. Admin actions are immutably logged. The system is designed for compliance from the ground up.

The auditor signs off: "Access control and audit logging meet SOC 2 requirements."

**Resolution:**
David no longer spends weeks assembling compliance evidence. The system provides it automatically. He runs quarterly compliance reports, not manually built spreadsheets. He can investigate suspicious activity (15 failed logins from the same IP) in seconds. He feels confident presenting this system to auditors, regulators, and customers. Security isn't a checkbox—it's architecture.

**Requirements Revealed:**
- Immutable audit logging of all auth and admin events
- Searchable, filterable audit dashboard
- Compliance report templates (SOC 2, etc.)
- Role-based audit visibility (security officers see full logs)
- Alert/detection for suspicious patterns (failed logins, privilege escalation)
- Audit log export and retention controls

---

### Journey Requirements Summary

**From Admin Journey:**
- OIDC configuration wizard with guided setup
- Validation before go-live
- Role mapping UI with preview
- Admin monitoring dashboard

**From Employee Success Journey:**
- Seamless OIDC redirect and session creation
- Automatic permission assignment from IdP claims
- Role-based data filtering (access enforced at API level)

**From Employee Recovery Journey:**
- Breakglass access for help desk
- Simple MFA re-enrollment
- Temporary access tokens with audit logging

**From Security Officer Journey:**
- Immutable, searchable audit logs
- Compliance report templates
- Admin action logging and alerting
- Data retention and export controls

## SaaS B2B Specific Requirements

### Multi-Tenant Architecture

**Tenant Isolation Model:**
Each enterprise customer is a separate logical tenant within todo-react. Tenant isolation is enforced at multiple layers:

- **Database Level:** All queries include tenant scoping (WHERE tenant_id = X). No query can accidentally return data from another tenant.
- **API Level:** Every API endpoint validates that the requesting user's tenant matches the resource's tenant before returning data.
- **UI Level:** Users only see data belonging to their tenant. Cross-tenant access attempts are denied with no ambiguity.

**Authentication Tenant Binding:**
When an employee authenticates via OIDC, their user account is automatically associated with their employer's tenant based on domain verification or explicit admin configuration.

### Permission Model & RBAC

**Role-Based Access Control (RBAC):**
- **Admin Role:** Full access to configuration, user management, audit logs, and compliance features
- **User Role:** Standard access to assigned tasks/resources within their tenant
- **Read-Only Role:** View-only access to tasks and data; cannot create, modify, or delete
- **Security Officer Role:** Access to audit logs, compliance reports, and security investigations without general admin permissions

**Permission Boundaries:**
Permissions are enforced at the API layer for every request, not just the UI. A user with read-only access cannot make an API call to modify a resource, even if they somehow bypass UI restrictions.

### GDPR Compliance & Data Handling

**Data Minimization:**
- Collect only identity data necessary for authentication: username, email, IdP groups/claims, MFA device associations, and authentication events
- No unnecessary personal data collection (e.g., don't store employee phone number unless required for MFA recovery)

**Data Retention Policy:**
- **Personal Data:** Deleted immediately upon employee request (via self-service or admin)
- **Audit Logs:** Retained for 12 months for security investigation and compliance purposes
- **Automatic Purge:** Audit logs are automatically deleted after 12 months
- **Immutable Records:** Audit entries cannot be modified, only archived/purged per retention policy

**Data Deletion Workflows:**
Both employee self-service and admin-initiated deletion are supported:
- **Self-Service:** Employee requests account deletion via their user profile
- **Admin-Initiated:** Enterprise admin can bulk offboard users and request data deletion
- **Approval Process:** Admin receives deletion request, approves/denies, and deletion executes with full audit trail

**Data Processing Agreement (DPA):**
- Standard DPA template provided to all enterprise customers
- Specifies that todo-react acts as a data processor
- Outlines data retention, access controls, breach notification (72-hour SLA), and sub-processor policies
- Signed before enterprise customer production deployment

### Integration & Extensibility

**OIDC Integration:**
- Standard OpenID Connect 1.0 protocol support
- Works with any OIDC-compliant IdP (Okta, Azure AD, Google Workspace, Keycloak, etc.)
- Automatic metadata discovery and validation

**Future Integrations (Growth Phase):**
- SCIM 2.0 for automated user provisioning/deprovisioning
- Webhook notifications for audit events (e.g., "failed login detected")
- API for compliance report generation and export

### Compliance & Security Posture

**SOC 2 Type II Roadmap:**
- MVP includes foundational controls: immutable audit logging, access control enforcement, session management
- Post-launch (months 3-6): Formal security procedures, change management, incident response documentation
- Full SOC 2 Type II audit completion by month 12

**Audit Logging & Monitoring:**
- All authentication events logged with WHO, WHAT, WHEN, WHERE, OUTCOME
- Admin actions (configuration changes, user role assignments) logged and immutable
- Security alerts for suspicious patterns (e.g., failed login attempts, privilege escalation attempts)
- Audit logs searchable and exportable for compliance investigations

### Implementation Considerations

**Authentication Flow for Tenants:**
1. Employee navigates to todo-react login page
2. Optionally enters company domain or selects from list of configured IdPs
3. Redirected to corporate IdP for OIDC authentication
4. IdP returns authenticated user and group claims
5. todo-react maps claims to internal roles and tenant
6. User session created, scoped to tenant
7. All subsequent requests include tenant context

**Tenant Onboarding Workflow:**
1. Enterprise admin signs up for todo-react
2. Creates tenant and configures OIDC with their IdP
3. Maps IdP groups to todo-react roles
4. Validates configuration with test login
5. Invites employees to use the system
6. Employees authenticate on first login, data is automatically available

## Project Scoping

### Strategy & Philosophy

**Approach:** Single-release launch with all core enterprise authentication capabilities

**Resource Requirements:** 3–5 engineers (backend + frontend + security), 2–3 months

**MVP Philosophy:** Problem-solving MVP — solves the "enterprise needs secure, mandatory MFA with OIDC" problem completely in one release, with no partial capabilities deferred to future versions

### Complete Feature Set

**Core User Journeys Supported:**
1. IT Admin OIDC Configuration & Deployment — setup wizard with validation
2. Employee SSO + MFA First Login — seamless access through identity provider and MFA device
3. MFA Device Loss Recovery — breakglass access and re-enrollment workflows
4. Security Officer Compliance Review — audit log access and evidence collection for SOC 2 roadmap

**Must-Have Capabilities (Non-Negotiable):**
- OIDC 1.0 integration with standard enterprise identity providers (Okta, Azure AD, Google Workspace, Keycloak, etc.)
- Mandatory TOTP-based MFA (Google Authenticator, Authy, Microsoft Authenticator) with SMS fallback for recovery
- Role-based access control (Admin, User, Read-Only, Security Officer roles)
- Multi-tenant isolation enforced at database, API, and UI layers
- Immutable audit logging with 12-month retention and automatic purge
- Breakglass access mechanism for MFA device loss recovery
- Admin configuration wizard with validation and test login capability
- Personal data deletion workflows (self-service and admin-initiated)
- Comprehensive API error handling with tenant validation to prevent data leakage

**Nice-to-Have Capabilities (Post-Launch Growth):**
- SAML 2.0 support for legacy enterprise systems
- SCIM provisioning for automated bulk user management
- WebAuthn/passkey authentication for modern passwordless access
- Adaptive MFA policies for advanced threat response
- Security Officer compliance dashboard with report generation and export
- Formal SOC 2 Type II audit completion (roadmap target: month 12)

### Risk Mitigation Strategy

**Technical Risks:**
- *OIDC implementation complexity:* Use battle-tested libraries and reference implementations; budget external security review if team lacks IdP integration experience
- *MFA delivery reliability:* Implement dual SMS provider strategy with automatic failover; comprehensive testing for SMS delivery edge cases
- *Multi-tenant data isolation bugs:* Invest heavily in boundary testing; each user journey must validate that data from other tenants is inaccessible

**Market Risks:**
- *Early adopter hesitation on mandatory MFA:* Position as differentiator ("we've removed the security compromise"); offer trial period for validation; provide admin override capability during initial setup
- *Compliance concerns delaying sales:* Publish SOC 2 roadmap and timeline upfront; provide interim DPA; share GDPR compliance checklist with prospects

**Resource Risks:**
- *If team < 3 engineers:* Consider deferring WebAuthn and advanced MFA policies to post-launch; focus on OIDC + TOTP + SMS + RBAC core
- *If limited security expertise:* Budget for external OIDC/MFA architecture review and penetration testing

### Scope Justification

This single-release scope ensures todo-react launches as a **complete, security-first enterprise authentication platform** rather than a feature-limited MVP. Every must-have capability is essential for:
- **Admin success:** OIDC setup, validation, and role mapping are non-negotiable for configuration without errors
- **Security positioning:** Mandatory MFA with no opt-out is the core differentiator
- **Compliance credibility:** Audit logging and GDPR workflows prove security and legal readiness
- **Enterprise adoption:** Multi-tenancy and RBAC are table-stakes for B2B SaaS

Nice-to-have features (SAML, SCIM, WebAuthn) enhance the platform post-launch but do not block initial value delivery or security positioning.

## Functional Requirements

### OIDC Authentication & IdP Integration

- **FR1:** Admin can configure OIDC integration by providing IdP metadata URL, and the system auto-discovers endpoints and certificate information
- **FR2:** Admin can manually configure OIDC endpoints (authorization, token, userinfo) when automatic discovery fails
- **FR3:** System validates OIDC configuration and confirms protocol compliance before accepting configuration as active
- **FR4:** Employee can initiate login and be redirected to configured corporate IdP for authentication
- **FR5:** System receives authenticated user claims and group memberships from IdP via OIDC token
- **FR6:** System maps IdP group claims to todo-react application roles (Admin, User, Read-Only, Security Officer)
- **FR7:** System updates user role mapping on every login based on current IdP group membership
- **FR8:** Admin can configure multiple IdPs, and employee can select IdP by company domain or dropdown on login page

### Multi-Factor Authentication (MFA)

- **FR9:** Employee can enroll TOTP-based MFA by scanning QR code with authenticator app (Google Authenticator, Authy, Microsoft Authenticator)
- **FR10:** System enforces MFA on every login attempt — TOTP code must be provided and validated before session creation
- **FR11:** Employee can provide SMS-delivered one-time code as fallback when TOTP device is unavailable
- **FR12:** System validates TOTP codes with 30-second time window tolerance and prevents replay attacks
- **FR13:** System tracks MFA enrollment status for each user and blocks login if MFA not enrolled
- **FR14:** Employee can view MFA enrollment status and see registered MFA devices
- **FR15:** Admin can manually unenroll user's MFA device and force re-enrollment on next login

### Role-Based Access Control (RBAC)

- **FR16:** Admin can assign application roles (Admin, User, Read-Only, Security Officer) to users
- **FR17:** Admin role can access admin console, configuration settings, user management, and audit logs
- **FR18:** User role can create, read, update, and delete tasks and resources they own or are assigned to
- **FR19:** Read-Only role can view tasks and resources but cannot create, update, or delete
- **FR20:** Security Officer role can access audit logs and compliance reports but cannot modify application settings or users
- **FR21:** System enforces role-based permissions on every API request — no permission-denied errors should reach UI
- **FR22:** Task/resource access is determined by: (1) resource tenant matches user tenant, (2) user role grants permission
- **FR23:** API returns HTTP 403 (Forbidden) when user lacks permission for requested action, with reason logged but not disclosed to user

### Multi-Tenant Isolation & Data Scoping

- **FR24:** Employee from Tenant A cannot see, access, or modify data from Tenant B under any circumstances
- **FR25:** System scopes all database queries with tenant_id filter — no cross-tenant data accessible without explicit override
- **FR26:** Employee's tenant is bound to their user account based on IdP domain verification or admin assignment
- **FR27:** Admin API endpoint that returns users list returns only users from admin's own tenant
- **FR28:** System validates API request user's tenant against resource's tenant before returning data (prevents confused deputy attacks)
- **FR29:** Each tenant has isolated OIDC configuration — Tenant A's IdP settings do not affect Tenant B

### Admin Capabilities & Configuration

- **FR30:** Admin can access step-by-step OIDC configuration wizard that guides through IdP selection, metadata configuration, and role mapping
- **FR31:** Admin can run validation test to confirm OIDC configuration is correct before activating for employees
- **FR32:** Admin can preview user roles and permissions that will result from current IdP claim-to-role mapping
- **FR33:** Admin can view admin dashboard showing: current authentication status, active sessions count, failed login attempts, MFA enrollment statistics
- **FR34:** Admin can view list of all users in tenant with current roles, last login time, and MFA enrollment status
- **FR35:** Admin can manually create user account without OIDC, assign role, and generate temporary password for manual onboarding

### Audit, Compliance & Reporting

- **FR36:** System logs all authentication events: successful login, failed login (reason), MFA enrollment, MFA re-enrollment, MFA bypass, role change
- **FR37:** Audit log entry captures: timestamp, actor (user), action, resource affected, result (success/failure), IP address/device info
- **FR38:** Audit logs are immutable — once written, cannot be modified or deleted except by time-based automatic purge after 12 months
- **FR39:** Security Officer can search and filter audit logs by: date range, actor, action type, result, resource
- **FR40:** Security Officer can export audit logs as CSV or JSON for compliance investigations and audit evidence

### User Session & Account Management

- **FR41:** System creates user session after successful OIDC + MFA authentication with session token and refresh token
- **FR42:** Session tokens expire after 1 hour; refresh tokens remain valid for 30 days
- **FR43:** System maintains refresh token validity across multiple devices — employee can authenticate on laptop and mobile without re-authenticating on original device
- **FR44:** Employee can manually log out, invalidating session and refresh tokens
- **FR45:** Employee can request permanent account and data deletion via self-service
- **FR46:** Admin can initiate account and data deletion for offboarded employees
- **FR47:** Data deletion is immutable and logged — cannot be reversed, recorded in audit log with full timestamp and approver

### Breakglass & Emergency Access

- **FR48:** Help desk agent can generate temporary breakglass access code for user locked out of MFA
- **FR49:** Breakglass code grants 15-minute access window without MFA requirement, used once
- **FR50:** System logs every breakglass access use: who generated it, who used it, when, for how long
- **FR51:** User with active breakglass access can immediately re-enroll MFA with new device
- **FR52:** MFA re-enrollment accepts QR code scan and confirms new device in <2 minutes
- **FR53:** Admin can see breakglass access history and usage patterns to detect abuse
