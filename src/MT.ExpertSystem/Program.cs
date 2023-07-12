using MT.ExpertSystem.Core;
using System.Xml.Serialization;

try
{
    if (1 != args.Length)
        throw new ArgumentException("Не указан xml с данными выбора.");

    var expert = Deserialize(args[0]);
    expert.AskQuestion += AskQuestion;
    var alt = expert.Start();

    Console.WriteLine("Ваш выбор:");
    Console.WriteLine(alt);
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}

Console.ReadKey();

static Answer AskQuestion(Question question)
{
    Console.WriteLine(question.Text);
    Console.WriteLine(" 2 - да");
    Console.WriteLine(" 1 - скорее да");
    Console.WriteLine(" 0 - не знаю");
    Console.WriteLine("-1 - скорее нет");
    Console.WriteLine("-2 - нет");
    Console.WriteLine();

    int ans;
    var line = Console.ReadLine();
    while (!int.TryParse(line, out ans))
    {
        Console.WriteLine("Неверный формат ответа.");
        line = Console.ReadLine();
    }
    Console.WriteLine();

    switch (ans)
    {
        case 2:
            return Answer.Yes;
        case 1:
            return Answer.YesLikely;
        case 0:
            return Answer.DontKnow;
        case -1:
            return Answer.NoLikely;
        case -2:
            return Answer.No;
    }

    return Answer.DontKnow;
}

static Expert Deserialize(string filePath)
{
    using (var fs = new FileStream(filePath, FileMode.Open))
    {
        var serializer = new XmlSerializer(typeof(Expert));
        return serializer.Deserialize(fs) as Expert;
    }
}
