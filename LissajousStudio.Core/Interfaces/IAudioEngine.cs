using LissajousStudio.Core.Models;

namespace LissajousStudio.Core.Interfaces;

public interface IAudioEngine : IDisposable
{
    StereoSampleBuffer SharedBuffer { get; }
    bool IsRunning { get; }
    void Start();
    void Stop();
}
