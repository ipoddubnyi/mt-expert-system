namespace MT.ExpertSystem.Core;

internal static class Extensions
{
    public static Question? GetById(this IEnumerable<Question> questions, int id)
        => questions.FirstOrDefault(q => q.Id == id);

    public static Question? GetFirstNotAnswered(this IEnumerable<Question> questions)
        => questions.FirstOrDefault(q => !q.IsAnswer);

    public static AlternativeQuestion? GetById(this IEnumerable<AlternativeQuestion> questions, int id)
        => questions.FirstOrDefault(q => q.QuestionId == id);

    public static void Reset(this IEnumerable<Alternative> alternatives)
    {
        foreach (var alternative in alternatives)
            alternative.Reset();
    }

    public static IEnumerable<Alternative> SortByProbability(this IEnumerable<Alternative> alternatives)
        => alternatives.OrderByDescending(a => a.P);

    /// <summary>
    /// Обновить цены свидетельств, чтобы задавать вопросы в актуальном порядке.
    /// </summary>
    public static IEnumerable<Question> UpdateCosts(
        this IEnumerable<Question> questions,
        IEnumerable<Alternative> alternatives)
    {
        var N = questions.Count();

        // количество отвеченных вопросов
        var M = questions.Count(q => q.IsAnswer);

        foreach (var question in questions)
        {
            double sum = 0d;

            foreach (var alternative in alternatives)
            {
                var p1 = alternative.GetProbabilityYes(question) * alternative.P * (N - M);
                var p2 = (1 - alternative.GetProbabilityYes(question)) * alternative.P * (1 - N + M);
                sum += Math.Abs(p1 - p2);
            }

            question.Cost = sum;
        }

        return questions
            .OrderBy(q => q.IsAnswer)
            .ThenByDescending(q => q.Cost);
    }
}
