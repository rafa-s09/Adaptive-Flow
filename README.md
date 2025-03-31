# Adaptive-Flow

***AdaptiveFlow*** é uma biblioteca reutilizável e um padrão de design para orquestração de fluxos de trabalho assíncronos em .NET. Ele permite configurar e executar sequências de passos (steps) com suporte a dependências, execução paralela, logging opcional, alta performance e configuração dinâmica via ***JSON***. Ideal para aplicações que precisam de pipelines flexíveis, testáveis e adaptáveis a diferentes contextos.

## The AdaptiveFlow Pattern

O ***AdaptiveFlow*** é mais do que uma biblioteca — é um padrão para construir fluxos de trabalho adaptáveis. Ele resolve problemas comuns em sistemas que requerem processamento em etapas, como validação, transformação e persistência de dados, oferecendo uma abordagem modular e extensível.

### Como Funciona

O padrão ***AdaptiveFlow*** organiza a execução de tarefas em um pipeline estruturado, mas flexível, baseado nos seguintes componentes:
1. **Passos (Steps):**
    - Representados por `IFlowStep` (sem retorno) ou `IFlowStep<TResponse>` (com retorno).
    - Cada step é uma unidade independente de lógica que opera em um `FlowContext` (um contêiner de dados compartilhado).
    - Exemplo: Um step pode validar uma entrada, enquanto outro criptografa dados.

2. **Configuração (FlowConfiguration):**
    - Define a sequência de steps, suas condições de execução, dependências e se devem rodar em paralelo.
    - Suporta configuração programática ou dinâmica via ***JSON***, permitindo ajustes em tempo de design ou execução.

3. **Gerenciador (FlowManager):**
    - Orquestra a execução dos steps, respeitando dependências e gerenciando concorrência e paralelismo.
    - Usa um canal (`Channel<FlowContext>`) para enfileirar e processar contextos assincronamente, com limites configuráveis para alta carga.
    - Permite logging opcional e substituição do processamento do canal via `IChannelProcessor`.

4. **Contexto (FlowContext):**
    - Um dicionário de chave-valor que transporta dados entre os steps, promovendo comunicação fluida e estado compartilhado.

### Princípios do Padrão

- **Modularidade:** Passos são isolados e intercambiáveis, facilitando reutilização e testes.
- **Flexibilidade:** Dependências e paralelismo são configuráveis, adaptando o fluxo a diferentes cenários.
- **Escalabilidade:** Suporte a concorrência e limites de canal garantem desempenho em alta carga.
- **Segurança:** Configuração dinâmica exige um registro explícito de tipos permitidos, evitando execução arbitrária.
- **Testabilidade:** Componentes abstraídos (como `IChannelProcessor`) permitem simulação em testes.

### Fluxo de Execução

1. O cliente define os steps e suas configurações em um `FlowConfiguration`.
2. O `FlowManager` recebe a configuração e inicializa um canal para enfileirar contextos.
3. Cada contexto é processado, executando os steps na ordem determinada por dependências:
    - Steps sequenciais rodam um após o outro.
    - Steps paralelos rodam simultaneamente, respeitando `maxParallelism`.
4. Resultados são coletados no `FlowResult`, incluindo sucesso/erro e saídas dos steps.

### Benefícios

- **Adaptabilidade:** Ajuste o fluxo dinamicamente sem recompilar o código.
- **Manutenção:** Steps isolados simplificam debugging e evolução do sistema.
- **Reutilização:** Use o mesmo padrão em diferentes partes do projeto ou em projetos distintos.

## Getting Started

Este guia vai ajudá-lo a começar a usar o ***AdaptiveFlow*** em seu projeto em poucos minutos.

### Pré-requisitos

- **.NET:** Versão 9.0 ou superior.
- **Dependências:** O pacote usa `Microsoft.Extensions.Logging` e `System.Threading.Channels`. Certifique-se de que essas dependências estão disponíveis no seu projeto. Adicione-os ao seu projeto se necessário:
    ```bash 
    dotnet add package Microsoft.Extensions.Logging
    ```

### Instalação

**1. Adicione o Pacote:**
- Instale via NuGet
    ```bash
    dotnet add package AdaptiveFlow
    ```
- Ou referencie a DLL manualmente no .csproj se estiver usando uma versão local:
    ```xml
    <ItemGroup>
        <Reference Include="AdaptiveFlow">
            <HintPath>path/to/AdaptiveFlow.dll</HintPath>
        </Reference>
    </ItemGroup>
    ```

**2. Configuração Básica:**
- Certifique-se de ter um `IServiceProvider` configurado. Em aplicações ASP.NET Core, isso é fornecido automaticamente via injeção de dependência.

### Organização e Nomenclatura

Para integrar o ***AdaptiveFlow*** ao seu projeto, siga uma estrutura clara e uma nomenclatura consistente.

**1. Organização**
- Estruture seu projeto para separar responsabilidades. Aqui está um exemplo para uma API:
    ```text
    /MyProject
    ├── /Controllers   # Controladores da API
    ├── /Models        # Modelos de dados
    ├── /Services      # Lógica de negócios
    ├── /Repositories  # Acesso a dados
    ├── /Interfaces    # Interfaces de serviços e repositórios
    └── /Steps         # Passos do AdaptiveFlow
    ```

**2. Nomenclatura**

Adote nomes significativos e padronizados:
- **Sufixos:** Use sufixos como `Service`, `Repository`, ou `Step` para indicar o propósito da classe.
- **Interfaces:** Prefixe com `I` maiúsculo (padrão C#), ex.: `IMyScopeService`.
    ```text
    /MyProject
    ├── /Controllers
    │   └── HomeController.cs
    ├── /Models
    │   └── UserModel.cs
    ├── /Services
    │   └── UserService.cs
    ├── /Repositories
    │   └── UserRepository.cs
    ├── /Interfaces
    │   └── IUserService.cs
    └── /Steps
        ├── LogStep.cs
        └── ComputeStep.cs
    ```

### Configurando e Executando um Fluxo

1. **Crie o Modelo e os Steps**
    - Defina o modelo de entrada e os steps que processarão os dados:
    ```csharp
    namespace MyProject.Models
    {
        public class UserModel
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
    }
    ```

    ```csharp
    using AdaptiveFlow;

    namespace MyProject.Steps
    {
        // Step que loga os dados recebidos
        public class LogStep : IFlowStep
        {
            public async Task ExecuteAsync(FlowContext context, CancellationToken   cancellationToken)
            {
                if (context.Data.TryGetValue("UserModel", out var userObj) &&   userObj is UserModel user)
                {
                    Console.WriteLine($"Usuário recebido: {user.Name}, Idade: {user.    Age}");
                }
                await Task.CompletedTask;
            }
        }

        // Step que calcula algo baseado na idade
        public class ComputeStep : IFlowStep<int>
        {
            public async Task<int> ExecuteAsync(FlowContext context,    CancellationToken cancellationToken)
            {
                if (context.Data.TryGetValue("UserModel", out var userObj) &&   userObj is UserModel user)
                {
                    return user.Age * 2; // Exemplo: dobra a idade
                }
                return 0;
            }
        }
    }
    ```

2. **Configure os Serviços**
    - Registre os Steps no contêiner de injeção de dependência:
    ```csharp
    var services = new ServiceCollection();
    services.AddTransient<MeuProjeto.Steps.LogStep>();
    services.AddTransient<MeuProjeto.Steps.ComputeStep>();
    var serviceProvider = services.BuildServiceProvider();
    ```

3. **Configure e Execute o Fluxo no Controlador**
    - Use o AddStep dentro de um controlador para processar uma requisição:
    ```csharp
    using AdaptiveFlow;
    using Microsoft.AspNetCore.Mvc;

    namespace MeuProjeto.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class HomeController : ControllerBase
        {
            private readonly FlowManager _flowManager;

            public HomeController(IServiceProvider serviceProvider)
            {
                var config = new FlowConfiguration()
                    .AddStep(serviceProvider.GetRequiredService<MeuProjeto.Steps.   LogStep>(), "Log")
                    .AddStep(serviceProvider.GetRequiredService<MeuProjeto.Steps.   ComputeStep>(), "Compute", dependsOn: new[] { "Log" });
                _flowManager = new FlowManager(config);
            }

            [HttpPost("mypost")]
            public async Task<IActionResult> MyPost([FromBody] MeuProjeto.Models.UserModel request, CancellationToken cancellationToken)
            {
                var context = new FlowContext();
                context.Data["UserModel"] = request; // Passa os dados da   requisição para o contexto

                var result = await _flowManager.RunAsync(context,   cancellationToken);
                if (result.Success)
                {
                    var dynamicResult = (dynamic)result.Result;
                    var computedValue = ((IList<object>)dynamicResult.StepResults). OfType<int>().FirstOrDefault();
                    return Ok(new { Name = request.Name, ComputedAge =  computedValue });
                }
                return BadRequest(result.ErrorMessage);
            }
        }
    }
    ```
    - **Explicação:**
        - "Log": Registra os dados do UserModel.
        - "Compute": Calcula o dobro da idade após o "Log", devido à dependência.

4. **Exemplo de Dados para a Requisição**
    - Envie uma requisição POST para `api/home/mypost` com este JSON:
    ```json
    {
    "Name": "João",
    "Age": 30
    }
    ```
    - Comando cURL para teste:
    ```bash
    curl -X POST "http://localhost:5000/api/home/mypost" -H "Content-Type:  application/json" -d "{\"Name\":\"João\",\"Age\":30}"
    ```
    - Resposta Esperada:
    ```json
    {
        "Name": "João",
        "ComputedAge": 60
    }
    ```
    - Saída no Console:
    ```text
    Usuário recebido: João, Idade: 30
    ```

> ***"Nota: Para configuração dinâmica, você pode usar FlowConfiguration.FromJson com um JSON e um registro de steps. Veja a documentação avançada para detalhes."***

# Contribuições
Sinta-se à vontade para abrir issues ou pull requests no repositório. Sugestões para melhorar a biblioteca são bem-vindas!

# MIT License

Copyright (c) 2025 Rafael Souza

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.