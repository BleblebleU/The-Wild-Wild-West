using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyAnimationController : MonoBehaviour
{
    private Animator enemyAnimator;
    
    private void Awake()
    {
        enemyAnimator = GetComponent<Animator>();
    }

    public void SetDead(bool dead)
    {
        enemyAnimator.SetBool("dead", dead);
    }

    public void SetDirection(Vector3 curVelocity)
    {
        Vector2 dir = new Vector2(Mathf.Sign(curVelocity.x), Mathf.Sign(curVelocity.y));
        enemyAnimator.SetFloat("X", dir.x);
        enemyAnimator.SetFloat("Y", dir.y);
    } 

    public void Walking(bool walking)
    {
        enemyAnimator.SetBool("Walking", walking);
    }

    public bool isWalking()
    {
        return enemyAnimator.GetBool("Walking");
    }

    private float IntOfFloat(float f)
    {
        return (f == 0) ? Mathf.Sign(f) : 0;
    }

}
