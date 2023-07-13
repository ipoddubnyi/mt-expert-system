using MT.ExpertSystem.Core;
using System.Linq;
using System.Xml.Serialization;

try
{
    if (args.Length == 0)
        throw new ArgumentException("Не указан xml с данными выбора.");

    var expert = Deserialize(args[0])
        ?? throw new ApplicationException("Не удалось загрузить xml с данными выбора.");

    expert.AskQuestion += AskQuestion;
    var alternative = expert.Start();

    PrintResult(expert, alternative);
    Console.ReadKey();
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}

static Answer AskQuestion(Expert expert, Question question)
{
    Console.Clear();

    PrintAlternatives(expert.Alternatives);
    Console.WriteLine();

    PrintQuestions(expert.Questions);
    Console.WriteLine();

    PrintCurrentQuestion(question);
    Console.WriteLine();

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

static void PrintAlternatives(IEnumerable<Alternative> alternatives)
{
    Console.WriteLine("Альтернативы:");

    foreach (var alternative in alternatives)
        Console.WriteLine($" - {alternative.Name} ({alternative.P:0.0%})");
}

static void PrintQuestions(IEnumerable<Question> questions)
{
    Console.WriteLine("Свидетельства:");

    foreach (var question in questions)
        Console.WriteLine($" - {question.Text} ({question.Cost:0.00}) - {AnswerToString(question.Answer)}");
}

static void PrintCurrentQuestion(Question question)
{
    Console.WriteLine(question.Text);
    foreach (var answer in Enum.GetValues<Answer>().OrderByDescending(a => a))
        Console.WriteLine($"{(int)answer,2} - {AnswerToString(answer)}");

    Console.WriteLine();
    Console.WriteLine("Для выхода из программы напечатайте: выйти");
    Console.WriteLine();
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

    Console.WriteLine("Ваш выбор:");
    Console.WriteLine($" - {alternative.Name} ({alternative.P:0.0%})");
    Console.WriteLine();

    Console.WriteLine("Нажмите любую клавишу для завершения.");
}

static Answer? ParseAnswer(string? line)
{
    if (line != null && line.Equals("выйти", StringComparison.OrdinalIgnoreCase))
        throw new ApplicationException("Выход");

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

static Expert? Deserialize(string filePath)
{
    using var fs = new FileStream(filePath, FileMode.Open);
    var serializer = new XmlSerializer(typeof(Expert));
    return serializer.Deserialize(fs) as Expert;
}
