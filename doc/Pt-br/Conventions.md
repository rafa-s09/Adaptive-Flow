# Usando Convenções em Projetos com AdaptiveFlow

Para garantir consistência, clareza e facilidade de manutenção em seu projeto ao integrar o **AdaptiveFlow**, é importante seguir convenções bem definidas de organização e nomenclatura. Este documento descreve as melhores práticas para estruturar seu projeto e nomear suas classes, interfaces e arquivos.

> Com o AdaptiveFlow, você realiza tarefas de forma eficiente, flexível e em perfeita harmonia — como em uma fábrica bem gerenciada.

---

## **1. Organização do Projeto**

Uma estrutura de diretórios limpa ajuda a separar responsabilidades e torna o projeto mais gerenciável. Aqui está um exemplo de estrutura para um projeto de API que utiliza o AdaptiveFlow:

```text
/MeuProjeto
├── /Controllers       # Lida com requisições e rotas da API
├── /Models            # Representa estruturas de dados e modelos de domínio
├── /Services          # Contém a lógica de negócio
├── /Repositories      # Gerencia o acesso a dados
├── /Interfaces        # Define contratos para serviços e repositórios
├── /Steps             # Implementa os steps do fluxo com AdaptiveFlow
├── /Utilities         # Contém classes auxiliares ou utilitárias
└── /Tests             # Inclui testes unitários e de integração
```

## Explicação dos Diretórios

- **Controllers**: Onde os endpoints da API são definidos. Interagem com a camada de Services para lidar com requisições e retornar respostas adequadas.
- **Models**: Representa o domínio do negócio, como informações de usuários ou produtos, geralmente correspondendo a entidades de banco de dados ou DTOs.
- **Services**: Contém a lógica central da aplicação e orquestra operações entre repositórios e componentes externos.
- **Repositories**: Abstrai e encapsula interações com bancos de dados ou outros mecanismos de armazenamento.
- **Interfaces**: Define contratos para services e repositórios, tornando a aplicação extensível e testável.
- **Steps**: Armazena implementações de steps do AdaptiveFlow que definem unidades independentes do fluxo de trabalho.
- **Utilities**: Armazena classes auxiliares reutilizáveis, como para parsing de JSON ou formatação de datas.
- **Tests**: Garante confiabilidade e robustez por meio de testes unitários e de integração.

## 2. Convenções de Nomenclatura

Nomes consistentes ajudam a identificar rapidamente o propósito de uma classe ou interface. Abaixo estão diretrizes para as convenções de nomenclatura:

### Nomeação de Classes
- Use nomes significativos que reflitam o propósito da classe.
- **Sufixos**: Inclua sufixos que descrevam o papel da classe:
    - **Para services**: `UserService`, `OrderService`
    - **Para repositories**: `UserRepository`, `ProductRepository`
    - **Para steps**: `LogStep`, `ComputeStep`

### Nomeação de Interfaces
- Prefixe nomes de interfaces com `I` (seguindo a convenção do C#).
    - Exemplos: `IUserService`, `IProductRepository`

### Nomeação de Arquivos
- Faça os nomes dos arquivos corresponderem aos nomes das classes, para garantir clareza:
    ```text
    ├── /Controllers
    │   └── HomeController.cs
    ├── /Models
    │   ├── UserModel.cs
    │   └── ProductModel.cs
    ├── /Services
    │   └── UserService.cs
    ├── /Repositories
    │   └── ProductRepository.cs
    ├── /Steps
    │   ├── FlowLogStep.cs
    │   └── FlowComputeStep.cs    
    ```

### Testes
- Inclua `Tests` como sufixo no nome do arquivo de teste, correspondendo à classe testada:
    ```text
    └── /Tests
        ├── UserServiceTests.cs
        ├── FlowManagerTests.cs
        └── FlowStepWrapperTests.cs
    ```

## 3. Exemplo: Implementação de um Step com AdaptiveFlow
- Abaixo está um exemplo de como criar e nomear uma classe de step com o AdaptiveFlow:
    
    **LogStep.cs**

    ```csharp
    public class LogStep : IFlowStep
    {
      public async Task ExecuteAsync(FlowContext context, CancellationToken cancellationToken = default)
      {
          var logMessage = context.Get<string>("logMessage");
          Console.WriteLine($"Log: {logMessage}");
          await Task.CompletedTask;
      }
    }
    ```

Este step deve estar no diretório `/Steps`, representando claramente sua responsabilidade no fluxo de trabalho.

## 4. Exemplo: Teste de um Step do AdaptiveFlow
- Um teste correspondente para a classe `LogStep` pode seguir este padrão:

    **LogStepTests.cs**

    ```csharp
    public class LogStepTests
    {
        [Fact]
        public async Task ExecuteAsync_Deve_Registrar_Mensagem()
        {
            // Arrange
            var logStep = new LogStep();
            var context = new FlowContext();
            context.Set("logMessage", "Mensagem de teste");

            // Act
            await logStep.ExecuteAsync(context);

            // Assert
            // Supondo que você faça mock ou spy no Console.WriteLine para verificação
        }
    }
    ```
    
Isso garante que o step desempenha sua função pretendida e se encaixa perfeitamente nos fluxos de trabalho.

## 5. Benefícios de Seguir as Convenções

1. **Maior Legibilidade:** Uma estrutura consistente ajuda os desenvolvedores a localizar e entender o código rapidamente.
2. **Melhor Colaboração:** Membros da equipe podem seguir um padrão comum, reduzindo confusão e tempo de onboarding.
3. **Manutenibilidade:** Componentes organizados e bem nomeados facilitam a depuração e extensão da aplicação.
4. **Testabilidade:** Separação de responsabilidades facilita a criação de testes unitários e de integração confiáveis.
