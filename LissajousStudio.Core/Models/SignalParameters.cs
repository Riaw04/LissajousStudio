namespace LissajousStudio.Core.Models;

/// <summary>Editable synthesis parameters shared by UI, DSP, audio, and rendering.</summary>
public sealed class SignalParameters
{
    public double FrequencyHz { get; set; } = 440.0;
    public double Amplitude { get; set; } = 0.75;
    public double PhaseDegrees { get; set; } = 90.0;
    public int SampleRate { get; set; } = 48_000;
    public int BufferFrames { get; set; } = 2048;
}
