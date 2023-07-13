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
        P = 0d;
    }

    public override string ToString()
        => $"{Name} ({P:0.0000})";

    public void Reset()
    {
        P = Priori;
    }

    public double CalculateProbability(Question question, Answer answer)
    {
        P = answer switch
        {
            Answer.Yes => CalcProbabilityYes(question),
            Answer.No => CalcProbabilityNo(question),
            Answer.YesLikely => CalcProbabilityYesLikely(question),
            Answer.NoLikely => CalcProbabilityNoLikely(question),
            Answer.DontKnow => P,
            _ => throw new ApplicationException($"Неизвестный ответ {answer}."),
        };

        return P;
    }

    /// <summary>
    /// Вероятность выбора альтернативы при ответе ДА на вопрос.
    /// </summary>
    public double GetProbabilityYes(Question question)
    {
        var relation = Questions.GetById(question.Id);

        return relation != null
            ? relation.Yes
            : throw new ApplicationException($"Не найдена связь альтернативы {Id} со свидетельством {question.Id}.");
    }

    /// <summary>
    /// Вероятность выбора альтернативы при ответе НЕТ на вопрос.
    /// </summary>
    public double GetProbabilityNo(Question question)
    {
        var relation = Questions.GetById(question.Id);

        return relation != null
            ? relation.No
            : throw new ApplicationException($"Не найдена связь альтернативы {Id} со свидетельством {question.Id}.");
    }

    private double CalcProbabilityYes(Question question)
    {
        var Pyes = GetProbabilityYes(question);
        var Pno = GetProbabilityNo(question);

        return Pyes * P / (Pyes * P + Pno * (1 - P));
    }

    private double CalcProbabilityNo(Question question)
    {
        var Pyes = GetProbabilityYes(question);
        var Pno = GetProbabilityNo(question);

        return Pno * P / (Pno * P + Pyes * (1 - P));
    }

    private double CalcProbabilityYesLikely(Question question)
    {
        var yes = (int)Answer.Yes;
        var yesLikely = (int)Answer.YesLikely;
        var dontKnow = (int)Answer.DontKnow;

        var P0 = P;
        var P2 = CalcProbabilityYes(question);

        return (P2 - P0) / (yes - dontKnow) * yesLikely + P0;
    }

    public double CalcProbabilityNoLikely(Question question)
    {
        var dontKnow = (int)Answer.DontKnow;
        var noLikely = (int)Answer.NoLikely;
        var no = (int)Answer.No;

        var P0 = P;
        var P2 = CalcProbabilityNo(question);

        return (P0 - P2) / (dontKnow - no) * noLikely + P0;
    }
}
