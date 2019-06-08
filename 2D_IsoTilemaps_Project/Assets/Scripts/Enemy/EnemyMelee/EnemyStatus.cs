using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(EnemyAttack1))]
public class EnemyStatus : MonoBehaviour
{
    public bool wipped = false;
    public float health = 5;
    public bool dead = false;

    public float wipStunDur = 0.45f;
    public float wipAttackStunDur = 1.4f;
    private float wipStunRemainDur = 0.7f;

    private EnemyAttack1 enemyAttack;

    public GameObject enemyHealthUIObj;
    public RectTransform enemyHealthUITrans;
    public Slider enemyHealthUI;
    public RectTransform canvas;

    public float uiOfset = 0.1f;
    public bool hit = false;

    private const float totalHealth = 5;
    [HideInInspector] public Vector3 knockBackDir = Vector3.zero;
    public float knockBackTime = 0.1f;
    private float remainingKnockBackTime = 0.0f;
    public float knockBackSpeed = 20.0f;

    public bool spawnedByBoss = false;

    public static Transform drops;
    public GameObject heartDrop;
    public int dropThreshold = 8;

    private bool once = false;
    public GameObject stars;

    private void Awake()
    {
        enemyAttack = GetComponent<EnemyAttack1>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //SetHealthUIPos();
        //enemyHealthUIObj.SetActive(true);
        remainingKnockBackTime = knockBackTime;
        drops = GameObject.FindGameObjectWithTag("drops").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        dead = (health <= 0);

        if (!once && !spawnedByBoss)
        {
            //enemyHealthUIObj = GameObject.FindGameObjectWithTag(enemyHealthUiObjTag);
            canvas = GameObject.FindGameObjectWithTag("GameUICanvas").GetComponent<RectTransform>();
            enemyHealthUITrans = enemyHealthUIObj.GetComponent<RectTransform>();
            enemyHealthUI = enemyHealthUIObj.GetComponent<Slider>();
            //Debug.Log("Assigned stuff to status");
            once = true;
        }

        if (!spawnedByBoss)
        {
            FillHealthBar();
        }
        if (!dead)
        {
            if (!spawnedByBoss)
            {
                enemyHealthUIObj.SetActive(true);
                SetHealthUIPos();
            }
            bool enemyWasAttacking = enemyAttack.isAttacking;
            if (enemyWasAttacking)
            {
                wipStunRemainDur = wipAttackStunDur;
            }

            if (wipStunRemainDur <= 0)
            {
                wipStunRemainDur = wipStunDur;
                wipped = false;

                stars.SetActive(false);
            }
            if (wipped)
            {
                //Debug.Log("Wipped for: " + wipStunRemainDur);
                wipStunRemainDur -= Time.deltaTime;

                stars.SetActive(true);
            }
        }
        else
        {
            EnemyDie();
        }
        CheckDoKnockBack();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Bullet"))
        {
            health--;
            if(health <= 0)
            {
                dead = true;
            }
        }
    }

    private void CheckDoKnockBack()
    {
        if (hit)
        {
            if(remainingKnockBackTime >= 0)
            {
                transform.Translate(knockBackDir * knockBackSpeed * Time.deltaTime);
                remainingKnockBackTime -= Time.deltaTime;
            }
            else
            {
                remainingKnockBackTime = knockBackTime;
                hit = false;
                knockBackDir = Vector3.zero;
            }
        }
    }

    private void SetHealthUIPos()
    {
        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(transform.position);
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * canvas.sizeDelta.x) - (canvas.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * canvas.sizeDelta.y) - (canvas.sizeDelta.y * 0.5f)) + uiOfset);
        //now you can set the position of the ui element
        enemyHealthUITrans.anchoredPosition = WorldObject_ScreenPosition;
    }

    private void FillHealthBar()
    {
        enemyHealthUI.value = health / totalHealth;
        //Debug.Log(health / totalHealth);
    }

    private void EnemyDie()
    {
        if (!spawnedByBoss)
            enemyHealthUIObj.SetActive(false);
        int randChance = Random.Range(0, 10);
        if (randChance >= 8)
        {
            GameObject heartDropClone = Instantiate(heartDrop, transform.position, Quaternion.identity, drops);
        }
        Destroy(gameObject);
    }
}
