using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemHolder : MonoBehaviour {

    Item item;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(item == null)
        {
            this.gameObject.GetComponent<Image>().sprite = null;
        }
        else
        {
            this.gameObject.GetComponent<Image>().sprite = item.itemImage;
        }
	}

    public void SetItem(Item item) { this.item = item; }

    public Item GetItem() { return item; }
}
