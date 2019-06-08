using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerStatus))]
public class PlayerAttack : MonoBehaviour
{
    public float wipAttackRange = 5.0f;
    public float wipStunDur = 0.15f;
    public float remainWipStun = 0.15f;
    public float enemyStunDuration = 0.3f;

    public Vector2 wippingDirection = Vector2.zero;
    public LayerMask wipMask;

    public GameObject bullet;
    public Transform Bullets;
    public float shootGap = 0.5f;
    private float nextShootTime = 0.0f;
    public float bulletSpeed = 13.0f;

    public bool isWipping = false;
    private const int maxHitEnemies = 5;

    private PlayerStatus pStatus;

    private LineRenderer lr;
    Vector3 mousePosCpy = Vector3.zero;

    public AudioSource wipSource;

    private void Awake()
    {
        pStatus = GetComponent<PlayerStatus>();
        Bullets = GameObject.FindGameObjectWithTag("BulletsParent").GetComponent<Transform>();
        lr = GetComponent<LineRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!pStatus.isStunned)
        {
            if (!isWipping)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos = mousePos - transform.position;
                mousePos = mousePos.normalized;

                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    wippingDirection = mousePos;
                    ActionWippedEnemies(wippingDirection);
                    isWipping = true;
                    mousePosCpy = mousePos;
                    wipSource.Play();
                    //Debug.Log(wippingDirection);
                    //Debug.DrawRay(transform.position, mousePos * wipAttackRange, Color.magenta, wipStunDur);
                }
                else if (Input.GetKeyDown(KeyCode.Mouse0) && (Time.time >= nextShootTime))
                {
                    nextShootTime = Time.time + shootGap;
                    Vector3 shootingDir = mousePos;
                    GameObject bulletClone = Instantiate(bullet, transform.position + mousePos, Quaternion.identity, Bullets);
                    Bullet bul = bulletClone.GetComponent<Bullet>();
                    bul.speed = bulletSpeed;
                    bul.fDir = new Vector2(mousePos.x, mousePos.y);
                }
            }
            else if (remainWipStun <= 0)
            {
                isWipping = false;
                remainWipStun = wipStunDur;
                wippingDirection = Vector2.zero;
                pStatus.attacking = false;
                lr.SetPosition(0, transform.position);
                lr.SetPosition(1, transform.position);
                wipSource.Stop();
            }
            else if (isWipping)
            {
                remainWipStun -= Time.deltaTime;
                pStatus.attacking = true;
                lr.SetPosition(0, transform.position);
                lr.SetPosition(1, transform.position + mousePosCpy * wipAttackRange);
            }
        }
    }

    private void ActionWippedEnemies(Vector3 wipDir)
    {
        RaycastHit2D[] hitInfo = new RaycastHit2D[16];

        int numHits = Physics2D.RaycastNonAlloc(transform.position, wipDir, hitInfo, wipAttackRange, wipMask);
        //Debug.Log(numHits);
        for (int i = 0; i < numHits; i++)
        {
            if (hitInfo[i].collider.CompareTag("Enemy"))
            {
                EnemyStatus thisEnemyStatus = hitInfo[i].collider.GetComponent<EnemyStatus>();
                thisEnemyStatus.wipped = true;
                //Debug.Log("Wipped Enemy");
            }
            else if (hitInfo[i].collider.CompareTag("EnemyArcher"))
            {
                EnemyStatus1 thisEnemyStatus = hitInfo[i].collider.GetComponent<EnemyStatus1>();
                thisEnemyStatus.wipped = true;
            }
        }
    }
}
