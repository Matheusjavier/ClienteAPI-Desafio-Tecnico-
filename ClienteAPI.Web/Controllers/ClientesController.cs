using ClienteAPI.Domain;
using ClienteAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ClienteAPI.Web.Controllers
{
    /// <summary>
    /// Controlador de API para a gestão de clientes.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Aplica autorização baseada em JWT a todos os endpoints deste controlador.
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _clienteService;
        private readonly ClienteAPI.Data.Contexts.ClienteDbContext _dbContext; // Utilizado para operações de Stored Procedure.

        public ClientesController(IClienteService clienteService,
                                  ClienteAPI.Data.Contexts.ClienteDbContext dbContext)
        {
            _clienteService = clienteService;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Obtém a lista de todos os clientes.
        /// </summary>
        /// <returns>Uma coleção de objetos Cliente.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
        {
            var clientes = await _clienteService.GetAllClientesAsync();
            return Ok(clientes); // Retorna status 200 OK com os clientes.
        }

        /// <summary>
        /// Obtém um cliente específico pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do cliente.</param>
        /// <returns>Um objeto Cliente ou status 404 Not Found se não encontrado.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(int id)
        {
            try
            {
                var cliente = await _clienteService.GetClienteByIdAsync(id);
                return Ok(cliente);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(); // Retorna 404 Not Found se o cliente não existe.
            }
            catch (Exception ex)
            {
                // Um sistema de log real registraria 'ex' aqui.
                return StatusCode(500, $"Um erro interno do servidor ocorreu: {ex.Message}");
            }
        }

        /// <summary>
        /// Adiciona um novo cliente ao sistema.
        /// </summary>
        /// <param name="cliente">Os dados do cliente a ser adicionado.</param>
        /// <returns>O cliente criado com status 201 Created.</returns>
        [HttpPost]
        public async Task<ActionResult<Cliente>> PostCliente([FromBody] Cliente cliente)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Retorna 400 Bad Request se a validação do modelo falhar.
            }

            try
            {
                var newCliente = await _clienteService.AddClienteAsync(cliente);
                // Retorna 201 Created e o caminho para o novo recurso.
                return CreatedAtAction(nameof(GetCliente), new { id = newCliente.Id }, newCliente);
            }
            catch (ApplicationException ex) // Captura exceções de regra de negócio, como e-mail duplicado.
            {
                return BadRequest(new { message = ex.Message }); // Retorna 400 Bad Request com a mensagem de erro.
            }
            catch (Exception ex)
            {
                // Um sistema de log real registraria 'ex' aqui.
                return StatusCode(500, $"Um erro interno do servidor ocorreu: {ex.Message}");
            }
        }

        /// <summary>
        /// Atualiza as informações de um cliente existente.
        /// </summary>
        /// <param name="id">O ID do cliente a ser atualizado (da URL).</param>
        /// <param name="cliente">Os novos dados do cliente.</param>
        /// <returns>Status 204 No Content se a atualização for bem-sucedida.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(int id, [FromBody] Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return BadRequest("O ID na URL não corresponde ao ID do cliente no corpo da requisição.");
            }

            // Garante que o ID do objeto cliente seja o da URL para processamento no serviço.
            cliente.Id = id;

            try
            {
                await _clienteService.UpdateClienteAsync(cliente);
                return NoContent(); // Retorna 204 No Content para uma atualização bem-sucedida sem conteúdo de retorno.
            }
            catch (KeyNotFoundException)
            {
                return NotFound(); // Retorna 404 Not Found se o cliente não for encontrado.
            }
            catch (ApplicationException ex) // Captura exceções de regra de negócio.
            {
                return BadRequest(new { message = ex.Message }); // Retorna 400 Bad Request com a mensagem de erro.
            }
            catch (Exception ex)
            {
                // Um sistema de log real registraria 'ex' aqui.
                return StatusCode(500, $"Um erro interno do servidor ocorreu: {ex.Message}");
            }
        }

        /// <summary>
        /// Exclui um cliente existente pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do cliente a ser excluído.</param>
        /// <returns>Status 204 No Content se a exclusão for bem-sucedida.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            try
            {
                await _clienteService.DeleteClienteAsync(id);
                return NoContent(); // Retorna 204 No Content para uma exclusão bem-sucedida.
            }
            catch (KeyNotFoundException)
            {
                return NotFound(); // Retorna 404 Not Found se o cliente não for encontrado.
            }
            catch (Exception ex)
            {
                // Um sistema de log real registraria 'ex' aqui.
                return StatusCode(500, $"Um erro interno do servidor ocorreu: {ex.Message}");
            }
        }

        /// <summary>
        /// Busca clientes por nome parcial utilizando uma Stored Procedure.
        /// </summary>
        /// <param name="nome">O nome parcial para buscar clientes.</param>
        /// <returns>Uma coleção de clientes que correspondem ao nome, ou 404 Not Found.</returns>
        [HttpGet("search-by-sp")]
        public async Task<ActionResult<IEnumerable<Cliente>>> SearchClientsBySp([FromQuery] string? nome)
        {
            // O EF Core parametrizará a consulta automaticamente, ajudando a prevenir SQL Injection.
            var clientes = await _dbContext.Clientes
                                         .FromSqlRaw("EXEC GetClientesByName @NomeParcial = {0}", nome)
                                         .ToListAsync();

            if (clientes == null || !clientes.Any())
            {
                return NotFound("Nenhum cliente encontrado com o nome parcial fornecido.");
            }

            return Ok(clientes);
        }
    }
}