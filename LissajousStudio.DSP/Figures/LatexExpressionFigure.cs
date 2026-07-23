using LissajousStudio.Core.Interfaces;
using LissajousStudio.Core.Models;
using LissajousStudio.DSP.Math;

namespace LissajousStudio.DSP.Figures;

/// <summary>Runtime-editable figure backed by two safe LaTeX-like expressions for X(t) and Y(t).</summary>
public sealed class LatexExpressionFigure : IEditableLatexFigure
{
    private readonly LatexExpressionCompiler _compiler;
    private Func<FigureParameters, double> _x;
    private Func<FigureParameters, double> _y;

    public LatexExpressionFigure(LatexExpressionCompiler compiler)
    {
        _compiler = compiler;
        XExpression = @"A\sin(2\pi t)";
        YExpression = @"A\sin(2\pi t + \pi/2)";
        _x = compiler.Compile(XExpression);
        _y = compiler.Compile(YExpression);
    }

    public string Id => "latex";
    public string DisplayName => "LaTeX Function";
    public string XExpression { get; private set; }
    public string YExpression { get; private set; }

    public void UpdateExpressions(string xExpression, string yExpression)
    {
        var nextX = _compiler.Compile(xExpression);
        var nextY = _compiler.Compile(yExpression);
        XExpression = xExpression;
        YExpression = yExpression;
        _x = nextX;
        _y = nextY;
    }

    public LissajousPoint Evaluate(in FigureParameters parameters)
        => new((float)_x(parameters), (float)_y(parameters));
}
