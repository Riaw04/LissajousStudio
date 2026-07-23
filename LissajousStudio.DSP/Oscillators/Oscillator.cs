namespace LissajousStudio.DSP.Oscillators;

/// <summary>Band-limited-ready oscillator facade for replaceable waveform generation.</summary>
public sealed class Oscillator
{
    private readonly Random _random = new(42);
    public float Next(Waveform waveform, double phase)
    {
        var p = phase - Math.Floor(phase);
        return waveform switch
        {
            Waveform.Sine => (float)Math.Sin(Math.Tau * p),
            Waveform.Square => p < 0.5 ? 1f : -1f,
            Waveform.Triangle => (float)(4.0 * Math.Abs(p - 0.5) - 1.0),
            Waveform.Saw => (float)(2.0 * p - 1.0),
            Waveform.Noise => (float)(_random.NextDouble() * 2.0 - 1.0),
            _ => 0f
        };
    }
}
