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
        Vector3 spawnPosition = new Vector3();
        Debug.Log("Creating Player");
        SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();
        foreach (SpawnPoint spawnPoint in spawnPoints)
        {
            if (!spawnPoint.GetOverlapped())
            {
                spawnPositions.Add(spawnPoint.transform.position);//adds all nonoverlapped spawn points to a list
            }
        }
        spawnPosition = spawnPositions[Random.Range(0, spawnPositions.Count)];//selects a random spawn position from the list
        transform.position = spawnPosition;//resets the position to 0
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerCharacter"), spawnPosition, Quaternion.identity);
    }
}
