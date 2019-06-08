using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButtonFuntion : MonoBehaviour
{
    public GameObject thisCanvas;

    public void StartButtonPressed()
    {
        thisCanvas.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
