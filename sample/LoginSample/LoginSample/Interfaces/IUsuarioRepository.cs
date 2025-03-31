using LoginSample.Models;

namespace LoginSample.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<UsuarioModel> ObterPorEmailAsync(string email);
        Task<UsuarioModel> ObterPorIdAsync(int id);
        Task<int> AdicionarAsync(UsuarioModel usuario);
        Task AtualizarAsync(UsuarioModel usuario);
    }
}
