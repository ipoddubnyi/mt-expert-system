using System.Diagnostics;
using System.Xml.Serialization;

namespace MT.ExpertSystem.Core
{
    /// <summary>Вопрос (свидетельство).</summary>
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

        [XmlIgnore]
        public bool IsAnswer => Answer.HasValue;

        public Question()
        {
            Cost = 0d;
            Answer = null;
        }

        public override string ToString()
        {
            return $"{Text} ({Cost:0.0000})";
        }
    }
}
