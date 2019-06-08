using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartMenuButtons : MonoBehaviour
{
    private TextMeshProUGUI buttonGUI;
    private string original;

    private void Awake()
    {
        buttonGUI = GetComponent<TextMeshProUGUI>();
    }

    // Start is called before the first frame update
    void Start()
    {
        original = buttonGUI.text;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ChangeHoverText()
    {
        Debug.Log("Entered");
        buttonGUI.SetText("(" + buttonGUI.text + ")");
    }

    public void ResetHoverText()
    {
        Debug.Log("Exit");
        buttonGUI.SetText(original);
    }
}
