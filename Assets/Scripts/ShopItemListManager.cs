using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopItemListManager : MonoBehaviour {

    public GameObject content;
    public GameObject shopItemPortraitPrefab;
    [Space]
    public List<Item> shopItems, shopEmeralds, shopCoins;

    GameObject[] itemsArray;

    // Use this for initialization
    void Start()
    {
        //fill the gameobject array
        itemsArray = new GameObject[shopItems.Count];

        for (int c = 0; c < itemsArray.Length; c++)
        {
            itemsArray[c] = Instantiate(shopItemPortraitPrefab, content.transform);

            itemsArray[c].name = shopItems[c].itemName;
            itemsArray[c].GetComponent<Image>().sprite = shopItems[c].itemImage;

            itemsArray[c].GetComponent<Button>().onClick.AddListener(test);
        }
    }

    public void test()
    {
        Debug.Log("I am the best programmer in the world");
    }

    public void test(Item item)
    {
        Debug.Log("I am the best programmer in the world");
    }

    public void fillItemShop()
    {
        int x = 0;
        do
        {
            Destroy(itemsArray[x]);
            x++;
        } while (x < itemsArray.Length);
        

        //fill the gameobject array
        itemsArray = new GameObject[shopItems.Count];

        for (int c = 0; c < itemsArray.Length; c++)
        {
            itemsArray[c] = Instantiate(shopItemPortraitPrefab, content.transform);

            itemsArray[c].name = shopItems[c].itemName;
            itemsArray[c].GetComponent<Image>().sprite = shopItems[c].itemImage;

            itemsArray[c].GetComponent<Button>().onClick.AddListener(test);
        }
    }

    public void fillEmeraldShop()
    {
        int x = 0;
        do
        {
            Destroy(itemsArray[x]);
            x++;
        } while (x < itemsArray.Length);

        //fill the gameobject array
        itemsArray = new GameObject[shopEmeralds.Count];

        for (int c = 0; c < itemsArray.Length; c++)
        {
            itemsArray[c] = Instantiate(shopItemPortraitPrefab, content.transform);

            itemsArray[c].name = shopEmeralds[c].itemName;
            itemsArray[c].GetComponent<Image>().sprite = shopEmeralds[c].itemImage;

            itemsArray[c].GetComponent<Button>().onClick.AddListener(test);
        }
    }

    public void fillCoinShop()
    {
        int x = 0;
        do
        {
            Destroy(itemsArray[x]);
            x++;
        } while (x < itemsArray.Length);

        //fill the gameobject array
        itemsArray = new GameObject[shopCoins.Count];

        for (int c = 0; c < itemsArray.Length; c++)
        {
            itemsArray[c] = Instantiate(shopItemPortraitPrefab, content.transform);

            itemsArray[c].name = shopCoins[c].itemName;
            itemsArray[c].GetComponent<Image>().sprite = shopCoins[c].itemImage;

            itemsArray[c].GetComponent<Button>().onClick.AddListener(test);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
