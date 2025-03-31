using LoginSample.Flow;
using LoginSample.Interfaces;
using LoginSample.Models;

namespace LoginSample.Steps
{
    public class SalvarUsuarioStep : IFlowStep
    {
        private readonly IUsuarioRepository _repository;

        public SalvarUsuarioStep(IUsuarioRepository repository)
        {
            _repository = repository;
        }

        public async Task ExecuteAsync(FlowContext context)
        {
            var usuario = new UsuarioModel
            {
                Email = context.Data["Email"].ToString(),
                Passport = context.Data["Passport"].ToString(),
                Bloqueado = false
            };
            var id = await _repository.AdicionarAsync(usuario);
            context.Data["UsuarioId"] = id;
        }
    }
}
