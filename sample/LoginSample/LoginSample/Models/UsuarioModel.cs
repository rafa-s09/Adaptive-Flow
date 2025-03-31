namespace LoginSample.Models
{
    public class UsuarioModel
    {
        public int Id { get; set; }
        public Guid UAID { get; set; } = Guid.NewGuid();
        public string Email { get; set; }
        public string Passport { get; set; } // Texto criptografado
        public bool Bloqueado { get; set; } = false;
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;
    }
}
