using System.Xml.Serialization;

namespace MT.ExpertSystem.Core;

[XmlRoot("expert")]
public class Expert
{
    public delegate Answer AskQuestionHandler(Question question);

    [XmlIgnore]
    public string Subject { get; set; }

    [XmlArray("questions")]
    [XmlArrayItem("question")]
    public Question[] Questions { get; set; }

    [XmlArray("alternatives")]
    [XmlArrayItem("alternative")]
    public Alternative[] Alternatives { get; set; }

    private Question currentQuestion;
    private int ansCount;
    public event AskQuestionHandler AskQuestion;

    public Expert()
    {
        Subject = string.Empty;
        Questions = new Question[0];
        Alternatives = new Alternative[0];
    }

    public Alternative Start()
    {
        CheckEntries();
        Reset();

        while (NextQuestion())
        {
            var answer = AskQuestionInvoke(currentQuestion);
            AnalizeAnswer(answer);
        }

        return Alternatives[0];
    }

    private void Reset()
    {
        currentQuestion = null;
        ansCount = 0;
        Alternatives.Reset();
        ReCountQuestionsCost();
        Alternatives = Alternatives.SortByProbability().ToArray();
    }

    private void CheckEntries()
    {
        if (0 == Questions.Length)
            throw new Exception("Список вопросов пуст.");

        if (0 == Alternatives.Length)
            throw new Exception("Список возможных альтернатив пуст.");
    }

    private Answer AskQuestionInvoke(Question question)
    {
        if (null != AskQuestion)
            return AskQuestion(question);

        return Answer.DontKnow;
    }

    private void ReCountQuestionsCost()
    {
        int N = Questions.Length;

        foreach (var question in Questions)
        {
            double sum = 0d;

            foreach (var alternative in Alternatives)
            {
                var p1 = alternative.Pyes(question) * alternative.P * (N - ansCount);
                var p2 = (1 - alternative.Pyes(question)) * alternative.P * (1 - N + ansCount);
                sum += Math.Abs(p1 - p2);
            }

            question.Cost = sum;
        }

        Questions = Questions.SortByCost().ToArray();
    }

    public bool NextQuestion()
    {
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
            ansCount += 1;

            Alternatives = Alternatives.SortByProbability().ToArray();
            ReCountQuestionsCost();
        }
    }
}
