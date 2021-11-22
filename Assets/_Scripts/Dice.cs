using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    Rigidbody rb;

    bool hasLanded;
    bool thrown;

    Vector3 initPosition;

    public DiceSide[] diceSides;
    public int diceValue;

    void Start()
    {
        initPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    public void RollDice ()
    {
        ResetDice();
        if (!thrown && !hasLanded)
        {
            thrown = true;
            rb.useGravity = true;
            rb.AddTorque(Random.Range(500, 800), Random.Range(500, 800), Random.Range(500, 800));
        }
        else if (thrown && hasLanded)
            ResetDice();
        
    }

    void ResetDice()
    {
        transform.position = initPosition;
        rb.isKinematic = false;
        thrown = false;
        hasLanded = false;
        rb.useGravity = false;
    }

    private void Update()
    {
        if (rb.IsSleeping() && !hasLanded && thrown)
        {
            hasLanded = true;
            rb.useGravity = false;
            rb.isKinematic = true;

            
            SideValueCheck();
        }
        else if (rb.IsSleeping() && hasLanded && thrown && diceValue == 0)
        {
            // roll the dice again
        }
    }

    private void RollAgain ()
    {
        ResetDice();
        thrown = true;
        rb.useGravity = true;
        rb.AddTorque(Random.Range(500, 800), Random.Range(500, 800), Random.Range(500, 800));
    }

    void SideValueCheck ()
    {
        diceValue = 0;
        for (int i = 0; i < diceSides.Length; i++)
        {
            if (diceSides[i].OnGround)
            {
                diceValue = diceSides[i].sideValue;
                GameManager.instance.RollDice(diceValue);
            }
        }
    }
}
