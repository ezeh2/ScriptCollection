using NUnit.Framework;

namespace JsonDepthFilter.Tests;

public class FilterJsonByDepthTests
{
    [Test]
    public void RemovesPropertiesBeyondSpecifiedDepth()
    {
        string input = "{\"a\":1,\"b\":{\"c\":2,\"d\":{\"e\":3}},\"f\":4}";
        string expected = "{\"a\":1,\"b\":{\"c\":2,\"d\":{}},\"f\":4}";
        string actual = JsonDepthFilter.FilterJsonContent(input, 2);
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void RemovesPropertiesBeyondSpecifiedDepth2()
    {
        string input = "{\"a\":1,\"b\":{\"c\":2,\"d\":{\"e\":3}},\"f\":4}";
        string expected = "{\"a\":1,\"b\":{},\"f\":4}";
        string actual = JsonDepthFilter.FilterJsonContent(input, 1);
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void HandlesCurlyBracesInStrings()
    {
        string input = "{\"text\":\"brace \\\\{ value \\\\}\",\"obj\":{\"inner\":{\"val\":1}}}";
        string expected = "{\"text\":\"brace \\\\{ value \\\\}\",\"obj\":{}}";
        string actual = JsonDepthFilter.FilterJsonContent(input, 1);
        Assert.That(actual, Is.EqualTo(expected));
    }
}
