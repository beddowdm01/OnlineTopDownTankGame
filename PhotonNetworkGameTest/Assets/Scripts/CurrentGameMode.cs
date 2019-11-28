using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentGameMode : MonoBehaviour
{
    [SerializeField]
    private Dropdown gameModeDropDown = null;

    public int currentGameMode = 0;
    public static CurrentGameMode instance;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance.gameObject);
            instance = this;
        }
        DontDestroyOnLoad(gameObject);//allows it to persist through levels
    }

    public void Start()
    {
        gameModeDropDown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(gameModeDropDown.value);
        });
    }

    public int GetGameMode()
    {
        return currentGameMode;
    }

    public string GetGameModeName()
    {
        if (currentGameMode == 0)
        {
            return "Death Match (10 kills)";
        }
        else if (currentGameMode == 1)
        {
            return "Destruction (500 damage)";
        }
        else
        {
            return "";
        }
    }

    void DropdownValueChanged(int change)
    {
        OnGameModeChanged(change);
    }

    public void OnGameModeChanged (int choiceIndex)
    {
        currentGameMode = choiceIndex;
    }
}
