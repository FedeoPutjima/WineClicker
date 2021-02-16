using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveLoad
{
    public static void Save(int pu1, int pu2, int pu3, int pu4, int pu5, int pu6, int pu7, int pu8, int pu9, float money, float berries, int redWine, int whiteWine, int roseWine) //int powerUp1 etc.
    {
        PlayerPrefs.SetString("IdleSave", pu1 + "|" + pu2 + "|" + pu3 + "|" + pu4 + "|" + pu5 + "|" + pu6 + "|" + pu7 + "|" + pu8 + "|" + pu9 + 
            "|" + money + "|" + berries + "|" + redWine + "|" + whiteWine + "|" + roseWine);
        Debug.Log("Game Saved!");
    }

    public static string Load()
    {
        string data = PlayerPrefs.GetString("IdleSave");
        Debug.Log("Game Loaded!");
        return data;
    }
}
