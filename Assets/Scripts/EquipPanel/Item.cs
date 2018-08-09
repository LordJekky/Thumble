using UnityEngine;

[CreateAssetMenu(fileName ="New Item", menuName ="Item")]
public class Item: ScriptableObject {

    public string itemName;
    public Sprite itemImage;
    public PowerUpType powerUpType;
}

public enum PowerUpType
{
    Null,
    Heart,
    Gold,
    Emerald
}
