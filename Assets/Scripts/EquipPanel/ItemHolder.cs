using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemHolder : MonoBehaviour {

    public Item item;

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

    public void EquipItem(Item item)
    {
        this.item = item;
    }
}
