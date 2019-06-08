using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonePickUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            collider.GetComponent<PlayerStatus>().bonesPickedUp++;
            Destroy(gameObject);
        }
    }
}
