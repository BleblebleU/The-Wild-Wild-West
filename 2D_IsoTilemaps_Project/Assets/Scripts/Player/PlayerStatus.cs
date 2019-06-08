using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public int health = 5;

    public int bonesPickedUp = 0;

    public bool dead = false;
    public bool isStunned = false;

    public bool attacking = false;

    public float normalStunTime = 0.3f;
    public float bossStunTime = 1.3f;

    [SerializeField]
    private float stunRemaining = 0.0f;
    [HideInInspector]public bool stunnedByBoss = false;
    public bool healthPickedup = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (healthPickedup)
        {
            if (health < 5)
                health++;
            healthPickedup = false;
        }
        if(health <= 0)
        {
            dead = true;
        }
        if (!dead)
        {
            if (isStunned && stunRemaining <= 0.0f)
            {
                stunRemaining = (stunnedByBoss ? bossStunTime : normalStunTime) + Time.deltaTime;
            }
            else if(stunRemaining > 0.0f)
            {
                stunRemaining -= Time.deltaTime;
                if (stunRemaining <= 0.0f)
                {
                    isStunned = false;
                    stunnedByBoss = false;
                }
            }
        }
    }


}
