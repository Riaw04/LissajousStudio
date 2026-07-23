using LissajousStudio.Core.Interfaces;
using LissajousStudio.Core.Models;

namespace LissajousStudio.DSP.Figures;

/// <summary>Classic 3:2 Lissajous figure included as an example of a custom figure function.</summary>
public sealed class ClassicLissajousFigure : ILissajousFigure
{
    public string Id => "classic-3-2";
    public string DisplayName => "Classic 3:2 Lissajous";

    public LissajousPoint Evaluate(in FigureParameters parameters)
    {
        var x = Math.Sin(Math.Tau * parameters.Phase * 3.0) * parameters.Amplitude;
        var y = Math.Sin(Math.Tau * (parameters.Phase * 2.0 + parameters.PhaseOffsetTurns)) * parameters.Amplitude;
        return new LissajousPoint((float)x, (float)y);
    }
}
