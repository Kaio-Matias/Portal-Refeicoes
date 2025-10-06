using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Portal_Refeicoes.Services;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Portal_Refeicoes.Pages.Usuarios
{
 
    public class CreateModel : PageModel
    {
        private readonly ApiClient _apiClient;

        public CreateModel(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "O nome de usu�rio � obrigat�rio.")]
            [Display(Name = "Nome de Usu�rio")]
            public string Username { get; set; }

            [Required(ErrorMessage = "A senha � obrigat�ria.")]
            [StringLength(100, ErrorMessage = "A {0} deve ter no m�nimo {2} e no m�ximo {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Senha")]
            public string Password { get; set; }

            [Required(ErrorMessage = "O perfil � obrigat�rio.")]
            public string Role { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var success = await _apiClient.CreateUsuarioAsync(Input);
            if (success)
            {
                return RedirectToPage("./Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "N�o foi poss�vel criar o usu�rio. Verifique se o nome de usu�rio j� existe e tente novamente.");
                return Page();
            }
        }
    }
}