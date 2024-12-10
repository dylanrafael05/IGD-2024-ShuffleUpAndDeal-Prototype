using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Card : MonoBehaviour
{
    SpriteRenderer[] suitRenderers;
    TextMeshPro rankText;

    [Header("Settings")]
    public Sprite heart;
    public Sprite diamond;
    public Sprite spade;
    public Sprite club;

    [Header("Values")]
    public int rank;
    public CardSuit suit;

    void Start()
    {
        suitRenderers = GetComponentsInChildren<SpriteRenderer>()
            .Where(i => i.transform != transform)
            .ToArray();
        rankText = GetComponentInChildren<TextMeshPro>();
    }

    Sprite SpriteFromSuit(CardSuit suit)
        => suit switch
        {
            CardSuit.Club => club,
            CardSuit.Heart => heart,
            CardSuit.Diamond => diamond,
            CardSuit.Spade => spade,

            _ => null
        };

    void Update()
    {
        rankText.text = CardRanks.RankToString(rank);
        foreach(var sprite in suitRenderers)
            sprite.sprite = SpriteFromSuit(suit);
    }
}
