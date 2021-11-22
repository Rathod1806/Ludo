using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class Entity
    {
        public string playerName;
        public Pawn[] myPawns;
        public bool hasTurn;
        public enum PlayerType
        {
            HUMAN,
            CPU,
            NO_PLAYER
        }
        public PlayerType playerType;
        public bool hasWon;
    }

    public static GameManager instance;

    public List<Entity> playerList = new List<Entity>(); 

    /// State Machine    
    public enum States
    {
        WAITING,
        ROLL_DICE,
        SWITCH_PLAYER
    }
    public States state;
    
    public int activePlayer;
    bool switchingPlayer;
    bool turnPossible = true;

    public GameObject rollHumanDiceButton;
    int rolledHumanDiceNumber;

    public Dice dice;

    public InfoDisplay infoDisplay;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        for (int i = 0; i < playerList.Count; i++)
        {
            playerList[i].playerName = SaveSettings.players[i].name;
            playerList[i].playerType = SaveSettings.players[i].playerType;
        }
    }

    private void Start()
    {
        ActivateButton(false);

        int randomPlayer = Random.Range(0, playerList.Count);
        activePlayer = randomPlayer;
        infoDisplay.ShowMessage(playerList[activePlayer].playerName + " starts first");
    }

    private void Update()
    {
        if (playerList[activePlayer].playerType == Entity.PlayerType.CPU)
        {

            switch (state)
            {
                case States.ROLL_DICE:
                    if (turnPossible)
                    {
                        StartCoroutine(RollDiceDelay());
                        state = States.WAITING;
                    }
                    break;

                case States.SWITCH_PLAYER:
                    if (turnPossible)
                    {
                        StartCoroutine(SwitchPlayer());
                        state = States.WAITING;
                    }
                    break;

                case States.WAITING:

                    break;
            }
        }

        if (playerList[activePlayer].playerType == Entity.PlayerType.HUMAN)
        {

            switch (state)
            {
                case States.ROLL_DICE:
                    if (turnPossible)
                    {
                        //  Deactivate highlights
                        ActivateButton(true); 
                        state = States.WAITING;
                    }
                    break;

                case States.SWITCH_PLAYER:
                    if (turnPossible)
                    {
                        ActivateButton(false);
                        //Deactivate highlight

                        StartCoroutine(SwitchPlayer());
                        state = States.WAITING;
                    }
                    break;

                case States.WAITING:

                    break;
            }
        }
    }

    #region HUMAN_INPUT

    void ActivateButton (bool on)
    {
        rollHumanDiceButton.SetActive(on);
    }

    public void DeactivateAllSelectors()
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            for (int j = 0; j < playerList[i].myPawns.Length; j++)
            {
                playerList[i].myPawns[j].SetSelector(false);
            }
        }
    }

    public void HumanDiceRoll ()
    {
        ActivateButton(false);
        dice.RollDice();
    }

    public void HumanRollDice ()
    {
        //rolledHumanDiceNumber = Random.Range(1, 7);

        List<Pawn> moveablePawns = new List<Pawn>();

        bool startNodeFull = false;
        for (int i = 0; i < playerList[activePlayer].myPawns.Length; i++)
        {
            if (playerList[activePlayer].myPawns[i].currentNode == playerList[activePlayer].myPawns[i].startNode)
            {
                startNodeFull = true;
                break;
            }
        }

        // The dice number roll < 6
        if (rolledHumanDiceNumber < 6)
        {
            moveablePawns.AddRange(PossiblePawns());
        }

        // number = 6 !start node
        if (rolledHumanDiceNumber == 6 && !startNodeFull)
        {
            for (int i = 0; i < playerList[activePlayer].myPawns.Length; i++)
            {
                if (!playerList[activePlayer].myPawns[i].ReturnIsOut())
                {
                    moveablePawns.Add(playerList[activePlayer].myPawns[i]);
                }
            }
            moveablePawns.AddRange(PossiblePawns());
        }
        else if (rolledHumanDiceNumber == 6 && startNodeFull)
        {
            moveablePawns.AddRange(PossiblePawns());
        }

        // Activate all possible selectors
        if (moveablePawns.Count > 0)
        {
            for (int i = 0; i < moveablePawns.Count; i++)
            {
                moveablePawns[i].SetSelector(true);
            }
        }
        else
            state = States.SWITCH_PLAYER;
    }

    List<Pawn> PossiblePawns ()
    {
        List<Pawn> tempList = new List<Pawn>();

        for (int i = 0; i < playerList[activePlayer].myPawns.Length; i++)
        {
            if (playerList[activePlayer].myPawns[i].ReturnIsOut())
            {
                if (playerList[activePlayer].myPawns[i].CheckPossibleKick(playerList[activePlayer].myPawns[i].pawnID, rolledHumanDiceNumber))
                {
                    tempList.Add(playerList[activePlayer].myPawns[i]);
                    continue;
                }

                if (playerList[activePlayer].myPawns[i].CheckPossibleMove(rolledHumanDiceNumber))
                {
                    tempList.Add(playerList[activePlayer].myPawns[i]);
                }
            }
        }

        return tempList;
    }

    public int GetHumanDiceRollNumber ()
    {
        return rolledHumanDiceNumber;
    }

    #endregion HUMAN_INPUT

    #region CPU_AI

    void CPUDice()
    {
        dice.RollDice();
    }

    public void RollDice(int _diceNumber)
    {
        int diceNumber = _diceNumber; //Random.Range(1, 7);
        //diceNumber = 6;

        if (playerList[activePlayer].playerType == Entity.PlayerType.CPU)
        {
            if (diceNumber == 6)
                CheckStartNode(6);
            if (diceNumber < 6)
            {
                MoveAPawn(diceNumber);
            }         
        }

        if (playerList[activePlayer].playerType == Entity.PlayerType.HUMAN)
        {
            rolledHumanDiceNumber = _diceNumber;
            HumanRollDice();
        }

        Debug.Log("Dice roll number : " + diceNumber);
        infoDisplay.ShowMessage(playerList[activePlayer].playerName + " has rolled " + _diceNumber);
    }

    IEnumerator RollDiceDelay ()
    {
        yield return new WaitForSeconds(0.5f);
        //RollDice();
        CPUDice();
    }

    void CheckStartNode(int diceNumber)
    {
        bool startNodeFull = false;
        for (int i = 0; i < playerList[activePlayer].myPawns.Length; i++)
        {
            if (playerList[activePlayer].myPawns[i].currentNode == playerList[activePlayer].myPawns[i].startNode)
            {
                startNodeFull = true;
                break;
            }
        }

        if (startNodeFull)
        {
            // Move a Pawn
            MoveAPawn(diceNumber);
        }
        else
        {
            // Start node is empty..
            for (int j = 0; j < playerList[activePlayer].myPawns.Length; j++)
            {
                if (!playerList[activePlayer].myPawns[j].ReturnIsOut())
                {
                    playerList[activePlayer].myPawns[j].LeaveBase();
                    state = States.WAITING;
                    return;
                }
            }
            // move the pawn
                
        }
        
    }

    void MoveAPawn (int diceNumber)
    {
        List<Pawn> moveablePawns = new List<Pawn>();
        List<Pawn> moveKickPawns = new List<Pawn>();

        for (int i = 0; i < playerList[activePlayer].myPawns.Length; i++)
        {
            if (playerList[activePlayer].myPawns[i].ReturnIsOut())
            {
                if (playerList[activePlayer].myPawns[i].CheckPossibleKick(playerList[activePlayer].myPawns[i].pawnID, diceNumber))
                {
                    moveKickPawns.Add(playerList[activePlayer].myPawns[i]);
                    continue;
                }

                if (playerList[activePlayer].myPawns[i].CheckPossibleMove(diceNumber))                
                    moveablePawns.Add(playerList[activePlayer].myPawns[i]);                    
                
            }
        }

        if (moveKickPawns.Count > 0)
        {
            int number = Random.Range(0, moveKickPawns.Count);
            moveKickPawns[number].StartTheMove(diceNumber);
            state = States.WAITING;
            return;
        }

        if (moveablePawns.Count > 0)
        {
            int number = Random.Range(0, moveablePawns.Count);
            moveablePawns[number].StartTheMove(diceNumber);
            state = States.WAITING;
            return;
        }

        state = States.SWITCH_PLAYER;
    }

    IEnumerator SwitchPlayer ()
    {
        if (switchingPlayer)
            yield break;

        switchingPlayer = true;

        yield return new WaitForSeconds(0.5f);

        SetNextActivePlayer();

        switchingPlayer = false;
    }

    void SetNextActivePlayer ()
    {
        activePlayer++;
        activePlayer %= playerList.Count;

        int available = 0;
        for (int i = 0; i < playerList.Count; i++)
        {
            if (!playerList[i].hasWon)
                available++;
        }

        if (playerList[activePlayer].hasWon && available > 1)
        {
            SetNextActivePlayer();
            return;
        }
        else if (available < 2)
        {
            // Game Over screen
            SceneManager.LoadScene("GameOver");
            state = States.WAITING;
            return;
        }
        infoDisplay.ShowMessage(playerList[activePlayer].playerName + "'s turn");
        state = States.ROLL_DICE;
    }

    public void ReportTurnPossile (bool possible)
    {
        turnPossible = possible;
    }

    public void ReportWinning ()
    {
        // Show some UI 
        playerList[activePlayer].hasWon = true;
        for (int i = 0; i < SaveSettings.winners.Length; i++)
        {
            if (string.IsNullOrEmpty(SaveSettings.winners[i].name))
            {
                SaveSettings.winners[i].name = playerList[activePlayer].playerName;
                SaveSettings.winners[i].playerType = playerList[activePlayer].playerType;
                break;
            }
        }
    }

    #endregion CPU_AI

}
