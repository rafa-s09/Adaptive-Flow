using LoginSample.Flow;
using LoginSample.Interfaces;
using LoginSample.Models;
using LoginSample.Steps;
using Microsoft.AspNetCore.Mvc;

namespace LoginSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ICriptografiaService _criptografiaService;
        private readonly IUsuarioRepository _usuarioRepository;

        public AuthController(ICriptografiaService criptografiaService, IUsuarioRepository usuarioRepository)
        {
            _criptografiaService = criptografiaService;
            _usuarioRepository = usuarioRepository;

        }

        [HttpPost("cadastro")]
        public async Task<IActionResult> Cadastro([FromBody] CadastroRequest request)
        {
            var context = new FlowContext();
            context.Data["Email"] = request.Email;
            context.Data["Senha"] = request.Senha;
            context.Data["Nome"] = request.Nome;

            var config = new FlowConfiguration()
                .AddStep(new ValidarEntradaStep())
                .AddStep(new CriptografarPassportStep(_criptografiaService))
                .AddStep(new SalvarUsuarioStep(_usuarioRepository));

            var manager = new FlowManager(config);
            await manager.RunAsync(context);

            return Ok(new { Id = context.Data["UsuarioId"] });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var context = new FlowContext();
            context.Data["Email"] = request.Email;
            context.Data["Senha"] = request.Senha;

            var config = new FlowConfiguration()
                .AddStep(new VerificarLoginStep(_usuarioRepository, _criptografiaService));

            var manager = new FlowManager(config);
            await manager.RunAsync(context);

            return Ok(new { Mensagem = "Login bem-sucedido", UsuarioId = ((UsuarioModel)context.Data["Usuario"]).Id });
        }

        [HttpPut("usuario/{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarRequest request)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(id);
            if (usuario == null) return NotFound();

            var context = new FlowContext();
            context.Data["Email"] = request.Email ?? usuario.Email;
            context.Data["Senha"] = request.Senha;
            context.Data["Nome"] = request.Nome;

            var config = new FlowConfiguration()
                .AddStep(new ValidarEntradaStep())
                .AddStep(new CriptografarPassportStep(_criptografiaService))
                .AddStep(new AtualizarUsuarioStep(_usuarioRepository, usuario)); // Nova classe concreta

            var manager = new FlowManager(config);
            await manager.RunAsync(context);

            return Ok(new { Mensagem = "Usuário atualizado" });
        }
    }

    // DTOs (mantidos iguais)
    public record CadastroRequest(string Email, string Senha, string Nome);
    public record LoginRequest(string Email, string Senha);
    public record AtualizarRequest(string Email, string Senha, string Nome);

}
