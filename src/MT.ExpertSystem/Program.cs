using MT.ExpertSystem;
using MT.ExpertSystem.Core;
using System.Text.Json;

try
{
    if (args.Length == 0)
        throw new ArgumentException("Не указан json с данными выбора.");

    var config = ReadConfig(args[0])
        ?? throw new ApplicationException("Не удалось загрузить json с данными выбора.");

    var expert = Expert.Start(config);
    var ui = new ConsoleUserInterface();

    while (expert.HasQuestions)
    {
        var question = expert.CurrentQuestion
            ?? throw new ApplicationException("Ошибка при формировании текущего вопроса.");

        var answer = ui.AskQuestion(expert, question);

        await expert.SendAnswer(answer);
    }

    var result = expert.GetResult()
        ?? throw new ApplicationException("Ошибка при формировании результата.");

    ui.PrintResult(expert, result);
}
catch (ExitException)
{
    Console.WriteLine("Выход...");
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}

static ExpertConfiguration? ReadConfig(string filePath)
{
    using var fs = new FileStream(filePath, FileMode.Open);
    return JsonSerializer.Deserialize<ExpertConfiguration>(fs, new JsonSerializerOptions()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
    });
}
