namespace LissajousStudio.Core.Interfaces;

/// <summary>Represents the runtime-editable LaTeX figure used by the UI expression editor.</summary>
public interface IEditableLatexFigure : ILissajousFigure
{
    string XExpression { get; }
    string YExpression { get; }
    void UpdateExpressions(string xExpression, string yExpression);
}
