using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LissajousStudio.Core.Interfaces;
using LissajousStudio.Core.Models;

namespace LissajousStudio.App.ViewModels;

/// <summary>Main MVVM surface for transport, synthesis parameters, editable figure functions, and measurements.</summary>
public sealed partial class MainViewModel : ObservableObject
{
    private readonly IAudioEngine _audioEngine;
    private readonly SignalParameters _parameters;
    private readonly IEditableLatexFigure _latexFigure;

    public MainViewModel(IAudioEngine audioEngine, SignalParameters parameters, IEditableLatexFigure latexFigure)
    {
        _audioEngine = audioEngine;
        _parameters = parameters;
        _latexFigure = latexFigure;
        Buffer = audioEngine.SharedBuffer;
        latexXExpression = parameters.LatexXExpression;
        latexYExpression = parameters.LatexYExpression;
        StartCommand.Execute(null);
    }

    public StereoSampleBuffer Buffer { get; }
    public int SampleRate => _parameters.SampleRate;
    [ObservableProperty] private double frequencyHz = 440.0;
    [ObservableProperty] private double amplitude = 0.75;
    [ObservableProperty] private double phaseDegrees = 90.0;
    [ObservableProperty] private double framesPerSecond = 60.0;
    [ObservableProperty] private string latexXExpression = string.Empty;
    [ObservableProperty] private string latexYExpression = string.Empty;
    [ObservableProperty] private string latexStatus = "Built-in circle figure active";
    public bool IsRunning => _audioEngine.IsRunning;

    partial void OnFrequencyHzChanged(double value) => _parameters.FrequencyHz = value;
    partial void OnAmplitudeChanged(double value) => _parameters.Amplitude = value;
    partial void OnPhaseDegreesChanged(double value) => _parameters.PhaseDegrees = value;

    [RelayCommand] private void Start() { _audioEngine.Start(); OnPropertyChanged(nameof(IsRunning)); }
    [RelayCommand] private void Stop() { _audioEngine.Stop(); OnPropertyChanged(nameof(IsRunning)); }

    [RelayCommand]
    private void ApplyLatexFigure()
    {
        try
        {
            _latexFigure.UpdateExpressions(LatexXExpression, LatexYExpression);
            _parameters.LatexXExpression = LatexXExpression;
            _parameters.LatexYExpression = LatexYExpression;
            _parameters.FigureId = _latexFigure.Id;
            LatexStatus = $"LaTeX figure active: X={LatexXExpression}, Y={LatexYExpression}";
        }
        catch (Exception ex) when (ex is FormatException or InvalidOperationException or ArgumentException)
        {
            LatexStatus = $"Expression error: {ex.Message}";
        }
    }

    [RelayCommand]
    private void UseCircleFigure()
    {
        _parameters.FigureId = "circle";
        LatexStatus = "Built-in circle figure active";
    }
}
