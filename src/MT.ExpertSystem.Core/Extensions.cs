namespace MT.ExpertSystem.Core;

internal static class Extensions
{
    /// <summary>
    /// Получить следующее свидетельство.
    /// </summary>
    /// <param name="questions"></param>
    public static QuestionWithAnswer? GetNext(this IEnumerable<QuestionWithAnswer> questions)
        => questions.Where(q => !q.Answer.HasValue).MaxBy(q => q.Cost);

    /// <summary>
    /// Получить связь альтернативы со свидетельством.
    /// </summary>
    /// <exception cref="ApplicationException"></exception>
    public static Relation Get(this IEnumerable<Relation> relations, string alternativeId, string questionId)
    {
        var relation = relations?.FirstOrDefault(r =>
            r.AlternativeId.Equals(alternativeId, StringComparison.OrdinalIgnoreCase) &&
            r.QuestionId.Equals(questionId, StringComparison.OrdinalIgnoreCase));

        return relation
            ?? throw new ApplicationException($"Не найдена связь альтернативы {alternativeId} со свидетельством {questionId}.");
    }

    /// <summary>
    /// Получить наиболее вероятную альтренативу.
    /// </summary>
    public static AlternativeWithProbability? GetBest(this IEnumerable<AlternativeWithProbability> alternatives)
        => alternatives.MaxBy(a => a.P);
}
