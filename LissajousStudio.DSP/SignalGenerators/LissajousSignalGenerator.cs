using LissajousStudio.Core.Interfaces;
using LissajousStudio.Core.Models;

namespace LissajousStudio.DSP.SignalGenerators;

/// <summary>Produces stereo Lissajous audio: left channel X, right channel Y.</summary>
public sealed class LissajousSignalGenerator : ISignalGenerator
{
    private double _phase;

    public void GenerateBuffer(Span<float> interleavedStereo, SignalParameters parameters)
    {
        var sampleRate = Math.Max(1, parameters.SampleRate);
        var increment = parameters.FrequencyHz / sampleRate;
        var phaseOffset = parameters.PhaseDegrees / 360.0;
        var amplitude = (float)Math.Clamp(parameters.Amplitude, 0.0, 1.0);

        for (var frame = 0; frame < interleavedStereo.Length / 2; frame++)
        {
            var x = Math.Sin(Math.Tau * _phase) * amplitude;
            var y = Math.Sin(Math.Tau * (_phase + phaseOffset)) * amplitude;
            interleavedStereo[frame * 2] = (float)x;
            interleavedStereo[frame * 2 + 1] = (float)y;
            _phase = (_phase + increment) % 1.0;
        }
    }
}
