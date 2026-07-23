using LissajousStudio.Core.Interfaces;
using LissajousStudio.Core.Models;

namespace LissajousStudio.DSP.Figures;

/// <summary>Adapter for registering custom Lissajous figures directly from a C# function.</summary>
public sealed class DelegateLissajousFigure : ILissajousFigure
{
    private readonly Func<FigureParameters, LissajousPoint> _evaluate;

    public DelegateLissajousFigure(string id, string displayName, Func<FigureParameters, LissajousPoint> evaluate)
    {
        Id = string.IsNullOrWhiteSpace(id) ? throw new ArgumentException("Figure id is required.", nameof(id)) : id;
        DisplayName = string.IsNullOrWhiteSpace(displayName) ? id : displayName;
        _evaluate = evaluate ?? throw new ArgumentNullException(nameof(evaluate));
    }

    public string Id { get; }
    public string DisplayName { get; }
    public LissajousPoint Evaluate(in FigureParameters parameters) => _evaluate(parameters);
}
