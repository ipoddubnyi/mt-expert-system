namespace MT.ExpertSystem.Core;

public class Expert
{
    private readonly ExpertConfiguration _config;

    private int _currentQuestionNumber;

    public string Title => _config.Title;

    public QuestionWithAnswer[] Questions { get; private set; }

    public AlternativeWithProbability[] Alternatives { get; private set; }

    public QuestionWithAnswer? CurrentQuestion { get; private set; }

    public bool HasQuestions => Questions.Any(q => !q.Answer.HasValue);

    private Expert(ExpertConfiguration data)
    {
        _config = data;
        _currentQuestionNumber = 0;

        Questions = _config.Questions.Select(q => new QuestionWithAnswer(q)).ToArray();
        Alternatives = _config.Alternatives.Select(a => new AlternativeWithProbability(a)).ToArray();

        UpdateQuestions();
    }

    public Task SendAnswer(Answer answer)
    {
        if (CurrentQuestion == null)
            throw new ApplicationException("Нет текущего вопроса. Нельзя дать ответ.");

        CurrentQuestion.SetAnswer(_currentQuestionNumber, answer);

        UpdateAlternatives(CurrentQuestion);
        UpdateQuestions();

#if DEBUG
        // имитация долгой операции подсчёта вероятностей
        return Task.Delay(700);
#endif

        return Task.CompletedTask;
    }

    public AlternativeWithProbability? GetResult()
    {
        // если есть ещё вопросы, то результата ещё нет
        if (CurrentQuestion != null)
            return null;

        return Alternatives.GetBest();
    }

    /// <summary>
    /// Обновить текущие вероятности альтренатив при ответе на свидетельство.
    /// </summary>
    private void UpdateAlternatives(QuestionWithAnswer question)
    {
        foreach (var alternative in Alternatives)
            alternative.P = GetAlternativeProbability(alternative, question);
    }

    private double GetAlternativeProbability(AlternativeWithProbability alternative, QuestionWithAnswer question)
    {
        var relation = _config.Relations.Get(alternative.Id, question.Id);
        var P = alternative.P;
        var Pyes = relation.Yes;
        var Pno = relation.No;

        return question.Answer switch
        {
            Answer.Yes => BayesMath.ProbabilityYes(P, Pyes, Pno),
            Answer.No => BayesMath.ProbabilityNo(P, Pyes, Pno),
            Answer.YesLikely => BayesMath.ProbabilityYesLikely(P, Pyes, Pno),
            Answer.NoLikely => BayesMath.ProbabilityNoLikely(P, Pyes, Pno),
            Answer.DontKnow => P,
            _ => throw new ApplicationException($"Неизвестный ответ {question.Answer}."),
        };
    }

    /// <summary>
    /// Обновить цены свидетельств и текущее свидетельство.
    /// </summary>
    private void UpdateQuestions()
    {
        UpdateQuestionsCosts(Questions, Alternatives);
        CurrentQuestion = Questions.GetNext();
        _currentQuestionNumber += 1;
    }

    private IEnumerable<QuestionWithAnswer> UpdateQuestionsCosts(
        IEnumerable<QuestionWithAnswer> questions,
        IEnumerable<AlternativeWithProbability> alternatives)
    {
        var N = questions.Count();

        // количество отвеченных вопросов
        var M = questions.Count(q => q.Answer.HasValue);

        foreach (var question in questions)
        {
            var cost = 0d;

            foreach (var alternative in alternatives)
            {
                var relation = _config.Relations.Get(alternative.Id, question.Id);
                var p1 = relation.Yes * alternative.P * (N - M);
                var p2 = (1 - relation.Yes) * alternative.P * (1 - N + M);
                cost += Math.Abs(p1 - p2);
            }

            question.Cost = cost;
        }

        return questions;
    }

    //

    /// <summary>
    /// Начать работу с экспертной системой.
    /// </summary>
    /// <param name="data">Конфигурация экспертной системы.</param>
    public static Expert Start(ExpertConfiguration data)
        => new(data);
}
