using ClienteAPI.Domain;
using ClienteAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClienteAPI.Web.Controllers
{
    /// <summary>
    /// Controlador de API para a gestão de logradouros.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Aplica autorização baseada em JWT a todos os endpoints deste controlador.
    public class LogradourosController : ControllerBase
    {
        private readonly ILogradouroService _logradouroService;

        public LogradourosController(ILogradouroService logradouroService)
        {
            _logradouroService = logradouroService;
        }

        /// <summary>
        /// Obtém a lista de todos os logradouros.
        /// </summary>
        /// <returns>Uma coleção de objetos Logradouro.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Logradouro>>> GetAllLogradouros()
        {
            var logradouros = await _logradouroService.GetAllLogradourosAsync();
            return Ok(logradouros);
        }

        /// <summary>
        /// Obtém um logradouro específico pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do logradouro.</param>
        /// <returns>Um objeto Logradouro ou status 404 Not Found se não encontrado.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Logradouro>> GetLogradouro(int id)
        {
            try
            {
                var logradouro = await _logradouroService.GetLogradouroByIdAsync(id);
                return Ok(logradouro);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(); // Retorna 404 Not Found se o logradouro não existe.
            }
            catch (Exception ex)
            {
                // Um sistema de log real registraria 'ex' aqui.
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Busca todos os logradouros associados a um cliente específico pelo ID do cliente.
        /// </summary>
        /// <param name="clienteId">O ID do cliente.</param>
        /// <returns>Uma coleção de objetos Logradouro ou status 404 Not Found se o cliente não for encontrado.</returns>
        [HttpGet("ByCliente/{clienteId}")]
        public async Task<ActionResult<IEnumerable<Logradouro>>> GetLogradourosByCliente(int clienteId)
        {
            try
            {
                var logradouros = await _logradouroService.GetLogradourosByClienteIdAsync(clienteId);
                return Ok(logradouros);
            }
            catch (KeyNotFoundException ex) // Captura exceção se o cliente não for encontrado.
            {
                return NotFound(new { message = ex.Message }); // Retorna 404 Not Found com a mensagem.
            }
            catch (Exception ex)
            {
                // Um sistema de log real registraria 'ex' aqui.
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Adiciona um novo logradouro ao sistema.
        /// </summary>
        /// <param name="logradouro">Os dados do logradouro a ser adicionado.</param>
        /// <returns>O logradouro criado com status 201 Created.</returns>
        [HttpPost]
        public async Task<ActionResult<Logradouro>> AddLogradouro([FromBody] Logradouro logradouro)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Retorna 400 Bad Request se a validação do modelo falhar.
            }

            try
            {
                var newLogradouro = await _logradouroService.AddLogradouroAsync(logradouro);
                // Retorna 201 Created e o caminho para o novo recurso.
                return CreatedAtAction(nameof(GetLogradouro), new { id = newLogradouro.Id }, newLogradouro);
            }
            catch (KeyNotFoundException ex) // Para quando o ClienteId fornecido não for encontrado.
            {
                return BadRequest(new { message = ex.Message }); // Retorna 400 Bad Request com a mensagem de erro.
            }
            catch (Exception ex)
            {
                // Um sistema de log real registraria 'ex' aqui.
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Atualiza as informações de um logradouro existente.
        /// </summary>
        /// <param name="id">O ID do logradouro a ser atualizado (da URL).</param>
        /// <param name="logradouro">Os novos dados do logradouro.</param>
        /// <returns>Status 204 No Content se a atualização for bem-sucedida.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLogradouro(int id, [FromBody] Logradouro logradouro)
        {
            if (id != logradouro.Id)
            {
                return BadRequest("O ID na URL não corresponde ao ID do logradouro no corpo da requisição.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            logradouro.Id = id; // Garante que o ID do corpo seja o da URL para consistência.

            try
            {
                await _logradouroService.UpdateLogradouroAsync(logradouro);
                return NoContent(); // Retorna 204 No Content para uma atualização bem-sucedida sem conteúdo de retorno.
            }
            catch (KeyNotFoundException ex) // Para logradouro não encontrado ou ClienteId inválido.
            {
                return NotFound(new { message = ex.Message }); // Retorna 404 Not Found com a mensagem de erro.
            }
            catch (Exception ex)
            {
                // Um sistema de log real registraria 'ex' aqui.
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Exclui um logradouro existente pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do logradouro a ser excluído.</param>
        /// <returns>Status 204 No Content se a exclusão for bem-sucedida.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLogradouro(int id)
        {
            try
            {
                await _logradouroService.DeleteLogradouroAsync(id);
                return NoContent(); // Retorna 204 No Content para uma exclusão bem-sucedida.
            }
            catch (KeyNotFoundException)
            {
                return NotFound(); // Retorna 404 Not Found se o logradouro não for encontrado.
            }
            catch (Exception ex)
            {
                // Um sistema de log real registraria 'ex' aqui.
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
    }
}