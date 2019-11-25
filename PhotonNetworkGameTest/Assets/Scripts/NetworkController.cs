using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkController : MonoBehaviourPunCallbacks
{
    /****************************
     * Refer to Photon Documentation
     * Documentation: https://doc.photonengine.com/en-us/pun/current/getting-started/pun-intro
     * API: https://doc-api.photonengine.com/en/pun/v2/index.html
     * 
     * If the unity editor is not connecting to a standalone build manually set the fixed region value.
     * 
     * **************************/

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // connects to the photon master servers
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Now connected to the: " + PhotonNetwork.CloudRegion + "Server!");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
