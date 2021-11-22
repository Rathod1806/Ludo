using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceSide : MonoBehaviour
{
    public bool OnGround { get; private set; }
    public int sideValue;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            OnGround = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            OnGround = false;
        }
    }
}
