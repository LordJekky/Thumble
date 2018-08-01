using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemListManager : MonoBehaviour {

    public GameObject EquipPanel, content;
    public GameObject itemPortraitPrefab;
    [Space]
    public List<Item> items;

    GameObject[] itemsArray;

	// Use this for initialization
	void Start () {
        //fill the gameobject array
        itemsArray = new GameObject[items.Count];

        for (int c = 0; c < itemsArray.Length; c++)
        {
            itemsArray[c] = Instantiate(itemPortraitPrefab, content.transform);

            itemsArray[c].name = items[c].itemName;
            itemsArray[c].GetComponent<Image>().sprite = items[c].itemImage;

            EventTrigger trigger = itemsArray[c].GetComponent<EventTrigger>();

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;

            Item actualItem = items[c];
            entry.callback.AddListener((eventData) => { EquipPanel.GetComponent<InventaryController>().EquipSelectedItemHolder(actualItem); });
            entry.callback.AddListener((eventData) => { EquipPanel.GetComponent<InventaryController>().DisposeItemList(); });

            trigger.triggers.Add(entry);
        }
    }//GameObject.Find("EquipPanel").GetComponent<InventaryController>().EquipItemHolder(items[c]); 
	
	// Update is called once per frame
	void Update () {
		
	}
}
