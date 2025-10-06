using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
// --- CORRE��O (CS0118): Refer�ncia expl�cita ao modelo para evitar conflito de nome ---
using Portal_Refeicoes.Models;
using Portal_Refeicoes.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Portal_Refeicoes.Pages.ParadaDeFabrica
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApiClient _apiClient;

        public IndexModel(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        // --- CORRE��O (CS0118): Usa o nome completo do tipo ---
        public IList<Models.ParadaDeFabrica> Paradas { get; set; } = new List<Models.ParadaDeFabrica>();

        [BindProperty]
        // --- CORRE��O (CS0118): Usa o nome completo do tipo ---
        public Models.ParadaDeFabrica NovaParada { get; set; }

        
    }
}