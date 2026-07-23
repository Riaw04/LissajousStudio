using LissajousStudio.Core.Interfaces;
using LissajousStudio.Core.Models;
using LissajousStudio.DSP.Figures;
using LissajousStudio.DSP.Math;
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

    [Fact]
    public void GenerateBuffer_WithCustomFigureId_UsesRegisteredFunction()
    {
        ILissajousFigure custom = new DelegateLissajousFigure(
            "diagonal",
            "Diagonal Test",
            p => new LissajousPoint((float)p.Amplitude, (float)-p.Amplitude));
        var generator = new LissajousSignalGenerator(new[] { custom });
        var parameters = new SignalParameters { FigureId = "diagonal", FrequencyHz = 1, SampleRate = 4, Amplitude = 0.5, PhaseDegrees = 0 };
        var buffer = new float[4];
        generator.GenerateBuffer(buffer, parameters);
        Assert.Equal(0.5f, buffer[0], 3);
        Assert.Equal(-0.5f, buffer[1], 3);
    }

    [Fact]
    public void LatexExpressionCompiler_ParsesLatexSineExpression()
    {
        var compiler = new LatexExpressionCompiler();
        var expression = compiler.Compile(@"A*\sin(2\pi t + \pi/2)");
        var value = expression(new FigureParameters(0, 440, 0.5, 0.25, 48_000, 0));
        Assert.Equal(0.5, value, 3);
    }
    [Fact]
    public void LatexExpressionCompiler_AcceptsUiAssignmentsImplicitFunctionMultiplicationAndFractions()
    {
        var compiler = new LatexExpressionCompiler();
        var expression = compiler.Compile(@"X(t)=\frac{A\sin(2\pi t)}{2}");
        var value = expression(new FigureParameters(0.25, 440, 0.8, 0, 48_000, 0));
        Assert.Equal(0.4, value, 3);
    }

}
