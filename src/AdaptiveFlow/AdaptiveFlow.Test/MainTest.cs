using Microsoft.Extensions.Logging;
using Moq;

namespace AdaptiveFlow.Tests;

public class MainTest
{
    [Fact]
    public async Task FlowManager_Should_Execute_Flow_Successfully()
    {
        var config = new FlowConfiguration()
            .AddStep(new Mock<IFlowStep>().Object, "Step1");

        var manager = new FlowManager(config);
        var context = new FlowContext();
        var result = await manager.RunAsync(context);

        Assert.True(result.Success);
    }

    [Fact]
    public void FlowConfiguration_Should_Add_Steps()
    {
        var stepMock = new Mock<IFlowStep>();

        var config = new FlowConfiguration()
            .AddStep(stepMock.Object, "Step1");

        Assert.Single(config.GetSteps());
    }

    [Fact]
    public async Task FlowManager_Should_Honor_Channel_And_SemaphoreLimits()
    {
        // Arrange
        const int maxConcurrency = 3; // Limite de concorrência
        const int numContexts = 10;  // Número de contextos para processar

        var semaphoreTracker = new SemaphoreSlim(0); // Para verificar limites durante a execução

        var stepMock = new Mock<IFlowStep>();
        stepMock.Setup(s => s.ExecuteAsync(It.IsAny<FlowContext>(), It.IsAny<CancellationToken>()))
                .Returns(async () =>
                {
                    await semaphoreTracker.WaitAsync(); // Simula execução e respeita os limites
                    await Task.Delay(100); // Simula trabalho na etapa
                    semaphoreTracker.Release();
                });

        var config = new FlowConfiguration()
            .AddStep(stepMock.Object, "Step");

        var manager = new FlowManager(config, maxConcurrency: maxConcurrency);

        // Act
        var processingTask = manager.StartProcessingAsync();

        // Adiciona vários contextos ao Channel
        for (int i = 0; i < numContexts; i++)
        {
            await manager.EnqueueAsync(new FlowContext());
        }

        // Fecha o Channel para sinalizar o fim do enfileiramento
        manager.EndChannel();

        // Libera o semáforo (maxConcurrency vezes) para simular trabalho
        for (int i = 0; i < maxConcurrency; i++)
        {
            semaphoreTracker.Release();
        }

        // Aguarde para garantir que todos os contextos foram processados
        await processingTask;

        // Assert
        stepMock.Verify(s => s.ExecuteAsync(It.IsAny<FlowContext>(), It.IsAny<CancellationToken>()), Times.Exactly(numContexts));
    }




}
