using System.Diagnostics;

namespace MT.ExpertSystem.Core;

/// <summary>
/// Альтернатива (гипотеза) с текущей вероятностью.
/// </summary>
[DebuggerDisplay("{Id}: {Name} ({P})")]
public class AlternativeWithProbability : Alternative
{
    /// <summary>
    /// Текущая вероятность выбора альтернативы.
    /// </summary>
    public double P { get; set; }

    public AlternativeWithProbability(Alternative alternative)
    {
        Id = alternative.Id;
        Name = alternative.Name;
        Priori = alternative.Priori;
        P = Priori;
    }

    public override string ToString()
        => $"{Name} ({P:0.0%})";
}
