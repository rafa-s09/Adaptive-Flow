using LoginSample.Interfaces;
using LoginSample.Models;

namespace LoginSample.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly List<UsuarioModel> _usuarios = new();

        public Task<UsuarioModel> ObterPorEmailAsync(string email)
        {
            return Task.FromResult(_usuarios.FirstOrDefault(u => u.Email == email));
        }

        public Task<UsuarioModel> ObterPorIdAsync(int id)
        {
            return Task.FromResult(_usuarios.FirstOrDefault(u => u.Id == id));
        }

        public Task<int> AdicionarAsync(UsuarioModel usuario)
        {
            usuario.Id = _usuarios.Count + 1;
            _usuarios.Add(usuario);
            return Task.FromResult(usuario.Id);
        }

        public Task AtualizarAsync(UsuarioModel usuario)
        {
            var existente = _usuarios.FirstOrDefault(u => u.Id == usuario.Id);
            if (existente != null)
            {
                existente.Email = usuario.Email;
                existente.Passport = usuario.Passport;
                existente.Bloqueado = usuario.Bloqueado;
                existente.DataAtualizacao = DateTime.UtcNow;
            }
            return Task.CompletedTask;
        }
    }
}
