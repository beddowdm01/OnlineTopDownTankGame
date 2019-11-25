using Photon.Realtime;
using System.IO;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    public float MovementSpeed = 8.0f;
    public float LifeSpan = 3.0f;
    public float bulletDamage = 25;
    public PlayerCharacter Owner;
    public GameObject ExplosionFX;
    public bool CollideWithEnvironment = true;

    private PlayerCharacter hitPlayer;
    private Rigidbody2D rigidBody;
    

    void Awake()
    {
        rigidBody = this.gameObject.GetComponent<Rigidbody2D>();//get rigidbody
    }

    public void InitialiseBullet(PlayerCharacter owner, float lag)
    {
        Owner = owner;
        rigidBody.velocity = transform.up * MovementSpeed;
        rigidBody.position += rigidBody.velocity * lag;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (LifeSpan > 0)
        {
            LifeSpan -= Time.unscaledDeltaTime;
        }
         else
         {
            Destroy(gameObject);//Destroys the bullet without exploding.
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerCharacter")//Destroys itself and spawns explosion and deals damage
        {
            hitPlayer = collision.gameObject.GetComponent<PlayerCharacter>();
            if(hitPlayer != Owner)//if its not the owner of the bullet
            {
                if (hitPlayer.GetHealth() > 0)
                {
                    Owner.IncDamageDealt(bulletDamage);
                    hitPlayer.LowerHealth(Owner, bulletDamage);//lowers player health
                    BlowUp();
                }
            }
        }
        else if (collision.tag == "Environment" && CollideWithEnvironment == true)//Destroys itself and spawns explosion and deals damage
        {
            BlowUp();
        }
    }

    private void BlowUp()
    {
        FindObjectOfType<AudioManager>().Play("MissileExplosion");
        Instantiate(ExplosionFX, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
