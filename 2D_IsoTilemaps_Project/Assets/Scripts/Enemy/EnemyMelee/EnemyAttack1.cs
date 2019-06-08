using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum AttackType
{
    Melee,
    DirectProjectile,
    Club,
    LongAxe,
}

[RequireComponent(typeof(EnemyStatus))]
[RequireComponent(typeof(EnemyAnimationController))]
public class EnemyAttack1 : MonoBehaviour
{
    public float normalWalkSpeed = 1.0f;
    public float attackWalkSpeed = 2.5f;

    public float chaseRange = 5.0f;
    public float attackRange = 1.0f;

    public AttackType attackType;
    public float attackDelayTime = 0.02f;

    public LayerMask attackMask;

    public bool isBoss = false;

    private static GameObject player;
    private static EnemyPathfindManager mapMaster;
    private EnemyAnimationController enemyAnimController;

    private EnemyStatus thisStatus;
    private CircleCollider2D playerDetectCol;

    private bool canChasePlayer = false;
    private bool canAttackPlayer = false;

    public Vector3[] path;
    private Vector3 lastFollowedPos;
    private int lastIndex;
    private bool searchNewPath = false;
    public Vector3 velocity = Vector3.zero;

    private float toAttackTime = 0.0f;
    public float numAttacksInSec = 1;
    private float timeBtwAttack;
    private float currentTimeBtwAttack = 0;
    [HideInInspector] public bool isAttacking = false;

    private Vector3 oldPlayerPos = Vector3.zero;
    private bool oldCanAttack = false;
    private Vector3 oldVel = Vector3.zero;

    private float nextCheckT = 0.0f;
    private Vector3 oldCheckPos;

    private void Awake()
    {
        thisStatus = GetComponent<EnemyStatus>();
        playerDetectCol = GetComponent<CircleCollider2D>();
        enemyAnimController = GetComponent<EnemyAnimationController>();

        if (mapMaster == null)
        {
            mapMaster = GameObject.FindGameObjectWithTag("EnemyColMap").GetComponent<EnemyPathfindManager>();
        }
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerDetectCol.radius = chaseRange;
        lastFollowedPos = transform.position;
        lastIndex = 0;
        timeBtwAttack = 1 / numAttacksInSec;
        oldCheckPos = transform.position;
        //Debug.Log(enemyPos);
    }

    // Update is called once per frame
    void Update()
    {
        if (!thisStatus.dead)
        {
            if (!thisStatus.wipped)
            {
                canAttackPlayer = Mathf.Abs((player.transform.position - transform.position).magnitude) <= attackRange;
                if (!canAttackPlayer && oldCanAttack)
                {
                    canChasePlayer = true;
                    searchNewPath = true;
                }
                if (canChasePlayer && !canAttackPlayer)
                {
                    ChasePlayer();
                    if (nextCheckT < Time.time)
                    {
                        nextCheckT += 0.5f;
                        if (Mathf.Abs(Mathf.Abs(oldCheckPos.x) - Mathf.Abs(transform.position.x)) <= 0.15f
                            || Mathf.Abs(Mathf.Abs(oldCheckPos.y) - Mathf.Abs(transform.position.y)) <= 0.21f)
                        {
                            //Debug.Log("Stuck");
                            enemyAnimController.Walking(false);
                            velocity = oldVel;
                            searchNewPath = true;
                        }
                        else
                        {
                            oldCheckPos = transform.position;
                            enemyAnimController.Walking(true);
                            Vector3 dir = (player.transform.position - transform.position).normalized;
                            enemyAnimController.SetDirection(velocity);
                            oldVel = velocity;
                        }
                    }
                }
                else if (canAttackPlayer)
                {
                    enemyAnimController.Walking(false);
                    Vector3 dir = (player.transform.position - transform.position).normalized;
                    enemyAnimController.SetDirection(dir);
                    currentTimeBtwAttack += Time.deltaTime;
                    if (currentTimeBtwAttack >= timeBtwAttack)
                    {
                        AttackPlayer(player.transform.position, out bool attacked);
                        currentTimeBtwAttack = attacked ? 0.0f : (currentTimeBtwAttack - Time.deltaTime);
                    }
                }
            }

            oldPlayerPos = player.transform.position;
            oldCanAttack = canAttackPlayer;
        }
        else
        {
            enemyAnimController.Walking(false);
            isAttacking = false;
            canAttackPlayer = false;
            canChasePlayer = false;
        }
    }

    private void LateUpdate()
    {
        if (thisStatus.wipped)
        {
            isAttacking = false;
            canAttackPlayer = false;
            canChasePlayer = false;
        }
    }

    private bool PlayerPosChanged()
    {
        return oldPlayerPos != player.transform.position;
    }

    private void ChasePlayer()
    {
        bool reachedLast = false;
        int fIndex = lastIndex;
        if (ReachedLastPoint() || searchNewPath)
        {
            searchNewPath = false;
            path = mapMaster.GetPath(transform.position, player.transform.position);
            fIndex = IndexToFollow(path, out reachedLast);
            if (reachedLast || fIndex >= path.Length)
            {
                searchNewPath = true;
            }
            else
            {
                lastFollowedPos = path[fIndex];
            }
        }
        else
        {
            if (!reachedLast)
            {
                //Debug.Log("moving to:" + fIndex);
                if (fIndex < path.Length)
                {
                    Vector3 dir = (path[fIndex] - transform.position).normalized;
                    velocity = dir * attackWalkSpeed;
                    transform.Translate(velocity * Time.deltaTime);
                }
                else
                {
                    searchNewPath = true;
                }
            }
            else
            {
                Vector3 dir = (player.transform.position - transform.position).normalized;
                velocity = dir * attackWalkSpeed;
                transform.Translate(velocity * Time.deltaTime);
                searchNewPath = true;
            }
        }
    }

    private void AttackPlayer(Vector3 playerPos, out bool attacked)
    {
        attacked = false;
        if (attackType == AttackType.Melee || attackType == AttackType.Club)
        {
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
                        PlayerStatus playerStatus = hitInfo[i].collider.GetComponentInParent<PlayerStatus>();
                        playerStatus.health--;
                        playerStatus.stunnedByBoss = isBoss;
                        playerStatus.isStunned = true;
                        attacked = true;
                        //Debug.Log("Attacked Player");
                    }
                }
                toAttackTime = attackDelayTime;
                isAttacking = false;
            }
        }
    }

    private bool ReachedLastPoint()
    {
        return (Mathf.Abs((lastFollowedPos - transform.position).sqrMagnitude) <= 0.225f);
    }

    private int IndexToFollow(Vector3[] path, out bool reachedLast)
    {
        if (lastIndex < path.Length && path[lastIndex] == lastFollowedPos)
        {
            if (lastIndex + 1 < path.Length)
            {
                reachedLast = false;
                lastIndex++;
                return lastIndex;
            }
            reachedLast = true;
            return lastIndex;
        }
        else
        {
            for (int i = 0; i < path.Length; i++)
            {
                if (path[i] == lastFollowedPos)
                {
                    if (i + 1 < path.Length)
                    {
                        reachedLast = false;
                        lastIndex = i + 1;
                        return lastIndex;
                    }
                    reachedLast = true;
                    return i;
                }
            }
            reachedLast = false;
            return 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            //Debug.Log("Player spotted.");
            canChasePlayer = true;
            searchNewPath = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (Mathf.Abs((collider.transform.position - transform.position).magnitude) > attackRange)
            {
                canChasePlayer = true;
                canAttackPlayer = false;
            }
            else
            {
                canAttackPlayer = true;
                canChasePlayer = true;
            }
            //playerPosition = collider.transform.position;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            canChasePlayer = false;
            searchNewPath = false;
        }
    }
}
