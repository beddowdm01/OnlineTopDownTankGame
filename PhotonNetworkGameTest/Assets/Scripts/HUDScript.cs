using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDScript : MonoBehaviour
{
    private Text[] HUDTexts;

    [SerializeField]
    private Text health = null;
    [SerializeField]
    private Text boost = null;
    [SerializeField]
    private Text currentGameModeText = null;

    private GameObject scoreBoard;
    private PlayerCharacter player;
    private CurrentGameMode currentGameMode;

    private void Awake()
    {
        scoreBoard = GameObject.Find("ScoreBoard");//gets the scoreboard
        currentGameMode = FindObjectOfType<CurrentGameMode>();
    }

    private void Start()
    {
        HUDTexts = GetComponentsInChildren<Text>();//Gets all the text objects in the game object
        player = FindObjectOfType<PlayerCharacter>();//gets the player characters

    }

    public void ActivateScoreBoard()
    {
        scoreBoard.SetActive(true);//Activates scoreboard
    }

    public void DeactivateScoreBoard()
    {
        scoreBoard.SetActive(false);//deactivates scoreboard
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            currentGameModeText.text = currentGameMode.GetGameModeName();//gets the game mode name to display in the UI
            ActivateScoreBoard();//Activates the scoreboard when tab is pressed
        }
        else
        {
            DeactivateScoreBoard();//Deactivates the scoreboard when tab is pressed
        }
        health.text = ((int)player.GetHealth()).ToString() + "%";//Displays tank health
        boost.text = ((int)player.GetBoost()).ToString() + "%";//Displays tank boost
    }
}
