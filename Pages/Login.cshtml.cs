using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt; // Necessário para ler o Token
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Portal_Refeicoes.Pages
{
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public LoginModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        private class TokenResponse
        {
            public string Token { get; set; }
        }

        public class InputModel
        {
            [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
            [Display(Name = "Usuário")]
            public string Username { get; set; }

            [Required(ErrorMessage = "A senha é obrigatória.")]
            [DataType(DataType.Password)]
            [Display(Name = "Senha")]
            public string Password { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }
            ReturnUrl = returnUrl;
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var client = _clientFactory.CreateClient("ApiClient");
            var loginData = new { username = Input.Username, password = Input.Password };
            var content = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");

            // Chama a API para obter o Token JWT
            var response = await client.PostAsync("api/auth/login", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Login ou senha inválidos.");
                return Page();
            }

            var responseStream = await response.Content.ReadAsStreamAsync();
            var tokenObject = await JsonSerializer.DeserializeAsync<TokenResponse>(responseStream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var tokenString = tokenObject?.Token;

            if (string.IsNullOrEmpty(tokenString))
            {
                ModelState.AddModelError(string.Empty, "Token não recebido da API.");
                return Page();
            }

            // --- CORREÇÃO PRINCIPAL: Decodificar e Extrair Claims ---
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(tokenString);
            var claimsFromToken = jwtToken.Claims;

            // Tenta obter o nome de usuário de diferentes padrões de claims
            var userName = claimsFromToken.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ??
                           claimsFromToken.FirstOrDefault(c => c.Type == "unique_name")?.Value ??
                           Input.Username;

            // Tenta obter a Role. O JWT pode enviar como "role" ou o URI completo do ClaimTypes.Role
            var roleClaim = claimsFromToken.FirstOrDefault(c => c.Type == "role" || c.Type == ClaimTypes.Role)?.Value;

            // Configura as Claims da Sessão (Cookie)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim("access_token", tokenString) // Salva o token para chamadas futuras
            };

            // Se encontrou uma role no token, adiciona à identidade como ClaimTypes.Role
            // Isso permite que User.IsInRole("Admin") funcione nas Views e Pages
            if (!string.IsNullOrEmpty(roleClaim))
            {
                claims.Add(new Claim(ClaimTypes.Role, roleClaim));
            }

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Mantém o login mesmo fechando o navegador (opcional)
                ExpiresUtc = System.DateTime.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return LocalRedirect(ReturnUrl);
        }
    }
}