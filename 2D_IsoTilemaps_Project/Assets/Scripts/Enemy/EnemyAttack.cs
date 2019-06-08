using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyStatus))]
public class EnemyAttack : MonoBehaviour
{
    public float normalWalkSpeed = 1.0f; 
    public float attackWalkSpeed = 2.5f; 

    public float chaseRange = 5.0f;
    public float attackRange = 3.0f;

    private EnemyStatus thisStatus;
    private CircleCollider2D playerDetectCol;

    private bool canChasePlayer = false;
    private bool canAttackPlayer = false;
    private Vector3 playerPosition = Vector3.zero;


    private void Awake()
    {
        thisStatus = GetComponent<EnemyStatus>();
        playerDetectCol = GetComponent<CircleCollider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerDetectCol.radius = chaseRange;
    }

    // Update is called once per frame
    void Update()
    {
        if (!thisStatus.dead && !thisStatus.wipped)
        {
            if (canChasePlayer)
            {
                Vector3 dispToPlayer = playerPosition - transform.position;
                canAttackPlayer = Mathf.Abs(dispToPlayer.magnitude) <= attackRange;
                if (!canAttackPlayer)
                {
                    Vector3 dir = dispToPlayer.normalized;
                    transform.Translate(dir * attackWalkSpeed * Time.deltaTime);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            Debug.Log("Player spotted.");
            canChasePlayer = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerPosition = collider.transform.position;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            canChasePlayer = false;
        }
    }
}
