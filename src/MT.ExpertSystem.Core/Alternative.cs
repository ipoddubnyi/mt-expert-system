using System.Diagnostics;
using System.Xml.Serialization;

namespace MT.ExpertSystem.Core;

/// <summary>
/// Альтернатива (гипотеза).
/// </summary>
[DebuggerDisplay("{Name} ({P})")]
public class Alternative
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; }

    /// <summary>
    /// Априорная вероятность выбора данной альтернативы.
    /// </summary>
    [XmlAttribute("priori")]
    public double Priori { get; set; }

    /// <summary>
    /// Вероятности принятия альтернативы при ответе на вопросы.
    /// </summary>
    [XmlElement("question")]
    public AlternativeQuestion[] Questions { get; set; }

    /// <summary>
    /// Текущая вероятность.
    /// </summary>
    [XmlIgnore]
    public double P { get; set; }

    public Alternative()
    {
        Name = string.Empty;
        Questions = Array.Empty<AlternativeQuestion>();
        P = 0.0;
    }

    public override string ToString()
        => $"{Name} ({P:0.0%})";

    public void Reset()
    {
        P = Priori;
    }

    public double CalculateProbability(Question question, Answer answer)
    {
        var Pyes = GetProbabilityYesForQuestion(question);
        var Pno = GetProbabilityNoForQuestion(question);

        P = answer switch
        {
            Answer.Yes => BayesMath.ProbabilityYes(P, Pyes, Pno),
            Answer.No => BayesMath.ProbabilityNo(P, Pyes, Pno),
            Answer.YesLikely => BayesMath.ProbabilityYesLikely(P, Pyes, Pno),
            Answer.NoLikely => BayesMath.ProbabilityNoLikely(P, Pyes, Pno),
            Answer.DontKnow => P,
            _ => throw new ApplicationException($"Неизвестный ответ {answer}."),
        };

        return P;
    }

    /// <summary>
    /// Вероятность выбора альтернативы при ответе ДА на вопрос.
    /// </summary>
    public double GetProbabilityYesForQuestion(Question question)
    {
        var relation = Questions.GetById(question.Id);

        return relation != null
            ? relation.Yes
            : throw new ApplicationException($"Не найдена связь альтернативы {Id} со свидетельством {question.Id}.");
    }

    /// <summary>
    /// Вероятность выбора альтернативы при ответе НЕТ на вопрос.
    /// </summary>
    public double GetProbabilityNoForQuestion(Question question)
    {
        var relation = Questions.GetById(question.Id);

        return relation != null
            ? relation.No
            : throw new ApplicationException($"Не найдена связь альтернативы {Id} со свидетельством {question.Id}.");
    }
}
