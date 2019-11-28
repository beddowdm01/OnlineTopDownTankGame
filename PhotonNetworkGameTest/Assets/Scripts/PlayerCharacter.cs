using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;

public class PlayerCharacter : MonoBehaviour, IPunObservable
{
    // Start is called before the first frame update
    public float Speed = 1;//speed multiplier
    public float BoostSpeed = 3;
    public GameObject BulletPrefab;
    public GameObject SpecialBulletPrefab;
    public GameObject MouseTarget;
    public float bulletSpawnDistance = 0.0f;//how far away the bullet spawns from player centre#
    public float ReloadTime = 1f;//Time it takes to reload
    public GameObject ExplosionFX;

    private PhotonView photonView;
    private SpriteRenderer[] sprites;
    private CameraWork cameraWork;
    private Rigidbody2D rigidBody;
    private SpriteRenderer gun;
    private Vector3 bulletSpawnPos;
    private GameObject mouseTarget;
    private bool controllable = true;
    private float PlayerHealth = 100;
    private float PlayerBoost = 100;
    private AudioManager audioManager;
    private ScoreRow playerScore;
    private int kills;
    private int deaths;
    private float damageDealt;
    private GameMode currentGameMode;

    private enum bulletPrefabs { baseBullet, specialBullet };
    private bulletPrefabs currentBullet = bulletPrefabs.baseBullet;



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
            currentGameMode = FindObjectOfType<GameMode>();
            if(currentGameMode)
            {
                currentGameMode.AddPlayer(this);
            }

            audioManager = FindObjectOfType<AudioManager>();
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
            ChangeGun();


            if (Input.GetKeyDown(KeyCode.Mouse0) && ReloadTime < 0)//Shoots a bullet
            {
                ReloadTime = 1;//reset timer
                photonView.RPC("Shoot", RpcTarget.All, bulletSpawnPos, gun.transform.rotation, currentBullet);//sends a message to all players to shoot
            }
            ReloadTime -= Time.unscaledDeltaTime;//Reload cooldown
        }
    }

    private void ChangeGun()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentBullet = bulletPrefabs.baseBullet;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentBullet = bulletPrefabs.specialBullet;
        }
    }

    private void MoveTank()
    {
        float turnAxis = Input.GetAxis("Horizontal");
        float moveAxis = Input.GetAxis("Vertical");
        if (Input.GetKey(KeyCode.Space) && PlayerBoost > 0)
        {

            transform.position += transform.up * Time.deltaTime * BoostSpeed * moveAxis;//boosts the tank forwards and backwards
            audioManager.ChangeVolume("BoostEngine", 0.15f);
            audioManager.ChangeVolume("MoveEngine", 0.0f);
            PlayerBoost -= 0.5f;
        }
        else
        {
            transform.position += transform.up * Time.deltaTime * Speed * moveAxis;//moves the tank forwards and backwards
            if(PlayerBoost <= 100f)
            {
                PlayerBoost += 0.1f;
                audioManager.ChangeVolume("MoveEngine", 0.15f);
                audioManager.ChangeVolume("BoostEngine", 0.0f);
            }
        }     
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

    public void LowerHealth(PlayerCharacter source, float damage)
    {
        PlayerHealth -= damage;//lowers the players health
        if (PlayerHealth <= 0)
        {
            source.IncKills();
            Invoke("Respawn", 3f);//Respawns the player
            KillPlayer();
        }
    }

    private void KillPlayer()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("KillPlayerSettings", RpcTarget.All, photonView.ViewID);//sends a message to all players to die
            Instantiate(ExplosionFX, transform.position, transform.rotation);
            transform.localScale = new Vector3(0f, 0f, 0f);//sets scale to 0
        }
    }

    private void Respawn()
    {
        photonView.RPC("RespawnPlayerSettings", RpcTarget.All, photonView.ViewID);//Resets player variables
        IncDeaths();

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
        transform.position = spawnPosition;//resets the position to 0
        transform.rotation = spawnRotation;//resets the rotation to 0
        transform.localScale = new Vector3(0.5f, 0.5f, 1f);
    }

    public void SetScoreTarget(ScoreRow scoreBoard)
    {
        playerScore = scoreBoard;//sets this players scoreboard
        int scoreID = scoreBoard.GetComponent<PhotonView>().ViewID;
        photonView.RPC("SetScoreBoard", RpcTarget.AllBufferedViaServer, scoreID);//sends a message to all players to set scoreboard
        playerScore.SetTargetPlayer(photonView.ViewID);//makes the scoreboards target this
    }

    public bool GetControllable()
    {
        return controllable;
    }

    public void SetControllable(bool canControl)
    {
        controllable = canControl;
    }


    public float GetHealth()
    {
        return PlayerHealth;
    }

    public float GetBoost()
    {
        return PlayerBoost;
    }

    public float GetDamage()
    {
        return damageDealt;
    }
    
    public int GetKills()
    {
        return kills;
    }
    
    public int GetDeaths()
    {
        return deaths;
    }

    public string GetName()
    {
        return PhotonNetwork.NickName;
    }

    public void IncDamageDealt(float damage)
    {
        damageDealt += damage;
        playerScore.UpdateScore();
        if (currentGameMode && photonView.IsMine)
        {
            currentGameMode.UpdateGameModeScore();
        }
    }

    public void IncKills()
    {
        kills++;
        playerScore.UpdateScore();
        if (currentGameMode && photonView.IsMine)
        {
            currentGameMode.UpdateGameModeScore();
        }
    }

    public void IncDeaths()
    {
        deaths++;
        playerScore.UpdateScore();
        if (currentGameMode && photonView.IsMine)
        {
            currentGameMode.UpdateGameModeScore();
        }
    }



    [PunRPC]
    private void Shoot(Vector3 bulletSpawnPos, Quaternion gunRotation, bulletPrefabs firedBullet, PhotonMessageInfo info)
    {
        if(firedBullet == bulletPrefabs.baseBullet)
        {
            GameObject bullet;//The bullet that will be created
            float lag = (float)(PhotonNetwork.Time - info.SentServerTime);
            FindObjectOfType<AudioManager>().Play("TankShot");
            bullet = Instantiate(BulletPrefab, bulletSpawnPos, gunRotation) as GameObject;
            bullet.GetComponent<BulletMovement>().InitialiseBullet(this, Mathf.Abs(lag));
        }
        else if (firedBullet == bulletPrefabs.specialBullet)
        {
            GameObject bullet;//The bullet that will be created
            float lag = (float)(PhotonNetwork.Time - info.SentServerTime);
            FindObjectOfType<AudioManager>().Play("TankShot");
            bullet = Instantiate(SpecialBulletPrefab, bulletSpawnPos, gunRotation) as GameObject;
            bullet.GetComponent<BulletMovement>().InitialiseBullet(this, Mathf.Abs(lag));
        }
        else
        {
            GameObject bullet;//The bullet that will be created
            float lag = (float)(PhotonNetwork.Time - info.SentServerTime);
            FindObjectOfType<AudioManager>().Play("TankShot");
            bullet = Instantiate(BulletPrefab, bulletSpawnPos, gunRotation) as GameObject;
            bullet.GetComponent<BulletMovement>().InitialiseBullet(this, Mathf.Abs(lag));
        }
    }

    [PunRPC]
    private void SetScoreBoard(int ScoreID, PhotonMessageInfo info)//Assigns a score row to this player
    {
        playerScore = (PhotonView.Find(ScoreID).gameObject).GetComponent<ScoreRow>();
    }

    [PunRPC]
    private void KillPlayerSettings(int sender, PhotonMessageInfo info)
    {
        if (sender == photonView.ViewID)
        {
            controllable = false;//stops controlling
        }
    }

    [PunRPC]
    private void RespawnPlayerSettings(int sender, PhotonMessageInfo info)
    {
        if (sender == photonView.ViewID)
        {
            PlayerHealth = 100;//Resets the player health
            PlayerBoost = 100;//Resets the player boost
            controllable = true;//allows it to be controlled
        }
    }


    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(damageDealt);
            stream.SendNext(kills);
            stream.SendNext(deaths);
        }
        else
        {
            damageDealt = (float)stream.ReceiveNext();
            kills = (int)stream.ReceiveNext();
            deaths = (int)stream.ReceiveNext();
        }
    }
}

