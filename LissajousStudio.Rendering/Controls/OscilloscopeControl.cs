using System.Windows;
using System.Windows.Input;
using LissajousStudio.Core.Models;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;

namespace LissajousStudio.Rendering.Controls;

/// <summary>SkiaSharp WPF control that renders the canonical stereo buffer as an XY oscilloscope.</summary>
public sealed class OscilloscopeControl : SKElement
{
    public static readonly DependencyProperty SampleBufferProperty = DependencyProperty.Register(
        nameof(SampleBuffer), typeof(StereoSampleBuffer), typeof(OscilloscopeControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    private float _zoom = 0.9f;
    private SKPoint _pan;
    private long _lastVersion = -1;

    public OscilloscopeControl()
    {
        PaintSurface += OnPaintSurface;
        MouseWheel += (_, e) => { _zoom = Math.Clamp(_zoom + e.Delta / 2400f, 0.2f, 4f); InvalidateVisual(); };
        CompositionTarget.Rendering += (_, _) => { if (SampleBuffer?.Version != _lastVersion) InvalidateVisual(); };
    }

    public StereoSampleBuffer? SampleBuffer { get => (StereoSampleBuffer?)GetValue(SampleBufferProperty); set => SetValue(SampleBufferProperty, value); }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(new SKColor(9, 13, 18));
        var info = e.Info;
        var center = new SKPoint(info.Width / 2f + _pan.X, info.Height / 2f + _pan.Y);
        DrawGrid(canvas, info, center);
        var samples = SampleBuffer?.Snapshot() ?? Array.Empty<float>();
        _lastVersion = SampleBuffer?.Version ?? -1;
        if (samples.Length < 4) return;
        using var glow = new SKPaint { Color = new SKColor(255, 210, 64, 80), StrokeWidth = 8, IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeCap = SKStrokeCap.Round };
        using var trace = new SKPaint { Color = new SKColor(255, 222, 88), StrokeWidth = 2, IsAntialias = true, Style = SKPaintStyle.Stroke };
        using var path = new SKPath();
        for (var i = 0; i < samples.Length / 2; i++)
        {
            var x = center.X + samples[i * 2] * info.Width * 0.45f * _zoom;
            var y = center.Y - samples[i * 2 + 1] * info.Height * 0.45f * _zoom;
            if (i == 0) path.MoveTo(x, y); else path.LineTo(x, y);
        }
        canvas.DrawPath(path, glow);
        canvas.DrawPath(path, trace);
    }

    private static void DrawGrid(SKCanvas canvas, SKImageInfo info, SKPoint center)
    {
        using var grid = new SKPaint { Color = new SKColor(36, 52, 64), StrokeWidth = 1, IsAntialias = true };
        for (var x = 0; x < info.Width; x += 64) canvas.DrawLine(x, 0, x, info.Height, grid);
        for (var y = 0; y < info.Height; y += 64) canvas.DrawLine(0, y, info.Width, y, grid);
        using var axis = new SKPaint { Color = new SKColor(0, 220, 255, 110), StrokeWidth = 1.5f, IsAntialias = true };
        canvas.DrawLine(center.X, 0, center.X, info.Height, axis);
        canvas.DrawLine(0, center.Y, info.Width, center.Y, axis);
    }
}
