using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDScript : MonoBehaviour
{
    private Text[] HUDTexts;
    private Text health;
    private Text boost;
    private GameObject scoreBoard;
    private PlayerCharacter player;

    private void Awake()
    {

        scoreBoard = GameObject.Find("ScoreBoard");//gets the scoreboard
    }

    private void Start()
    {
        HUDTexts = GetComponentsInChildren<Text>();//Gets all the text objects in the game object
        player = FindObjectOfType<PlayerCharacter>();//gets the player characters
        foreach (Text text in HUDTexts)
        {
            if(text.name == "Health")
            {
                health = text;
            }
            else if(text.name == "Boost")
            {
                boost = text;
            }
        }
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
