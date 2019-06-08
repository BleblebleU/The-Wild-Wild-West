using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(EnemyStatus1))]
[RequireComponent(typeof(EnemyAnimationController))]
public class EnemyAttack2 : MonoBehaviour
{
    public float attackRange = 1.0f;

    public float attackDelayTime = 0.02f;

    public LayerMask attackMask;

    public bool isBoss = false;

    private static GameObject player;
    private EnemyAnimationController enemyAnimController;

    private EnemyStatus1 thisStatus;
    private CircleCollider2D playerDetectCol;
    private bool canAttackPlayer = false;

    private float toAttackTime = 0.0f;
    public float numAttacksInSec = 1;
    private float timeBtwAttack;
    private float currentTimeBtwAttack = 0;
    [HideInInspector] public bool isAttacking = false;
    private float nextTimeAttack = 0.0f;

    public GameObject bullet;
    public Transform Bullets;

    private void Awake()
    {
        thisStatus = GetComponent<EnemyStatus1>();
        playerDetectCol = GetComponent<CircleCollider2D>();
        enemyAnimController = GetComponent<EnemyAnimationController>();

        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if(bullet == null)
        {
            throw new System.Exception("BULLET PREFAB WAS NOT ADDED, CANNOT SHOOT");
        }

        Bullets = GameObject.FindGameObjectWithTag("BulletsParent").GetComponent<Transform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerDetectCol.radius = attackRange;
        timeBtwAttack = 1 / numAttacksInSec;
        //Debug.Log(enemyPos);
    }

    // Update is called once per frame
    void Update()
    {
        if (!thisStatus.dead)
        {
            if (!thisStatus.wipped)
            {
                if (canAttackPlayer)
                {
                    enemyAnimController.SetDirection((player.transform.position - transform.position).normalized);

                    if (Time.time >= nextTimeAttack)
                        AttackPlayer(player.transform.position, out bool attacked);
                }
            }
        }
        else
        {
            enemyAnimController.Walking(false);
            isAttacking = false;
            canAttackPlayer = false;
        }
    }

    private void LateUpdate()
    {
        if (thisStatus.wipped)
        {
            isAttacking = false;
            canAttackPlayer = false;
        }
    }

    private void AttackPlayer(Vector3 playerPos, out bool attacked)
    {
        attacked = false;
        if (toAttackTime > 0)
        {
            toAttackTime -= Time.deltaTime;
            isAttacking = true;
        }
        else
        {
            RaycastHit2D[] hitInfo = new RaycastHit2D[1];
            Vector3 attackDir = (playerPos - transform.position).normalized;
            int numHits = Physics2D.RaycastNonAlloc(transform.position, attackDir, hitInfo, attackDir.magnitude * attackRange, attackMask);
            //Debug.DrawRay(transform.position, attackDir * attackRange, Color.red);
            //Debug.Log(numHits);
            for (int i = 0; i < numHits; i++)
            {
                if (hitInfo[i].collider.CompareTag("Player"))
                {
                    Vector3 shootingDir = (hitInfo[i].transform.position - transform.position).normalized;
                    GameObject bulletClone = Instantiate(bullet, transform.position + shootingDir, Quaternion.identity, Bullets);
                    Bullet bulletData = bulletClone.GetComponent<Bullet>();
                    bulletData.enemyBullet = true;
                    bulletData.bossBullet = isBoss;
                    bulletData.fDir = new Vector2(shootingDir.x, shootingDir.y);
                    //Debug.Log("Attacked Player");
                }
            }
            toAttackTime = attackDelayTime;
            nextTimeAttack = Time.time + timeBtwAttack;
            isAttacking = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            canAttackPlayer = true;
            //Debug.Log("Player spotted.");
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            canAttackPlayer = false;
        }
    }
}
