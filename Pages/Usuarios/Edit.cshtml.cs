using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Portal_Refeicoes.Services;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Portal_Refeicoes.Pages.Usuarios
{
   
    public class EditModel : PageModel
    {
        private readonly ApiClient _apiClient;

        public EditModel(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
            [Display(Name = "Nome de Usuário")]
            public string Username { get; set; }

            [StringLength(100, ErrorMessage = "A {0} deve ter no mínimo {2} e no máximo {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Nova Senha")]
            public string? Password { get; set; }

            [Required(ErrorMessage = "O perfil é obrigatório.")]
            public string Role { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var usuario = await _apiClient.GetUsuarioByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            Input = new InputModel
            {
                Username = usuario.Username,
                Role = usuario.Role
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var success = await _apiClient.UpdateUsuarioAsync(id, Input);
            if (success)
            {
                return RedirectToPage("./Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Não foi possível atualizar o usuário. Tente novamente.");
                return Page();
            }
        }
    }
}