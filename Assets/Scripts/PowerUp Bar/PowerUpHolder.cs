using UnityEngine;

public class PowerUpHolder : MonoBehaviour {

    public ItemHolder itemHolder;
    Item item;
    PowerUpType powerUpType;

	// Update is called once per frame
	void Update () {
        item = itemHolder.GetItem();
        this.gameObject.GetComponent<UnityEngine.UI.Image>().sprite = item.itemImage;
        powerUpType = item.powerUpType;
    }

    public PowerUpType GetPowerUpType() { return powerUpType; }
}
