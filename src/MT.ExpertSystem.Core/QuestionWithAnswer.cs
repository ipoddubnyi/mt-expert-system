using System.Diagnostics;

namespace MT.ExpertSystem.Core;

/// <summary>
/// Вопрос (свидетельство) с ответом.
/// </summary>
[DebuggerDisplay("{Id}: {Text} ({Cost})")]
public class QuestionWithAnswer : Question
{
    /// <summary>
    /// Текущая цена свидетельства (чтобы задавать вопросы в актуальном порядке).
    /// </summary>
    public double Cost { get; set; }

    /// <summary>
    /// Ответ на свидетельство.
    /// </summary>
    public Answer? Answer { get; private set; }

    /// <summary>
    /// Номер свидетельства по порядку ответа.
    /// </summary>
    public int? Number { get; private set; }

    public QuestionWithAnswer(Question question)
    {
        Id = question.Id;
        Text = question.Text;

        Cost = 0.0;
        Answer = null;
        Number = null;
    }

    public void SetAnswer(int number, Answer answer)
    {
        Number = number;
        Answer = answer;
    }

    public override string ToString()
        => $"{Text} ({Cost:0.00})";
}
