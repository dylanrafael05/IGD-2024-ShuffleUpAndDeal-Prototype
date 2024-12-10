public class Subtract : Operator
{
    public override Rarity Rarity => Rarity.Common;

    public override string Symbol => "-";

    public override string Description => "Subtract the played card's value from your score";

    public override int Score(int current, Card card)
        => current - card.rank;

    public override bool Default => true;
}
