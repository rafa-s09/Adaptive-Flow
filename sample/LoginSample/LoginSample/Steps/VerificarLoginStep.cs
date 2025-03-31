using LoginSample.Flow;
using LoginSample.Interfaces;

namespace LoginSample.Steps
{
    public class VerificarLoginStep : IFlowStep
    {
        private readonly IUsuarioRepository _repository;
        private readonly ICriptografiaService _criptografia;

        public VerificarLoginStep(IUsuarioRepository repository, ICriptografiaService criptografia)
        {
            _repository = repository;
            _criptografia = criptografia;
        }

        public async Task ExecuteAsync(FlowContext context)
        {
            var email = context.Data["Email"].ToString();
            var senha = context.Data["Senha"].ToString();
            var usuario = await _repository.ObterPorEmailAsync(email);

            if (usuario == null || usuario.Bloqueado)
            {
                throw new Exception("Usuário não encontrado ou bloqueado");
            }

            var passportEsperado = _criptografia.Criptografar(email, senha); // Nome padrão do e-mail
            if (passportEsperado != usuario.Passport)
            {
                throw new Exception("Senha inválida");
            }

            context.Data["Usuario"] = usuario;
        }
    }
}
