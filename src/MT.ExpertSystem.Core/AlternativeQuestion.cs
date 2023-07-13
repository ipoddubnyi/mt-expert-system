using System.Xml.Serialization;

namespace MT.ExpertSystem.Core;

/// <summary>
/// Связь альтернатива-свидетельство.
/// <br/>
/// Какая вероятность принять данную альтернативу,
/// если ответ на свидетельство будет да/нет.
/// </summary>
public class AlternativeQuestion
{
    [XmlAttribute("id")]
    public int QuestionId { get; set; }

    [XmlAttribute("yes")]
    public double Yes { get; set; }

    [XmlAttribute("no")]
    public double No { get; set; }
}
