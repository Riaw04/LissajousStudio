using System.Globalization;
using System.Text.RegularExpressions;
using LissajousStudio.Core.Models;

namespace LissajousStudio.DSP.Math;

/// <summary>Compiles a safe, small LaTeX-like math expression into a realtime evaluatable function.</summary>
public sealed class LatexExpressionCompiler
{
    public Func<FigureParameters, double> Compile(string expression)
    {
        var parser = new Parser(Normalize(expression));
        var node = parser.ParseExpression();
        parser.ExpectEnd();
        return parameters => node.Evaluate(parameters);
    }

    private static string Normalize(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression)) throw new FormatException("Expression is empty.");

        var normalized = StripAssignment(expression.Trim());
        normalized = ConvertFractions(normalized);
        normalized = Regex.Replace(normalized, @"(?<=[A-Za-z0-9\)\}])\\(sin|cos|tan|sqrt|abs)", @"*\$1");
        normalized = normalized
            .Replace(@"\left", string.Empty, StringComparison.Ordinal)
            .Replace(@"\right", string.Empty, StringComparison.Ordinal)
            .Replace(@"\cdot", "*", StringComparison.Ordinal)
            .Replace(@"\times", "*", StringComparison.Ordinal)
            .Replace(@"\pi", "(pi)", StringComparison.Ordinal)
            .Replace(@"\sin", "sin", StringComparison.Ordinal)
            .Replace(@"\cos", "cos", StringComparison.Ordinal)
            .Replace(@"\tan", "tan", StringComparison.Ordinal)
            .Replace(@"\sqrt", "sqrt", StringComparison.Ordinal)
            .Replace(@"\abs", "abs", StringComparison.Ordinal)
            .Replace('{', '(')
            .Replace('}', ')')
            .Replace(" ", string.Empty, StringComparison.Ordinal);

        normalized = Regex.Replace(normalized, @"(?<=[0-9])(?=[A-Za-z(])", "*");
        normalized = Regex.Replace(normalized, @"(?<=\))(?=[A-Za-z0-9(])", "*");
        return normalized;
    }

    private static string StripAssignment(string expression)
    {
        var equalsIndex = expression.IndexOf('=', StringComparison.Ordinal);
        return equalsIndex >= 0 ? expression[(equalsIndex + 1)..] : expression;
    }

    private static string ConvertFractions(string expression)
    {
        const string marker = @"\frac";
        while (expression.Contains(marker, StringComparison.Ordinal))
        {
            var markerIndex = expression.IndexOf(marker, StringComparison.Ordinal);
            var numeratorStart = markerIndex + marker.Length;
            var numerator = ReadBracedExpression(expression, numeratorStart, out var afterNumerator);
            var denominator = ReadBracedExpression(expression, afterNumerator, out var afterDenominator);
            expression = string.Concat(
                expression.AsSpan(0, markerIndex),
                "((", numerator, ")/(", denominator, "))",
                expression.AsSpan(afterDenominator));
        }

        return expression;
    }

    private static string ReadBracedExpression(string expression, int start, out int nextIndex)
    {
        while (start < expression.Length && char.IsWhiteSpace(expression[start])) start++;
        if (start >= expression.Length || expression[start] != '{') throw new FormatException("Expected '{' after \\frac.");

        var depth = 0;
        for (var i = start; i < expression.Length; i++)
        {
            if (expression[i] == '{') depth++;
            if (expression[i] == '}') depth--;
            if (depth == 0)
            {
                nextIndex = i + 1;
                return expression[(start + 1)..i];
            }
        }

        throw new FormatException("Unclosed brace in \\frac expression.");
    }

    private interface INode { double Evaluate(FigureParameters p); }
    private sealed record NumberNode(double Value) : INode { public double Evaluate(FigureParameters p) => Value; }
    private sealed record VariableNode(string Name) : INode
    {
        public double Evaluate(FigureParameters p) => Name.ToLowerInvariant() switch
        {
            "t" or "phase" => p.Phase,
            "a" or "amp" or "amplitude" => p.Amplitude,
            "f" or "freq" or "frequency" => p.FrequencyHz,
            "pi" => System.Math.PI,
            _ => throw new InvalidOperationException($"Unknown variable '{Name}'.")
        };
    }
    private sealed record UnaryNode(char Op, INode Value) : INode { public double Evaluate(FigureParameters p) => Op == '-' ? -Value.Evaluate(p) : Value.Evaluate(p); }
    private sealed record BinaryNode(char Op, INode Left, INode Right) : INode
    {
        public double Evaluate(FigureParameters p) => Op switch
        {
            '+' => Left.Evaluate(p) + Right.Evaluate(p),
            '-' => Left.Evaluate(p) - Right.Evaluate(p),
            '*' => Left.Evaluate(p) * Right.Evaluate(p),
            '/' => Left.Evaluate(p) / Right.Evaluate(p),
            '^' => System.Math.Pow(Left.Evaluate(p), Right.Evaluate(p)),
            _ => 0.0
        };
    }
    private sealed record FunctionNode(string Name, INode Argument) : INode
    {
        public double Evaluate(FigureParameters p)
        {
            var value = Argument.Evaluate(p);
            return Name.ToLowerInvariant() switch
            {
                "sin" => System.Math.Sin(value),
                "cos" => System.Math.Cos(value),
                "tan" => System.Math.Tan(value),
                "sqrt" => System.Math.Sqrt(value),
                "abs" => System.Math.Abs(value),
                _ => throw new InvalidOperationException($"Unknown function '{Name}'.")
            };
        }
    }

    private sealed class Parser
    {
        private readonly string _text;
        private int _position;
        public Parser(string text) => _text = text;
        public INode ParseExpression()
        {
            var node = ParseTerm();
            while (Match('+') || Match('-')) { var op = _text[_position - 1]; node = new BinaryNode(op, node, ParseTerm()); }
            return node;
        }
        private INode ParseTerm()
        {
            var node = ParsePower();
            while (Match('*') || Match('/')) { var op = _text[_position - 1]; node = new BinaryNode(op, node, ParsePower()); }
            return node;
        }
        private INode ParsePower()
        {
            var node = ParseUnary();
            if (Match('^')) node = new BinaryNode('^', node, ParsePower());
            return node;
        }
        private INode ParseUnary() => Match('+') ? ParseUnary() : Match('-') ? new UnaryNode('-', ParseUnary()) : ParsePrimary();
        private INode ParsePrimary()
        {
            if (Match('(')) { var node = ParseExpression(); Require(')'); return node; }
            if (char.IsDigit(Current) || Current == '.') return ParseNumber();
            if (char.IsLetter(Current))
            {
                var name = ParseIdentifier();
                return Match('(') ? ParseFunction(name) : new VariableNode(name);
            }
            throw new FormatException($"Unexpected character '{Current}' at position {_position}.");
        }
        private INode ParseFunction(string name) { var arg = ParseExpression(); Require(')'); return new FunctionNode(name, arg); }
        private INode ParseNumber()
        {
            var start = _position;
            while (char.IsDigit(Current) || Current == '.') _position++;
            return new NumberNode(double.Parse(_text[start.._position], CultureInfo.InvariantCulture));
        }
        private string ParseIdentifier()
        {
            var start = _position;
            while (char.IsLetter(Current)) _position++;
            return _text[start.._position];
        }
        public void ExpectEnd() { if (_position != _text.Length) throw new FormatException($"Unexpected token at position {_position}."); }
        private char Current => _position < _text.Length ? _text[_position] : '\0';
        private bool Match(char c) { if (Current != c) return false; _position++; return true; }
        private void Require(char c) { if (!Match(c)) throw new FormatException($"Expected '{c}' at position {_position}."); }
    }
}
