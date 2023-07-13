using System.Diagnostics;
using System.Xml.Serialization;

namespace MT.ExpertSystem.Core;

/// <summary>
/// Вопрос (свидетельство).
/// </summary>
[DebuggerDisplay("{Text} ({Cost})")]
public class Question
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlText]
    public string Text { get; set; }

    [XmlIgnore]
    public double Cost { get; set; }

    [XmlIgnore]
    public Answer? Answer { get; set; }

    /// <summary>
    /// Номер по порядку.
    /// </summary>
    [XmlIgnore]
    public int? Number { get; set; }

    [XmlIgnore]
    public bool HasAnswer => Answer.HasValue;

    public Question()
    {
        Text = string.Empty;
        Cost = 0d;
        Answer = null;
    }

    public void SetAnswer(int number, Answer answer)
    {
        Number = number;
        Answer = answer;
    }

    public void Reset()
    {
        Number = null;
        Answer = null;
    }

    public override string ToString()
        => $"{Text} ({Cost:0.00})";
}
