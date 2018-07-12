using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManagerScript : MonoBehaviour
{

    [SerializeField]
    private Transform[] shopBtns;

    public Text CoinsTxt, SelectedItemTxt;
    public Color SelectedColor, NonSelectedColor;

    private int prevSelectedIndex;

    private void Start()
    {
        for (int i = 0; i < shopBtns.Length; i++)
        {
            shopBtns[i].GetChild(0).GetComponent<Text>().text = CoinsCurrency.Items[i].Name;
        }

        UpdateUI();
    }

    public void BuyOrSelectItem(int index)
    {
        if (CoinsCurrency.Items[index].Bought)
            CoinsCurrency.Instance.SelectItem(index);
        else
            CoinsCurrency.Instance.BuyItem(index);

        UpdateUI();
    }

    private void UpdateUI()
    {
        CoinsTxt.text = "Coins: " + CoinsCurrency.Coins.ToString();
        SelectedItemTxt.text = CoinsCurrency.Items[CoinsCurrency.SelectedItemIndex].Name;

        shopBtns[prevSelectedIndex].GetComponent<Image>().color = NonSelectedColor;
        shopBtns[CoinsCurrency.SelectedItemIndex].GetComponent<Image>().color = SelectedColor;

        prevSelectedIndex = CoinsCurrency.SelectedItemIndex;
    }
}