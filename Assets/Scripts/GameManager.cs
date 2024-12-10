using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public int round;
    [HideInInspector] public int goal;
    [HideInInspector] public int score;
    [HideInInspector] public int money = 5;
    [HideInInspector] public bool inShop = false;
    [HideInInspector] public List<Card> allCards;
    [HideInInspector] public List<Card> hand;
    [HideInInspector] public Card current;
    [HideInInspector] public List<Operator> allOperators;
    [HideInInspector] public List<Operator> remainingCommon;
    [HideInInspector] public List<Operator> remainingUncommon;
    [HideInInspector] public List<Operator> remainingRare;
    [HideInInspector] public List<Operator> operators;
    [HideInInspector] public int selectedOperator;

    public OperatorVisual firstVisual;

    public TextMeshPro roundText;
    public TextMeshPro moneyText;
    public TextMeshPro scoreText;
    public TextMeshPro cardsText;

    public int handSize = 20;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public Card prefab;

    public void CopyCurrent()
    {
        var card = GameObject.Instantiate(prefab);
        card.transform.position = Vector3.zero;
        card.suit = current.suit;
        card.rank = current.rank;
        card.gameObject.SetActive(false);

        hand.Add(card);
    }
    public void DeleteCurrent()
    {
        allCards.Remove(current);
        GameObject.Destroy(current.gameObject);
    }

    public void Start()
    {
        // Spawn operator visuals //
        for(int i = 0; i < 10; i++)
        {
            var newVisual = GameObject.Instantiate(firstVisual);
            newVisual.transform.SetParent(firstVisual.transform.parent);
            newVisual.transform.position = firstVisual.transform.position;
            newVisual.transform.position += Vector3.down * (i + 1) * 1f;
            newVisual.operatorIndex += i + 1;
        }

        // Spawn cards //
        for(int rank = CardRanks.Ace; rank <= CardRanks.King; rank++)
        {
            for(int suit_int = 0; suit_int < 4; suit_int++)
            {
                var suit = (CardSuit)suit_int;

                var card = GameObject.Instantiate(prefab);
                card.transform.position = Vector3.zero;
                card.suit = suit;
                card.rank = rank;
                card.gameObject.SetActive(false);

                allCards.Add(card);
            }
        }

        // Find and spawn operators //
        allOperators = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsSubclassOf(typeof(Operator)) && !t.IsAbstract)
            .Select(t => Activator.CreateInstance(t))
            .Cast<Operator>()
            .ToList();

        operators = allOperators.Where(o => o.Default)
            .ToList();

        remainingCommon = allOperators.Where(o => o.Rarity is Rarity.Common && !o.Default).ToList();
        remainingUncommon = allOperators.Where(o => o.Rarity is Rarity.Uncommon && !o.Default).ToList();
        remainingRare = allOperators.Where(o => o.Rarity is Rarity.Rare && !o.Default).ToList();

        // Begin game //
        StartCoroutine(Game());
    }

    public void DealHand()
    {
        hand.Clear();

        var deck = allCards.ToList();

        while(deck.Count > 0 && hand.Count < handSize)
        {
            var index = UnityEngine.Random.Range(0, deck.Count);
            var card = deck[index];

            hand.Add(card);
            card.gameObject.SetActive(false);

            deck.RemoveAt(index);
        }
    }

    public IEnumerator PlayCard()
    {
        score = operators[selectedOperator].Score(score, current);
        yield return operators[selectedOperator].OnEffectCard(current);
        current.gameObject.SetActive(false);
    }

    public void Loss()
    {
        Debug.Log("YOU LOST! restart game to replay.");
    }

    public IEnumerator Round()
    {
        DealHand();

        while (hand.Count > 0)
        {
            current = hand[0];
            hand.RemoveAt(0);

            current.gameObject.SetActive(true);

            while(true)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                    break;

                if (Input.GetKeyDown(KeyCode.Alpha1))
                    selectedOperator = 0;
                if (Input.GetKeyDown(KeyCode.Alpha2))
                    selectedOperator = 1;
                if (Input.GetKeyDown(KeyCode.Alpha3))
                    selectedOperator = 2;
                if (Input.GetKeyDown(KeyCode.Alpha4))
                    selectedOperator = 3;
                if (Input.GetKeyDown(KeyCode.Alpha5))
                    selectedOperator = 4;

                yield return null;
            }

            yield return PlayCard();

            yield return null;

            if (Mathf.Abs(score - goal) <= 1)
            {
                yield break;
            }
        }

        // Lose game //
        Loss();
    }

    [Header("Costs")]
    public int commonCost = 4;
    public int uncommonCost = 7;
    public int rareCost = 10;
    public float sellBack = 0.5f;

    public int ValueFromRarity(Rarity rarity)
        => rarity switch
        {
            Rarity.Common => commonCost,
            Rarity.Uncommon => uncommonCost,
            Rarity.Rare => rareCost,

            _ => 0
        };

    public void SellOperator(int opIndex)
    {
        var op = operators[opIndex];
        operators.RemoveAt(opIndex);

        money += Mathf.CeilToInt(ValueFromRarity(op.Rarity) * sellBack);

        ReturnOperator(op);
    }

    public List<Operator> StorageFor(Rarity rarity)
        => rarity switch 
        {
            Rarity.Common => remainingCommon,
            Rarity.Uncommon => remainingUncommon,
            Rarity.Rare => remainingRare,
            _ => null
        };

    public void ReturnOperator(Operator op)
    {
        StorageFor(op.Rarity).Add(op);
        op.Reset();
    }
    public Operator TakeRandom(Rarity rarity)
    {
        var list = StorageFor(rarity);
        if (list.Count == 0) return null;

        var index = UnityEngine.Random.Range(0, list.Count);
        var result = list[index];
        list.RemoveAt(index);
        return result;
    }

    [Header("Chances")]
    public float chanceCommon;
    public float chanceUncommon;

    public Operator ShopChoice()
    {
        float chance = UnityEngine.Random.value;

        Rarity rarity;

        if (chance < chanceCommon)
        {
            rarity = Rarity.Common;
        }
        else if (chance - chanceCommon < chanceUncommon)
        {
            rarity = Rarity.Uncommon;
        }
        else rarity = Rarity.Rare;

        return TakeRandom(rarity);
    }

    [Header("Shop")]
    public OperatorVisual shopLeft;
    public OperatorVisual shopRight;
    public int maxOperators;
    

    public IEnumerator Shop()
    {
        shopLeft.gameObject.SetActive(true);
        shopRight.gameObject.SetActive(true);

        var left = ShopChoice();
        var right = ShopChoice();

        var leftCost = ValueFromRarity(left.Rarity);
        var rightCost = ValueFromRarity(right.Rarity);

        shopLeft.isShop = true;
        shopLeft.shopOverride = left;

        shopRight.isShop = true;
        shopRight.shopOverride = right;

        Operator choice = null;

        while(true)
        {
            if (operators.Count < maxOperators)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1) && money >= leftCost)
                {
                    choice = left;
                    money -= leftCost;
                    break;
                }

                if (Input.GetKeyDown(KeyCode.Alpha2) && money >= rightCost)
                {
                    choice = right;
                    money -= rightCost;
                    break;
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
                break;

            yield return null;
        }

        if (left != choice) ReturnOperator(left);
        if (right != choice) ReturnOperator(right);

        if (choice != null)
            operators.Add(choice);

        shopLeft.gameObject.SetActive(false);
        shopRight.gameObject.SetActive(false);

        yield return null;
    }

    public int startingMoney = 5;

    public IEnumerator Game()
    {
        money = startingMoney;
        round = 0;

        while (true)
        {
            round++;

            score = 0;
            goal = (int)(21 * (1 + round / 2f) + 10 * Mathf.Pow((round - 1) / 4f, 2));

            yield return Round();
            money += (hand.Count + 3) / 4;

            inShop = true;
            yield return Shop();
            inShop = false;
        }
    }

    private void Update()
    {
        roundText.text = $"Round {round}";
        scoreText.text = $"{score}/{goal}";
        cardsText.text = $"{hand.Count} cards left";
        moneyText.text = $"${money}";

        if (hand.Count > 0)
        {
            cardsText.text += $"; next is {CardRanks.RankToString(hand[0].rank)} of {hand[0].suit}";
        }
    }
}
