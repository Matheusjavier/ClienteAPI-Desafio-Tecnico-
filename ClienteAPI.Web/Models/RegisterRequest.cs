namespace ClienteAPI.Web.Models
{
    /// <summary>
    /// Representa os dados da requisição para registro de um novo usuário.
    /// </summary>
    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
    }
}