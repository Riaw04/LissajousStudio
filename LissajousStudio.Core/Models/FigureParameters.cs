namespace LissajousStudio.Core.Models;

/// <summary>Immutable per-sample inputs supplied to a Lissajous figure function.</summary>
public readonly record struct FigureParameters(
    double Phase,
    double FrequencyHz,
    double Amplitude,
    double PhaseOffsetTurns,
    int SampleRate,
    int FrameIndex);
