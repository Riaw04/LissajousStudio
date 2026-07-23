using System.Text.Json;
using LissajousStudio.Core.Models;

namespace LissajousStudio.Presets.Services;

/// <summary>JSON preset persistence service.</summary>
public sealed class PresetManager
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };
    public async Task SaveAsync(string path, SignalParameters parameters, CancellationToken cancellationToken = default)
        => await File.WriteAllTextAsync(path, JsonSerializer.Serialize(parameters, Options), cancellationToken);
    public async Task<SignalParameters> LoadAsync(string path, CancellationToken cancellationToken = default)
        => JsonSerializer.Deserialize<SignalParameters>(await File.ReadAllTextAsync(path, cancellationToken), Options) ?? new SignalParameters();
}
