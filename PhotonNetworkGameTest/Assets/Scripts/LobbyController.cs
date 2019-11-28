using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject quickStartButton = null;
    [SerializeField]
    private GameObject quickCancelButton = null;
    [SerializeField]
    private GameObject delayStartButton = null;
    [SerializeField]
    private GameObject delayCancelButton = null;

    [SerializeField]
    private GameObject lobbyConnectButton = null;
    [SerializeField]
    private GameObject lobbyPanel = null;
    [SerializeField]
    private GameObject mainPanel = null;
    [SerializeField]
    private InputField playerNameInput = null;

    [SerializeField]
    private Text roomNameText = null;
    [SerializeField]
    private Text roomSizeText = null;

    [SerializeField]
    private Transform roomsContainer = null;
    [SerializeField]
    private GameObject roomListingPrefab = null;
    [SerializeField]
    private CurrentGameMode currentGameMode = null;

    [SerializeField]
    private int roomSize = 4;
    private string roomName;
    private List<RoomInfo> roomListings;




    private enum gameMode {none, quick, delay, priv};
    gameMode selectedGameMode;


    public void DelayStart()
    {
        delayStartButton.SetActive(false);
        delayCancelButton.SetActive(true);
        selectedGameMode = gameMode.delay;
        PhotonNetwork.JoinRandomRoom();
        currentGameMode = FindObjectOfType<CurrentGameMode>();

    }

    public void DelayCancel()
    {
        delayStartButton.SetActive(true);
        delayCancelButton.SetActive(false);
        selectedGameMode = gameMode.none;
        PhotonNetwork.LeaveRoom();
    }

    public void QuickStart()
    {
        PlayerNameUpdate("");
        quickStartButton.SetActive(false);
        quickCancelButton.SetActive(true);
        selectedGameMode = gameMode.quick;
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("Started");
    }

    public void QuickCancel()
    {
        quickStartButton.SetActive(true);
        quickCancelButton.SetActive(false);
        selectedGameMode = gameMode.none;
        PhotonNetwork.LeaveRoom();
    }

    public void PrivateStart()
    {
        quickStartButton.SetActive(false);
        quickCancelButton.SetActive(true);
        selectedGameMode = gameMode.quick;
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("Started");

    }

    public void PrivateCancel()
    {
        quickStartButton.SetActive(true);
        quickCancelButton.SetActive(false);
        selectedGameMode = gameMode.none;
        PhotonNetwork.LeaveRoom();
    }

    public string getGameMode()
    {
        return selectedGameMode.ToString();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join a room");
        CreateRandomRoom();
    }

    void CreateRandomRoom()
    {
        Debug.Log("Creating our own room");
        int randomRoomNumber = Random.Range(0, 10000);
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize };
        PhotonNetwork.CreateRoom("Room" + randomRoomNumber, roomOps);
        Debug.Log("Room Number: " + randomRoomNumber);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create room... retrying");
        CreateRandomRoom();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        lobbyConnectButton.SetActive(true);
        delayStartButton.SetActive(true);
        quickStartButton.SetActive(true);
        roomListings = new List<RoomInfo>();

        if (PlayerPrefs.HasKey("NickName"))
        {
            if (PlayerPrefs.GetString("NickName") == "")
            {
                PhotonNetwork.NickName = "Player " + Random.Range(0, 1000);
            }
            else
            {
                PhotonNetwork.NickName = PlayerPrefs.GetString("NickName");
            }
        }
        else
        {
            PhotonNetwork.NickName = "Player " + Random.Range(0, 1000);
        }
        playerNameInput.text = PhotonNetwork.NickName;
    }

    public void PlayerNameUpdate(string nameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
        PlayerPrefs.SetString("NickName", playerNameInput.text);
    }

    public void JoinLobbyOnClick()
    {
        selectedGameMode = gameMode.priv;
        PhotonNetwork.NickName = playerNameInput.text;
        PlayerPrefs.SetString("NickName", playerNameInput.text);
        mainPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int tempIndex;
        foreach (RoomInfo room in roomList)
        {
            if (roomListings != null)
            {
                tempIndex = roomListings.FindIndex(ByName(room.Name));
            }
            else
            {
                tempIndex = -1;
            }
            if (tempIndex != -1)
            {
                roomListings.RemoveAt(tempIndex);
                Destroy(roomsContainer.GetChild(tempIndex).gameObject);
            }
            if (room.PlayerCount > 0)
            {
                roomListings.Add(room);
                ListRoom(room);
            }
        }
    }

    static System.Predicate<RoomInfo> ByName(string name)
    {
        return delegate (RoomInfo room)
        {
            return room.Name == name;
        };
    }

    void ListRoom(RoomInfo room)
    {
        if (room.IsOpen && room.IsVisible)
        {
            GameObject tempListing = Instantiate(roomListingPrefab, roomsContainer);
            RoomButton tempButton = tempListing.GetComponent<RoomButton>();
            tempButton.SetRoom(room.Name, room.MaxPlayers, room.PlayerCount);
        }
    }

    public void OnRoomNameChanged(string nameIn)
    {
        roomName = roomNameText.text;
        Debug.Log(roomName);
    }

    public void OnRoomSizeChanged(string sizeIn)
    {
        int number;
        bool result = int.TryParse(roomSizeText.text, out number);
        if (result)
        {
            roomSize = int.Parse(roomSizeText.text);
        }
        Debug.Log(roomSize);
    }

    public void CreateRoom()
    {
        roomSize = int.Parse(roomSizeText.text);
        roomName = roomNameText.text;
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize };
        PhotonNetwork.CreateRoom(roomName, roomOps);
    }

    public void MatchmakingCancel()
    {
        mainPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        PhotonNetwork.LeaveLobby();
    }
}
