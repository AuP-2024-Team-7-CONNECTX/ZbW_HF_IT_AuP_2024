using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly OAuth2Configuration _config;

    public AuthenticationController(OAuth2Configuration config)
    {
        _config = config;
    }

    [HttpGet]
    [Route("start")]
    public IActionResult StartAuthentication()
    {
        var authorizationRequest = $"{_config.AuthorizationEndpoint}?response_type=code&client_id={_config.ClientId}&redirect_uri={_config.RedirectUri}&scope=email";
        return Redirect(authorizationRequest);
    }

    [HttpGet]
    [Route("callback")]
    public async Task<IActionResult> CallbackAsync(string code)
    {
        using var client = new HttpClient();
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("redirect_uri", _config.RedirectUri),
            new KeyValuePair<string, string>("client_id", _config.ClientId),
            new KeyValuePair<string, string>("client_secret", _config.ClientSecret),
        });

        var response = await client.PostAsync(_config.TokenEndpoint, content);
        if (!response.IsSuccessStatusCode)
        {
            return BadRequest();
        }

        var tokenResponse = await response.Content.ReadAsStringAsync();
        // Hier solltest du das Token speichern und den Benutzer als authentifiziert markieren.

        // Weiterleitung zur E-Mail-Verifizierung oder zur Startseite
        return Redirect("/home");
    }
}
