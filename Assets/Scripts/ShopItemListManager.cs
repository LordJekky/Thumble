using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemListManager : MonoBehaviour {

    public GameObject content;
    public GameObject shopItemPortraitPrefab;
    [Space]
    public List<Item> shopItems, shopEmeralds, shopCoins;

    GameObject[] itemsArray;

    GameObject buttonSelected = null;
    Vector2 buttonSelectedOriginalPosition;
    /*
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
    }*/

    public void test()
    {
        Debug.Log("I am the best programmer in the world");
    }

    public void fillItemShop()
    {
        if(itemsArray != null) {
            int x = 0;
            do
            {
                Destroy(itemsArray[x]);
                x++;
            } while (x < itemsArray.Length);
        }

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
        if (itemsArray != null)
        {
            int x = 0;
            do
            {
                Destroy(itemsArray[x]);
                x++;
            } while (x < itemsArray.Length);
        }

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
        if (itemsArray != null)
        {
            int x = 0;
            do
            {
                Destroy(itemsArray[x]);
                x++;
            } while (x < itemsArray.Length);
        }

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


    public void SelectButton(GameObject buttonSelected)
    {
        if(this.buttonSelected != null)//restore previous selected button in his original position
        {
            this.buttonSelected.transform.position = buttonSelectedOriginalPosition;
        }

        this.buttonSelected = buttonSelected;
        buttonSelectedOriginalPosition = buttonSelected.transform.position;

        buttonSelected.transform.position = buttonSelected.transform.position + new Vector3(0, 40f);
    }
    
}
