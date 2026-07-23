using LissajousStudio.Core.Models;

namespace LissajousStudio.Core.Interfaces;

/// <summary>Defines a replaceable mathematical function that converts synthesis phase into XY samples.</summary>
public interface ILissajousFigure
{
    /// <summary>Stable identifier used by presets and UI selection.</summary>
    string Id { get; }

    /// <summary>Human-readable name for menus and preset documentation.</summary>
    string DisplayName { get; }

    /// <summary>Evaluates one normalized XY point. Returned values should generally stay within -1..1.</summary>
    LissajousPoint Evaluate(in FigureParameters parameters);
}
