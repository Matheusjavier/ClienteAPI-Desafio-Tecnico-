namespace ClienteAPI.Web.Models
{
    /// <summary>
    /// Representa os dados da requisição para autenticação de usuário (login).
    /// </summary>
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}