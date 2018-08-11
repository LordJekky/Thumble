using UnityEngine;
using UnityEngine.UI;

public class PowerUpBarManager : MonoBehaviour {

    public GameObject leftButton, rightButton;
    public Color selectedButtonColor;
    [Space]
    public GameObject powerUpBar;

    public void SelectRightPosition()
    {
        leftButton.GetComponent<Image>().color = new Vector4(1, 1, 1, 1);
        rightButton.GetComponent<Image>().color = selectedButtonColor;

        float x = Screen.width - 90;
        powerUpBar.transform.position = new Vector2(x, powerUpBar.transform.position.y);

        PlayerPrefs.SetString("PowerUpBarPosition", "right");
    }

    public void SelectLeftPosition()
    {
        rightButton.GetComponent<Image>().color = new Vector4(1, 1, 1, 1);
        leftButton.GetComponent<Image>().color = selectedButtonColor;

        float x = 90;
        powerUpBar.transform.position = new Vector2(x, powerUpBar.transform.position.y);

        PlayerPrefs.SetString("PowerUpBarPosition", "left");
    }

    public void restorePowerUpBarPosition(string position = "left")
    {
        if (position.Equals("right"))
        {
            SelectRightPosition();
        }
        else
        if (position.Equals("left"))
        {
            SelectLeftPosition();
        }
    }
}
