using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour {

    public void ActivatePowerUp(PowerUpHolder powerUp)
    {
        switch (powerUp.GetPowerUpType())
        {
            case PowerUpType.Heart:
                {
                    break;
                }
            case PowerUpType.Gold:
                {
                    break;
                }
            case PowerUpType.Emerald:
                {
                    break;
                }


            default:
                {
                    break;
                }
        }
    }

    
}
