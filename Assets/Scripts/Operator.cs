using System.Collections;

public abstract class Operator
{
    public abstract Rarity Rarity { get; }
    public abstract int Score(int current, Card card);
    public abstract string Symbol { get; }
    public abstract string Description { get; }

    public virtual bool Default => false;
    public virtual IEnumerator OnEffectCard(Card card)
    {
        yield break;
    }
    public virtual void Reset()
    {

    }
}
