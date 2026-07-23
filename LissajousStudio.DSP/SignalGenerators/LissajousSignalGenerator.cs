using LissajousStudio.Core.Interfaces;
using LissajousStudio.Core.Models;
using LissajousStudio.DSP.Figures;

namespace LissajousStudio.DSP.SignalGenerators;

/// <summary>Produces stereo Lissajous audio: left channel X, right channel Y.</summary>
public sealed class LissajousSignalGenerator : ISignalGenerator
{
    private readonly IReadOnlyDictionary<string, ILissajousFigure> _figures;
    private readonly ILissajousFigure _fallbackFigure;
    private double _phase;

    public LissajousSignalGenerator(IEnumerable<ILissajousFigure>? figures = null)
    {
        var configuredFigures = (figures ?? new ILissajousFigure[] { new CircleFigure(), new ClassicLissajousFigure() })
            .GroupBy(figure => figure.Id, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .ToDictionary(figure => figure.Id, StringComparer.OrdinalIgnoreCase);

        _figures = configuredFigures;
        _fallbackFigure = configuredFigures.TryGetValue("circle", out var circle) ? circle : configuredFigures.Values.First();
    }

    public void GenerateBuffer(Span<float> interleavedStereo, SignalParameters parameters)
    {
        var sampleRate = System.Math.Max(1, parameters.SampleRate);
        var increment = parameters.FrequencyHz / sampleRate;
        var amplitude = System.Math.Clamp(parameters.Amplitude, 0.0, 1.0);
        var phaseOffset = parameters.PhaseDegrees / 360.0;
        var figure = ResolveFigure(parameters.FigureId);

        for (var frame = 0; frame < interleavedStereo.Length / 2; frame++)
        {
            var point = figure.Evaluate(new FigureParameters(_phase, parameters.FrequencyHz, amplitude, phaseOffset, sampleRate, frame));
            interleavedStereo[frame * 2] = System.Math.Clamp(point.X, -1f, 1f);
            interleavedStereo[frame * 2 + 1] = System.Math.Clamp(point.Y, -1f, 1f);
            _phase = (_phase + increment) % 1.0;
        }
    }

    private ILissajousFigure ResolveFigure(string? figureId)
        => !string.IsNullOrWhiteSpace(figureId) && _figures.TryGetValue(figureId, out var figure) ? figure : _fallbackFigure;
}
