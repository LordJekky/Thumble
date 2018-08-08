using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventaryController : MonoBehaviour
{

    public GameObject itemList;
    public GameObject firstItemHolder, secondItemHolder, thirdItemHolder;
    GameObject itemHolderSelected; //the itemHolder i've actually selected
    bool _itemListActived = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenDisposeItemList(GameObject itemHolder)
    {
        if (_itemListActived)//if item list is active
        {
            // se ci ho già tappato sopra, deve chiudersi, se sto tappando su un'altro itemHolder devo invece switchare il
            if (itemHolder.name.Equals(itemHolderSelected.name))//if the itemHolder have the same name of the itemHolder that was selected
            {
                _itemListActived = false;
                itemList.SetActive(false);// i have to close the item list

                itemHolderSelected = null; //there is not an itemholder selected anymore
            }
            else //if i'm selecting another itemHolder
            {
                itemHolderSelected = itemHolder; //i just switch the itemHolder selected
            }
        }
        else
        {
            _itemListActived = true;
            itemHolderSelected = itemHolder; //i've selected a new item holder
            itemList.SetActive(true);
        }

    }

    public void DisposeItemList()
    {
        _itemListActived = false;
        itemList.SetActive(false);// i have to close the item list

        itemHolderSelected = null; //there is not an itemholder selected anymore
    }

    public void EquipSelectedItemHolder(Item item)
    {
        if (itemHolderSelected == null)
        {
            DisposeItemList();
            return;
        }


        itemHolderSelected.GetComponent<ItemHolder>().SetItem(item);
    }


}
