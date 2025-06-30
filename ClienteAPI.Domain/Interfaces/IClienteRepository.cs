using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClienteAPI.Domain.Interfaces
{
    public interface IClienteRepository
    {
        // CRUD para Cliente
        Task<Cliente> GetByIdAsync(int id);
        Task<IEnumerable<Cliente>> GetAllAsync();
        Task<Cliente> AddAsync(Cliente cliente);
        Task UpdateAsync(Cliente cliente);
        Task DeleteAsync(int id);

        // Método específico para verificar a unicidade do e-mail
        Task<Cliente> GetByEmailAsync(string email);
    }
}