using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSetupController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CreatePlayer();
    }

    private void CreatePlayer()
    {
        List<Vector3> spawnPositions = new List<Vector3>();
        List<Quaternion> spawnRotations = new List<Quaternion>();
        SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();
        foreach (SpawnPoint spawnPoint in spawnPoints)
        {
            if (!spawnPoint.GetOverlapped())
            {
                spawnPositions.Add(spawnPoint.transform.position);//adds all nonoverlapped spawn points to a list
                spawnRotations.Add(spawnPoint.transform.rotation);//adds all nonoverlapped spawn points to a list
            }
        }
        int randomPoint = Random.Range(0, spawnPositions.Count);
        Vector3 spawnPosition = spawnPositions[randomPoint];//selects a random spawn position from the list
        Quaternion spawnRotation = spawnRotations[randomPoint];
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerCharacter"), spawnPosition, spawnRotation);
    }
}
