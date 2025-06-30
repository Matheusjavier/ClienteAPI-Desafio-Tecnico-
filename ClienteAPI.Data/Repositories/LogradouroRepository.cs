using ClienteAPI.Data.Contexts;
using ClienteAPI.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClienteAPI.Data.Repositories
{
    /// <summary>
    /// Fornece métodos para acesso a dados de Logradouros no banco de dados.
    /// </summary>
    public class LogradouroRepository
    {
        private readonly ClienteDbContext _context;

        public LogradouroRepository(ClienteDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adiciona um novo logradouro ao banco de dados.
        /// </summary>
        /// <param name="logradouro">O logradouro a ser adicionado.</param>
        /// <returns>O logradouro adicionado, com seu Id atualizado.</returns>
        public async Task<Logradouro> AddAsync(Logradouro logradouro)
        {
            _context.Logradouros.Add(logradouro);
            await _context.SaveChangesAsync();
            return logradouro;
        }

        /// <summary>
        /// Remove um logradouro do banco de dados pelo seu Id.
        /// </summary>
        /// <param name="id">O Id do logradouro a ser removido.</param>
        /// <returns>True se o logradouro foi removido com sucesso, False caso contrário (não encontrado).</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            var logradouro = await _context.Logradouros.FindAsync(id);
            if (logradouro == null)
            {
                return false;
            }

            _context.Logradouros.Remove(logradouro);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Obtém todos os logradouros do banco de dados.
        /// </summary>
        /// <returns>Uma coleção de todos os logradouros.</returns>
        public async Task<IEnumerable<Logradouro>> GetAllAsync()
        {
            return await _context.Logradouros.ToListAsync();
        }

        /// <summary>
        /// Obtém logradouros associados a um Cliente específico pelo ClienteId.
        /// </summary>
        /// <param name="clienteId">O Id do cliente.</param>
        /// <returns>Uma coleção de logradouros do cliente especificado.</returns>
        public async Task<IEnumerable<Logradouro>> GetByClienteIdAsync(int clienteId)
        {
            return await _context.Logradouros.Where(l => l.ClienteId == clienteId).ToListAsync();
        }

        /// <summary>
        /// Obtém um logradouro específico pelo seu Id.
        /// </summary>
        /// <param name="id">O Id do logradouro.</param>
        /// <returns>O logradouro encontrado, ou null se não for encontrado.</returns>
        public async Task<Logradouro?> GetByIdAsync(int id)
        {
            return await _context.Logradouros.FirstOrDefaultAsync(l => l.Id == id);
        }

        /// <summary>
        /// Atualiza as informações de um logradouro existente no banco de dados.
        /// </summary>
        /// <param name="logradouro">O logradouro com as informações atualizadas.</param>
        /// <returns>O logradouro atualizado.</returns>
        public async Task<Logradouro> UpdateAsync(Logradouro logradouro)
        {
            _context.Entry(logradouro).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return logradouro;
        }
    }
}