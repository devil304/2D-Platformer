using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { private set; get; }
    public MainIput MainInputAsset;
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
                Time.timeScale = Time.timeScale == 1 ? 0 : 1;
            }
        };
    }
[SerializeField] TextMeshProUGUI CoinCounter;
    int coins = 0;
    public void AddCoin()
    {
        coins++;
        CoinCounter.text = coins.ToString();
    }
}
