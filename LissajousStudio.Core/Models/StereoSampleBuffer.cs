namespace LissajousStudio.Core.Models;

/// <summary>Thread-safe snapshot of interleaved stereo samples where left is X and right is Y.</summary>
public sealed class StereoSampleBuffer
{
    private float[] _interleaved;
    private long _version;
    private readonly object _gate = new();

    public StereoSampleBuffer(int frameCount = 2048, int sampleRate = 48_000)
    {
        FrameCount = frameCount;
        SampleRate = sampleRate;
        _interleaved = new float[frameCount * 2];
    }

    public int FrameCount { get; private set; }
    public int SampleRate { get; private set; }
    public long Version => Interlocked.Read(ref _version);

    public void Publish(ReadOnlySpan<float> interleaved, int frameCount, int sampleRate)
    {
        lock (_gate)
        {
            if (_interleaved.Length != interleaved.Length) _interleaved = new float[interleaved.Length];
            interleaved.CopyTo(_interleaved);
            FrameCount = frameCount;
            SampleRate = sampleRate;
            Interlocked.Increment(ref _version);
        }
    }

    public float[] Snapshot()
    {
        lock (_gate) return (float[])_interleaved.Clone();
    }
}
