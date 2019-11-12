using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomController : MonoBehaviourPunCallbacks
{
    //navigate scenes
    [SerializeField]
    private int waitingRoomSceneIndex = 0;
    [SerializeField]
    private int gameRoomSceneIndex = 0;

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnJoinedRoom()
    {
        LobbyController delayStartLobby = FindObjectOfType<LobbyController>();
        string selectedGameMode = delayStartLobby.getGameMode();
        Debug.Log(selectedGameMode);
        if(selectedGameMode == "delay")
        {
            SceneManager.LoadScene(waitingRoomSceneIndex);
        }
        else
        {
            SceneManager.LoadScene(gameRoomSceneIndex);
        }

    }
}

