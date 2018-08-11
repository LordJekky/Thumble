using UnityEngine;

public class PlayerPreferencesManager : MonoBehaviour {

    public PowerUpBarManager powerUpBarManager;

	// Use this for initialization
	void Start () {
        powerUpBarManager.restorePowerUpBarPosition(PlayerPrefs.GetString("PowerUpBarPosition", "left"));
	}

    public void ResetPlayerPreferences()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Player preferences resetted to default");
    }
}


