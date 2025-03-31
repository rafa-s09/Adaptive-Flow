using LoginSample.Flow;
using LoginSample.Interfaces;
using LoginSample.Models;

namespace LoginSample.Steps
{
    public class AtualizarUsuarioStep : IFlowStep
    {
        private readonly IUsuarioRepository _repository;
        private readonly UsuarioModel _usuario;

        public AtualizarUsuarioStep(IUsuarioRepository repository, UsuarioModel usuario)
        {
            _repository = repository;
            _usuario = usuario;
        }

        public async Task ExecuteAsync(FlowContext context)
        {
            _usuario.Email = context.Data["Email"].ToString();
            _usuario.Passport = context.Data["Passport"].ToString();
            await _repository.AtualizarAsync(_usuario);
        }
    }
}
