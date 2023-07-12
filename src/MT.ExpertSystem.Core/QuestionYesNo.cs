using System.Xml.Serialization;

namespace MT.ExpertSystem.Core;

public class QuestionYesNo
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("yes")]
    public double Yes { get; set; }

    [XmlAttribute("no")]
    public double No { get; set; }
}
