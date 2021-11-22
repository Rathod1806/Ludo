using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
   public void SetRedHumanType (bool on)
    {
        //if (on) SaveSettings.Players[0] = "HUMAN";

        if (on)
        {
            SaveSettings.players[0].name = "Red Player"; // can take this name as an input from player
            SaveSettings.players[0].playerType = GameManager.Entity.PlayerType.HUMAN;
        }

    }
    public void SetRedCPUType(bool on)
    {
        //if (on) SaveSettings.Players[0] = "CPU";

        if (on)
        {
            SaveSettings.players[0].name = "Red Player"; // can take this name as an input from player
            SaveSettings.players[0].playerType = GameManager.Entity.PlayerType.CPU;
        }

    }

    public void SetGreenHumanType(bool on)
    {
        if (on)
        {
            SaveSettings.players[1].name = "Green Player"; // can take this name as an input from player
            SaveSettings.players[1].playerType = GameManager.Entity.PlayerType.HUMAN;
        }

    }
    public void SetGreenCPUType(bool on)
    {
        //if (on) SaveSettings.Players[1] = "CPU";

        if (on)
        {
            SaveSettings.players[1].name = "Green Player"; // can take this name as an input from player
            SaveSettings.players[1].playerType = GameManager.Entity.PlayerType.CPU;
        }

    }

    public void SetBlueHumanType(bool on)
    {
        //if (on) SaveSettings.Players[2] = "HUMAN";

        if (on)
        {
            SaveSettings.players[2].name = "Blue Player"; // can take this name as an input from player
            SaveSettings.players[2].playerType = GameManager.Entity.PlayerType.HUMAN;
        }

    }
    public void SetBlueCPUType(bool on)
    {
        // if (on) SaveSettings.Players[2] = "CPU";

        if (on)
        {
            SaveSettings.players[2].name = "Blue Player"; // can take this name as an input from player
            SaveSettings.players[2].playerType = GameManager.Entity.PlayerType.CPU;
        }

    }

    public void SetYellowHumanType(bool on)
    {
        //if (on) SaveSettings.Players[3] = "HUMAN";

        if (on)
        {
            SaveSettings.players[3].name = "Yellow Player"; // can take this name as an input from player
            SaveSettings.players[3].playerType = GameManager.Entity.PlayerType.HUMAN;
        }

    }
    public void SetYellowCPUType(bool on)
    {
        //if (on) SaveSettings.Players[3] = "CPU";

        if (on)
        {
            SaveSettings.players[3].name = "Yellow Player"; // can take this name as an input from player
            SaveSettings.players[3].playerType = GameManager.Entity.PlayerType.CPU;
        }

    }

}

public struct PlayerData
{
    public string name;
    public GameManager.Entity.PlayerType playerType;
}

public static class SaveSettings
{
    // Red Green Blue Yello
    //public static string[] Players = new string[4];
    public static PlayerData[] players = new PlayerData[4];
    public static PlayerData[] winners = new PlayerData[3]
    {
        new PlayerData { name = string.Empty, playerType = GameManager.Entity.PlayerType.CPU},
        new PlayerData { name = string.Empty, playerType = GameManager.Entity.PlayerType.CPU},
        new PlayerData { name = string.Empty, playerType = GameManager.Entity.PlayerType.CPU},
    }; 
    
}
