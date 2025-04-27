using AdaptiveFlow.Workbook;
using FluentAssertions;

namespace AdaptiveFlow.Tests;

public class FlowKeyTests
{
    [Fact]
    public void Constructor_NullName_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new FlowKey(null!));
    }

    [Fact]
    public void Equals_SameNameAndHash_ReturnsTrue()
    {
        // Arrange
        var key1 = new FlowKey("Key1");
        var key2 = new FlowKey("Key1");

        // Act
        var result = key1.Equals(key2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentName_ReturnsFalse()
    {
        // Arrange
        var key1 = new FlowKey("Key1");
        var key2 = new FlowKey("Key2");

        // Act
        var result = key1.Equals(key2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_SameName_ReturnsSameHash()
    {
        // Arrange
        var key1 = new FlowKey("Key1");
        var key2 = new FlowKey("Key1");

        // Act
        var hash1 = key1.GetHashCode();
        var hash2 = key2.GetHashCode();

        // Assert
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void ToString_ReturnsName()
    {
        // Arrange
        var key = new FlowKey("Key1");

        // Act
        var result = key.ToString();

        // Assert
        result.Should().Be("Key1");
    }
}