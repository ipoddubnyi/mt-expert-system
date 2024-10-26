using System.Diagnostics;

namespace MT.ExpertSystem.Core;

/// <summary>
/// Вопрос (свидетельство).
/// </summary>
[DebuggerDisplay("{Id}: {Text}")]
public class Question
{
    /// <summary>
    /// Идентификатор.
    /// </summary>
    public string Id { get; set; } = default!;

    /// <summary>
    /// Текст вопроса.
    /// </summary>
    public string Text { get; set; } = default!;
}
