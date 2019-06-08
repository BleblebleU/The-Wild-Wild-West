using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[RequireComponent(typeof(EnemyAttack2))]
public class EnemyStatus1 : MonoBehaviour
{
    public bool wipped = false;
    public float health = 3;
    public bool dead = false;
    private const float totalHealth = 3;

    public float wipStunDur = 0.45f;
    public float wipAttackStunDur = 1.4f;
    private float wipStunRemainDur = 0.7f;

    private EnemyAttack2 enemyAttack;

    public GameObject enemyHealthUIObj;
    public RectTransform enemyHealthUITrans;
    public Slider enemyHealthUI;
    public RectTransform canvas;

    public float uiOfset = 0.1f;
    public bool hit = false;

    [HideInInspector] public Vector3 knockBackDir = Vector3.zero;
    public float knockBackTime = 0.1f;
    private float remainingKnockBackTime = 0.0f;
    public float knockBackSpeed = 2.0f;

    public bool spawnedByBoss = false;
    private bool once = false;

    public GameObject bonesDrop;
    public static Transform drops;
    public GameObject stars;

    private void Awake()
    {
        enemyAttack = GetComponent<EnemyAttack2>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //SetHealthUIPos();
        //enemyHealthUIObj.SetActive(true);
        remainingKnockBackTime = knockBackTime;
        drops = GameObject.FindGameObjectWithTag("drops").GetComponent<Transform>();
        health = totalHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (!once && !spawnedByBoss)
        {
            //enemyHealthUIObj = GameObject.FindGameObjectWithTag(enemyHealthUiObjTag);
            canvas = GameObject.FindGameObjectWithTag("GameUICanvas").GetComponent<RectTransform>();
            enemyHealthUITrans = enemyHealthUIObj.GetComponent<RectTransform>();
            enemyHealthUI = enemyHealthUIObj.GetComponent<Slider>();
            //Debug.Log("assigned stuff");
            once = true;
        }
        dead = (health <= 0);
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
        //Debug.Log("THIS ONE: " + health / totalHealth);
    }

    private void EnemyDie()
    {
        if(!spawnedByBoss)
            enemyHealthUIObj.SetActive(false);
        GameObject bonesDropClone = Instantiate(bonesDrop, transform.position, Quaternion.identity, drops);
        Destroy(gameObject);
    }
}
