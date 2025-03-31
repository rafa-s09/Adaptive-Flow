namespace LoginSample.Interfaces
{
    public interface ICriptografiaService
    {
        string Criptografar(string texto, string chave);
    }
}
