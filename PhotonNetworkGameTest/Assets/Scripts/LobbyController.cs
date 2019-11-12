using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject quickStartButton;
    [SerializeField]
    private GameObject quickCancelButton;
    [SerializeField]
    private GameObject delayStartButton;
    [SerializeField]
    private GameObject delayCancelButton;

    [SerializeField]
    private int roomSize = 4;

    private enum gameMode {none, quick, delay};
    gameMode selectedGameMode;

    public void DelayStart()
    {
        delayStartButton.SetActive(false);
        delayCancelButton.SetActive(true);
        selectedGameMode = gameMode.delay;
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("Started");

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

    public string getGameMode()
    {
        return selectedGameMode.ToString();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        delayStartButton.SetActive(true);
        quickStartButton.SetActive(true);
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
}
