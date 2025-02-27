namespace JsonParserLib.Tests;

public class JsonParserTests
{

    private readonly JsonParser _parser = new ();

    [Fact]
    public void WhenPassedAnEmptyJsonString_ReturnsEmptyList()
    {
        var json = "";
        List<Token> tokens = _parser.Lex(json);
        Assert.Empty(tokens);
    }

    [Fact]
    public void WhenPassedAStringWithJsonSyntaxChars_ReturnsIndividualTokens()
    {
        var json = "[]{},;:";
        List<Token> tokens = _parser.Lex(json);
        Assert.Equal(json.Length, tokens.Count);
        for (int i = 0; i < json.Length; i++)
        {
            string currentValue = tokens[i].Value;
            Assert.Equal(1, currentValue.Length);
            char c = currentValue[0];
            Assert.Equal(json[i], c);
        }
    }

    [Fact]
    public void WhenPassedASimpleString_ReturnsStringToken()
    {
        var json = "\"Hello\"";
        List<Token> tokens = _parser.Lex(json);
        Assert.Single(tokens);
        Assert.Equal(TokenType.String, tokens[0].Type);
        Assert.Equal(json.Replace("\"", ""), tokens[0].Value);
    }

    [Fact]
    public void WhenPassedJsonSyntaxCharactersAndString_ReturnsSyntaxAndStringTokens()
    {
        var json = "[]{}\"Hello\",;:";
        List<Token> tokens = _parser.Lex(json);
        Assert.Equal(8, tokens.Count());
    }

    [Fact]
    public void WhenPassedASimpleNumber_ReturnsNumberToken()
    {
        var json = "12345";
        List<Token> tokens = _parser.Lex(json);
        Assert.Single(tokens);
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal(json, tokens[0].Value);
    }


    [Fact]
    public void WhenPassedANumericString_ReturnsString()
    {
        var json = "\"12345\"";
        List<Token> tokens = _parser.Lex(json);
        Assert.Single(tokens);
        Assert.Equal(TokenType.String, tokens[0].Type);
        Assert.Equal(json.Replace("\"", ""), tokens[0].Value);
    }
}