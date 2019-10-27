using Photon.Realtime;
using System.IO;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    public float MovementSpeed = 5.0f;
    public float LifeSpan = 3.0f;
    public float bulletDamage = 25;
    public PlayerCharacter Owner;
    public GameObject ExplosionFX;

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
            BlowUp();//blows up bullet
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
            Debug.Log("Collision happened");
            if (collision.tag == "PlayerCharacter")//Destroys itself and spawns explosion and deals damage
            {
                hitPlayer = collision.gameObject.GetComponent<PlayerCharacter>();
                if (hitPlayer.GetControllable())
                {
                    hitPlayer.LowerHealth(bulletDamage);//lowers player health
                    BlowUp();
                }
            }
            else if (collision.tag == "Environment")//Destroys itself and spawns explosion and deals damage
            {
                BlowUp();
            }
    }

    private void BlowUp()
    {
        Instantiate(ExplosionFX, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
