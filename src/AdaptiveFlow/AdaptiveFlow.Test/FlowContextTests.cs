using AdaptiveFlow.Workbook;
using AdaptiveFlow.Interfaces;
using Moq;
using FluentAssertions;

namespace AdaptiveFlow.Tests;

public class FlowContextTests
{
    private readonly FlowContext _context;

    public FlowContextTests()
    {
        _context = new FlowContext();
    }

    [Fact]
    public void TrySet_ShouldAddValue()
    {
        // Arrange
        var key = new FlowKey("Key1");
        var value = "Value1";

        // Act
        var result = _context.TrySet(key, value);

        // Assert
        result.Should().BeTrue();
        _context.GetOrDefault(key, string.Empty).Should().Be(value);
    }

    [Fact]
    public void TrySet_NullValue_ThrowsArgumentNullException()
    {
        // Arrange
        var key = new FlowKey("Key1");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _context.TrySet<string?>(key, null));
    }

    [Fact]
    public async Task GetAsync_ValueExists_ReturnsValue()
    {
        // Arrange
        var key = new FlowKey("Key1");
        var value = "Value1";
        _context.TrySet(key, value);

        // Act
        var result = await _context.GetAsync<string>(key);

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public async Task GetAsync_KeyNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var key = new FlowKey("NoKey1");

        ;

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _context.GetAsync<string>(key).AsTask());
    }

    [Fact]
    public void Contains_KeyExists_ReturnsTrue()
    {
        // Arrange
        var key = new FlowKey("Key1");
        _context.TrySet(key, "Value1");

        // Act
        var result = _context.Contains(key);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Remove_KeyExists_RemovesAndReturnsTrue()
    {
        // Arrange
        var key = new FlowKey("Key1");
        _context.TrySet(key, "Value1");

        // Act
        var result = _context.Remove(key);

        // Assert
        result.Should().BeTrue();
        _context.Contains(key).Should().BeFalse();
    }

    [Fact]
    public void Clear_ShouldRemoveAllEntries()
    {
        // Arrange
        _context.TrySet(new FlowKey("Key1"), "Value1");
        _context.TrySet(new FlowKey("Key2"), "Value2");

        // Act
        _context.Clear();

        // Assert
        _context.Contains(new FlowKey("Key1")).Should().BeFalse();
        _context.Contains(new FlowKey("Key2")).Should().BeFalse();
    }
}