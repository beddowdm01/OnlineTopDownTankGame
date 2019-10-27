using Photon.Pun;
using UnityEngine;

public class QuickStartRoom : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private int multiplayerSceneIndex = 1; //build index number
    // Start is called before the first frame update

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
        Debug.Log("Joined Room");
        StartGame();
    }

    private void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Starting Game");
            PhotonNetwork.LoadLevel(multiplayerSceneIndex);
        }
    }
}

