using LissajousStudio.Core.Interfaces;
using LissajousStudio.Core.Models;
using NAudio.Wave;
using Serilog;

namespace LissajousStudio.Audio.Services;

/// <summary>NAudio-backed non-blocking audio engine that publishes every callback buffer.</summary>
public sealed class AudioEngine : IAudioEngine, IWaveProvider
{
    private readonly ISignalGenerator _generator;
    private readonly SignalParameters _parameters;
    private WaveOutEvent? _output;

    public AudioEngine(ISignalGenerator generator, SignalParameters parameters, StereoSampleBuffer sharedBuffer)
    {
        _generator = generator;
        _parameters = parameters;
        SharedBuffer = sharedBuffer;
        WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(parameters.SampleRate, 2);
    }

    public StereoSampleBuffer SharedBuffer { get; }
    public WaveFormat WaveFormat { get; }
    public bool IsRunning { get; private set; }

    public int Read(byte[] buffer, int offset, int count)
    {
        var floatsRequested = count / sizeof(float);
        var workBuffer = new float[floatsRequested];
        _generator.GenerateBuffer(workBuffer, _parameters);
        Buffer.BlockCopy(workBuffer, 0, buffer, offset, count);
        SharedBuffer.Publish(workBuffer, floatsRequested / 2, _parameters.SampleRate);
        return count;
    }

    public void Start()
    {
        if (IsRunning) return;
        _output = new WaveOutEvent { DesiredLatency = 40, NumberOfBuffers = 3 };
        _output.Init(this);
        _output.Play();
        IsRunning = true;
        Log.Information("Audio engine started at {SampleRate} Hz", _parameters.SampleRate);
    }

    public void Stop()
    {
        _output?.Stop();
        _output?.Dispose();
        _output = null;
        IsRunning = false;
        Log.Information("Audio engine stopped");
    }

    public void Dispose() => Stop();
}
