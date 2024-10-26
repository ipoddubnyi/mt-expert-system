namespace MT.ExpertSystem.Core;

/// <summary>
/// Конфигурация экспертной системы.
/// Содержит вопросы, альтернативы, матрицу связей и др.
/// </summary>
public class ExpertConfiguration
{
    /// <summary>
    /// Заголовок выбора.
    /// </summary>
    public string Title { get; set; } = default!;

    /// <summary>
    /// Свидетельства (вопросы).
    /// </summary>
    public Question[] Questions { get; set; } = default!;

    /// <summary>
    /// Альтернативы.
    /// </summary>
    public Alternative[] Alternatives { get; set; } = default!;

    /// <summary>
    /// Матрица связей альтернатив и свидетельств.
    /// </summary>
    public Relation[] Relations { get; set; } = default!;
}
