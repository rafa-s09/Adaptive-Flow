using LoginSample.Flow;

namespace LoginSample.Steps
{
    public class ValidarEntradaStep : IFlowStep
    {
        public async Task ExecuteAsync(FlowContext context)
        {
            var email = context.Data["Email"]?.ToString();
            var senha = context.Data["Senha"]?.ToString();
            var nome = context.Data["Nome"]?.ToString();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha) || (context.Data.ContainsKey("Nome") && string.IsNullOrEmpty(nome)))            
                throw new Exception("Dados inválidos");
            
            await Task.CompletedTask;
        }
    }
}
