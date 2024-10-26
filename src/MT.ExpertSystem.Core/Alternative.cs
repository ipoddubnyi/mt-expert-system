using System.Diagnostics;

namespace MT.ExpertSystem.Core;

/// <summary>
/// Альтернатива (гипотеза).
/// </summary>
[DebuggerDisplay("{Id}: {Name}")]
public class Alternative
{
    /// <summary>
    /// Идентификатор.
    /// </summary>
    public string Id { get; set; } = default!;

    /// <summary>
    /// Название альтернативы.
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Априорная вероятность выбора данной альтернативы.
    /// </summary>
    public double Priori { get; set; } = default;
}
