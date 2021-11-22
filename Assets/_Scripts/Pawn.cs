using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour
{
    [Header("Routes")]
    public Route commonRoute;
    public Route finalRoute;
    
    public List<Node> fullRoute = new List<Node>();

    [Header("Nodes")]
    public Node startNode;
    public Node baseNode;
    public Node currentNode;
    public Node goalNode;

    public int pawnID;

    int routePosition;
    int startNodeIndex;
    int steps;
    int doneSteps = 0;

    [Header("Bools")]
    public bool isOut;
    bool isMoving;
    bool hasTurn;

    [Header("Selector")]
    public GameObject selector;

    // Arc movement
    float amplitude = 0.25f;
    float cTime = 0f;

    private void Start()
    {
        startNodeIndex = commonRoute.RequestPosition(startNode.gameObject.transform);
        CreateFullRoute();

        SetSelector(false);
    }

    private void CreateFullRoute ()
    {
        for (int i = 0; i < commonRoute.childNodeList.Count; i++)
        {
            int tempPos = startNodeIndex + i;
            tempPos %= commonRoute.childNodeList.Count;

            if ((startNodeIndex == 0 && tempPos == commonRoute.childNodeList.Count))
                continue;

            if (tempPos == startNodeIndex - 1)
                continue;

            fullRoute.Add(commonRoute.childNodeList[tempPos].GetComponent<Node>());
        }

        for (int i = 0; i < finalRoute.childNodeList.Count; i++)        
            fullRoute.Add(finalRoute.childNodeList[i].GetComponent<Node>());        
    }

    IEnumerator Move(int diceNumber)
    {
        if (isMoving)
            yield break;
        isMoving = true;

        while(steps > 0)
        {
            routePosition++;
            Vector3 nextPos = fullRoute[routePosition].gameObject.transform.position;
            while (MoveToNextNode(nextPos, 8f)) { yield return null; }
            //Vector3 startPos = fullRoute[routePosition - 1].gameObject.transform.position;
            //while (MoveInArcToNextNode(startPos, nextPos, 8f)) { yield return null; }

            yield return new WaitForSeconds(0.1f);
            cTime = 0;
            steps--;
            doneSteps++;
        }

        goalNode = fullRoute[routePosition];
        if (goalNode.isTaken)
        {
            // Kick the other pawn
            goalNode.pawn.ReturnToBase();
        }
        currentNode.pawn = null;
        currentNode.isTaken = false;

        goalNode.pawn = this;
        goalNode.isTaken = true;

        currentNode = goalNode;
        goalNode = null;

        if (WinCondition())
        {
            GameManager.instance.ReportWinning();
        }    

        if (diceNumber < 6)
            GameManager.instance.state = GameManager.States.SWITCH_PLAYER;
        else
            GameManager.instance.state = GameManager.States.ROLL_DICE;

        isMoving = false;
    }

    bool MoveToNextNode(Vector3 goalPos, float speed)
    {
        return goalPos != (transform.position = Vector3.MoveTowards(transform.position, goalPos, speed * Time.deltaTime));
    }

    bool MoveInArcToNextNode (Vector3 startPos, Vector3 goalPos, float speed)
    {
        cTime += Time.deltaTime;
        Vector3 myPosition = Vector3.Lerp(startPos, goalPos, cTime);
        myPosition.y += amplitude * Mathf.Sin(Mathf.Clamp01(cTime) * Mathf.PI);

        return goalPos != (transform.position = Vector3.Lerp(transform.position, myPosition, cTime));
    }

    public bool ReturnIsOut()
    {
        return isOut;
    }

    public void LeaveBase ()
    {
        steps = 1;
        isOut = true;
        routePosition = 0;
        StartCoroutine(MoveOut());        
    }

    private IEnumerator MoveOut ()
    {        
        if (isMoving)
            yield break;
        isMoving = true;
        
        while (steps > 0)
        {           
            Vector3 nextPos = fullRoute[routePosition].gameObject.transform.position;
            while (MoveToNextNode(nextPos, 8f)) { yield return null; }
            yield return new WaitForSeconds(0.1f);
            steps--;
            doneSteps++;
        }
        isMoving = false;

        goalNode = fullRoute[routePosition];
        if (goalNode.isTaken)
        {            
            goalNode.pawn.ReturnToBase();
        }

        goalNode.pawn = this;
        goalNode.isTaken = true;

        currentNode = goalNode;
        goalNode = null;
        GameManager.instance.state = GameManager.States.ROLL_DICE;
    }

    public bool CheckPossibleMove(int diceNumber)
    {
        int tempPos = routePosition + diceNumber;
        if (tempPos >= fullRoute.Count)
            return false;
        return !fullRoute[tempPos].isTaken;
    }

    public bool CheckPossibleKick (int pawnID, int diceNumber)
    {
        int tempPos = routePosition + diceNumber;
        if (tempPos >= fullRoute.Count)
            return false;

        if (fullRoute[tempPos].isTaken && pawnID == fullRoute[tempPos].pawn.pawnID)        
            return false;
        else
            return true;
        
    }

    public void StartTheMove (int diceNumber)
    {
        steps = diceNumber;
        StartCoroutine(Move(diceNumber));
    }

    public void ReturnToBase ()
    {
        StartCoroutine(ReturnToBaseRoutine());
    }

    IEnumerator ReturnToBaseRoutine ()
    {
        GameManager.instance.ReportTurnPossile(false);

        routePosition = 0;
        currentNode = null;
        goalNode = null;
        isOut = false;
        doneSteps = 0;

        Vector3 baseNodePosition = baseNode.gameObject.transform.position;
        while (MoveToNextNode(baseNodePosition, 8f)) { yield return null; }
        GameManager.instance.ReportTurnPossile(true);
    }

    bool WinCondition()
    {
        for (int i = 0; i < finalRoute.childNodeList.Count; i++)
        {
            if (!finalRoute.childNodeList[i].GetComponent<Node>().isTaken)
                return false;
        }
        return true;
    }

    #region HUMAN_INPUT

    public void SetSelector (bool active)
    {
        selector.SetActive(active);
        hasTurn = active;
    }

    private void OnMouseDown()
    {
        if (hasTurn)
        {
            if (!isOut)
                LeaveBase();
            else
                StartTheMove(GameManager.instance.GetHumanDiceRollNumber());

            GameManager.instance.DeactivateAllSelectors();
        }
    }

    #endregion HUMAN_INPUT
}
