# Vulnerability Assessment

## 1. Global CSRF protection disabled
- **Location:** `Program.cs`, lines 9-12
- **Description:** The application registers `IgnoreAntiforgeryTokenAttribute` as a global Razor Pages filter, effectively disabling ASP.NET Core's anti-forgery protection for every page. Any future form or API handler that relies on antiforgery tokens becomes vulnerable to cross-site request forgery because requests from other origins will be accepted without validation.
- **Impact:** High. Attackers can trick authenticated users into issuing state-changing requests, potentially altering pipeline configurations or triggering builds under the victim's identity.
- **Recommendation:** Remove the global `IgnoreAntiforgeryTokenAttribute` registration and rely on the default antiforgery protections. Only opt out per-handler when there is a strong justification, such as read-only GET endpoints.

## 2. Sensitive Azure DevOps identifiers logged verbatim
- **Location:** `Services/AzureCliBuildRunService.cs`, lines 54-56
- **Description:** The Azure CLI command, including the organization URL, project name, and requested-for user, is logged at information level before execution. These values can contain personally identifiable information (e.g., user email addresses) or internal project names. Persisting them in logs risks exposing sensitive data to unauthorized readers or log aggregation systems with broader access.
- **Impact:** Medium. Information disclosure through logs can aid attackers in reconnaissance or violate privacy requirements.
- **Recommendation:** Downgrade the log level and redact or omit sensitive parameters before logging. Alternatively, log a generic message that the CLI command is being executed without echoing user-specific values.
