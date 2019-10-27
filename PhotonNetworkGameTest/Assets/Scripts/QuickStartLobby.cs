using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickStartLobby : MonoBehaviourPunCallbacks
{
    public GameObject startButton;
    public GameObject cancelButton;

    [SerializeField]
    private int roomSize = 4;

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        startButton.SetActive(true);
    }

    public void Start()
    {
        startButton.SetActive(false);
        cancelButton.SetActive(false);
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("Started");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join a room");
        CreateRoom();
    }

    void CreateRoom()
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
        CreateRoom();
    }

    public void Cancel()
    {
        cancelButton.SetActive(false);
        startButton.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }
}
