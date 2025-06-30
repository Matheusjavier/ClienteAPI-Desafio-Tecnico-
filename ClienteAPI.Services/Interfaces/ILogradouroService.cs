using ClienteAPI.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClienteAPI.Services.Interfaces
{
    /// <summary>
    /// Define os contratos de serviço para operações relacionadas a logradouros.
    /// </summary>
    public interface ILogradouroService
    {
        /// <summary>
        /// Obtém um logradouro pelo seu Id.
        /// </summary>
        /// <param name="id">O Id do logradouro.</param>
        /// <returns>O logradouro encontrado.</returns>
        Task<Logradouro> GetLogradouroByIdAsync(int id);

        /// <summary>
        /// Obtém todos os logradouros associados a um cliente específico.
        /// </summary>
        /// <param name="clienteId">O Id do cliente.</param>
        /// <returns>Uma coleção de logradouros do cliente especificado.</returns>
        Task<IEnumerable<Logradouro>> GetLogradourosByClienteIdAsync(int clienteId);

        /// <summary>
        /// Adiciona um novo logradouro.
        /// </summary>
        /// <param name="logradouro">O logradouro a ser adicionado.</param>
        /// <returns>O logradouro adicionado.</returns>
        Task<Logradouro> AddLogradouroAsync(Logradouro logradouro);

        /// <summary>
        /// Atualiza as informações de um logradouro existente.
        /// </summary>
        /// <param name="logradouro">O logradouro com as informações atualizadas.</param>
        Task UpdateLogradouroAsync(Logradouro logradouro);

        /// <summary>
        /// Exclui um logradouro pelo seu Id.
        /// </summary>
        /// <param name="id">O Id do logradouro a ser excluído.</param>
        Task DeleteLogradouroAsync(int id);

        /// <summary>
        /// Obtém todos os logradouros.
        /// </summary>
        /// <returns>Uma coleção de todos os logradouros.</returns>
        Task<IEnumerable<Logradouro>> GetAllLogradourosAsync();
    }
}