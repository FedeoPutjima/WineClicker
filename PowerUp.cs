using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName="New Power Up", menuName="Idle Game/Power Ups")]
public class PowerUp : ScriptableObject
{
    public string powerUpName;
    public float basePrice;
    public float priceMultiplier = 1.1f;
    public float baseIncome;

    public Sprite powerUpImage;
    public Sprite unknownPowerUpImage;


    //formula for cost 
    //Price = basePrice * priceMultiplier(#N)
    public float CalculateCost(int amountOfPowerUp)
    {
        float newPrice = basePrice * Mathf.Pow(priceMultiplier, amountOfPowerUp);
        float rounded = (float)Mathf.Round(newPrice*100)/100;
        return rounded;
        //exponential
    }

    public float CalcutaleIncome(int amountOfPowerUp)
    {
        return baseIncome * amountOfPowerUp;
        //linear 
    }

}
