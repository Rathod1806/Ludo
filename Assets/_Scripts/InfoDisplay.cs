using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoDisplay : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI infoDisplay;

    private void Start()
    {
        infoDisplay.text = "";
    }

    public void ShowMessage (string text)
    {
        infoDisplay.text = text;
    }
}
