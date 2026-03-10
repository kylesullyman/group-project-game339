using Game339.Shared.Services.Implementation;
using NUnit.Framework;

namespace Game339.Tests;

public class StringServiceTests
{
    private StringService _svc;

    [SetUp]
    public void SetUp()
    {
        _svc = new StringService(EmptyGameLog.Instance);
    }

    [TestCase("hello", "olleh")]
    [TestCase("", "")]
    [TestCase("a", "a")]
    [TestCase("racecar", "racecar")]
    public void Reverse_ReturnsExpectedString(string input, string expected)
    {
        // Act
        var result = _svc.Reverse(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void Reverse_NullString_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<System.ArgumentNullException>(() => _svc.Reverse(null));
    }

    [TestCase("hello world", "world hello")]
    [TestCase("", "")]
    [TestCase("hello", "hello")]
    [TestCase("the quick brown fox", "fox brown quick the")]
    public void ReverseWords_ReturnsExpectedString(string input, string expected)
    {
        // Act
        var result = _svc.ReverseWords(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [TestCase("hello world!", "world hello!")]
    [TestCase("hello world.", "world hello.")]
    [TestCase("hello world?", "world hello?")]
    [TestCase("the quick brown fox!!", "fox brown quick the!!")]
    public void ReverseWords_WithTrailingPunctuation_PreservesPunctuationAtEnd(string input, string expected)
    {
        // Act
        var result = _svc.ReverseWords(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [TestCase("hello, world", "world hello,")]
    [TestCase("it's a test", "test a it's")]
    [TestCase("rock & roll", "roll & rock")]
    [TestCase("100% done", "done 100%")]
    public void ReverseWords_WithSpecialCharacters_ReversesWords(string input, string expected)
    {
        // Act
        var result = _svc.ReverseWords(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ReverseWords_NullString_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<System.NullReferenceException>(() => _svc.ReverseWords(null));
    }
}
