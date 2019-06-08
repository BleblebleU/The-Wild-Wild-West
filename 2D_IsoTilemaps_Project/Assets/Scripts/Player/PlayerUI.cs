using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerStatus))]
public class PlayerUI : MonoBehaviour
{
    public Image playerHealthImg;

    public Sprite[] healthBars;
    private PlayerStatus playerStatus;
    private int oldHealth = 5;
    private void Awake()
    {
        playerHealthImg = GameObject.FindGameObjectWithTag("PlayerHealthUI").GetComponent<Image>();
        playerStatus = GetComponent<PlayerStatus>();
        oldHealth = playerStatus.health;
        playerHealthImg.sprite = healthBars[oldHealth];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!playerStatus.dead && oldHealth != playerStatus.health)
        {
            playerHealthImg.sprite = healthBars[playerStatus.health];
            oldHealth = playerStatus.health;
        }
    }
}
