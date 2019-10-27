using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using System.IO;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun.UtilityScripts;

public class PlayerCharacter : MonoBehaviour, IPunObservable
{
    // Start is called before the first frame update
    public float Speed = 1;//speed multiplier
    public GameObject BulletPrefab;
    public GameObject MouseTarget;
    public float bulletSpawnDistance = 0.7f;//how far away the bullet spawns from player centre#
    public float PlayerHealth = 100;
    public float ReloadTime = 1f;//Time it takes to reload

    private PhotonView photonView;
    private SpriteRenderer[] sprites;
    private CameraWork cameraWork;
    private Rigidbody2D rigidBody;
    private SpriteRenderer gun;
    private Vector3 bulletSpawnPos;
    private GameObject mouseTarget;
    private bool controllable = true;


    void Awake()
    {
        photonView = GetComponent<PhotonView>();

        photonView.ObservedComponents.Add(this);
        if (!photonView.IsMine)
        {
            enabled = false;
        }
    }

    void Start()
    {
        if (photonView.IsMine)
        {
            rigidBody = this.gameObject.GetComponent<Rigidbody2D>();
            sprites = this.gameObject.GetComponentsInChildren<SpriteRenderer>();//gets all the player sprites
            foreach (SpriteRenderer sprite in sprites)
            {
                if (sprite.name == "GunBase")
                {
                    gun = sprite;
                }
            }
        }

        cameraWork = this.gameObject.GetComponent<CameraWork>();
        if (cameraWork != null)
        {
            if (photonView.IsMine)
            {
                cameraWork.OnStartFollowing();
            
            }
        }
        else
        {
            Debug.LogError("Camera Not Working");
        }
        //instantiates the mouse target
        if(MouseTarget)
        {
            mouseTarget = Instantiate(MouseTarget, Camera.main.ScreenToWorldPoint(Input.mousePosition), new Quaternion(0,0,0,0));
            Cursor.visible = false;
        }
    }

    void Update()
    {
        if(photonView.IsMine && controllable)
        {
            RotateGun();//Rotates the gun
            MoveCursor();//Moves the target cursor
            MoveTank();//Moves and rotates the tank

            if (Input.GetKeyDown(KeyCode.Mouse0) && ReloadTime < 0)//Shoots a bullet
            {
                ReloadTime = 1;//reset timer
                photonView.RPC("Shoot", RpcTarget.All, bulletSpawnPos, gun.transform.rotation);//sends a message to all players to shoot
            }
            ReloadTime -= Time.unscaledDeltaTime;//Reload cooldown
        }
    }

    private void MoveTank()
    {
        float turnAxis = Input.GetAxis("Horizontal");
        float moveAxis = Input.GetAxis("Vertical");

        transform.position += transform.up * Time.deltaTime * Speed * moveAxis;//moves the tank forwards and backwards
        transform.Rotate(0, 0, -turnAxis);//rotates the tank
    }

    private void MoveCursor()
    {
        float x = (Camera.main.ScreenToWorldPoint(Input.mousePosition).x);
        float y = (Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        mouseTarget.transform.position = new Vector3(x,y,0f);
    }

    private void RotateGun()
    {
            Vector2 lookAtPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);//sets a variable Vector2 to mouse pos
            Vector2 direction = (lookAtPos - (Vector2)transform.position).normalized;
            gun.transform.up = direction;//makes the up vector face the mouse cursor
            bulletSpawnPos = gun.transform.position + gun.transform.up * bulletSpawnDistance;//Sets bullet spawn position
    }

    public void LowerHealth(float damage)
    {
        PlayerHealth -= damage;//lowers the players health
        if (PlayerHealth <= 0)
        {
            Invoke("Respawn", 3f);//Respawns the player
            //KillPlayer();
            Debug.Log("Dead" + PlayerHealth);
            photonView.RPC("KillPlayer", RpcTarget.All, photonView.ViewID);//sends a message to all players to shoot
            //photonView.RPC("Respawn", RpcTarget.All, photonView.ViewID);
        }
    }

    public void Respawn()
    {
        photonView.RPC("RespawnPlayer", RpcTarget.All, photonView.ViewID);
    }

    public bool GetControllable()
    {
        return controllable;
    }

    [PunRPC]
    private void Shoot(Vector3 bulletSpawnPos, Quaternion gunRotation, PhotonMessageInfo info)
    {
        GameObject bullet;//The bullet that will be created
        float lag = (float)(PhotonNetwork.Time - info.SentServerTime);
        bullet = Instantiate(BulletPrefab, bulletSpawnPos, gunRotation) as GameObject;
        bullet.GetComponent<BulletMovement>().InitialiseBullet(this, Mathf.Abs(lag));
    }

    [PunRPC]
    private void KillPlayer(int sender, PhotonMessageInfo info)
    {
        transform.localScale = new Vector3(0f, 0f, 0f);//sets scale to 0
        controllable = false;//stops controlling
    }

    [PunRPC]
    private void RespawnPlayer(int sender, PhotonMessageInfo info)
    {
        List<Vector3> spawnPositions = new List<Vector3>();
        Vector3 spawnPosition = new Vector3();
        Quaternion spawnRotation = new Quaternion();
        SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();
        foreach(SpawnPoint spawnPoint in spawnPoints)
        {
            if(!spawnPoint.GetOverlapped())
            {
               spawnPositions.Add(spawnPoint.transform.position);//adds all nonoverlapped spawn points to a list
            }
        }
        spawnPosition = spawnPositions[Random.Range(0, spawnPositions.Count)];//selects a random spawn position from the list
        transform.position = spawnPosition;//resets the position to 0
        transform.rotation = spawnRotation;//resets the rotation to 0
        transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        PlayerHealth = 100;//Resets the player health
        controllable = true;//allows it to be controlled
    }


    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            
        }
        else
        {

        }
    }
}

