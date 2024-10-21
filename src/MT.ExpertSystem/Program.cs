using MT.ExpertSystem;
using MT.ExpertSystem.Core;

try
{
    if (args.Length == 0)
        throw new ArgumentException("Не указан xml с данными выбора.");

    //var expert = Expert.FromFileXml(args[0])
    //    ?? throw new ApplicationException("Не удалось загрузить xml с данными выбора.");

    var expert = Expert.FromFileJson(args[0])
        ?? throw new ApplicationException("Не удалось загрузить json с данными выбора.");

    while (expert.HasQuestions)
    {
        var question = expert.CurrentQuestion
            ?? throw new ApplicationException("Ошибка при формировании текущего вопроса.");

        var answer = AskQuestion(expert, question);

        await expert.SendAnswer(answer);
    }

    var result = expert.GetResult()
        ?? throw new ApplicationException("Ошибка при формировании результата.");

    PrintResult(expert, result);
}
catch (ExitException)
{
    Console.WriteLine("Выход...");
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}

const string CommandExit = "выход";

static Answer AskQuestion(Expert expert, Question question)
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

static void PrintTitle(Expert expert)
{
    Console.WriteLine(expert.Title);
}

static void PrintAlternatives(IEnumerable<Alternative> alternatives)
{
    Console.WriteLine("Варианты:");

    foreach (var alternative in alternatives)
        Console.WriteLine($" - {alternative}");
}

static void PrintQuestions(IEnumerable<Question> questions)
{
    Console.WriteLine("Вопросы:");

    foreach (var question in questions)
        Console.WriteLine($" - {question} - {AnswerToString(question.Answer)}");
}

static void PrintCurrentQuestion(Question question)
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

static string AnswerToString(Answer? answer)
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

static void PrintResult(Expert expert, Alternative alternative)
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

static Answer? ParseAnswer(string? line)
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
