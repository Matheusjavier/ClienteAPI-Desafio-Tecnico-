using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClienteAPI.Domain.Interfaces
{
    public interface ILogradouroRepository
    {
        // CRUD para Logradouro
        Task<Logradouro> GetByIdAsync(int id);
        Task<IEnumerable<Logradouro>> GetAllAsync(); // Pode não ser necessário um "GetAll" para Logradouros soltos
        Task<Logradouro> AddAsync(Logradouro logradouro);
        Task UpdateAsync(Logradouro logradouro);
        Task DeleteAsync(int id);

        // Método para obter logradouros por ClienteId, se necessário
        Task<IEnumerable<Logradouro>> GetByClienteIdAsync(int clienteId);
    }
}