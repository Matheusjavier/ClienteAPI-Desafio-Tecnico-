using ClienteAPI.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClienteAPI.Services.Interfaces
{
    /// <summary>
    /// Define os contratos de serviço para operações relacionadas a clientes.
    /// </summary>
    public interface IClienteService
    {
        /// <summary>
        /// Obtém um cliente pelo seu Id.
        /// </summary>
        /// <param name="id">O Id do cliente.</param>
        /// <returns>O cliente encontrado.</returns>
        Task<Cliente> GetClienteByIdAsync(int id);

        /// <summary>
        /// Obtém todos os clientes.
        /// </summary>
        /// <returns>Uma coleção de todos os clientes.</returns>
        Task<IEnumerable<Cliente>> GetAllClientesAsync();

        /// <summary>
        /// Adiciona um novo cliente.
        /// </summary>
        /// <param name="cliente">O cliente a ser adicionado.</param>
        /// <returns>O cliente adicionado.</returns>
        Task<Cliente> AddClienteAsync(Cliente cliente);

        /// <summary>
        /// Atualiza as informações de um cliente existente.
        /// </summary>
        /// <param name="cliente">O cliente com as informações atualizadas.</param>
        Task UpdateClienteAsync(Cliente cliente);

        /// <summary>
        /// Exclui um cliente pelo seu Id.
        /// </summary>
        /// <param name="id">O Id do cliente a ser excluído.</param>
        Task DeleteClienteAsync(int id);
    }
}