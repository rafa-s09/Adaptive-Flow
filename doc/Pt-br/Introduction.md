# Introdução ao AdaptiveFlow

**AdaptiveFlow** é uma biblioteca robusta e reutilizável, além de um padrão de design, criada para orquestrar fluxos de trabalho assíncronos em .NET. Seu objetivo é simplificar a configuração e execução de sequências de etapas, oferecendo suporte a recursos avançados como **dependências**, **paralelismo**, **configuração dinâmica via JSON** e **log opcional**. Ideal para aplicações que exigem pipelines de alto desempenho e adaptáveis, o AdaptiveFlow se destaca em cenários onde flexibilidade e testabilidade são essenciais

## Como Funciona

O AdaptiveFlow organiza tarefas em um pipeline modular e flexível, baseado nos seguintes componentes principais:

1. **Steps (Etapas):**
   - Representadas pela interface `IFlowStep`, para operações sem valor de retorno.
   - Cada etapa é uma unidade lógica autocontida, operando dentro de um `FlowContext` — um contêiner compartilhado de dados.
   - Exemplo: Uma etapa valida os dados de entrada, enquanto outra os transforma em um novo formato.

2. **FlowConfiguration (Configuração do Fluxo):**
   - Define a sequência do fluxo de trabalho, especificando dependências, condições de execução e paralelismo.
  - Suporta configuração tanto programaticamente quanto dinamicamente via JSON, permitindo adaptação em tempo de execução.

3. **FlowManager (Gerenciador de Fluxo):**
   - Gerencia a execução do fluxo de trabalho, garantindo que as dependências sejam respeitadas, equilibrando concorrência e execução paralela.
  - Utiliza `Channel<FlowContext>` para enfileirar e processar contextos de forma assíncrona, com limites de recursos configuráveis.
  - Extensível com processadores personalizados via a interface `IChannelProcessor`, com suporte a log opcional.

4. **FlowContext (Contexto do Fluxo):**
   - Um dicionário chave-valor thread-safe que facilita o compartilhamento de dados e a comunicação entre as etapas.
  - Suporta armazenamento com escopo específico para etapas ou para todo o fluxo de trabalho.



---

### Exemplo 1: Configuração Programática do Fluxo Sem JSON

#### Configuração Programática:
```csharp
var flowConfig = new FlowConfiguration()
    .AddStep(new DataValidationStep(), "Validation")
    .AddStep(new DataTransformationStep(), "Transformation", dependsOn: new[] { "Validation" }, isParallel: true)
    .AddStep(new SaveToDatabaseStep(), "Save", dependsOn: new[] { "Transformation" });

var flowManager = new FlowManager(flowConfig);
await flowManager.StartProcessingAsync();
```

---

### Exemplo 2: Um Fluxo de Trabalho Básico Configurado com JSON

#### Configuração via JSON:
```csharp
[
  {
    "StepType": "DataValidationStep",
    "StepName": "Validation",
    "IsParallel": false
  },
  {
    "StepType": "DataTransformationStep",
    "StepName": "Transformation",
    "IsParallel": true,
    "DependsOn": ["Validation"]
  },
  {
    "StepType": "SaveToDatabaseStep",
    "StepName": "Save",
    "DependsOn": ["Transformation"]
  }
]
```

#### Execução do Fluxo:
```csharp
var stepRegistry = new Dictionary<string, Type>
{
    { "DataValidationStep", typeof(DataValidationStep) },
    { "DataTransformationStep", typeof(DataTransformationStep) },
    { "SaveToDatabaseStep", typeof(SaveToDatabaseStep) }
};

var serviceProvider = new ServiceCollection()
    .AddSingleton<DataValidationStep>()
    .AddSingleton<DataTransformationStep>()
    .AddSingleton<SaveToDatabaseStep>()
    .BuildServiceProvider();

var jsonConfig = File.ReadAllText("workflow.json");
var flowConfig = FlowConfiguration.FromJson(jsonConfig, serviceProvider, stepRegistry);

var flowManager = new FlowManager(flowConfig);
await flowManager.StartProcessingAsync();
```

---

## Benefícios

- **Configuração Dinâmica:** Adapte fluxos de trabalho em tempo de execução usando JSON, facilitando atualizações sem necessidade de recompilação.
- **Concorrência e Escalabilidade:** Suporte nativo à execução paralela e limites configuráveis para otimização de desempenho.
- **Flexibilidade:** Design modular e desacoplado que permite componentes e comportamentos personalizados.
- **Facilidade de Testes:** Interfaces como `IChannelProcessor` permitem testes com mocks para maior confiabilidade.
- **Segurança por Design:** O registro de tipos garante que apenas componentes confiáveis e explicitamente definidos sejam utilizados dinamicamente.

---

## Casos de Uso

O **AdaptiveFlow** brilha em cenários que exigem adaptabilidade e precisão. Aplicações comuns incluem:
- **Processamento de Dados em Lote:** Transformar grandes conjuntos de dados com várias etapas interdependentes.
- **Validação de Requisições de API:** Validar e transformar requisições recebidas por meio de pipelines flexíveis.
- **Pipelines ETL:** Extração, Transformação e Carga de dados em fluxos de trabalho complexos.
- **Pipelines de Integração Contínua (CI):** Orquestrar etapas de build, testes e deploy em ambientes de CI/CD.

