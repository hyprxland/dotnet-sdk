using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Xunit;

namespace Hyprx.Collections.Generic.Tests;

public class OrderedDictionaryTests
{
    [Fact]
    public void Constructor_Empty_CreatesEmptyDictionary()
    {
        var dict = new OrderedDictionary<string, int>();
        Assert.Empty(dict);
    }

    [Fact]
    public void Constructor_WithCapacity_CreatesDictionaryWithCapacity()
    {
        var dict = new OrderedDictionary<string, int>(3)
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
        };

        Assert.Equal(4, dict.Count);
    }

    [Fact]
    public void Constructor_WithDictionary_CreatesDictionaryWithItems()
    {
        var original = new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
        };

        var dict = new OrderedDictionary<string, int>(original);

        Assert.Equal(2, dict.Count);
        Assert.Equal(1, dict["one"]);
        Assert.Equal(2, dict["two"]);
    }

    [Fact]
    public void Add_PreservesInsertionOrder()
    {
        var dict = new OrderedDictionary<string, int>();
        dict.Add("one", 1);
        dict.Add("two", 2);
        dict.Add("three", 3);

        var expected = new[] { "one", "two", "three" };
        Assert.Equal(expected, dict.Keys);
    }

    [Fact]
    public void Insert_AddsItemAtSpecifiedIndex()
    {
        var dict = new OrderedDictionary<string, int>
        {
            { "one", 1 },
            { "three", 3 },
        };

        dict.Insert(1, "two", 2);

        Assert.Equal(2, dict["two"]);
        Assert.Equal(1, dict.IndexOf("two"));
    }

    [Fact]
    public void RemoveAt_RemovesItemAtIndex()
    {
        var dict = new OrderedDictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
        };

        dict.RemoveAt(1);

        Assert.Equal(2, dict.Count);
        Assert.False(dict.ContainsKey("two"));
    }

    [Fact]
    public void Indexer_GetsByIndex()
    {
        var dict = new OrderedDictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
        };

        Assert.Equal(1, dict[0]);
        Assert.Equal(2, dict[1]);
    }

    [Fact]
    public void Enumerator_EnumeratesInOrder()
    {
        var dict = new OrderedDictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
        };

        var expected = new[]
        {
            new KeyValuePair<string, int>("one", 1),
            new KeyValuePair<string, int>("two", 2),
            new KeyValuePair<string, int>("three", 3),
        };

        Assert.Equal(expected, dict);
    }

    [Fact]
    public void Clear_RemovesAllItems()
    {
        var dict = new OrderedDictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
        };

        dict.Clear();

        Assert.Empty(dict);
    }

    [Fact]
    public void Constructor_CopiesOrderedDictionary()
    {
        var original = new OrderedDictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
        };

        var copy = new OrderedDictionary<string, int>(original);

        Assert.Equal(original.Keys, copy.Keys);
        Assert.Equal(original.Values, copy.Values);
    }

    [Fact]
    public void ContainsKey_ReturnsTrueIfKeyExists()
    {
        var dict = new OrderedDictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
        };

        Assert.True(dict.ContainsKey("one"));
        Assert.False(dict.ContainsKey("three"));
    }

    [Fact]
    public void ContainsValue_ReturnsTrueIfValueExists()
    {
        var dict = new OrderedDictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
        };

        Assert.True(dict.ContainsValue(1));
        Assert.False(dict.ContainsValue(3));
    }

    [Fact]
    public void Keys_ReturnsOrderedKeys()
    {
        var dict = new OrderedDictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
        };

        var expectedKeys = new[] { "one", "two", "three" };
        Assert.Equal(expectedKeys, dict.Keys);
        var keys = dict.Keys.ToList();
        Assert.Collection(
            keys,
            (k) => Assert.Equal("one", k),
            (k) => Assert.Equal("two", k),
            (k) => Assert.Equal("three", k));
    }

    [Fact]
    public void Values_ReturnsOrderedValues()
    {
        var dict = new OrderedDictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
        };

        var expectedValues = new[] { 1, 2, 3 };
        Assert.Equal(expectedValues, dict.Values);
        var values = dict.Values.ToList();
        Assert.Collection(
            values,
            (v) => Assert.Equal(1, v),
            (v) => Assert.Equal(2, v),
            (v) => Assert.Equal(3, v));
    }
}