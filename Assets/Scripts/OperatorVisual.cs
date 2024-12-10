using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class OperatorVisual : MonoBehaviour
{
    public GameObject visualContainer;
    public TextMeshPro symbol;
    public TextMeshPro description;
    public TextMeshPro score;
    public GameObject tickContainer;

    [HideInInspector] public int operatorIndex;
    [HideInInspector] public bool isShop = false;
    [HideInInspector] public Operator shopOverride = null;

    public Button useButton;
    public Button sellButton;

    private void Awake()
    {
        useButton.onClick.AddListener(() => GameManager.Instance.selectedOperator = operatorIndex);
        sellButton.onClick.AddListener(() => GameManager.Instance.SellOperator(operatorIndex));
    }

    private void Update()
    {
        if (!isShop)
        {
            var outOfBounds = GameManager.Instance.operators.Count <= operatorIndex;
            visualContainer.SetActive(!outOfBounds);

            if (outOfBounds)
                return;
        }

        var op = isShop ? shopOverride : GameManager.Instance.operators[operatorIndex];

        symbol.text = op.Symbol;
        description.text = op.Description;

        if (isShop)
        {
            score.text = $"${GameManager.Instance.ValueFromRarity(op.Rarity)}";
            tickContainer.SetActive(false);
            sellButton.gameObject.SetActive(false);
        }
        else
        {
            score.text = $"= {op.Score(GameManager.Instance.score, GameManager.Instance.current)}";
            tickContainer.SetActive(GameManager.Instance.selectedOperator == operatorIndex);
            sellButton.gameObject.SetActive(true);
        }
    }
}
