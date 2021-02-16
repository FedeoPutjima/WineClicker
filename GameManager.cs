using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [System.Serializable]
    public class AnyPowerUp
    {
        [HideInInspector] public int powerUpAmount;
        public PowerUp powerUp;
        public bool unlocked;//Has been unlocked
        [HideInInspector] public bool instanciated;//Has been instanciated yes or no
        [HideInInspector] public PowerUpHolder holder;
    }

    public List<AnyPowerUp> powerUpList = new List<AnyPowerUp>();
    //everything about click power
    [Header("CLICK POWER")]
    public Text clickPowerText;
    public GameObject grape;
    //everything about money
    [Header("MONEY")]
    public float money;
    public Text totalMoneyText;
    public Text totalBpsText;


    //everything about berries
    [Header("BERRIES")]
    public Text totalBerriesText;
    public float berries;

    //everything about wine
    [Header("WINE")]
    public int redWineCost;
    public int redWineBottlesAmount;
    public float baseRedWineSellCost;
    public float weatherRedWineSellCost;
    public Text redWineText;
    public Text berriesToRedWineCost;

    public int whiteWineCost;
    public int whiteWineBottlesAmount;
    public float baseWhiteWineSellCost;
    public float weatherWhiteWineSellCost;
    public Text whiteWineText;
    public Text berriesToWhiteWineCost;

    public int roseWineCost;
    public int roseWineBottlesAmount;
    public float baseRoseWineSellCost;
    public float weatherRoseWineSellCost;
    public Text roseWineText;
    public Text berriesToRoseWineCost;

    //everything about the weather
    [Header("WEATHER")]
    public Image background;
    public Sprite winter;
    public Sprite fall;
    public Sprite spring;
    public Sprite summer;

    //User interface
    [Header("USER INTERFACE")]
    public GameObject powerUpHolderUI;
    public Transform grid;

    //SAVING
    public GameObject saveText;

    //CONSTANTS
    float TIME_BETWEEN_SAVES = 15f;
    float TIME_BETWEEN_WEATHER_CHANGE = 30f;
    float LOWEST_WINE_PRICE = 0.5f;
    float LOWER_WINE_PRICE = 0.75f;
    float HIGHEST_WINE_PRICE = 2f;
    float HIGHER_WINE_PRICE = 1.5f;


    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        saveText.SetActive(false);
        if (PlayerPrefs.HasKey("IdleSave"))
        {
            LoadTheGame();
        }
        else FillList();
        SetWineCost();
        UpdateBerriesUI();
        UpdateMoneyUI();
        CalculateBPS();
        UpdateClickPowerUI();
        UpdateRedWineUI();
        UpdateRoseWineUI();
        UpdateWhiteWineUI();
        StartCoroutine(Tick());
        SetBerriesToWineCost();
        RandomizeWeather();
        AutoSave();
    }

    IEnumerator Tick()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            foreach (AnyPowerUp p in powerUpList)
            {
                if (p.powerUpAmount > 0)
                {
                    berries += p.powerUp.baseIncome * p.powerUpAmount;
                    berries = (float)Mathf.Round(berries * 100) / 100;
                    //Update berries UI
                    UpdateBerriesUI();
                }
            }
        }
    }

    void FillList()
    {
        for (int i = 0; i < powerUpList.Count; i++)
        {
            if (powerUpList[i].unlocked)
            {
                if (powerUpList[i].powerUpAmount > 0 || powerUpList[i].instanciated)
                {
                    //skip the power up
                    continue;
                }
                GameObject powerUpHolder = Instantiate(powerUpHolderUI, grid, false) as GameObject;
                powerUpList[i].holder = powerUpHolder.GetComponent<PowerUpHolder>();
                if (powerUpList[i].powerUpAmount > 0)
                {
                    if (i == 0)
                    {
                        powerUpList[i].holder.powerUpImage.sprite = powerUpList[i].powerUp.powerUpImage;
                        powerUpList[i].holder.powerUpName.text = powerUpList[i].powerUp.powerUpName;
                        powerUpList[i].holder.amountText.text = "Amount: " + powerUpList[i].powerUpAmount.ToString("N0");
                        powerUpList[i].holder.bpsText.text = "Pick Power: " + 1.ToString("N2");
                        powerUpList[i].holder.costText.text = "Cost: " + powerUpList[i].powerUp.CalculateCost(powerUpList[i].powerUpAmount).ToString("N2") + "$";
                    }
                    else
                    {
                        powerUpList[i].holder.powerUpImage.sprite = powerUpList[i].powerUp.powerUpImage;
                        powerUpList[i].holder.powerUpName.text = powerUpList[i].powerUp.powerUpName;
                        powerUpList[i].holder.amountText.text = "Amount: " + powerUpList[i].powerUpAmount.ToString("N0");
                        powerUpList[i].holder.bpsText.text = "BPS: " + powerUpList[i].powerUp.baseIncome.ToString("N2");
                        powerUpList[i].holder.costText.text = "Cost: " + powerUpList[i].powerUp.CalculateCost(powerUpList[i].powerUpAmount).ToString("N2") + "$";
                    }
                }
                else
                {
                    powerUpList[i].holder.powerUpImage.sprite = powerUpList[i].powerUp.unknownPowerUpImage;
                    powerUpList[i].holder.powerUpName.text = "????";
                    powerUpList[i].holder.amountText.text = "Amount: " + powerUpList[i].powerUpAmount.ToString("N0");
                    powerUpList[i].holder.bpsText.text = "BPS: " + powerUpList[i].powerUp.baseIncome.ToString("N2");
                    powerUpList[i].holder.costText.text = "Cost: " + powerUpList[i].powerUp.CalculateCost(powerUpList[i].powerUpAmount).ToString("N2") + "$";
                }

                powerUpList[i].holder.buyButton.id = i;
                powerUpList[i].instanciated = true;
            }
        }
    }

    public void BuyAPowerUp(int id)
    {
        //Check if enough Money
        if (money < powerUpList[id].powerUp.CalculateCost(powerUpList[id].powerUpAmount))
        {
            Debug.Log("No money");
            return;
        }
        //Subtract money for buying a power up
        money -= powerUpList[id].powerUp.CalculateCost(powerUpList[id].powerUpAmount);
        //Change image and text if it's first time buying a power up
        if (powerUpList[id].powerUpAmount < 1)
        {
            powerUpList[id].holder.powerUpImage.sprite = powerUpList[id].powerUp.powerUpImage;
            powerUpList[id].holder.powerUpName.text = powerUpList[id].powerUp.powerUpName;
        }
        //Add to the total number of this power up
        powerUpList[id].powerUpAmount++;
        //Change the texts
        if (id == 0)
        {
            powerUpList[id].holder.amountText.text = "Amount: " + powerUpList[id].powerUpAmount.ToString("N0");
            powerUpList[id].holder.bpsText.text = "Pick Power: " + 1.ToString("N2");
            powerUpList[id].holder.costText.text = "Cost: " + powerUpList[id].powerUp.CalculateCost(powerUpList[id].powerUpAmount).ToString("N2") + "$";
            grape.GetComponent<GrapeBerry>().clickAmount++;
        }
        else
        {
            powerUpList[id].holder.amountText.text = "Amount: " + powerUpList[id].powerUpAmount.ToString("N0");
            powerUpList[id].holder.bpsText.text = "BPS: " + powerUpList[id].powerUp.baseIncome.ToString("N2");
            powerUpList[id].holder.costText.text = "Cost: " + powerUpList[id].powerUp.CalculateCost(powerUpList[id].powerUpAmount).ToString("N2") + "$";
        }

        //Unlock next power up
        if (id < powerUpList.Count - 1 && powerUpList[id].powerUpAmount > 0)
        {
            powerUpList[id + 1].unlocked = true;
            FillList();
        }
        //Update the bps
        CalculateBPS();
        //Update moneyUI
        UpdateMoneyUI();
        //Update Click Power UI
        UpdateClickPowerUI();

    }

    public void BuyRedWine()
    {
        if (berries >= redWineCost)
        {
            redWineBottlesAmount++;
            berries -= redWineCost;
            UpdateBerriesUI();
            UpdateRedWineUI();
        }
        else
        {
            Debug.Log("Not enough money for red wine!");
            return;
        }
    }

    public void SellRedWine()
    {
        if (redWineBottlesAmount > 0)
        {
            redWineBottlesAmount--;
            money += weatherRedWineSellCost;
            UpdateRedWineUI();
            UpdateMoneyUI();
        }
        else
        {
            Debug.Log("No Red Wine Bottles");
            return;
        }
    }

    public void UpdateRedWineUI()
    {
        redWineText.text = redWineBottlesAmount + " Bottles";
    }

    public void BuyWhiteWine()
    {
        if (berries >= whiteWineCost)
        {
            whiteWineBottlesAmount++;
            berries -= whiteWineCost;
            UpdateBerriesUI();
            UpdateWhiteWineUI();
        }
        else
        {
            Debug.Log("Not enough money for white wine!");
            return;
        }
    }

    public void SellWhiteWine()
    {
        if (whiteWineBottlesAmount > 0)
        {
            whiteWineBottlesAmount--;
            money += weatherWhiteWineSellCost;
            UpdateWhiteWineUI();
            UpdateMoneyUI();
        }
        else
        {
            Debug.Log("No White Wine Bottles");
            return;
        }
    }

    public void UpdateWhiteWineUI()
    {
        whiteWineText.text = whiteWineBottlesAmount + " Bottles";
    }

    public void BuyRoseWine()
    {
        if (berries >= roseWineCost)
        {
            roseWineBottlesAmount++;
            berries -= roseWineCost;
            UpdateBerriesUI();
            UpdateRoseWineUI();
        }
        else
        {
            Debug.Log("Not enough money for rose wine!");
            return;
        }
    }

    public void SellRoseWine()
    {
        if (roseWineBottlesAmount > 0)
        {
            roseWineBottlesAmount--;
            money += weatherRoseWineSellCost;
            UpdateRoseWineUI();
            UpdateMoneyUI();
        }
        else
        {
            Debug.Log("No Red Wine Bottles");
            return;
        }
    }

    public void UpdateRoseWineUI()
    {
        roseWineText.text = roseWineBottlesAmount + " Bottles";
    }

    public void AddBerries(int amount)
    {
        berries += amount;
        //UPDATE BERRIES UI
        UpdateBerriesUI();
    }

    void UpdateBerriesUI()
    {
        totalBerriesText.text = "Total Berries: " + berries.ToString("N2");
    }

    void UpdateMoneyUI()
    {
        totalMoneyText.text = "Total Money: " + money.ToString("N2") + "$";
    }

    void UpdateClickPowerUI()
    {
        clickPowerText.text = "Pick Power: " + grape.GetComponent<GrapeBerry>().clickAmount;
    }

    void CalculateBPS()
    {
        float allBps = 0;
        foreach (AnyPowerUp a in powerUpList)
        {
            if (a.powerUpAmount > 0)
            {
                allBps += a.powerUp.CalcutaleIncome(a.powerUpAmount);
                totalBpsText.text = "Total BPS: " + allBps;
            }
        }
        if (allBps == 0)
        {
            totalBpsText.text = "Total BPS: " + allBps;
        }
    }

    void SaveTheGame()
    {
        SaveLoad.Save(powerUpList[0].powerUpAmount, powerUpList[1].powerUpAmount, powerUpList[2].powerUpAmount, powerUpList[3].powerUpAmount, powerUpList[4].powerUpAmount,
            powerUpList[5].powerUpAmount, powerUpList[6].powerUpAmount, powerUpList[7].powerUpAmount, powerUpList[8].powerUpAmount, money, berries,
            redWineBottlesAmount, whiteWineBottlesAmount, roseWineBottlesAmount);
    }

    void AutoSave()
    {
        SaveTheGame();
        saveText.SetActive(true);
        Invoke(nameof(AutoSave), TIME_BETWEEN_SAVES);
    }

    void LoadTheGame()
    {
        //Check if player has saved data
        if (PlayerPrefs.HasKey("IdleSave"))
        {
            //load up the save into a string
            string data = SaveLoad.Load();
            string[] stringList = data.Split("|"[0]);
            //Go through the each data to get all the info on power ups
            for (int i = 0; i < stringList.Length - 5; i++)
            {
                int temp = int.Parse(stringList[i]);
                powerUpList[i].powerUpAmount = temp;
                //if player has more then 0 of power up, unlock next one
                if (temp > 0)
                {
                    if (i + 1 < powerUpList.Count)
                    {
                        powerUpList[i + 1].unlocked = true;
                    }
                    //fill the list individualy
                    FillSinglePowerUp(i);
                }
            }
            //load the data for pick power
            grape.GetComponent<GrapeBerry>().clickAmount = int.Parse(stringList[0]);
            if (grape.GetComponent<GrapeBerry>().clickAmount == 0)
            {
                grape.GetComponent<GrapeBerry>().clickAmount = 1;
            }          
            //load the data for total money
            money = float.Parse(stringList[9]);
            //load the data for total berries
            berries = float.Parse(stringList[10]);
            //load the data for the wines
            redWineBottlesAmount = int.Parse(stringList[11]);
            whiteWineBottlesAmount = int.Parse(stringList[12]);
            roseWineBottlesAmount = int.Parse(stringList[13]);
            //load up the "panel" for the last unlocked power up
            FillList();
            //Update UI with load data
            UpdateBerriesUI();
            CalculateBPS();
            UpdateMoneyUI();
            UpdateRedWineUI();
            UpdateRoseWineUI();
            UpdateWhiteWineUI();
        }
    }

    void FillSinglePowerUp(int id)
    {
        //Same as FillList() but just for one power up
        if (powerUpList[id].unlocked)
        {
            GameObject powerUpHolder = Instantiate(powerUpHolderUI, grid, false) as GameObject;
            powerUpList[id].holder = powerUpHolder.GetComponent<PowerUpHolder>();
            if (powerUpList[id].powerUpAmount > 0)
            {
                if (id == 0)
                {
                    powerUpList[id].holder.powerUpImage.sprite = powerUpList[id].powerUp.powerUpImage;
                    powerUpList[id].holder.powerUpName.text = powerUpList[id].powerUp.powerUpName;
                    powerUpList[id].holder.amountText.text = "Amount: " + powerUpList[id].powerUpAmount.ToString("N0");
                    powerUpList[id].holder.bpsText.text = "Pick Power " + 1.ToString("N2");
                    powerUpList[id].holder.costText.text = "Cost: " + powerUpList[id].powerUp.CalculateCost(powerUpList[id].powerUpAmount).ToString("N2") + "$";
                }
                else
                {
                    powerUpList[id].holder.powerUpImage.sprite = powerUpList[id].powerUp.powerUpImage;
                    powerUpList[id].holder.powerUpName.text = powerUpList[id].powerUp.powerUpName;
                    powerUpList[id].holder.amountText.text = "Amount: " + powerUpList[id].powerUpAmount.ToString("N0");
                    powerUpList[id].holder.bpsText.text = "BPS: " + powerUpList[id].powerUp.baseIncome.ToString("N2");
                    powerUpList[id].holder.costText.text = "Cost: " + powerUpList[id].powerUp.CalculateCost(powerUpList[id].powerUpAmount).ToString("N2") + "$";
                }
            }
            else
            {
                powerUpList[id].holder.powerUpImage.sprite = powerUpList[id].powerUp.unknownPowerUpImage;
                powerUpList[id].holder.powerUpName.text = "????";
                powerUpList[id].holder.amountText.text = "Amount: " + powerUpList[id].powerUpAmount.ToString("N0");
                powerUpList[id].holder.bpsText.text = "BPS: " + powerUpList[id].powerUp.baseIncome.ToString("N2");
                powerUpList[id].holder.costText.text = "Cost: " + powerUpList[id].powerUp.CalculateCost(powerUpList[id].powerUpAmount).ToString("N2") + "$";
            }

            powerUpList[id].holder.buyButton.id = id;
            powerUpList[id].instanciated = true;

        }
    }

    void SetWineCost()
    {
        weatherRedWineSellCost = baseRedWineSellCost;
        weatherRoseWineSellCost = baseRoseWineSellCost;
        weatherWhiteWineSellCost = baseWhiteWineSellCost;
    }

    void RandomizeWeather()
    {
        int randomWeather = Random.Range(1, 5);
        if (randomWeather == 1)
        {
            background.sprite = winter;
            weatherRedWineSellCost = baseRedWineSellCost * HIGHEST_WINE_PRICE;
            weatherRoseWineSellCost = baseRoseWineSellCost * LOWEST_WINE_PRICE;
            weatherWhiteWineSellCost = baseWhiteWineSellCost * HIGHER_WINE_PRICE;
            Debug.Log("It's winter!");
        }
        else if (randomWeather == 2)
        {
            background.sprite = spring;
            weatherRedWineSellCost = baseRedWineSellCost;
            weatherRoseWineSellCost = baseRoseWineSellCost * LOWER_WINE_PRICE;
            weatherWhiteWineSellCost = baseWhiteWineSellCost * HIGHER_WINE_PRICE;
            Debug.Log("It's spring!");
        }
        else if (randomWeather == 3)
        {
            background.sprite = summer;
            weatherRedWineSellCost = baseRedWineSellCost * LOWEST_WINE_PRICE;
            weatherRoseWineSellCost = baseRoseWineSellCost * HIGHEST_WINE_PRICE;
            weatherWhiteWineSellCost = baseWhiteWineSellCost * HIGHER_WINE_PRICE;
            Debug.Log("It's summer!");
        }
        else if (randomWeather == 4)
        {
            background.sprite = fall;
            weatherRedWineSellCost = baseRedWineSellCost * HIGHER_WINE_PRICE;
            weatherRoseWineSellCost = baseRoseWineSellCost * LOWER_WINE_PRICE;
            weatherWhiteWineSellCost = baseWhiteWineSellCost;
            Debug.Log("It's fall!");
        }
        else return;
        Invoke(nameof(RandomizeWeather), TIME_BETWEEN_WEATHER_CHANGE);
    }

    void SetBerriesToWineCost()
    {
        berriesToRedWineCost.text = redWineCost + " Berries";
        berriesToRoseWineCost.text = roseWineCost + " Berries";
        berriesToWhiteWineCost.text = whiteWineCost + " Berries";
    }

}
