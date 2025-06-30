using ClienteAPI.Domain;
using ClienteAPI.Data.Repositories; // Usar namespace dos repositórios
using ClienteAPI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClienteAPI.Services.Implementations
{
    /// <summary>
    /// Implementa a lógica de negócio para a gestão de logradouros.
    /// </summary>
    public class LogradouroService : ILogradouroService
    {
        private readonly LogradouroRepository _logradouroRepository;
        private readonly ClienteRepository _clienteRepository;

        public LogradouroService(LogradouroRepository logradouroRepository, ClienteRepository clienteRepository)
        {
            _logradouroRepository = logradouroRepository;
            _clienteRepository = clienteRepository;
        }

        /// <summary>
        /// Adiciona um novo logradouro, garantindo que o cliente associado exista.
        /// </summary>
        /// <param name="logradouro">O logradouro a ser adicionado.</param>
        /// <returns>O logradouro adicionado.</returns>
        /// <exception cref="KeyNotFoundException">Lançada se o cliente associado não for encontrado.</exception>
        public async Task<Logradouro> AddLogradouroAsync(Logradouro logradouro)
        {
            var clienteExiste = await _clienteRepository.GetByIdAsync(logradouro.ClienteId);
            if (clienteExiste == null)
            {
                throw new KeyNotFoundException($"Cliente com ID {logradouro.ClienteId} não encontrado. Não é possível adicionar logradouro.");
            }
            return await _logradouroRepository.AddAsync(logradouro);
        }

        /// <summary>
        /// Obtém todos os logradouros.
        /// </summary>
        /// <returns>Uma coleção de todos os logradouros.</returns>
        public async Task<IEnumerable<Logradouro>> GetAllLogradourosAsync()
        {
            return await _logradouroRepository.GetAllAsync();
        }

        /// <summary>
        /// Exclui um logradouro pelo seu Id.
        /// </summary>
        /// <param name="id">O Id do logradouro a ser excluído.</param>
        /// <exception cref="KeyNotFoundException">Lançada se o logradouro não for encontrado.</exception>
        public async Task DeleteLogradouroAsync(int id)
        {
            var wasDeleted = await _logradouroRepository.DeleteAsync(id);
            if (!wasDeleted)
            {
                throw new KeyNotFoundException($"Logradouro com ID {id} não encontrado para exclusão.");
            }
        }

        /// <summary>
        /// Obtém um logradouro pelo seu Id.
        /// </summary>
        /// <param name="id">O Id do logradouro.</param>
        /// <returns>O logradouro encontrado.</returns>
        /// <exception cref="KeyNotFoundException">Lançada se o logradouro não for encontrado.</exception>
        public async Task<Logradouro> GetLogradouroByIdAsync(int id)
        {
            var logradouro = await _logradouroRepository.GetByIdAsync(id);
            if (logradouro == null)
            {
                throw new KeyNotFoundException($"Logradouro com ID {id} não encontrado.");
            }
            return logradouro;
        }

        /// <summary>
        /// Obtém todos os logradouros associados a um cliente específico.
        /// </summary>
        /// <param name="clienteId">O Id do cliente.</param>
        /// <returns>Uma coleção de logradouros do cliente especificado.</returns>
        /// <exception cref="KeyNotFoundException">Lançada se o cliente não for encontrado.</exception>
        public async Task<IEnumerable<Logradouro>> GetLogradourosByClienteIdAsync(int clienteId)
        {
            var clienteExiste = await _clienteRepository.GetByIdAsync(clienteId);
            if (clienteExiste == null)
            {
                throw new KeyNotFoundException($"Cliente com ID {clienteId} não encontrado.");
            }
            return await _logradouroRepository.GetByClienteIdAsync(clienteId);
        }

        /// <summary>
        /// Atualiza as informações de um logradouro existente, verificando a existência do cliente associado.
        /// </summary>
        /// <param name="logradouro">O logradouro com as informações atualizadas.</param>
        /// <exception cref="KeyNotFoundException">Lançada se o logradouro ou o novo cliente associado não for encontrado.</exception>
        public async Task UpdateLogradouroAsync(Logradouro logradouro)
        {
            var existingLogradouro = await _logradouroRepository.GetByIdAsync(logradouro.Id);
            if (existingLogradouro == null)
            {
                throw new KeyNotFoundException($"Logradouro com ID {logradouro.Id} não encontrado para atualização.");
            }

            // Verifica se o ClienteId foi alterado e se o novo cliente existe.
            if (existingLogradouro.ClienteId != logradouro.ClienteId)
            {
                var clienteExiste = await _clienteRepository.GetByIdAsync(logradouro.ClienteId);
                if (clienteExiste == null)
                {
                    throw new KeyNotFoundException($"Cliente com ID {logradouro.ClienteId} não encontrado. Não é possível associar logradouro.");
                }
            }

            // Atualiza as propriedades do logradouro existente.
            existingLogradouro.NomeLogradouro = logradouro.NomeLogradouro;
            existingLogradouro.Numero = logradouro.Numero;
            existingLogradouro.Complemento = logradouro.Complemento;
            existingLogradouro.Bairro = logradouro.Bairro;
            existingLogradouro.Cidade = logradouro.Cidade;
            existingLogradouro.Estado = logradouro.Estado;
            existingLogradouro.CEP = logradouro.CEP;
            existingLogradouro.ClienteId = logradouro.ClienteId;

            await _logradouroRepository.UpdateAsync(existingLogradouro);
        }
    }
}