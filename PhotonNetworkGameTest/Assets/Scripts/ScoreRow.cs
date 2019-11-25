using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using System.IO;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun.UtilityScripts;
using UnityEngine.UI;


public class ScoreRow : MonoBehaviour, IPunObservable
{
    public GameObject scoreHUD;

    private Text[] scoreTexts;
    private Text playerName;
    private Text playerKills;
    private Text playerDeaths;
    private Text damageDealt;
    private PhotonView photonView;
    private PlayerCharacter player;

    void Awake()
    {
        scoreTexts = GetComponentsInChildren<Text>();
        photonView = GetComponent<PhotonView>();
        foreach (Text text in scoreTexts)//gets all the text elements in this object
        {
            if (text.name == "NameElement")
            {
                playerName = text;
            }
            if (text.name == "KillsElement")
            {
                playerKills = text;
            }
            else if (text.name == "DeathsElement")
            {
                playerDeaths = text;
            }
            else if (text.name == "DamageDealtElement")
            {
                damageDealt = text;
            }
        }
        HUDScript hud = FindObjectOfType<HUDScript>();//Gets the HUD game object
        hud.ActivateScoreBoard();//To ensure this object can find the scoreboard sets it to active
        GameObject scoreRow = GameObject.Find("ScoreRows");
        transform.SetParent(scoreRow.transform);//Sets the parent to the score rows

        transform.localPosition = new Vector3(0, 0, 0);//resets the position
        transform.rotation = new Quaternion(0, 0, 0, 0);//resets the rotation

        ScoreRow[] scoreBoards = FindObjectsOfType<ScoreRow>();
        foreach (ScoreRow scoreBoard in scoreBoards)//Sets the position of the rows under each other.
        {
            this.gameObject.transform.position -= new Vector3(0, 25, 0);
        }

        hud.DeactivateScoreBoard();//Deactivates the scoreboard
    }

    public void SetTargetPlayer(int PlayerID)
    {
        photonView.RPC("SetPlayer", RpcTarget.AllBufferedViaServer, PlayerID);//sends a message to set all the players scoreboards
        SetName();
    }

    [PunRPC]
    private void SetPlayer(int PlayerID, PhotonMessageInfo info)
    {
        player = (PhotonView.Find(PlayerID).gameObject).GetComponent<PlayerCharacter>();
    }

    public void SetName()
    {
        if (photonView.IsMine)
        {
            playerName.text = PhotonNetwork.NickName;
        }
    }

    // Update is called once per frame
    public void UpdateScore()
    {
        playerKills.text = ((int)player.GetKills()).ToString();
        playerDeaths.text = ((int)player.GetDeaths()).ToString();
        damageDealt.text = ((int)player.GetDamage()).ToString();
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)//syncs all the score data to one and other.
        {
            stream.SendNext(playerName.text);
            stream.SendNext(playerKills.text);
            stream.SendNext(playerDeaths.text);
            stream.SendNext(damageDealt.text);
        }
        else
        {
            playerName.text = (string)stream.ReceiveNext();
            playerKills.text = (string)stream.ReceiveNext();
            playerDeaths.text = (string)stream.ReceiveNext();
            damageDealt.text = (string)stream.ReceiveNext();
        }
    }
}
