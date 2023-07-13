using System.Xml.Serialization;

namespace MT.ExpertSystem.Core;

[XmlRoot("expert")]
public class Expert
{
    [XmlIgnore]
    public string Subject { get; set; }

    [XmlArray("questions")]
    [XmlArrayItem("question")]
    public Question[] Questions { get; set; }

    [XmlArray("alternatives")]
    [XmlArrayItem("alternative")]
    public Alternative[] Alternatives { get; set; }

    private Question? currentQuestion;
    private int? currentQuestionNumber;
    public event Func<Expert, Question, Answer>? AskQuestion;

    public Expert()
    {
        Subject = string.Empty;
        Questions = Array.Empty<Question>();
        Alternatives = Array.Empty<Alternative>();
    }

    public Alternative Start()
    {
        if (Questions.Length == 0)
            throw new ApplicationException("Список вопросов пуст.");

        if (Alternatives.Length == 0)
            throw new ApplicationException("Список возможных альтернатив пуст.");

        Reset();

        while (NextQuestion() && currentQuestion != null)
        {
            var answer = AskQuestionInvoke(currentQuestion);
            AnalizeAnswer(answer);
        }

        return Alternatives[0];
    }

    private void Reset()
    {
        currentQuestion = null;
        currentQuestionNumber = 0;
        Alternatives.Reset();
        Alternatives = Alternatives.SortByProbability().ToArray();
        Questions = Questions.UpdateCosts(Alternatives).ToArray();
    }

    private Answer AskQuestionInvoke(Question question)
    {
        if (null != AskQuestion)
            return AskQuestion(this, question);

        return Answer.DontKnow;
    }

    public bool NextQuestion()
    {
        currentQuestionNumber += 1;
        currentQuestion = Questions.GetFirstNotAnswered();
        return null != currentQuestion;
    }

    public void AnalizeAnswer(Answer answer)
    {
        // пересчёт вероятностей (в начале делать не надо!)
        if (null != currentQuestion)
        {
            foreach (var alternative in Alternatives)
            {
                alternative.CalculateProbability(currentQuestion, answer);
            }

            currentQuestion.Answer = answer;
            currentQuestion.Number = currentQuestionNumber;

            Alternatives = Alternatives.SortByProbability().ToArray();
            Questions = Questions.UpdateCosts(Alternatives).ToArray();
        }
    }
}
