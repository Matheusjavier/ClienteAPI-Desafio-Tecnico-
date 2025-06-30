using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClienteAPI.Domain
{
    /// <summary>
    /// Representa um cliente no sistema.
    /// </summary>
    public class Cliente
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Logotipo { get; set; }

        [JsonIgnore] // Impede ciclos de referência na serialização JSON.
        public ICollection<Logradouro>? Logradouros { get; set; }
    }
}