using ClienteAPI.Data.Contexts;
using ClienteAPI.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClienteAPI.Data.Repositories
{
    /// <summary>
    /// Fornece métodos para acesso a dados de Clientes no banco de dados.
    /// </summary>
    public class ClienteRepository
    {
        private readonly ClienteDbContext _context;

        public ClienteRepository(ClienteDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adiciona um novo cliente ao banco de dados.
        /// </summary>
        /// <param name="cliente">O cliente a ser adicionado.</param>
        /// <returns>O cliente adicionado, com seu Id atualizado.</returns>
        public async Task<Cliente> AddAsync(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            return cliente;
        }

        /// <summary>
        /// Remove um cliente do banco de dados pelo seu Id.
        /// </summary>
        /// <param name="id">O Id do cliente a ser removido.</param>
        /// <returns>True se o cliente foi removido com sucesso, False caso contrário (não encontrado).</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return false;
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Obtém todos os clientes do banco de dados, incluindo seus logradouros.
        /// </summary>
        /// <returns>Uma coleção de todos os clientes.</returns>
        public async Task<IEnumerable<Cliente>> GetAllAsync()
        {
            return await _context.Clientes.Include(c => c.Logradouros).ToListAsync();
        }

        /// <summary>
        /// Obtém um cliente específico pelo seu Id, incluindo seus logradouros.
        /// </summary>
        /// <param name="id">O Id do cliente.</param>
        /// <returns>O cliente encontrado, ou null se não for encontrado.</returns>
        public async Task<Cliente?> GetByIdAsync(int id)
        {
            return await _context.Clientes.Include(c => c.Logradouros).FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <summary>
        /// Atualiza as informações de um cliente existente no banco de dados.
        /// </summary>
        /// <param name="cliente">O cliente com as informações atualizadas.</param>
        /// <returns>O cliente atualizado.</returns>
        public async Task<Cliente> UpdateAsync(Cliente cliente)
        {
            _context.Entry(cliente).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return cliente;
        }
    }
}