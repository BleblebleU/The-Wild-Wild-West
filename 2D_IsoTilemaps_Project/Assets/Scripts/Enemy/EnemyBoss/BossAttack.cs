using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(BossStatus))]
[RequireComponent(typeof(EnemyAnimationController))]
public class BossAttack : MonoBehaviour
{
    public LayerMask attackMask;

    public GameObject bullet;
    public Transform Bullets;

    public float walkSpeed = 1.2f;
    public bool phaseChanged = false;

    public float screamActivateDist = 9.0f;
    public Vector2 dashActivateDist = new Vector2(7.0f, 9.0f);
    public Vector2 barageActivateDist = new Vector2(0.0f, 7.0f);

    public AudioSource ScreamPlayer;

    #region BULLET BARRAGE
    [Header("Bullet Barage Properties")]
    public float bulletSpeed = 7;
    public int bulletDamage = 1;
    public float barageForTime = 5.0f;
    public float gapBtwRounds = 0.45f;
    public Transform[] baragePositions;
    public float barageAfter = 6.0f;
    private float remainingBarageTime = 0.0f;
    private float remainingGapTime = 0.0f;
    private bool fireingBarage = false;
    private float nextBarageTime = 0.0f;
    #endregion

    #region SCREAM POWER
    [Header("Scream Properties")]
    public float screamDuration = 2.0f;
    public float screamAfter = 12.0f;
    public GameObject screamEnemy1;
    public GameObject screamEnemy2;
    public Transform screamEnemies;
    public Transform[] spawnPoints;
    private float remainingScraemDuration = 0.0f;
    /*[HideInInspector] */
    public bool screaming = false;
    private float nextScreamTime = 0.0f;
    public int wippedDuringScreamCount = 0;
    #endregion

    #region DASH BABY DASH
    [Header("Dash Settings")]
    public float dashSpeed = 4.0f;
    public bool dashing = false;
    public float dashAfter = 10.0f;
    private Vector3 dashDir = Vector3.zero;
    private CircleCollider2D circleCol;
    private float nextDashTime = 0.0f;
    #endregion

    #region Private Script ref
    private static GameObject player;
    private EnemyAnimationController enemyAnimController;
    private BossStatus bs;
    #endregion

    private float actualHealth = 0;
    private bool once = false;
    private int numAliveSpawns = 0;
    private Transform spawnPointsParent;

    public GameObject startCanvas;

    private void Awake()
    {
        bs = GetComponent<BossStatus>();
        enemyAnimController = GetComponent<EnemyAnimationController>();
        circleCol = GetComponent<CircleCollider2D>();
        bs = GetComponent<BossStatus>();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (bullet == null)
        {
            throw new System.Exception("BULLET PREFAB WAS NOT ADDED, CANNOT SHOOT");
        }

        Bullets = GameObject.FindGameObjectWithTag("BulletsParent").GetComponent<Transform>();
        screamEnemies = GameObject.FindGameObjectWithTag("ScreamEnemies").GetComponent<Transform>();
        spawnPointsParent = GameObject.FindGameObjectWithTag("ScreamEmeniesSpawn").GetComponent<Transform>();

        SetSpawnPoints();
    }

    // Start is called before the first frame update
    void Start()
    {
        remainingBarageTime = barageForTime;
        remainingScraemDuration = screamDuration;
        actualHealth = bs.health;
    }

    // Update is called once per frame
    void Update()
    {
        if (!startCanvas.activeSelf)
        {
            if (!bs.stunned && !bs.dead)
            {
                if (screaming)
                {
                    Scream();
                    if (fireingBarage)
                    {
                        ResetBarage();
                    }
                    if (dashing)
                    {
                        ResetDash();
                    }
                }
                if (phaseChanged && !once)
                {
                    //Debug.Log("Did this");
                    //phaseChanged = true;
                    ResetDash();
                    ResetScream();
                    ResetBarage();
                    Scream();
                    once = true;
                }

                if (!screaming)
                {
                    float distToP = DistToPlayer();
                    //Debug.Log("Dist to Player from BOSS:" + distToP);
                    if (distToP >= screamActivateDist && CanScream())
                        Scream();

                    if (!screaming)
                    {
                        Vector3 dir = (player.transform.position - transform.position).normalized;

                        if (CanDash())
                        {
                            if (distToP >= dashActivateDist.x && distToP < dashActivateDist.y)
                                Dash(dir);
                            else
                                ResetDash();
                        }
                        else if (CanBarage() && distToP >= barageActivateDist.x && distToP < barageActivateDist.y)
                        {
                            BulletBarrage();
                        }
                        else if (!fireingBarage && !dashing)
                        {
                            transform.Translate(dir * walkSpeed * Time.deltaTime);
                        }
                    }
                }
            }

        }
    }

    private float DistToPlayer()
    {
        return Mathf.Abs((transform.position - player.transform.position).magnitude);
    }

    private bool CanDash()
    {
        return (dashing && !screaming && !fireingBarage) || nextDashTime <= Time.time;
    }

    private void Dash(Vector3 dir)
    {
        transform.Translate(dir * dashSpeed * Time.deltaTime);
        //Debug.Log("Dashing");
        dashing = true;
    }

    private void ResetDash()
    {
        dashing = false;
        nextDashTime = Time.time + dashAfter;
    }

    private bool CanScream()
    {
        return (screaming && !dashing && !fireingBarage) || nextScreamTime <= Time.time;
    }

    private void Scream()
    {
        if (remainingScraemDuration > 0)
        {
            if (!ScreamPlayer.isPlaying)
            {
                ScreamPlayer.Play();
            }
            remainingScraemDuration -= Time.deltaTime;
            screaming = true;
            //Debug.Log("Screaming");
        }
        else
        {
            SpawnCreatureOnScream();
            ResetScream();
        }
    }

    private void SpawnCreatureOnScream()
    {
        int numSpawns = (phaseChanged) ? 6 : 3;
        for (int i = 0; i < numSpawns; i++)
        {
            if (screamEnemies.childCount >= 8)
                break;
            bool changed = false;
            GameObject enemyToSpawn = screamEnemy1;
            int randIndex = Random.Range(0, 10);
            if (randIndex > 7)
            {
                enemyToSpawn = screamEnemy2;
                changed = true;
            }

            GameObject enemyClone = Instantiate(enemyToSpawn, spawnPoints[i].position, Quaternion.identity, screamEnemies);
            if (!changed)
            {
                enemyClone.GetComponent<EnemyStatus>().spawnedByBoss = true;
            }
            else
            {
                enemyClone.GetComponent<EnemyStatus1>().spawnedByBoss = true;
            }

        }
    }

    private void ResetScream()
    {
        nextScreamTime = Time.time + screamAfter;
        screaming = false;
        remainingScraemDuration = screamDuration;
        wippedDuringScreamCount = 0;
        //ScreamPlayer.Stop();
    }

    private bool CanBarage()
    {
        return (fireingBarage && !dashing && !screaming) || nextBarageTime <= Time.time;
    }

    private void BulletBarrage()
    {
        if (remainingBarageTime > 0)
        {
            fireingBarage = true;
            if (remainingGapTime <= 0)
            {
                FireBarage();
                remainingGapTime = gapBtwRounds;
            }
            else
            {
                remainingGapTime -= Time.deltaTime;
            }

            remainingBarageTime -= Time.deltaTime;
            //Debug.Log("Fireing");
        }
        else
        {
            ResetBarage();
        }
    }

    private void ResetBarage()
    {
        fireingBarage = false;
        remainingBarageTime = barageForTime;
        remainingGapTime = gapBtwRounds;
        nextBarageTime = Time.time + barageAfter;
    }

    private void FireBarage()
    {
        int numActivate = phaseChanged ? 4 : 2;
        for (int i = 0; i < numActivate; i++)
        {
            GameObject bulletClone = Instantiate(bullet, baragePositions[i].position, Quaternion.identity, Bullets);
            Vector3 bulDir = (i < 3) ? (player.transform.position - transform.position).normalized : (player.transform.position - baragePositions[i].position).normalized;
            Bullet bul = bulletClone.GetComponent<Bullet>();
            bul.enemyBullet = true;
            bul.speed = bulletSpeed;
            bul.fDir = bulDir;
        }
    }

    private void SetSpawnPoints()
    {
        if(spawnPointsParent != null)
        {
            int numChildren = spawnPointsParent.childCount;
            spawnPoints = new Transform[numChildren];
            for (int i = 0; i < numChildren; i++)
            {
                spawnPoints[i] = spawnPointsParent.GetChild(i);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("BossColMap") || collision.collider.CompareTag("Player"))
        {
            dashing = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BossColMap"))
        {
            dashing = false;
        }
    }
}
