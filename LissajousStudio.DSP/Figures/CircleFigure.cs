using LissajousStudio.Core.Interfaces;
using LissajousStudio.Core.Models;

namespace LissajousStudio.DSP.Figures;

/// <summary>Default circle figure: X is sine and Y is the same sine shifted by the configured phase.</summary>
public sealed class CircleFigure : ILissajousFigure
{
    public string Id => "circle";
    public string DisplayName => "Circle / Phase Shifted Sine";

    public LissajousPoint Evaluate(in FigureParameters parameters)
    {
        var x = Math.Sin(Math.Tau * parameters.Phase) * parameters.Amplitude;
        var y = Math.Sin(Math.Tau * (parameters.Phase + parameters.PhaseOffsetTurns)) * parameters.Amplitude;
        return new LissajousPoint((float)x, (float)y);
    }
}
