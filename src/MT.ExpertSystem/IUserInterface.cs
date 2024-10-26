using MT.ExpertSystem.Core;

namespace MT.ExpertSystem;

/// <summary>
/// Пользовательский интерфейс.
/// </summary>
interface IUserInterface
{
    /// <summary>
    /// Задать вопрос пользователю.
    /// </summary>
    Answer AskQuestion(Expert expert, QuestionWithAnswer question);

    /// <summary>
    /// Вывести результат пользователю.
    /// </summary>
    void PrintResult(Expert expert, AlternativeWithProbability alternative);
}
