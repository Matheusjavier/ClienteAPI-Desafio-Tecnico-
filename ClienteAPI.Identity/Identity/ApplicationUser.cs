using Microsoft.AspNetCore.Identity;

namespace ClienteAPI.Identity
{
    /// <summary>
    /// Classe de usuário personalizada para o ASP.NET Core Identity.
    /// Estende as funcionalidades padrão de IdentityUser.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public string Nome { get; set; } = string.Empty;
    }
}