using LoginSample.Interfaces;

namespace LoginSample.Services
{
    public class HmacSha256CriptografiaService : ICriptografiaService
    {
        private const string TEXTO_FIXO = "SEGREDO_FIXO_"; // Texto fixo para criptografia

        public string Criptografar(string texto, string chave)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(chave));
            var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(TEXTO_FIXO + texto));
            return Convert.ToBase64String(hash);
        }
    }
}
