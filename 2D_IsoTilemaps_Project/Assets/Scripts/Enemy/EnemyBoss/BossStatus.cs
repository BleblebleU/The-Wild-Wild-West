using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[RequireComponent(typeof(EnemyAttack2))]
public class BossStatus : MonoBehaviour
{
    public bool wipped = false;
    public float health = 50.0f;
    public bool dead = false;
    public bool stunned = false;

    public int numWipsToStun = 3;
    public float wipStunDur = 2.0f;
    private float wipStunRemainDur = 0.7f;

    private BossAttack bossAttack;

    public GameObject enemyHealthUIObj;
    public RectTransform enemyHealthUITrans;
    public Slider enemyHealthUI;
    public RectTransform canvas;

    public float uiOfset = 140f;
    public bool hit = false;

    private EnemyAnimationController enemyAnimCtrl;

    private const float totalHealth = 50;

    private void Awake()
    {
        canvas = GameObject.FindGameObjectWithTag("GameUICanvas").GetComponent<RectTransform>();
        bossAttack = GetComponent<BossAttack>();
        enemyHealthUITrans = enemyHealthUIObj.GetComponent<RectTransform>();
        enemyHealthUI = enemyHealthUIObj.GetComponent<Slider>();
        enemyAnimCtrl = GetComponent<EnemyAnimationController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        health = totalHealth;
        SetHealthUIPos();
        enemyHealthUIObj.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(hit && !bossAttack.screaming)
        {
            health--;
            hit = false;
        }
        FillHealthBar();
        dead = (health <= 0);

        if(health == totalHealth / 2 && !bossAttack.phaseChanged)
        {
            bossAttack.phaseChanged = true;
        }

        if (!dead)
        {
            enemyAnimCtrl.SetDead(false);
            enemyHealthUIObj.SetActive(true);
            SetHealthUIPos();

            if (wipStunRemainDur <= 0)
            {
                wipStunRemainDur = wipStunDur;
                stunned = false;
                wipped = false;
            }
            else if (stunned)
            {
                //Debug.Log("Wipped for: " + wipStunRemainDur);
                wipStunRemainDur -= Time.deltaTime;
            }

            else if (bossAttack.screaming && wipped)
            {
                bossAttack.wippedDuringScreamCount++;

                if (bossAttack.wippedDuringScreamCount >= numWipsToStun)
                {
                    stunned = true;
                }
            }
        }
        else
        {
            enemyHealthUIObj.SetActive(false);
            enemyAnimCtrl.SetDead(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Bullet"))
        {
            health--;
            if(health <= 0)
            {
                dead = true;
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
    }
}
