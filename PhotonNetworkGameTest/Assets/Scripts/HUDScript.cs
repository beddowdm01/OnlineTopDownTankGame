using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDScript : MonoBehaviour
{
    private Text[] HUDTexts;
    private Text health;
    private Text boost;
    private PlayerCharacter player;

    private void Start()
    {
        HUDTexts = GetComponentsInChildren<Text>();
        player = FindObjectOfType<PlayerCharacter>();
        foreach(Text text in HUDTexts)
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

    // Update is called once per frame
    void Update()
    {
        health.text = player.GetHealth().ToString() + "%";
        boost.text = player.GetBoost().ToString() + "%";
    }
}
