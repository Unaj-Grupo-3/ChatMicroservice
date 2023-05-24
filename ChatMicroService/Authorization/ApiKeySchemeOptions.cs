using Microsoft.AspNetCore.Authentication;
using Microsoft.Net.Http.Headers;

namespace Presentation.Authorization;

public class ApiKeySchemeOptions : AuthenticationSchemeOptions
{
    public const string Scheme = "ApiKeyScheme";

    /// Nombre del Header donde se buscará la API Key
    /// Default: Authorization
    public string HeaderName { get; set; } = HeaderNames.Authorization;
}