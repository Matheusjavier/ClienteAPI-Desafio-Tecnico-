using System.Text.Json.Serialization;

namespace ClienteAPI.Domain
{
    /// <summary>
    /// Representa um logradouro associado a um cliente.
    /// </summary>
    public class Logradouro
    {
        public int Id { get; set; }

        public string? NomeLogradouro { get; set; }
        public string? Numero { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? CEP { get; set; }

        public int ClienteId { get; set; }

        [JsonIgnore] // Impede ciclos de referência na serialização JSON.
        public Cliente? Cliente { get; set; }
    }
}