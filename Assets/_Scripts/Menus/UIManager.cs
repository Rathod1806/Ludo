using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI first;
    [SerializeField] TextMeshProUGUI second;
    [SerializeField] TextMeshProUGUI third;

    private void Start()
    {
        for (int i = 0; i < SaveSettings.players.Length; i++)
        {
            SaveSettings.players[i].name = i == 0 ? "Red Player" : 
                                            i == 1 ? "Green Player" :
                                            i == 2 ? "Blue Player" : "Yellow Player";
            SaveSettings.players[i].playerType = GameManager.Entity.PlayerType.CPU;
        }

        if (first)
            first.text = "1st: " + SaveSettings.winners[0].name;
        if (second)
            second.text = "2nd: " + SaveSettings.winners[1].name;
        if (third)
            third.text = "3rd: " + SaveSettings.winners[2].name;

    }

    public void OnClickBackButton (string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void StartTheGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
