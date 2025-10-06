using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Portal_Refeicoes.Pages.Usuarios
{
    
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        public IndexModel(IHttpClientFactory factory) { _clientFactory = factory; }

        public List<Portal_Refeicoes.Models.Usuario> Usuarios { get; set; } = new();

        public async Task OnGetAsync()
        {
            var client = _clientFactory.CreateClient("ApiClient");
            var token = User.FindFirst("access_token")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync("/api/usuarios");
            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                Usuarios = await JsonSerializer.DeserializeAsync<List<Portal_Refeicoes.Models.Usuario>>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
        }
    }
}