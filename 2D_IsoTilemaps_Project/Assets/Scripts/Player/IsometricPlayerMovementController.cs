using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerAttack))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerStatus))]
public class IsometricPlayerMovementController : MonoBehaviour
{

    public float walkSpeed = 1.0f;
    public float runSpeed = 3.0f;

    private float movementSpeed = 1f;
    private float horizontalInput = 0.0f;
    private float verticalInput = 0.0f;

    IsometricCharacterRenderer isoRenderer;
    PlayerAttack playerAttackS;
    PlayerStatus playerStatus;
    Rigidbody2D rbody;

    private void Awake()
    {
        rbody = GetComponent<Rigidbody2D>();
        playerAttackS = GetComponent<PlayerAttack>();
        playerStatus = GetComponent<PlayerStatus>();
        isoRenderer = GetComponentInChildren<IsometricCharacterRenderer>();
    }

    private void Update()
    {
        if (!playerStatus.dead && !playerAttackS.isWipping && !playerStatus.isStunned)
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");

            if (Input.GetKey(KeyCode.LeftShift))
            {
                movementSpeed = runSpeed;
            }
            else
            {
                movementSpeed = walkSpeed;
            }
        }
        else
        {
            horizontalInput = 0;
            verticalInput = 0;
            isoRenderer.SetDirection(Vector2.zero);
            movementSpeed = 0.0f;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 currentPos = rbody.position;

        Vector2 inputVector = new Vector2(horizontalInput, verticalInput);
        inputVector = inputVector.normalized;

        Vector2 velocity = inputVector * movementSpeed;
        Vector2 newPos = currentPos + velocity * Time.fixedDeltaTime;

        if (!playerAttackS.isWipping && !playerStatus.isStunned)
        {
            isoRenderer.SetDirection(velocity);
        }
        else if(playerAttackS.isWipping)
        {
            isoRenderer.SetDirection(playerAttackS.wippingDirection);
        }
        rbody.MovePosition(newPos);
    }
}
