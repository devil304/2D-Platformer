using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { private set; get; }
    public MainIput MainInputAsset;

    [SerializeField] Canvas PauseScreen;
    [SerializeField] Canvas DeathScreen;

    //Make Game manager singleton and setup
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        MainInputAsset = new MainIput();
        MainInputAsset.Enable();

        MainInputAsset.Main.Pause.performed += perf => {
            if (perf.phase == InputActionPhase.Performed)
            {
                PauseScreen.enabled = !PauseScreen.enabled;
                Time.timeScale = Time.timeScale == 1 ? 0 : 1;
            }
        };
    }

    //Display death screen
    public void RaportPlayerDeath()
    {
        DeathScreen.enabled = true;
    }

    [SerializeField] TextMeshProUGUI CoinCounter;
    int coins = 0;

    public void AddCoin()
    {
        coins++;
        if (coins % 10 == 0)
            Player.instance.Heal();
        CoinCounter.text = coins.ToString();
    }
}
