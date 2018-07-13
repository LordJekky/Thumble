using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopPanel : MonoBehaviour {

    public GameObject shopPanel;
    public GameObject itemShopPanel;
    public GameObject diamondsPanel;
    public GameObject coinsPanel;

    private Animator shopAnim;

    public void Diamonds_PurchasesOpen()
    {
        diamondsPanel.SetActive(true);
        coinsPanel.SetActive(false);
        itemShopPanel.SetActive(false);
    }

    public void Diamonds_PurchasesClose()
    {
        diamondsPanel.SetActive(false);
        coinsPanel.SetActive(false);
        itemShopPanel.SetActive(true);
    }

    public void Coins_PurchasesOpen()
    {
        coinsPanel.SetActive(true);
        diamondsPanel.SetActive(false);
        itemShopPanel.SetActive(false);
    }

    public void Coins_PurchasesClose()
    {
        coinsPanel.SetActive(false);
        diamondsPanel.SetActive(false);
        itemShopPanel.SetActive(true);
    }
    
    public void OpenShop()
    {
        shopPanel.SetActive(true);
        itemShopPanel.SetActive(true);
        coinsPanel.SetActive(false);
        diamondsPanel.SetActive(false);
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        itemShopPanel.SetActive(false);
        diamondsPanel.SetActive(false);
        coinsPanel.SetActive(false);
    }
}
