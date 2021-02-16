using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyButton : MonoBehaviour
{
    [HideInInspector]public int id;
    public int bottlesOfRedWine;
    public int bottlesOfWhiteWine;
    public int bottlesOfRoseWine;

    public void BuyPowerUp()
    {
        GameManager.instance.BuyAPowerUp(id);
    }

    public void BuyRedWine()
    {
        GameManager.instance.BuyRedWine();
    }

    public void SellRedWine()
    {
        GameManager.instance.SellRedWine();
    }

    public void BuyWhiteWine()
    {
        GameManager.instance.BuyWhiteWine();
    }

    public void SellWhiteWine()
    {
        GameManager.instance.SellWhiteWine();
    }

    public void BuyRoseWine()
    {
        GameManager.instance.BuyRoseWine();
    }

    public void SellRoseWine()
    {
        GameManager.instance.SellRoseWine();
    }
}
