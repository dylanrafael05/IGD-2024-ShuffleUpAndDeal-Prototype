using System.Collections;
using System.IO;
using UnityEngine;

public class Add : Operator
{
    public override Rarity Rarity => Rarity.Common;

    public override string Symbol => "+";

    public override string Description => "Add the played card's value to your score";

    public override int Score(int current, Card card)
        => current + card.rank;

    public override bool Default => true;
}

public class Duplicate : Operator
{
    public override Rarity Rarity => Rarity.Uncommon;

    public override string Symbol => "@";

    public override string Description => "Make a copy of the played card";

    public override int Score(int current, Card card)
        => current;

    public override IEnumerator OnEffectCard(Card card)
    {
        GameManager.Instance.CopyCurrent();
        yield break;
    }
}

public class Destroy : Operator
{
    public override Rarity Rarity => Rarity.Uncommon;

    public override string Symbol => "x";

    public override string Description => "Destroy the played card";

    public override int Score(int current, Card card)
        => current;

    public override IEnumerator OnEffectCard(Card card)
    {
        GameManager.Instance.DeleteCurrent();
        yield break;
    }
}

public class ShuffleSuit : Operator
{
    public override Rarity Rarity => Rarity.Uncommon;

    public override string Symbol => "♡";

    public override string Description => "Swap the suit of the played card with the next in order";

    public override int Score(int current, Card card)
        => current;

    public override IEnumerator OnEffectCard(Card card)
    {
        var suitint = (int)card.suit;
        card.suit = (CardSuit)((suitint + 1) % 4);
        yield return new WaitForSeconds(1f);
    }
}

public class RaiseRank : Operator
{
    public override Rarity Rarity => Rarity.Uncommon;

    public override string Symbol => "^#";

    public override string Description => "Increase the rank of the played card by two";

    public override int Score(int current, Card card)
        => current;

    public override IEnumerator OnEffectCard(Card card)
    {
        card.rank += 2;
        yield return new WaitForSeconds(1f);
    }
}

public class DoubleRank : Operator
{
    public override Rarity Rarity => Rarity.Rare;

    public override string Symbol => "*#";

    public override string Description => "Double the rank of the played card";

    public override int Score(int current, Card card)
        => current;

    public override IEnumerator OnEffectCard(Card card)
    {
        card.rank *= 2;
        yield return new WaitForSeconds(1f);
    }
}

public class Multiply : Operator
{
    public override Rarity Rarity => Rarity.Common;

    public override string Symbol => "*";

    public override string Description => "Multiply the played card's value with your score";

    public override int Score(int current, Card card)
        => current * card.rank;
}


public class AddSpade : Operator
{
    public override Rarity Rarity => Rarity.Common;

    public override string Symbol => "+♠";

    public override string Description => "Add twice the score of the current card to your score if it is a spade";

    public override int Score(int current, Card card)
        => card.suit is CardSuit.Spade ? current + 2*card.rank : current;
}

public class AddClub : Operator
{
    public override Rarity Rarity => Rarity.Common;

    public override string Symbol => "+♣";

    public override string Description => "Add twice the score of the current card to your score if it is a club";

    public override int Score(int current, Card card)
        => card.suit is CardSuit.Club ? current + 2 * card.rank : current;
}

public class AddHeart : Operator
{
    public override Rarity Rarity => Rarity.Common;

    public override string Symbol => "+♡";

    public override string Description => "Add twice the score of the current card to your score if it is a heart";

    public override int Score(int current, Card card)
        => card.suit is CardSuit.Heart ? current + 2 * card.rank : current;
}

public class AddDiamond : Operator
{
    public override Rarity Rarity => Rarity.Common;

    public override string Symbol => "+◆";

    public override string Description => "Add twice the score of the current card to your score if it is a heart";

    public override int Score(int current, Card card)
        => card.suit is CardSuit.Diamond ? current + 2 * card.rank : current;
}

public class AddSquared : Operator
{
    public override Rarity Rarity => Rarity.Uncommon;

    public override string Symbol => "+²";

    public override string Description => "Add the square of the current card's rank to your score";

    public override int Score(int current, Card card)
        => current + card.rank * card.rank;
}

public class SubSquared : Operator
{
    public override Rarity Rarity => Rarity.Uncommon;

    public override string Symbol => "-²";

    public override string Description => "Subtract the square of the current card's rank to your score";

    public override int Score(int current, Card card)
        => current + card.rank * card.rank;
}


public class Snowball : Operator
{
    int streak = 1;
    CardSuit suit;

    public override Rarity Rarity => Rarity.Uncommon;

    public override string Symbol => "+++";

    public override string Description => "Multiply the current score by the number of consecutive times the card with the given suit has been played with this operator active.";

    public override int Score(int current, Card card)
        => current * streak;

    public override IEnumerator OnEffectCard(Card card)
    {
        streak++;
        if (card.suit != suit)
        {
            streak = 1;
            suit = card.suit;
        }
        yield break;
    }

    public override void Reset()
    {
        streak = 1;
    }
}


public class EvenMadness : Operator
{
    public override Rarity Rarity => Rarity.Uncommon;

    public override string Symbol => "/2/";

    public override string Description => "Gain $2 if played card is even.";

    public override int Score(int current, Card card)
        => current;

    public override IEnumerator OnEffectCard(Card card)
    {
        if (card.rank % 2 == 0 && !CardRanks.IsSpecial(card.rank))
        {
            GameManager.Instance.money += 2;
        }
        yield break;
    }
}