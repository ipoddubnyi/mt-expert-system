using System.Xml.Serialization;

namespace MT.ExpertSystem.Core;

[XmlRoot("expert")]
public class Expert
{
    [XmlAttribute("title")]
    public string Title { get; set; }

    [XmlArray("questions")]
    [XmlArrayItem("question")]
    public Question[] Questions { get; set; }

    [XmlArray("alternatives")]
    [XmlArrayItem("alternative")]
    public Alternative[] Alternatives { get; set; }

    [XmlIgnore]
    public bool HasQuestions => Questions.Any(q => !q.HasAnswer);

    [XmlIgnore]
    public Question? CurrentQuestion { get; private set; }

    private int currentQuestionNumber;

    private Expert()
    {
        Title = string.Empty;
        Questions = Array.Empty<Question>();
        Alternatives = Array.Empty<Alternative>();
    }

    public void Reset()
    {
        Alternatives.Reset();
        Questions.Reset();

        Alternatives = Alternatives.SortByProbability().ToArray();
        Questions = Questions.UpdateCostsAndSort(Alternatives).ToArray();
        CurrentQuestion = Questions.GetFirstNotAnswered();
        currentQuestionNumber = 1;
    }

    public Task SendAnswer(Answer answer)
    {
        if (CurrentQuestion == null)
            throw new ApplicationException("Нет текущего вопроса. Нельзя дать ответ.");

        CurrentQuestion.SetAnswer(currentQuestionNumber, answer);

        // пересчёт вероятностей
        foreach (var alternative in Alternatives)
            alternative.CalculateProbability(CurrentQuestion, answer);

        Alternatives = Alternatives.SortByProbability().ToArray();
        Questions = Questions.UpdateCostsAndSort(Alternatives).ToArray();
        CurrentQuestion = Questions.GetFirstNotAnswered();
        currentQuestionNumber += 1;

#if DEBUG
        // долгая операция подсчёта вероятностей
        return Task.Delay(700);
#endif

        return Task.CompletedTask;
    }

    public Alternative? GetResult()
    {
        // если есть ещё вопросы, то результата ещё нет
        if (CurrentQuestion != null)
            return null;

        return Alternatives.FirstOrDefault();
    }

    public static Expert? FromFile(string filePath)
    {
        using var fs = new FileStream(filePath, FileMode.Open);
        var serializer = new XmlSerializer(typeof(Expert));
        var expert = serializer.Deserialize(fs) as Expert;
        expert?.Reset();
        return expert;
    }
}
