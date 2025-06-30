using ClienteAPI.Domain;
using ClienteAPI.Data.Repositories;
using ClienteAPI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClienteAPI.Services.Implementations
{
    /// <summary>
    /// Implementa a lógica de negócio para a gestão de clientes.
    /// </summary>
    public class ClienteService : IClienteService
    {
        private readonly ClienteRepository _clienteRepository;

        public ClienteService(ClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        /// <summary>
        /// Adiciona um novo cliente.
        /// </summary>
        /// <param name="cliente">O cliente a ser adicionado.</param>
        /// <returns>O cliente adicionado.</returns>
        public async Task<Cliente> AddClienteAsync(Cliente cliente)
        {
            return await _clienteRepository.AddAsync(cliente);
        }

        /// <summary>
        /// Exclui um cliente pelo seu Id.
        /// </summary>
        /// <param name="id">O Id do cliente a ser excluído.</param>
        /// <exception cref="KeyNotFoundException">Lançada se o cliente não for encontrado.</exception>
        public async Task DeleteClienteAsync(int id)
        {
            var wasDeleted = await _clienteRepository.DeleteAsync(id);
            if (!wasDeleted)
            {
                throw new KeyNotFoundException($"Cliente com ID {id} não encontrado para exclusão.");
            }
        }

        /// <summary>
        /// Obtém todos os clientes.
        /// </summary>
        /// <returns>Uma coleção de todos os clientes.</returns>
        public async Task<IEnumerable<Cliente>> GetAllClientesAsync()
        {
            return await _clienteRepository.GetAllAsync();
        }

        /// <summary>
        /// Obtém um cliente pelo seu Id.
        /// </summary>
        /// <param name="id">O Id do cliente.</param>
        /// <returns>O cliente encontrado.</returns>
        /// <exception cref="KeyNotFoundException">Lançada se o cliente não for encontrado.</exception>
        public async Task<Cliente> GetClienteByIdAsync(int id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null)
            {
                throw new KeyNotFoundException($"Cliente com ID {id} não encontrado.");
            }
            return cliente;
        }

        /// <summary>
        /// Atualiza as informações de um cliente existente.
        /// </summary>
        /// <param name="cliente">O cliente com as informações atualizadas.</param>
        /// <exception cref="KeyNotFoundException">Lançada se o cliente não for encontrado para atualização.</exception>
        public async Task UpdateClienteAsync(Cliente cliente)
        {
            var existingCliente = await _clienteRepository.GetByIdAsync(cliente.Id);
            if (existingCliente == null)
            {
                throw new KeyNotFoundException($"Cliente com ID {cliente.Id} não encontrado para atualização.");
            }

            // Atualiza as propriedades do cliente existente.
            existingCliente.Nome = cliente.Nome;
            existingCliente.Email = cliente.Email;
            existingCliente.Logotipo = cliente.Logotipo;

            await _clienteRepository.UpdateAsync(existingCliente);
        }
    }
}