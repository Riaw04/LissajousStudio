using LissajousStudio.Core.Models;

namespace LissajousStudio.Core.Interfaces;

/// <summary>Generates the canonical interleaved stereo sample buffer.</summary>
public interface ISignalGenerator
{
    void GenerateBuffer(Span<float> interleavedStereo, SignalParameters parameters);
}
