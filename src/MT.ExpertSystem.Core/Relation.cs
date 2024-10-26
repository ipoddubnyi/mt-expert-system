namespace MT.ExpertSystem.Core;

/// <summary>
/// Связь альтернативы и свидетельства.
/// </summary>
public class Relation
{
    /// <summary>
    /// Идентификатор альтренативы.
    /// </summary>
    public string AlternativeId { get; set; } = default!;

    /// <summary>
    /// Идентификатор свидетельства (вопроса).
    /// </summary>
    public string QuestionId { get; set; } = default!;

    /// <summary>
    /// Вероятность выбора альтренативы при ответе Да на вопрос.
    /// </summary>
    public double Yes { get; set; } = default;

    /// <summary>
    /// Вероятность выбора альтренативы при ответе Нет на вопрос.
    /// </summary>
    public double No { get; set; } = default;
}
