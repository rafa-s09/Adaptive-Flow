using LoginSample.Flow;
using LoginSample.Interfaces;

namespace LoginSample.Steps
{
    public class CriptografarPassportStep : IFlowStep
    {
        private readonly ICriptografiaService _criptografia;

        public CriptografarPassportStep(ICriptografiaService criptografia)
        {
            _criptografia = criptografia;
        }

        public async Task ExecuteAsync(FlowContext context)
        {
            var email = context.Data["Email"].ToString(); // Nome padrão do e-mail se não fornecido
            var senha = context.Data["Senha"].ToString();
            context.Data["Passport"] = _criptografia.Criptografar(email, senha);
            await Task.CompletedTask;
        }
    }
}
