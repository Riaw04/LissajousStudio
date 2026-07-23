using LissajousStudio.Core.Models;

namespace LissajousStudio.Core.Interfaces;
public interface IWaveFileExporter { Task ExportAsync(string path, StereoSampleBuffer buffer, CancellationToken cancellationToken = default); }
