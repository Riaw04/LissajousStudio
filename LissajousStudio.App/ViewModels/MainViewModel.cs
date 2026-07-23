using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LissajousStudio.Core.Interfaces;
using LissajousStudio.Core.Models;

namespace LissajousStudio.App.ViewModels;

/// <summary>Main MVVM surface for transport, synthesis parameters, and measurements.</summary>
public sealed partial class MainViewModel : ObservableObject
{
    private readonly IAudioEngine _audioEngine;
    private readonly SignalParameters _parameters;
    private DateTime _lastFps = DateTime.UtcNow;
    private int _frames;

    public MainViewModel(IAudioEngine audioEngine, SignalParameters parameters)
    {
        _audioEngine = audioEngine;
        _parameters = parameters;
        Buffer = audioEngine.SharedBuffer;
        StartCommand.Execute(null);
    }

    public StereoSampleBuffer Buffer { get; }
    public int SampleRate => _parameters.SampleRate;
    [ObservableProperty] private double frequencyHz = 440.0;
    [ObservableProperty] private double amplitude = 0.75;
    [ObservableProperty] private double phaseDegrees = 90.0;
    [ObservableProperty] private double framesPerSecond = 60.0;
    public bool IsRunning => _audioEngine.IsRunning;

    partial void OnFrequencyHzChanged(double value) => _parameters.FrequencyHz = value;
    partial void OnAmplitudeChanged(double value) => _parameters.Amplitude = value;
    partial void OnPhaseDegreesChanged(double value) => _parameters.PhaseDegrees = value;

    [RelayCommand] private void Start() { _audioEngine.Start(); OnPropertyChanged(nameof(IsRunning)); }
    [RelayCommand] private void Stop() { _audioEngine.Stop(); OnPropertyChanged(nameof(IsRunning)); }
}
