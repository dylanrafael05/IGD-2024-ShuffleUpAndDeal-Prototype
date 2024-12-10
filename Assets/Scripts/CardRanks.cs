public static class CardRanks
{
    public const int Ace = 1;
    public const int Jack = 11;
    public const int Queen = 12;
    public const int King = 13;

    public static string RankToString(int rank)
        => rank switch
        {
            Ace => "A",
            Jack => "J",
            Queen => "Q",
            King => "K",
            var x => x.ToString()
        };

    public static bool IsSpecial(int rank)
        => rank is Ace or Jack or Queen or King;
}
