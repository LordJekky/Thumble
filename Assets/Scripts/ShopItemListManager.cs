using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopItemListManager : MonoBehaviour {

    public GameObject content;
    public GameObject shopItemPortraitPrefab;
    [Space]
    public List<Item> items;

    GameObject[] itemsArray;

    // Use this for initialization
    void Start()
    {
        //fill the gameobject array
        itemsArray = new GameObject[items.Count];

        for (int c = 0; c < itemsArray.Length; c++)
        {
            itemsArray[c] = Instantiate(shopItemPortraitPrefab, content.transform);

            itemsArray[c].name = items[c].itemName;
            itemsArray[c].GetComponent<Image>().sprite = items[c].itemImage;

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

    // Update is called once per frame
    void Update () {
		
	}
}
