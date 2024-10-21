﻿using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MT.ExpertSystem.Core;

/// <summary>
/// Вопрос (свидетельство).
/// </summary>
[DebuggerDisplay("{Text} ({Cost})")]
public class Question
{
    [XmlAttribute("id")]
    public string Id { get; set; }

    [XmlText]
    public string Text { get; set; }

    [XmlIgnore]
    [JsonIgnore]
    public double Cost { get; set; }

    [XmlIgnore]
    [JsonIgnore]
    public Answer? Answer { get; set; }

    /// <summary>
    /// Номер по порядку.
    /// </summary>
    [XmlIgnore]
    [JsonIgnore]
    public int? Number { get; set; }

    [XmlIgnore]
    [JsonIgnore]
    public bool HasAnswer => Answer.HasValue;

    public Question()
    {
        Id = string.Empty;
        Text = string.Empty;
        Cost = 0.0;
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
