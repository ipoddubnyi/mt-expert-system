namespace MT.ExpertSystem.Core;

internal static class BayesMath
{
    public static double ProbabilityYes(double P, double Pyes, double Pno)
        => Pyes * P / (Pyes * P + Pno * (1 - P));

    public static double ProbabilityNo(double P, double Pyes, double Pno)
        => Pno * P / (Pno * P + Pyes * (1 - P));

    public static double ProbabilityYesLikely(double P, double Pyes, double Pno)
    {
        var yes = (int)Answer.Yes;
        var yesLikely = (int)Answer.YesLikely;
        var dontKnow = (int)Answer.DontKnow;

        var P0 = P;
        var P2 = ProbabilityYes(P, Pyes, Pno);

        return (P2 - P0) / (yes - dontKnow) * yesLikely + P0;
    }

    public static double ProbabilityNoLikely(double P, double Pyes, double Pno)
    {
        var dontKnow = (int)Answer.DontKnow;
        var noLikely = (int)Answer.NoLikely;
        var no = (int)Answer.No;

        var P0 = P;
        var P2 = ProbabilityNo(P, Pyes, Pno);

        return (P0 - P2) / (dontKnow - no) * noLikely + P0;
    }
}
