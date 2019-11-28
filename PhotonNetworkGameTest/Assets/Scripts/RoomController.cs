using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using Photon.Realtime;

public class RoomController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject lobbyPanel = null;
    [SerializeField]
    public GameObject roomPanel = null;

    [SerializeField]
    private GameObject masterClientObjects = null;

    [SerializeField]
    private Transform playersContainer = null;
    [SerializeField]
    private GameObject playerListingPrefab = null;

    [SerializeField]
    private Text roomNameDisplay = null;

    //navigate scenes
    [SerializeField]
    private int waitingRoomSceneIndex = 0;
    [SerializeField]
    private int gameRoomSceneIndex = 0;

    [SerializeField]
    private int gameRoomsSceneStartIndex = 0;

    [SerializeField]
    private Dropdown mapDropDown = null;

    private void Start()
    {
        mapDropDown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(mapDropDown.value);
        });
    }

    void DropdownValueChanged(int change)
    {
        gameRoomSceneIndex = gameRoomsSceneStartIndex + change;
    }

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
        else if(selectedGameMode == "priv")
        {
            roomPanel.SetActive(true);
            lobbyPanel.SetActive(false);
            roomNameDisplay.text = PhotonNetwork.CurrentRoom.Name;
            if (PhotonNetwork.IsMasterClient)
            {
                masterClientObjects.SetActive(true);
            }
            else
            {
                masterClientObjects.SetActive(false);
            }
            ClearPlayerListings();
            ListPlayers();
        }          
        else
        {
            SceneManager.LoadScene(gameRoomSceneIndex);
        }

    }
    void ClearPlayerListings()
    {
        for (int i = playersContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(playersContainer.GetChild(i).gameObject);
        }
    }

    void ListPlayers()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject tempListing = Instantiate(playerListingPrefab, playersContainer);
            Text tempText = tempListing.transform.GetChild(0).GetComponent<Text>();
            tempText.text = player.NickName;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        ClearPlayerListings();
        ListPlayers();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ClearPlayerListings();
        ListPlayers();
        if (PhotonNetwork.IsMasterClient)
        {
            masterClientObjects.SetActive(true);
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;//comment this out to be able to join in session games.
            PhotonNetwork.LoadLevel(gameRoomSceneIndex);
        }
    }

    IEnumerator rejoinLobby()
    {
        yield return new WaitForSeconds(1);
        PhotonNetwork.JoinLobby();
    }

    public void BackOnClick()
    {
        lobbyPanel.SetActive(true);
        roomPanel.SetActive(false);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
        StartCoroutine(rejoinLobby());
    }

}

