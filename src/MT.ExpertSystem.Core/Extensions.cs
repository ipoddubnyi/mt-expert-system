using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.ExpertSystem.Core
{
    public static class Extensions
    {
        public static Question GetById(this IEnumerable<Question> questions, int id)
        {
            foreach (var question in questions)
            {
                if (question.Id == id)
                    return question;
            }

            return null;
        }

        public static Question GetFirstNotAnswered(this IEnumerable<Question> questions)
        {
            foreach (var question in questions)
            {
                if (!question.IsAnswer)
                    return question;
            }

            return null;
        }

        public static IEnumerable<Question> SortByCost(this IEnumerable<Question> questions)
        {
            var sorted = questions.OrderByDescending(q => q.Cost).OrderBy(q => q.IsAnswer);
            questions = sorted.ToArray();
            return questions;
        }

        public static QuestionYesNo GetById(this IEnumerable<QuestionYesNo> questions, int id)
        {
            foreach (var question in questions)
            {
                if (question.Id == id)
                    return question;
            }

            return null;
        }

        public static void Reset(this IEnumerable<Alternative> alternatives)
        {
            foreach (var alternative in alternatives)
                alternative.Reset();
        }

        public static IEnumerable<Alternative> SortByProbability(this IEnumerable<Alternative> alternatives)
        {
            var sorted = alternatives.OrderByDescending(a => a.P);
            alternatives = sorted.ToArray();
            return alternatives;
        }
    }
}
