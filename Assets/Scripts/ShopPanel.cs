using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopPanel : MonoBehaviour {

    public GameObject shopPanel;
    public GameObject itemShopPanel;
    public GameObject diamondsPanel;
    public GameObject coinsPanel;

    public Animator itemAnim;
    public Animator diamondsAnim;
    public Animator coinsAnim;

    public void Awake()
    {
        shopPanel.SetActive(true);
        itemAnim = itemShopPanel.GetComponent<Animator>();
        diamondsAnim = diamondsPanel.GetComponent<Animator>();
        coinsAnim = coinsPanel.GetComponent<Animator>();
    }

    public void Diamonds_PurchasesOpen()
    {
        diamondsAnim.SetBool("Open", true);
        diamondsAnim.SetBool("Close", false);
        shopPanel.SetActive(true);
        diamondsPanel.SetActive(true);
        coinsPanel.SetActive(false);
        itemShopPanel.SetActive(false);
    }

    public void Diamonds_PurchasesClose()
    {
        diamondsAnim.SetBool("Close", true);
        diamondsAnim.SetBool("Open", false);
        diamondsPanel.SetActive(false);
        coinsPanel.SetActive(false);
        itemShopPanel.SetActive(true);
    }

    public void Coins_PurchasesOpen()
    {
        coinsAnim.SetBool("Open", true);
        coinsAnim.SetBool("Close", false);
        shopPanel.SetActive(true);
        coinsPanel.SetActive(true);
        diamondsPanel.SetActive(false);
        itemShopPanel.SetActive(false);
    }

    public void Coins_PurchasesClose()
    {
        coinsAnim.SetBool("Close", true);
        coinsAnim.SetBool("Open", false);
        coinsPanel.SetActive(false);
        diamondsPanel.SetActive(false);
        itemShopPanel.SetActive(true);
    }
    
    public void OpenShop()
    {
        itemAnim.SetBool("Open", true);
        itemAnim.SetBool("Close", false);
        shopPanel.SetActive(true);
        itemShopPanel.SetActive(true);
        coinsPanel.SetActive(false);
        diamondsPanel.SetActive(false);
    }

    public void CloseShop()
    {
        itemAnim.SetBool("Close", true);
        itemAnim.SetBool("Open", false);
        shopPanel.SetActive(false);
        itemShopPanel.SetActive(false);
        diamondsPanel.SetActive(false);
        coinsPanel.SetActive(false);
    }
}
