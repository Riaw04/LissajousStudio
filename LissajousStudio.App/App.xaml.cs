using System.Windows;
using LissajousStudio.App.ViewModels;
using LissajousStudio.Audio.Services;
using LissajousStudio.Core.Interfaces;
using LissajousStudio.Core.Models;
using LissajousStudio.DSP.SignalGenerators;
using LissajousStudio.Shared.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LissajousStudio.App;

public partial class App : Application
{
    private IHost? _host;
    protected override void OnStartup(StartupEventArgs e)
    {
        LoggingBootstrapper.Configure();
        _host = Host.CreateDefaultBuilder().ConfigureServices(s => s
            .AddSingleton<SignalParameters>()
            .AddSingleton<StereoSampleBuffer>()
            .AddSingleton<ISignalGenerator, LissajousSignalGenerator>()
            .AddSingleton<IAudioEngine, AudioEngine>()
            .AddSingleton<MainViewModel>()).Build();
        Resources["Services"] = _host.Services;
        base.OnStartup(e);
    }
    protected override void OnExit(ExitEventArgs e) { _host?.Dispose(); base.OnExit(e); }
}
