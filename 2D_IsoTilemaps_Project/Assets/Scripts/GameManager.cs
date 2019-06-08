using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject canvasStart;

    public BossStatus bs;
    public PlayerStatus ps;

    public GameObject winCanvas;
    public GameObject looseCanvas;
    public GameObject pauseCanvas;

    public AudioSource winMusic;
    public AudioSource looseMusic;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canvasStart.activeSelf)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }

        if (bs.dead)
        {
            winCanvas.SetActive(true);
            if(!winMusic.isPlaying)
                winMusic.Play();
        }
        else if (ps.dead)
        {
            looseCanvas.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseCanvas.SetActive(!pauseCanvas.activeSelf);
            Time.timeScale = reverseTimeScale();
        }
    }

    private int reverseTimeScale()
    {
        if(Time.timeScale == 0)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}
