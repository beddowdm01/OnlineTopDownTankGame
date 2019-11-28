using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameMode : MonoBehaviour
{
    protected List<PlayerCharacter> players = new List<PlayerCharacter>();
    public GameObject GameOverPanel = null;
    [SerializeField]
    private Text winnerName = null;
    [SerializeField]
    private int mainMenuIndex = 0;
    [SerializeField]
    private int killsToWin = 10;
    [SerializeField]
    private int damageToWin = 500;
    [SerializeField]
    private enum gameModes {DM, Points};
    [SerializeField]
    gameModes chosenGameMode = gameModes.DM;

    private CurrentGameMode currentGameMode;

    protected PhotonView photonView;

    private void Awake()
    {

        photonView = GetComponent<PhotonView>();
        PlayerCharacter[] playersToAdd = FindObjectsOfType<PlayerCharacter>();
        foreach (PlayerCharacter player in playersToAdd)
        {
            players.Add(player);
        }
        currentGameMode = FindObjectOfType<CurrentGameMode>();
    }

    void Start()
    {
        if(currentGameMode)
        {
            chosenGameMode = (gameModes)currentGameMode.GetGameMode();
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("UpdateGameMode", RpcTarget.All, chosenGameMode);
            }
        }
    }

    public void AddPlayer(PlayerCharacter player)
    {
        players.Add(player);
    }

    public virtual void UpdateGameModeScore()
    {
        foreach (PlayerCharacter player in players)
        {
            if (chosenGameMode == gameModes.DM && player.GetKills() >= killsToWin)
            {
                photonView.RPC("EndGame", RpcTarget.All, player.GetName());
            }
            else if (chosenGameMode == gameModes.Points && player.GetDamage() >= damageToWin)
            {
                photonView.RPC("EndGame", RpcTarget.All, player.GetName());
            }
        }
    }

    public void Quit()
    {
        StartCoroutine(DoSwitchLevel(mainMenuIndex));
    }

    IEnumerator DoSwitchLevel(int level)
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
            yield return null;
        SceneManager.LoadScene(level);
    }

    [PunRPC]
    protected void EndGame(string playerName)//When Someone wins end the game
    {
        foreach (PlayerCharacter player in players)
        {
            player.SetControllable(false);
        }
        Debug.Log("Game Ended");
        Cursor.visible = true;
        winnerName.text = playerName;
        GameOverPanel.SetActive(true);
    }

    [PunRPC]
    private void UpdateGameMode(gameModes gameMode)
    {
        chosenGameMode = gameMode;
        currentGameMode.OnGameModeChanged((int)gameMode);
    }
}
