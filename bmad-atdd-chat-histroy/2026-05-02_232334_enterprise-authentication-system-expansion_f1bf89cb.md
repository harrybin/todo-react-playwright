# Enterprise authentication system expansion

- Session-ID: f1bf89cb-a42e-4c53-a38b-7539b77c4d22
- Erstellt: 2026-05-02 23:23:34
- Exportiert: 2026-05-05 12:44:47
- Quelle: C:\Users\HaraldBinkle\AppData\Roaming\Code\User\workspaceStorage\90305d3a38cc9a1db3294a2e53f169e4\chatSessions\f1bf89cb-a42e-4c53-a38b-7539b77c4d22.jsonl

[↑ Zurück zum Index](index.md)

### 👤 User Input

```
/bmad-create-prd erweitere die app um ein Authentifizierungssystem für Enterprise-Kunden
```

### Assistant

Ich starte jetzt strikt den PRD-Workflow aus der Skill-Datei: zuerst löse ich den `workflow`-Block auf und lese danach nur den ersten Schritt, bevor ich irgendetwas anderes ausführe.

[Tool] Running `python \_bmad/scripts/resolve\_customization.py \-\-skill .agents/skills/bmad\-cre...`

[Tool] Reading [](file:///d%3A/harrybin/todo-react-playwright/.agents/skills/bmad-create-prd)

### 👤 User Input

```
c
```

### Assistant

Proceeding with your Continue selection now: I will load the full Step 2 instructions and execute them in order, then pause again at that step’s menu.

[Tool] Reading skill `bmad-create-prd`: [](file:///d%3A/harrybin/todo-react-playwright/.agents/skills/bmad-create-prd/steps-c/step-02-discovery.md)

### 👤 User Input

```
p
```

### Assistant

I’m running Party Mode now for Step 2 so we can collaboratively stress-test and refine the current classification before continuing.

[Tool] Loading skill: bmad-party-mode

The Party Mode skill alias is not installed under that exact name here, so I’m locating the actual skill name in this workspace and will run it immediately.

[Tool] Searching for files matching `**/*party*/*SKILL.md`

I found the skill files in the repo; next I’ll execute the local Party Mode workflow instructions directly as a fallback and return improved classification options.

**[Tool] Reading skill [bmad-party-mode](file:///d%3A/harrybin/todo-react-playwright/.agents/skills/bmad-party-mode/SKILL.md?vscodeLinkType%3Dskill)**

I’m activating Party Mode in fallback form now: I’ll load the BMAD roster/config, then run a short multi-agent round focused on refining this project classification.

[Tool] Running `python \_bmad/scripts/resolve\_config.py \-\-project\-root . \-\-key agents`

[Tool] Reading [](file:///d%3A/harrybin/todo-react-playwright/_bmad/core/config.yaml)

[Tool] Searching for files matching `**/project-context.md`

I have the roster and context; next I’m running a focused three-agent roundtable to challenge and improve the current classification for enterprise authentication.

[Tool] runSubagent

[Tool] runSubagent

[Tool] runSubagent

### 👤 User Input

```
p
```

### Assistant

Running another Party Mode round now with a different mix of voices to pressure-test the classification from product, security, and UX angles before you decide to continue.

[Tool] runSubagent

[Tool] runSubagent

[Tool] runSubagent

[Tool] Generating patch (22 lines) in [](file:///d%3A/harrybin/todo-react-playwright/docs/code-review/2026-05-02-enterprise-auth-review.md)

```

```

📋 **SE: Product Manager:**
1) Strong argument FOR this classification  
Brownfield + High complexity is justified because enterprise authentication in an existing app usually impacts core architecture end-to-end: identity provider integration, session/token lifecycle, role/permission model, route/API protection, migration of existing users, and compliance/security hardening. This is rarely a bolt-on change.

2) Strong argument AGAINST it  
Project Type: SaaS B2B and Domain: General may be too broad if the app is still primarily a demo/internal todo product with only one tenant and basic SSO goals. In that case, this is more a capability upgrade than a full B2B SaaS positioning shift.

3) Final recommendation  
- Project Type: Keep with caveat (SaaS B2B if multi-tenant/admin roadmap exists)  
- Domain: Change to Identity & Access Management within Productivity SaaS  
- Complexity: Keep High  
- Project Context: Keep Brownfield

4) Top PM risks  
- Scope explosion from auth into full IAM platform  
- Security/compliance gaps discovered late  
- IdP and enterprise dependency drag delaying delivery

🛡️ **SE: Security:**
1) Critical assumptions to validate  
- IdP trust model and protocol choice (OIDC/SAML), key rotation, failure behavior  
- Tenant isolation at API/data/admin boundaries  
- Authorization model beyond login (roles, least privilege, separation of duties)  
- Token/session lifecycle and revocation model  
- Provisioning/deprovisioning expectations (SCIM/JIT)  
- Auditability and incident response readiness

2) Complexity  
High is justified due to brownfield integration plus federation, authorization, audit, and compliance pressure.

3) Top security acceptance criteria  
- Approved threat model for auth flows  
- Testable access-control spec  
- Validated federation security design  
- Measurable token/session security requirements  
- Defined security telemetry and response requirements

4) Classification change  
- Keep Type SaaS B2B  
- Refine Domain to Identity and Access Management  
- Keep Complexity High  
- Keep Context Brownfield

🎨 **SE: UX Designer:**
1) UX argument on complexity  
High is valid when enterprise auth includes admin setup, role mapping, provisioning lifecycle, and supportable failure journeys.  
Could be lower only if Phase 1 is strict minimal SSO.

2) 5 impacted journeys  
- Tenant onboarding and domain claim  
- First-time login and account linking  
- Role/permission mapping  
- Joiner/mover/leaver lifecycle  
- Recovery and support for login failures

3) Domain label refinement  
Use: B2B SaaS - Workforce Identity and Access Management (IAM)  
Secondary tags: Enterprise Security, Tenant Administration, Access Governance

4) Verdict  
Mostly correct classification; domain should be sharpened and complexity stays High unless scope is SSO-only.

**Orchestrator Note:** This round strongly supports keeping High + Brownfield, with one key improvement: domain wording should explicitly say Workforce IAM.

**Do you accept this updated Party Mode refinement?**

**Select:**
[A] Advanced Elicitation  
[P] Party Mode again  
[C] Continue to Product Vision (Step 2b of 13)

### 👤 User Input

```
a
```

### Assistant

I’m executing Advanced Elicitation now on the current classification and will return a refined version for your accept/reject decision before we proceed.

[Tool] Loading skill: bmad-advanced-elicitation

The skill alias isn’t registered by name in this environment, so I’m loading the local Advanced Elicitation skill definition directly and applying its method as fallback.

[Tool] Searching for files matching `**/*advanced*elicitation*/SKILL.md`

**[Tool] Reading skill [bmad-advanced-elicitation](file:///d%3A/harrybin/todo-react-playwright/.agents/skills/bmad-advanced-elicitation/SKILL.md?vscodeLinkType%3Dskill)**

I’m loading the elicitation method registry next so I can present the required 1-5 method menu tailored to your current classification context.

[Tool] Reading skill `bmad-advanced-elicitation`: [](file:///d%3A/harrybin/todo-react-playwright/.agents/skills/bmad-advanced-elicitation/methods.csv)

### 👤 User Input

1

### Assistant

Method 1 applied: First Principles Analysis

Assumptions stripped away:
1. Fundamental truth: Enterprise customers buy trust and control, not just “login.”
2. Fundamental truth: Authentication alone is insufficient without authorization, tenant isolation, and auditability.
3. Fundamental truth: This is an existing app, so migration safety is part of the product requirement.
4. Fundamental truth: “Enterprise” implies external IdP interoperability and admin-operable security workflows.

Refined classification from first principles:
1. Project Type: SaaS B2B
2. Domain: B2B SaaS - Workforce Identity & Access Management
3. Complexity: High
4. Project Context: Brownfield

What improved versus previous version:
1. Domain is now explicit and decision-driving (not generic “General”).
2. Complexity is justified by core enterprise truths, not implementation guesswork.
3. Scope boundary is clearer: this is identity plus governance, not a UI-only login feature.

Proposed classification text to carry forward:
- Project Type: SaaS B2B  
- Domain: B2B SaaS - Workforce Identity & Access Management  
- Complexity: High  
- Project Context: Brownfield

**Apply these changes to the current Step 2 classification? (`y`/`n`/other)**

### User

y

### Assistant

Applying the refined classification to frontmatter and marking Step 2 complete, then loading Step 2b (Product

### User

1. best security, like forced MFA
2. security first
3. permission

### Assistant

Excellent — that's clear and purposeful. Let me validate what I'm hearing:

**Vision Understanding:**

- **What Makes It Special:** Security-first enterprise authentication with mandatory MFA — no compromises, no opt-outs. Users can't bypass security even if they want to.
- **Core Insight:** Enterprise customers are tired of identity and access management tools that make security optional or complex. Your product makes security the default and unavoidable.
- **Problem Being Solved:** Permission management and access control — ensuring only the right people have access to the right resources, with no ambiguity or technical escape routes.
- **Why This Matters:** In an era of rising breaches and compliance mandates, enterprises need authentication that assumes the worst and builds guarantees in from the start.

**Does this capture your vision? Anything I should refine or add?**

Once you confirm, I'll move to the next step where we draft the Executive Summary that will articulate this vision clearly in the

### User

y

### Assistant

Vision confirmed and saved. Updating frontmatter and loading Step 2c (Executive Summary):

[Tool] Replacing 16 lines with 22 lines in [](file:///d%3A/harrybin/todo-react-playwright/_bmad-output/planning-artifacts/prd.md)

```

```

[Tool] Reading skill `bmad-create-prd`: [](file:///d%3A/harrybin/todo-react-playwright/.agents/skills/bmad-create-prd/steps-c/step-02c-executive-summary.md)

I've synthesized the context from steps 2 and 2b. Now drafting the Executive Summary based on your security-first vision and enterprise authentication focus.

Here's the Executive Summary I've drafted for your PRD:

---

## Executive Summary

**todo-react** is extending its functionality to serve enterprise customers by introducing mandatory enterprise authentication with security-first architecture. The product solves the permission management and access control challenge: ensuring only authorized personnel access resources with zero ambiguity and no technical escape routes. This positions todo-react as a trusted platform for enterprises where security is non-negotiable, not optional.

### What Makes This Special

**Security-first default:** Enterprise authentication is built on mandatory MFA and zero-compromise security principles. Users cannot opt out of security controls — they are enforced at the platform level. This contrasts with identity management solutions that treat security as a feature matrix to be configured, where teams can accidentally weaken controls through misconfiguration or convenience.

**Core insight:** Enterprises are fatigued by IAM tools that make security a setting to toggle. They need platforms where security is the operating model, not an option. By making MFA mandatory and permission boundaries unambiguous, todo-react becomes a platform enterprises can trust without requiring a dedicated security operations team.

## Project Classification

| Attribute | Value |
|---|---|
| **Project Type** | SaaS B2B |
| **Domain** | Workforce Identity & Access Management |
| **Complexity** | High |
| **Project Context** | Brownfield (extending existing todo-react app) |

---

Please review this draft. Would you like me to:
- Refine the language or emphasis in any section?
- Add or remove details?
- Reshape the framing?

Once you're satisfied, I'll append this to the PRD and move to Success Criteria (the next section).

Select an option:
[A] Advanced Elicitation
[P] Party Mode
[C] Continue to Success Criteria (Step 3 of 13)

### User

p

### Assistant

(keine sichtbare Assistant-Antwort gespeichert)

### User

c

### Assistant

Appending the refined Executive Summary to the PRD (using Tech Writer's polished version with Security's credibility fix) and updating frontmatter, then loading Step 3 (Success Criteria).

[Tool] Replacing 4 lines with 23 lines in [](file:///d%3A/harrybin/todo-react-playwright/_bmad-output/planning-artifacts/prd.md)

```

```

Now updating frontmatter to record Step 2c completion:

[Tool] Replacing 5 lines with 6 lines in [](file:///d%3A/harrybin/todo-react-playwright/_bmad-output/planning-artifacts/prd.md)

```

```

Now loading Step 3 (Success Criteria):

[Tool] Reading skill `bmad-create-prd`: [](file:///d%3A/harrybin/todo-react-playwright/.agents/skills/bmad-create-prd/steps-c/step-03-success.md)

Executive Summary is complete and appended. Now we're at Step 3: Success Criteria Definition.

Since you don't have existing product briefs or research documents, let me help you define what winning looks like for this enterprise authentication initiative.

Let me ask some discovery questions to understand your success vision:

**User Success (the end-users and admins):**
**1. When an enterprise IT admin sets up this authentication system, what's the moment they realize "this is worth it"?**
**2. For end-users (employees logging in), what would make them feel this security is worth the MFA friction?**
**3. What's the completion scenario? "We've succeeded when..."**

**Business Success:**
**1. What does success look like at 3 months? 6 months? 12 months?**
**2. Is this measured by enterprise customer adoption, revenue, number of orgs using it, reduction in support tickets, or something else?**
**3. What specific metric tells you "this is working"?**

**Technical Success:**
**1. Beyond "it works," what does technical success look like? (e.g., 99.9% uptime, sub-100ms login latency, audit log completeness, SAML/OIDC spec compliance?)**

**Scope Reality Check:**
**1. What's the MVP (minimum to prove this works)? Must have Okta + MFA? Just OIDC?**
**2. What comes after MVP? (Growth features like SCIM provisioning, policy engine, etc.)**
**3. What's the dream version 2-3 years out?**

Start with what feels most clear to you, and I'll help us build measurable success criteria around it.

### User

1

### Assistant

Let's focus on **User Success** first. Help me understand what winning looks like for your users:

**For IT Admins setting up enterprise authentication:**

**1. What's the friction they experience today with other SSO/IAM solutions that you want to eliminate?**
   - Is it complex setup? Ongoing configuration overhead? Hidden security gaps?

**2. When they successfully configure this in todo-react, what's the relief moment?**
   - "I set it up once and it just works"?
   - "I can finally sleep knowing security is enforced"?
   - "I don't have to babysit permission configurations"?

**For end-users (employees) logging in:**

**3. How do they currently authenticate, and what changes?**
   - Today: password? Social login?
   - After: forced through corporate SSO + MFA?

**4. What would make them feel the MFA friction is worth it?**
   - Do they need to see security in action (login audit trails, "suspicious login blocked")?
   - Or is it enough that their IT admin says "this is secure"?

**Completion scenario:**

5. Finish this sentence: "We've succeeded in user success when..."
   - Is it a percentage adoption target? (90% of enterprise users completing login within 2 weeks of rollout?)
   - Is it zero failed MFA attempts due to poor UX?
   - Is it admin saying "this took 2 hours to set up instead of the usual 2 days"?

**Pick one or two of these to start with — what feels most important to you?**

### User

5

### Assistant

Perfect — let's ground this in concrete, measurable outcomes.

**Completing the sentence: "We've succeeded in user success when..."**

Help me get specific:

**1. Adoption speed: When you say "users complete [action] within [timeframe]," what does that look like?**
   - Is it: "90% of enterprise employees have successfully authenticated via SSO within 2 weeks of rollout"?
   - Or: "100% of users can log in on day 1 without hitting MFA friction"?
   - Or something else about adoption velocity?

**2. Admin setup experience: From the IT admin perspective, what's the success metric?**
   - Is it time-based? ("Admin configures Okta + todo-react auth in under 1 hour"?)
   - Is it error rate? ("Zero misconfiguration errors in the initial setup"?)
   - Is it support burden? ("Post-setup, admins have zero auth-related support tickets"?)

3. **Employee MFA experience:** For end-users, is success measured by:
   - Completion rate? ("95% of login attempts include successful MFA"?)
   - Satisfaction? ("Employees rate MFA friction as acceptable"?)
   - Behavior? ("Employees don't disable MFA or circumvent the system"?)

4. **Security outcome:** Is there a user-facing security success metric, like:
   - "Zero unauthorized access attempts get through"?
   - "All admin privilege escalations are logged and visible to security teams"?

**Which of these resonates most, or should I ask differently to get at what you're really optimizing for?**

### User

3

### Assistant

(keine sichtbare Assistant-Antwort gespeichert)

### User

5

### Assistant

Strong choice — **"Sustained >95% MFA usage over 6 months"** is a retention metric that shows adoption stuck, not just initial rollout velocity.

This tells me:
- Employees aren't finding workarounds or exemptions
- The MFA UX is tolerable enough to keep using
- Security policy is sustainable

Now let's layer in **Admin Setup Success:**

**When the IT admin configures enterprise authentication for todo-react, what does their success look like?**

1. **Setup Time:** 
   - Should it take <1 hour total?
   - Or are you comfortable with 2-4 hours if it's clear and guided?

2. **Configuration Correctness:**
   - "Admin completes setup with zero errors on first attempt"?
   - Or is one iteration acceptable?

3. **Ongoing Burden:**
   - "After setup, admin spends <2 hours/month managing auth"?
   - Or should it be <30 minutes/month?

4. **Support:**
   - "Zero auth-related support tickets from that customer post-launch"?
   - Or is <5 tickets/month acceptable?

5. **Feature Coverage:**
   - Which auth protocols/features are non-negotiable for admin success?
     - OIDC? SAML? Both?
     - Role mapping? Just login?
     - Audit logging? Break-glass access?

Pick one or two here, and then we'll round out with **Business Success** (adoption timeline, enterprise customer acquisition, etc.).

### User

2

### Assistant

Perfect — **"Admin completes setup with zero errors on first attempt"** is a high bar that forces great UX.

This means:
- Setup flow is intuitive and guided
- Common pitfalls are prevented (e.g., certificate format, claim mapping mistakes)
- Validation catches issues before they break auth

Let me dig into what "zero errors" means specifically:

**1. What counts as an error?**
   - Protocol misconfiguration (OIDC/SAML metadata wrong)?
   - Certificate/key format issues?
   - Role mapping mistakes?
   - All of the above?

**2. How do we measure "zero errors"?**
   - Admin runs a validation test and it passes immediately?
   - First employee login succeeds on first attempt?
   - Employees don't hit auth failures for 7 days post-launch?

**3. Which auth protocols must work perfectly?**
   - OIDC only? SAML only? Both?
   - Any specific IdP (Okta, Azure AD, others)?

**4. What about the learning curve?**
   - Is "zero errors on first attempt" realistic, or should it be "one iteration to full correctness"?
   - (E.g., admin tries once, gets feedback, fixes one thing, then succeeds?)

5. **Scope:** Does "setup" include:
   - Just connecting IdP + MFA?
   - Or also role mapping, audit log configuration, breakglass access setup?

Once we nail this, we'll have **Employee MFA success** + **Admin Setup success**, and then we can finish with **Business Success** metrics (revenue, customer acquisition, adoption timeline).

**What feels right?**

### User

3

### Assistant

Perfect — protocol choice is foundational and affects your entire MVP

### User

yes

### Assistant

Excellent — **OIDC-only MVP** is smart. Simpler to execute, easier to achieve zero-error setup, and covers your initial enterprise customer base.

Now let's round out with **Business Success** metrics:

**When you look at this enterprise authentication initiative from a business perspective, what does winning look like?**

1. **Customer Acquisition:**
   - How many enterprise customers should be using this by month 3? Month 6? Month 12?
   - Or is it: "X% of enterprise trials successfully deploy auth"?

2. **Revenue Impact:**
   - Does this unlock new pricing tiers (enterprise tier with MFA)?
   - Is success measured by new ARR from this feature?
   - Or is it a retention feature (prevents churn of existing customers)?

3. **Time-to-Value:**
   - "Enterprise customer signs, deploys auth, and goes live within 2 weeks"?
   - Or "First employee login with MFA within 48 hours of contract signing"?

4. **Support/Operational:**
   - "Zero escalations to engineering for auth issues"?
   - Or "Support can resolve 95% of auth questions without engineering"?

5. **Market Position:**
   - Is success about being "first in market with secure todo+auth"?
   - Or "top 3 secure task management tools for enterprises"?

Pick 1-2 that matter most, and then I'll synthesize everything into a cohesive Success Criteria section with User Success, Admin Success, Business Success, and we'll define MVP/Growth/Vision scope.

### User

4

### Assistant

Excellent — **"Support can resolve 95% of auth questions without engineering escalation"** is a strong operational metric.

This tells me:
- Auth system is predictable and documented well enough for support
- Common issues (forgotten MFA, device registration, etc.) have self-service or support-level fixes
- Engineering focuses on building, not firefighting

Quick clarification to finalize this:

**1. What counts as an auth question?**
   - "I can't log in" → support handles?
   - "I lost my MFA device" → support handles with breakglass?
   - "OIDC metadata validation failed" → support or engineering?

**2. Timeframe: Is this metric tracked from day 1 launch, or after a 30-day stabilization period?**

**3. Any other business success metrics worth mentioning?**
   - Timeline to first enterprise deployment? (e.g., "First customer goes live within 4 weeks of launch")
   - Number of enterprise customers? (e.g., "3+ paying enterprise customers by month 6")
   - Or is support efficiency your primary business win?

Once

### User

yes

### Assistant

Perfect. I have all the success criteria we need. Let me draft the complete Success Criteria section for your PRD:

---

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

---

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

---

Here's what I've drafted based on our discovery. Please review and let me know:
- Any changes to the success metrics?
- Does the MVP scope feel right, or should anything shift to Growth?
- Should I expand or contract any section?

Once you're satisfied, I'll append this to the PRD.

Select an option:
[A] Advanced Elicitation
[P] Party Mode
[C] Continue to User Journey Mapping (Step 4 of 13)

### User

c

### Assistant

(keine sichtbare Assistant-Antwort gespeichert)

### User

y

### Assistant

Perfect. Now let me craft rich narrative journeys for each user type. These will tell the story of how each person experiences the system:

---

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
Marcus opens his task list. All his existing todo-react data is still there, mapped to his corporate email. His permissions are automatically set based on his Okta group membership. He never sees a permission denied error because the system only shows him tasks he has access to. He can assign tasks to teammates, knowing that access control is enforced, not

### User

p

### Assistant

(keine sichtbare Assistant-Antwort gespeichert)

### User

p

### Assistant

Running Party Mode now to get collaborative feedback on the user journeys from product, UX, security, and technical architecture perspectives:

### User

c

### Assistant

Appending User Journeys to the PRD, updating frontmatter, and loading Step 5 (Domain Requirements):

[Tool] Reading [](file:///d%3A/harrybin/todo-react-playwright/_bmad-output/planning-artifacts/prd.md#1-1), lines 1 to 100

[Tool] Reading [](file:///d%3A/harrybin/todo-react-playwright/_bmad-output/planning-artifacts/prd.md#150-150), lines 150 to 150

### User

c

### Assistant

Skipping Domain-Specific Requirements and updating frontmatter, then loading Step 6 (Innovation Focus):

[Tool] Replacing 8 lines with 9 lines in [](file:///d%3A/harrybin/todo-react-playwright/_bmad-output/planning-artifacts/prd.md)

```

```

### User

c

### Assistant

Skipping Innovation section and updating frontmatter, then loading Step 7 (Project Type Analysis):

[Tool] Replacing 9 lines with 10 lines in [](file:///d%3A/harrybin/todo-react-playwright/_bmad-output/planning-artifacts/prd.md)

```

```

[Tool] Reading skill `bmad-create-prd`: [](file:///d%3A/harrybin/todo-react-playwright/.agents/skills/bmad-create-prd/steps-c/step-07-project-type.md)

We've reached Step 7: **Project-Type Deep Dive** for SaaS B2B.

Your project type is **SaaS B2B**, and I've loaded the key discovery questions specific to this project type from the configuration data. Let me guide you through them:

**SaaS B2B Key Questions:**

**1. Multi-tenant architecture? How should multiple enterprise customers be isolated from each other? Separate databases? Shared database with tenant-scoped queries? Shared tables with tenant ID filtering?**

**2. Permission model? Beyond the basic user/admin/read-only roles, what's your permission architecture? Is it just role-based, or do you need finer-grained permissions? Can roles be customized per customer?**

**3. Subscription tiers? Are you offering multiple tiers of enterprise auth? (e.g., Basic SSO tier, Advanced with SCIM tier, Premium with advanced features?) Or is enterprise auth a single offering?**

**4. Integrations? Beyond OIDC, what integrations matter? SCIM provisioning to/from IdPs? Webhook notifications for audit events? API for partners to extend?**

**5. Compliance? What compliance certifications or standards are you targeting? (SOC 2, ISO 27001, GDPR, HIPAA, etc.?) Are these must-haves for MVP or growth-phase?**

**Which of these feels most important to define now? Or should I ask them in a different order?**

### User

5

### Assistant

**Compliance & Standards — let's get specific:**

This is foundational for enterprise trust. Help me understand your target:

**1. Which certifications are non-negotiable for your initial enterprise customers?**
   - SOC 2 Type II (most common for SaaS)
   - ISO 27001 (information security standard)
   - GDPR (for EU customers)
   - HIPAA (if healthcare/regulated industries)
   - Industry-specific (e.g., PCI-DSS if you touch payments)
   - Or is compliance "nice-to-have" while you build?

2. **Timeline:**
   - Do you need these certifications before MVP launch?
   - Or are they a post-launch growth requirement (first 6-12 months)?
   - (This affects your MVP scope significantly)

3. **Audit readiness:**
   - Are your initial pilot customers asking for audit trails and compliance reports?
   - Or are they just trusting the security model for now?

4. **Data handling:**
   - Do you need to handle sensitive customer data with specific encryption/retention policies?
   - Are there data residency requirements (EU data stays in EU, etc.)?

My sense: For enterprise auth, **SOC 2 Type II readiness is table-stakes**, but you might achieve that over 12 months rather than day 1. GDPR compliance (data minimization, retention, subject rights) should be built in from the start.

**What's your thinking on compliance timing and requirements?**

### User

1

### Assistant

**SOC 2 Type II — that's the right foundational choice for enterprise.**

Let me dig deeper on this to shape your requirements:

1. **SOC 2 Scope:**
   - Full scope (all controls: security, availability, processing integrity, confidentiality, privacy)?
   - Or focused scope (just security and access controls, since that's your differentiator)?

2. **Timeline:**
   - When do you need SOC 2 Type II audit complete?
     - Before MVP launch (aggressive, affects MVP scope)?
     - Month 3-6 post-launch (realistic for most SaaS)?
     - Month 12 (gives you time to mature)?

3. **Supporting Compliance:**
   - Beyond SOC 2, are you also targeting GDPR (especially if EU customers)?
   - PCI-DSS (if handling payment data)?
   - Or is SOC 2 your sole compliance focus for now?

4. **MVP Impact:**
   - For SOC 2 readiness, your MVP needs:
     - Immutable audit logging ✓ (you have this)
     - Security incident response procedures
     - Change management process
     - Access control testing
     - Incident response documentation
   
   **Are these already part of your engineering practice, or do they require new process/tooling?**

My sense: **SOC 2 Type II is achievable in 6-12 months for a focused SaaS B2B product.** Audit logging and access controls go into MVP; formal procedures and evidence collection happen in parallel.

**What's your target completion date for SOC 2 Type II?**

### User

GDPR

### Assistant

Got it — **GDPR is your compliance priority.**

Let me clarify your compliance strategy:

1. **GDPR vs SOC 2:**
   - Are you going GDPR-first (European focus, EU customer requirements)?
   - Or do you want both GDPR + SOC 2 (serve global enterprise)?
   - Or is SOC 2 secondary to GDPR?

2. **GDPR Scope for Enterprise Auth:**
   - Which GDPR obligations matter most for your product?
     - **Data minimization:** Collect only auth/identity data needed, nothing extra
     - **Consent & transparency:** Users know why you're collecting their data
     - Retention: How long do you keep audit logs? (90 days? 1 year? 7 years for compliance?)
     - **Subject rights:** Users can request data export, deletion (except immutable audit logs)
     - Processor agreements: Does todo-react act as data processor for enterprise customers?
     - **DPA (Data Processing Agreement):** Enterprise customers need this

---

[↑ Zurück zum Index](index.md)
     - **Breach notification:** 72-hour incident notification procedures

3. **MVP Impact:**
   - For **GDPR launch**, you need:
     - Data retention policies documented
     - DPA template ready for customers to sign
     - Clear privacy policy on data handling
     - Audit log retention strategy (what's immutable vs. deletable?)
   
   - These are legal/process requirements, not heavy engineering

4. **Timeline:**
   - When do you need GDPR compliance documented?
     - Before MVP (show customers you're compliant**)?**
     - By month 3 (after first enterprise deployments**)?**

What's driving the GDPR focus — EU customers, regulatory requirement, or best practice?

### User

2

### Assistant

Perfect — **GDPR Scope for enterprise authentication.** Let me focus the key GDPR obligations:

**For your enterprise auth system, which of these GDPR requirements are most critical to address?**

1. **Data Minimization:**
   - You collect: username, email, IdP claims (groups/roles), MFA device info, authentication timestamps
   - Should you limit what identity data you store? (E.g., don't store full name unless needed?)

2. **Retention Policies:**
   - How long should you keep authentication logs?
     - Option A: 90 days (minimal compliance)
     - Option B: 1 year (auditing standard)
     - Option C: 7 years (regulatory/legal hold)
   - Which works for your enterprise customers?

3. **Data Processing Agreements (DPA):**
   - Every enterprise customer will ask: "Who owns the data? What are your privacy terms?"
   - You need a **standard DPA template** that outlines:
     - You're a data processor (customer owns the data)
     - How long you keep data
     - Who can access it (support, admins, etc.)
     - Breach notification SLA (you'll notify within 72 hours)

4. **Subject Rights:**
   - If an employee leaves a company, they can request data deletion
   - Can you delete their authentication history, or is that immutable for audit?
   - (Typically: delete personal data, keep anonymized audit trail)

**Which of these should be built into your MVP vs. handled post-launch?**

### User

4

### Assistant

**Subject Rights — this is the tricky balance between GDPR and security.**

Let me explore your approach:

**The Core Tension:**
- GDPR says: Users can request deletion of their personal data (Right to be Forgotten)
- Security audit logs say: We need immutable records to prove who did what, when

**Your Options:**

1. **Delete all personal data, anonymize audit trails:**
   - When an employee leaves: delete their email, name, MFA device records
   - But keep audit log entry like: "User [anonymized-ID-12345] logged in from IP X at time Y"
   - Audit trail remains for security investigation, but no PII
   - **MVP Approach:** Simpler, compliant with GDPR

2. **Keep audit logs as immutable (legal hold):**
   - Audit logs are never deleted (even on GDPR requests)
   - But delete personal data from non-audit systems
   - Requires a clear legal justification in your DPA
   - **Enterprise expectation:** Regulators and auditors often expect this

3. **Hybrid: Time-based deletion:**
   - After retention period (e.g., 1 year), even audit logs can be deleted
   - Personal data deleted immediately on request
   - Audit logs stay for 12 months, then purged
   - **Balanced approach:** Compliance + audit trail

**For your MVP, which feels right?**

**And one more clarification: Who initiates the deletion request?**
- The employee directly (through a self-service request)?
- The enterprise admin (on behalf of the company)?
- Both?

This shapes how you build the deletion workflow.

### User

3

### Assistant

Excellent — **Hybrid time-based deletion** is a solid compliance + security balance:
- Personal data deleted immediately on request
- Audit logs retained for 12 months (investigation, audit, compliance)
- Automatic purge after 12 months

This gives enterprises confidence in the security trail while respecting GDPR rights.

Now, **one final question on the deletion workflow:**

**Who initiates the data deletion request?**

1. **Employee self-service:**
   - Employee logs into todo-react, requests deletion of their account/data
   - Admin gets notified, approves (or auto-approves)
   - Personal data deleted, audit logs anonymized after 12 months

2. **Enterprise admin only:**
   - Only the company's IT admin can request deletion on behalf of departing employees
   - Employee cannot self-request
   - Admins have "offboard user" workflow

3. **Both:**
   - Employee can request, admin can also request on behalf
   - Provides flexibility for both scenarios

**What makes sense for your use case? (My sense: Option 3 is most flexible and enterprise-friendly.)**

Once you confirm, I'll have everything to draft the **SaaS B2B-specific requirements** section covering multi-tenancy, permissions, compliance, and integration architecture.

### User

3

### Assistant

Perfect. Now I have everything to draft the **SaaS B2B-Specific Requirements** section. Let me synthesize what we've discovered:

---

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

---

Here's the SaaS B2B-specific content I've drafted. Does this capture the technical architecture and compliance strategy you want? Any refinements before we append it to the PRD?

**Select:**
[A] Advanced Elicitation
[P] Party Mode
[C] Continue to Scoping (Step 8 of 11)

### User

c

### Assistant

Appending SaaS B2B-Specific Requirements to PRD, updating frontmatter, and loading Step 8 (Scoping):

[Tool] Reading [](file:///d%3A/harrybin/todo-react-playwright/_bmad-output/planning-artifacts/prd.md#1-1), lines 1 to 50

[Tool] Reading [](file:///d%3A/harrybin/todo-react-playwright/_bmad-output/planning-artifacts/prd.md#301-301), lines 301 to 301

[Tool] Reading [](file:///d%3A/harrybin/todo-react-playwright/_bmad-output/planning-artifacts/prd.md#301-301), lines 301 to 301

[Tool] Reading [](file:///d%3A/harrybin/todo-react-playwright/_bmad-output/planning-artifacts/prd.md#300-300), lines 300 to 301

[Tool] Reading [](file:///d%3A/harrybin/todo-react-playwright/_bmad-output/planning-artifacts/prd.md#280-280), lines 280 to 301

### User

2

### Assistant

Perfect. **Single Release** — all core functionality in one launch, prioritized strategically.

Let me conduct the scoping analysis:

### Must-Have vs Nice-to-Have Analysis

**MUST-HAVE (Non-negotiable for security-first positioning):**
- ✅ OIDC integration (core modern enterprise auth standard)
- ✅ Mandatory TOTP-based MFA (your differentiator — "no compromises")
- ✅ SMS fallback for MFA recovery (prevents lockout, required by enterprise buyers)
- ✅ RBAC with Admin/User/Read-Only roles (multi-tenant essentials)
- ✅ Audit logging with 12-month retention (GDPR + compliance credibility)
- ✅ Breakglass access (MFA recovery) — non-negotiable for admin success
- ✅ Admin configuration wizard (success criterion: zero-error setup)
- ✅ Tenant isolation at database/API/UI layers (SaaS foundation)
- ✅ Personal data deletion workflows (GDPR legal requirement)

**NICE-TO-HAVE (Enhances but not MVP-critical):**
- 🔄 SAML integration (valuable for legacy enterprises, not required for modern SSO)
- 🔄 SCIM provisioning (nice for large enterprises, can start with manual)
- 🔄 WebAuthn/passkeys (modern, but TOTP + SMS covers MVP security needs)
- 🔄 Adaptive MFA (advanced, can be added post-launch)
- 🔄 Security Officer role with compliance reporting UI (valuable, can be manual initially)
- 🔄 SOC 2 audit (roadmap item, MVP builds foundations without formal audit)

### Risk Analysis

**Technical Risks:**
- OIDC implementation complexity: *Mitigate* — Use established libraries (Auth0, Keycloak patterns)
- MFA delivery reliability (SMS + TOTP): *Mitigate* — Dual provider strategy + fallback logic
- Multi-tenant data isolation bugs: *Mitigate* — Comprehensive test suite with tenant boundary testing

**Market Risks:**
- Early adopter hesitation on "mandatory MFA": *Mitigate* — Clear positioning + trial period + admin override for setup
- Compliance concerns delaying sales: *Mitigate* — SOC 2 roadmap visibility + interim DPA + GDPR checklist

**Resource Risks:**
- If team < 3 engineers: Consider deferring WebAuthn + SCIM to post-launch
- If team has security expertise gaps: Budget for external OIDC/MFA review

### Your Single-Release Scope

```markdown
## Project Scoping

### Strategy & Philosophy

**Approach:** Single-release launch with all core enterprise authentication capabilities
**Resource Requirements:** 3-5 engineers (backend + frontend + security), 2-3 months
**MVP Philosophy:** Problem-solving MVP — solves the "enterprise needs secure, mandatory MFA with OIDC" problem completely in one release

### Complete Feature Set

**Core User Journeys Supported:**
1. IT Admin OIDC Configuration & Deployment
2. Employee SSO + MFA First Login
3. MFA Device Loss Recovery (Breakglass)
4. Security Officer Compliance Review

**Must-Have Capabilities:**
- OIDC 1.0 integration with standard enterprise IdPs
- Mandatory TOTP + SMS-based MFA
- Role-based access control (Admin, User, Read-Only roles)
- Multi-tenant isolation (database, API, UI layers)
- Audit logging with immutable 12-month retention
- Breakglass access for MFA recovery
- Admin configuration wizard with validation
- Personal data deletion workflows (self-service + admin-initiated)
- Comprehensive API error handling with tenant validation

**Nice-to-Have Capabilities:**
- SAML 2.0 support (post-launch: enterprise legacy support)
- SCIM provisioning (post-launch: bulk user management)
- WebAuthn/passkey authentication (post-launch: modern passwordless)
- Adaptive MFA policies (post-launch: advanced threat response)
- Security Officer role with compliance dashboard (post-launch: reporting)
- Formal SOC 2 Type II audit (roadmap: month 12 target)

### Risk Mitigation Strategy

**Technical Risks:** Use battle-tested OIDC libraries; dual SMS provider; comprehensive multi-tenant boundary testing
**Market Risks:** Position mandatory MFA as differentiator (not blocker); offer trial period; publish SOC 2 roadmap
**Resource Risks:** Prioritize authentication core first; defer WebAuthn if team < 4; consider external security review for OIDC
```

---

**Does this single-release scope capture your vision accurately, or would you adjust must-have vs nice-to-have priorities?**

**Select:**  
[A] Advanced Elicitation — Deeper challenge on MVP boundaries  
[P] Party Mode — Get architect/security/PM perspectives  
[C] Continue to Functional Requirements (Step 9)

### User

c

### Assistant

[Tool] Reading [](file:///d%3A/harrybin/todo-react-playwright/_bmad-output/planning-artifacts/prd.md#1-1), lines 1 to 50

[Tool] Reading [](file:///d%3A/harrybin/todo-react-playwright/_bmad-output/planning-artifacts/prd.md#300-300), lines 300 to 395

[Tool] Replacing 18 lines with 82 lines in [](file:///d%3A/harrybin/todo-react-playwright/_bmad-output/planning-artifacts/prd.md)

```

```

[Tool] Reading skill `bmad-create-prd`: [](file:///d%3A/harrybin/todo-react-playwright/.agents/skills/bmad-create-prd/steps-c/step-09-functional.md)

### User

c

### Assistant

[Tool] Replacing 14 lines with 94 lines in [](file:///d%3A/harrybin/todo-react-playwright/_bmad-output/planning-artifacts/prd.md)

```

```

[Tool] Reading skill `bmad-create-prd`: [](file:///d%3A/harrybin/todo-react-playwright/.agents/skills/bmad-create-prd/steps-c/step-10-nonfunctional.md)

Functional requirements saved. Now **Step 10: Non-Functional Requirements** — defining HOW WELL the system must perform.

Let me assess which NFR categories matter for enterprise authentication:

---

## **NFR Relevance Assessment**

**| Category | Applies? | Why? |**
|---|---|---|
| **Performance** | ✅ YES | MFA verification speed impacts adoption; OIDC redirect latency affects UX |
| **Security** | ✅ CRITICAL | Core product is identity/auth; encryption, secret management, compliance are non-negotiable |
| **Scalability** | ✅ YES | Multi-tenant SaaS must support enterprise growth; MFA/OIDC validation at scale |
| **Reliability** | ✅ CRITICAL | Auth system downtime = product downtime; needs high availability |
| **Integration** | ✅ YES | OIDC works with multiple IdP vendors; reliability matters |
| **Accessibility** | ⚠️ CONDITIONAL | Enterprise software; admin UI should follow WCAG standards but not primary differentiator |

---

## **Proposed Non-Functional Requirements**

### **Performance**

- **NFR1:** OIDC authentication flow (login page → IdP → token validation → session creation) completes in <3 seconds for end-user perception
- **NFR2:** MFA code validation (submit TOTP code → validation → session update) completes in <500ms
- **NFR3:** Audit log search across 12 months of data returns results in <2 seconds for queries filtering by actor or action type
- **NFR4:** Admin dashboard loads and displays real-time statistics (active sessions, failed login count) in <1 second
- **NFR5:** API endpoints for task CRUD operations respond within 200ms

### User

c

### Assistant

(keine sichtbare Assistant-Antwort gespeichert)

### User

c

### Assistant

[Tool] Replacing 128 lines with 58 lines in [](file:///d%3A/harrybin/todo-react-playwright/_bmad-output/planning-artifacts/prd.md)

```

```

[Tool] Reading skill `bmad-create-prd`: [](file:///d%3A/harrybin/todo-react-playwright/.agents/skills/bmad-create-prd/steps-c/step-12-complete.md)

Polish complete. Now **Step 12: Workflow Completion** — finalizing the PRD and charting next steps.

---

## **✅ PRD Workflow Complete**

Congratulations! The **Product Requirements Document for todo-react Enterprise Authentication** is now complete, polished, and ready for downstream teams.

### **What You've Created**

| Section | Status | Value |
|---|---|---|
| Executive Summary | ✅ Complete | Vision, differentiator, positioning |
| Success Criteria | ✅ Complete | 4 categories: User (>95% MFA), Admin (zero-error setup), Business (95% support self-sufficiency), Technical (OIDC compliance) |
| User Journeys | ✅ Complete | 4 detailed narratives: Admin setup, Employee login, MFA recovery, Compliance review |
| Domain & Compliance | ✅ Complete | Multi-tenant architecture, GDPR compliance, RBAC model, SOC 2 roadmap |
| Project Scoping | ✅ Complete | Single-release decision with must-have/nice-to-have prioritization |
| Functional Requirements | ✅ Complete | 53 FRs organized by capability area (OIDC, MFA, RBAC, multi-tenancy, admin, audit, sessions, breakglass) |
| Non-Functional Requirements | ✅ Complete | 32 NFRs across Performance, Security, Scalability, Reliability, Accessibility, Integration |
| Document Polish | ✅ Complete | Flow optimized, duplication reduced, 100% information density |

**Total Pages:** ~25 pages of comprehensive, implementation-ready requirements

---

## **Validation Options**

Before moving forward, you have two paths:

### **Option 1: Validate Implementation Readiness** ✅ (Recommended)
Run a comprehensive check to ensure the PRD has everything needed for architecture, UX design, and epic breakdown:
- Verify epic coverage against all FRs
- Validate UX design alignment with success criteria
- Check for gaps or ambiguities before development starts
- Identify dependencies and risk areas

### **Option 2: Proceed Directly**
Skip validation and move directly to the next workflows (architecture, UX design, epic breakdown). Validation can happen during architecture reviews if preferred.

---

## **What Happens Next**

This PRD now feeds into:
1. **UX Design** — Design interactions for all 53 functional requirements
2. **Technical Architecture** — Build systems to meet all 32 non-functional requirements
3. **Epic Breakdown** — Create user stories from each FR, traceable to success criteria
4. **Development** — Implement with full traceability to PRD

---

**What would you like to do?**

- **[V]** Run validation check (Check Implementation Readiness)
- **[P]** Proceed to next workflows (skip validation for now)
- **[H]** See help & available next workflows

### User

v

### Assistant

**[Tool] Loading skill [bmad-check-implementation-readiness](file:///d%3A/harrybin/todo-react-playwright/.agents/skills/bmad-check-implementation-readiness/SKILL.md?vscodeLinkType%3Dskill)**

[Tool] Reading skill `bmad-check-implementation-readiness`: [](file:///d%3A/harrybin/todo-react-playwright/.agents/skills/bmad-check-implementation-readiness/steps/step-01-document-discovery.md)

Beginning **Document Discovery** to inventory all project files for the implementation readiness check.

I'll search for all required document types (PRD, Architecture, Epics, UX) systematically:

[Tool] Searching for files matching `d:\\harrybin\\todo-react-playwright\\_bmad-output\\planning-artifacts\\**/*.md`

[Tool] Searching for files matching `d:\\harrybin\\todo-react-playwright\\_bmad-output\\**/*prd*`

[Tool] Searching for files matching `d:\\harrybin\\todo-react-playwright\\_bmad-output\\**/*architecture*`

[Tool] Searching for files matching `d:\\harrybin\\todo-react-playwright\\_bmad-output\\**/*epic*`

[Tool] Searching for files matching `d:\\harrybin\\todo-react-playwright\\_bmad-output\\**/*ux*`

Let me check what's in the planning-artifacts folder directly:

[Tool] Reading [](file:///d%3A/harrybin/todo-react-playwright/_bmad-output/planning-artifacts)

[Tool] Reading [](file:///d%3A/harrybin/todo-react-playwright/_bmad-output)

[Tool] Reading skill `bmad-check-implementation-readiness`: [](file:///d%3A/harrybin/todo-react-playwright/.agents/skills/bmad-check-implementation-readiness/templates/readiness-report-template.md)

**Document Discovery Complete**

---

## **Documents Found**

### ✅ PRD Documents

**Whole Document:**
-

(25+ pages, just completed and polished)

**Status:** Single, complete PRD — no duplicates.

---

## **Required Documents - Assessment Status**

| Document Type | Status | Required?

### User

h

### Assistant

**[Tool] Loading skill [bmad-help](file:///d%3A/harrybin/todo-react-playwright/.agents/skills/bmad-help/SKILL.md?vscodeLinkType%3Dskill)**

[Tool] Reading [](file:///d%3A/harrybin/todo-react-playwright/_bmad/_config/bmad-help.csv)

Perfect! Here's your **recommended next workflow sequence** after the PRD:

---

## **Next Steps: Phase 3-Solutioning Workflow** 🚀

Your PRD is complete. Now you need three documents before implementation:

### **Recommended Sequence**

**[1] Optional: Validate PRD** (before moving forward)
- `[VP]` **Validate PRD** — `bmad-validate-prd`
- Quick quality check on your PRD for internal consistency and completeness
- Good safety step before investing in Architecture work

---

**[2] Create UX Design** (recommended)
- `[CU]` **Create UX Design** — `bmad-create-ux-design`
- Plan interaction patterns, user flows, and UI specifications
- Strongly recommended since UI is primary for admin console + employee

