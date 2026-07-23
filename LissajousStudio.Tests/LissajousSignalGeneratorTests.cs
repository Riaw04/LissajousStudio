using LissajousStudio.Core.Models;
using LissajousStudio.DSP.SignalGenerators;

namespace LissajousStudio.Tests;

public sealed class LissajousSignalGeneratorTests
{
    [Fact]
    public void GenerateBuffer_WithNinetyDegreePhase_ProducesUnitCircleSamples()
    {
        var generator = new LissajousSignalGenerator();
        var parameters = new SignalParameters { FrequencyHz = 1, SampleRate = 4, Amplitude = 1, PhaseDegrees = 90, BufferFrames = 4 };
        var buffer = new float[8];
        generator.GenerateBuffer(buffer, parameters);
        Assert.Equal(0f, buffer[0], 3);
        Assert.Equal(1f, buffer[1], 3);
        Assert.Equal(1f, buffer[2], 3);
        Assert.Equal(0f, buffer[3], 3);
    }
}
