using AdaptiveFlow.Workbook;
using FluentAssertions;

namespace AdaptiveFlow.Tests;

public class ConcurrentListTests
{
    private readonly ConcurrentList<string> _list;

    public ConcurrentListTests()
    {
        _list = new ConcurrentList<string>();
    }

    [Fact]
    public void Add_ShouldAddItem()
    {
        // Act
        _list.Add("Item1");

        // Assert
        _list.Count.Should().Be(1);
        _list.Snapshot().Should().Contain("Item1");
    }

    [Fact]
    public void AddRange_ShouldAddMultipleItems()
    {
        // Arrange
        var items = new[] { "Item1", "Item2" };

        // Act
        _list.AddRange(items);

        // Assert
        _list.Count.Should().Be(2);
        _list.Snapshot().Should().BeEquivalentTo(items);
    }

    [Fact]
    public void Remove_ItemExists_RemovesAndReturnsTrue()
    {
        // Arrange
        _list.Add("Item1");

        // Act
        var result = _list.Remove("Item1");

        // Assert
        result.Should().BeTrue();
        _list.Count.Should().Be(0);
    }

    [Fact]
    public void Clear_ShouldRemoveAllItems()
    {
        // Arrange
        _list.Add("Item1");
        _list.Add("Item2");

        // Act
        _list.Clear();

        // Assert
        _list.Count.Should().Be(0);
    }

    [Fact]
    public void CountWhere_ShouldReturnMatchingItemsCount()
    {
        // Arrange
        _list.Add("Item1");
        _list.Add("Item2");
        _list.Add("Item11");

        // Act
        var count = _list.CountWhere(s => s.Contains("1"));

        // Assert
        count.Should().Be(2);
    }

    [Fact]
    public void Filter_ShouldReturnMatchingItems()
    {
        // Arrange
        _list.Add("Item1");
        _list.Add("Item2");
        _list.Add("Item11");

        // Act
        var filtered = _list.Filter(s => s.Contains("1"));

        // Assert
        filtered.Should().HaveCount(2);
        filtered.Should().Contain("Item1");
        filtered.Should().Contain("Item11");
    }

    [Fact]
    public void Any_WithPredicate_ReturnsTrueForMatchingItems()
    {
        // Arrange
        _list.Add("Item1");
        _list.Add("Item2");

        // Act
        var result = _list.Any(s => s.Contains("1"));

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ForEach_ShouldExecuteActionForEachItem()
    {
        // Arrange
        _list.Add("Item1");
        _list.Add("Item2");
        var count = 0;

        // Act
        _list.ForEach(_ => count++);

        // Assert
        count.Should().Be(2);
    }

    [Fact]
    public void ToEnumerable_ShouldReturnAllItems()
    {
        // Arrange
        _list.Add("Item1");
        _list.Add("Item2");

        // Act
        var enumerable = _list.ToEnumerable();

        // Assert
        enumerable.Should().HaveCount(2);
        enumerable.Should().Contain("Item1");
        enumerable.Should().Contain("Item2");
    }
}