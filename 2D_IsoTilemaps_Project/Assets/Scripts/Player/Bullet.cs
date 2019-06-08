using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int dmg = 1;
    public bool bossBullet = false;

    private const float bulletDieTime = 4.0f;
    private float nextDieTime = 0.0f;

    [HideInInspector] public Vector2 fDir;
    private Vector2 velocity;
    public float speed = 5.0f;

    public bool enemyBullet = false;

    private void Awake()
    {
        //rb2d = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        nextDieTime = Time.time + bulletDieTime;
    }

    // Update is called once per frame
    void Update()
    {
        velocity = fDir * speed;
        transform.Translate(velocity * Time.deltaTime);
        if(Time.time > nextDieTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.isTrigger)
        {
            if (!enemyBullet)
            {
                if (collider.CompareTag("Enemy"))
                {
                    EnemyStatus enemyStats = collider.GetComponent<EnemyStatus>();
                    enemyStats.health -= dmg;
                    enemyStats.hit = true;
                    enemyStats.knockBackDir = velocity.normalized;
                    Destroy(gameObject);
                    //Debug.Log("Knocked back");
                }
                else if (collider.CompareTag("EnemyArcher"))
                {
                    EnemyStatus1 enemyStats = collider.GetComponent<EnemyStatus1>();
                    enemyStats.health -= dmg;
                    enemyStats.hit = true;
                    enemyStats.knockBackDir = velocity.normalized;
                    Destroy(gameObject);
                }
                else if (collider.CompareTag("EnemyBoss"))
                {
                    BossStatus bs = collider.GetComponent<BossStatus>();
                    bs.hit = true;
                    Destroy(gameObject);
                }
                else if (!collider.CompareTag("Player"))
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                if (collider.CompareTag("Player"))
                {
                    PlayerStatus pstats = collider.GetComponentInParent<PlayerStatus>();
                    pstats.health -= dmg;
                    pstats.stunnedByBoss = bossBullet;
                    Destroy(gameObject);
                }
                else if(!collider.CompareTag("Enemy") && !collider.CompareTag("EnemyArcher") && !collider.CompareTag("EnemyBoss"))
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
