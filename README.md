# LissajousStudio

LissajousStudio is a .NET 8 WPF real-time vector synthesizer for Windows 10/11. The left audio channel is the X axis, the right audio channel is the Y axis, and the interleaved stereo sample buffer is the single source of truth for audio playback, oscilloscope rendering, oscilloscope hardware output, WAV export, and future analysis.

## Architecture

The solution follows Clean Architecture and MVVM:

- `LissajousStudio.App` — WPF shell, dependency injection, view models, views, themes.
- `LissajousStudio.Core` — interfaces, models, application state, configuration contracts.
- `LissajousStudio.DSP` — oscillators, signal generators, DSP math, stereo synthesis engine foundation.
- `LissajousStudio.Audio` — NAudio output engine, callback buffer generation, latency setup.
- `LissajousStudio.Rendering` — SkiaSharp WPF XY oscilloscope control with grid, glow, zoom, high-DPI rendering.
- `LissajousStudio.Presets` — JSON serialization for presets and projects.
- `LissajousStudio.Shared` — shared logging and utilities.
- `LissajousStudio.Tests` — xUnit coverage for deterministic DSP behavior.

## Build and run

1. Install Visual Studio 2022 with the .NET desktop development workload.
2. Clone the repository.
3. Open `LissajousStudio.sln`.
4. Restore NuGet packages.
5. Set `LissajousStudio.App` as the startup project.
6. Build and run.

CLI on Windows:

```powershell
dotnet restore
dotnet build LissajousStudio.sln
dotnet run --project LissajousStudio.App
```

## Initial behavior

On startup the app synthesizes a stereo signal where left is sine and right is sine shifted by 90 degrees, producing a circle on the SkiaSharp oscilloscope. Start/Stop, frequency, amplitude, phase, sample-rate, FPS, and measurements are exposed through MVVM bindings.

## Editing figures in the UI with LaTeX-like expressions

Use the **LaTeX Function Editor** in the bottom control panel to type separate `X(t)` and `Y(t)` expressions, then click **Apply LaTeX Figure**. Supported tokens are `t`, `A`, `f`, `\pi`, `\sin`, `\cos`, `\tan`, `\sqrt`, `\abs`, `\frac{a}{b}`, `+`, `-`, `*`, `/`, and `^`. You can paste either the right-hand side or a full assignment such as `X(t)=A\sin(2\pi t)`.

Example UI expressions:

```text
X(t)=A\sin(3*2\pi t)
Y(t)=\frac{A\sin(2*2\pi t + \pi/2)}{1}
```

When applied, the UI switches `SignalParameters.FigureId` to `latex`, so the same evaluated stereo buffer continues to feed audio output, rendering, and future exporters.

## Adding your own Lissajous figures

Add a C# class that implements `ILissajousFigure`, return one normalized `LissajousPoint` from `Evaluate`, and register it in `LissajousStudio.App/App.xaml.cs` as another `ILissajousFigure` service. Set `SignalParameters.FigureId` to your figure `Id` from presets or UI selection.

Example:

```csharp
public sealed class MyButterflyFigure : ILissajousFigure
{
    public string Id => "my-butterfly";
    public string DisplayName => "My Butterfly";

    public LissajousPoint Evaluate(in FigureParameters p)
    {
        var x = Math.Sin(Math.Tau * p.Phase * 5.0) * p.Amplitude;
        var y = Math.Sin(Math.Tau * p.Phase * 4.0 + Math.Cos(Math.Tau * p.Phase)) * p.Amplitude;
        return new LissajousPoint((float)x, (float)y);
    }
}
```

For quick experiments, instantiate `DelegateLissajousFigure` with a lambda. Keep output values in `-1..1`; the generator clamps samples before publishing them to the shared stereo buffer.

## Developer notes

- Never generate graphics independently from audio. Rendering consumes `StereoSampleBuffer` snapshots only.
- Keep audio callback code allocation-free where possible and never block the callback thread.
- UI, render, audio, and background work should stay separated.
- Add new DSP modules behind interfaces and register implementations in `App.xaml.cs`.

## Roadmap

- ASIO and WASAPI device picker abstraction.
- WAV export wired into UI.
- Preset browser and project documents.
- Persistence decay controls and screenshot export.
- SIMD/vectorized DSP kernels.
- Calibration tools for real oscilloscope output.
