using System.Diagnostics;
using System.Xml.Serialization;

namespace MT.ExpertSystem.Core
{
    /// <summary>Альтернатива (гипотеза).</summary>
    [DebuggerDisplay("{Name} ({P})")]
    public class Alternative
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("aprior")]
        public double Aprior { get; set; }

        [XmlElement("question")]
        public QuestionYesNo[] Questions { get; set; }

        /// <summary>Текущая вероятность.</summary>
        [XmlIgnore]
        public double P { get; set; }

        public Alternative()
        {
            Questions = new QuestionYesNo[0];
            P = 0d;
        }

        public override string ToString()
        {
            return $"{Name} ({P:0.0000})";
        }

        public void Reset()
        {
            P = Aprior;
        }

        /// <summary>Вероятность выбора гипотезы при ответе ДА на вопрос.</summary>
        public double Pyes(Question question)
        {
            return Questions.GetById(question.Id).Yes;
        }

        /// <summary>Вероятность выбора гипотезы при ответе НЕТ на вопрос.</summary>
        public double Pno(Question question)
        {
            return Questions.GetById(question.Id).No;
        }

        public double GetPYes(Question question)
        {
            return Pyes(question) * P / (Pyes(question) * P + Pno(question) * (1 - P));
        }

        public double GetPNo(Question question)
        {
            return Pno(question) * P / (Pno(question) * P + Pyes(question) * (1 - P));
        }

        public double GetPYesLikely(Question question)
        {
            //P(H|R) = ( P(H|E) - P(H) ) / (ansYes - ansDontKnow) + P(H)

            var ansYes = (int)Answer.Yes;
            var ansYesLikely = (int)Answer.YesLikely;
            var ansDontKnow = (int)Answer.DontKnow;

            var p0 = P;
            var p2 = GetPYes(question);
            //return (p2 - p0) / (2 - 0) * 1 + p0;
            return (p2 - p0) / (ansYes - ansDontKnow) * ansYesLikely + p0;
        }

        public double GetPNoLikely(Question question)
        {
            //P(H|R) = ( P(H) - P(H|неE) ) / (ansDontKnow + ansNo) + 0

            var ansDontKnow = (int)Answer.DontKnow;
            var ansNoLikely = (int)Answer.NoLikely;
            var ansNo = (int)Answer.No;

            var p0 = P;
            var p2 = GetPNo(question);
            //return (p0 - p2) / (0 - -2) * -1 + p0;
            return (p0 - p2) / (ansDontKnow - ansNo) * ansNoLikely + p0;
        }

        public double CalculateProbability(Question question, Answer answer)
        {
            switch (answer)
            {
                case Answer.Yes:
                    P = GetPYes(question);
                    break;
                case Answer.No:
                    P = GetPNo(question);
                    break;
                case Answer.YesLikely:
                    P = GetPYesLikely(question);
                    break;
                case Answer.NoLikely:
                    P = GetPNoLikely(question);
                    break;
                case Answer.DontKnow:
                    break;
            }

            return P;
        }
    }
}
