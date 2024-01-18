using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public Door d;
    void OnTriggerEnter(Collider other)
    {
        d.doorOpen();
    }
}
