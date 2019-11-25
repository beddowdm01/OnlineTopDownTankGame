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

    protected PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        PlayerCharacter[] playersToAdd = FindObjectsOfType<PlayerCharacter>();
        foreach(PlayerCharacter player in playersToAdd)
        {
            players.Add(player);
        }
    }

    public void AddPlayer(PlayerCharacter player)
    {
        players.Add(player);
    }

    public virtual void UpdateGameModeScore()
    {

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
}
