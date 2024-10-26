using MT.ExpertSystem.Core;

namespace MT.ExpertSystem;

class ConsoleUserInterface : IUserInterface
{
    private const string CommandExit = "выход";

    public Answer AskQuestion(Expert expert, QuestionWithAnswer question)
    {
        Console.Clear();

        PrintTitle(expert);
        Console.WriteLine();

        PrintAlternatives(expert.Alternatives);
        Console.WriteLine();

        PrintQuestions(expert.Questions);
        Console.WriteLine();

        PrintCurrentQuestion(question);
        Console.WriteLine();
        Console.Write("> ");

        Answer? answer = null;

        do
        {
            var line = Console.ReadLine();
            answer = ParseAnswer(line);

            if (answer == null)
                Console.WriteLine("Неверный формат ответа. Попробуйте снова:");
        }
        while (answer == null);

        Console.WriteLine();

        return answer.Value;
    }

    private void PrintTitle(Expert expert)
    {
        Console.WriteLine(expert.Title);
    }

    private void PrintAlternatives(IEnumerable<AlternativeWithProbability> alternatives)
    {
        Console.WriteLine("Варианты:");

        var alternativesSorted = alternatives.OrderByDescending(a => a.P);

        foreach (var alternative in alternativesSorted)
            Console.WriteLine($" - {alternative}");
    }

    private void PrintQuestions(IEnumerable<QuestionWithAnswer> questions)
    {
        Console.WriteLine("Вопросы:");

        var questionsSorted = questions
            .OrderBy(q => q.Answer.HasValue)
            .ThenByDescending(q => q.Cost);

        foreach (var question in questionsSorted)
            Console.WriteLine($" - {question} - {AnswerToString(question.Answer)}");
    }

    private void PrintCurrentQuestion(QuestionWithAnswer question)
    {
        Console.WriteLine(new string('-', 10));
        Console.WriteLine();

        Console.WriteLine($"Для выхода из программы напечатайте: {CommandExit}");
        Console.WriteLine();

        Console.WriteLine("Вопрос:");
        Console.WriteLine(question.Text.ToUpper());
        Console.WriteLine();

        foreach (var answer in Enum.GetValues<Answer>().OrderByDescending(a => a))
            Console.WriteLine($"{(int)answer,2} - {AnswerToString(answer)}");
    }

    private string AnswerToString(Answer? answer)
    {
        return answer switch
        {
            Answer.Yes => "Да",
            Answer.YesLikely => "Скорее да",
            Answer.DontKnow => "Не знаю",
            Answer.NoLikely => "Скорее нет",
            Answer.No => "Нет",
            _ => string.Empty,
        };
    }

    public void PrintResult(Expert expert, AlternativeWithProbability alternative)
    {
        Console.Clear();

        PrintAlternatives(expert.Alternatives);
        Console.WriteLine();

        PrintQuestions(expert.Questions.OrderBy(q => q.Number));
        Console.WriteLine();

        Console.WriteLine(new string('-', 10));
        Console.WriteLine();

        Console.WriteLine("Ваш выбор:");
        Console.WriteLine(alternative.ToString().ToUpper());
        Console.WriteLine();

        Console.WriteLine("Нажмите любую клавишу для завершения...");
        Console.ReadKey();
    }

    private Answer? ParseAnswer(string? line)
    {
        if (string.IsNullOrEmpty(line))
            return null;

        if (line.Equals(CommandExit, StringComparison.OrdinalIgnoreCase))
            throw new ExitException();

        if (!int.TryParse(line, out var answer))
            return null;

        return answer switch
        {
            2 => Answer.Yes,
            1 => Answer.YesLikely,
            0 => Answer.DontKnow,
            -1 => Answer.NoLikely,
            -2 => Answer.No,
            _ => null,
        };
    }
}
