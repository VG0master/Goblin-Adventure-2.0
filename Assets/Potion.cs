
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Potion : MonoBehaviour
{
    //stats
    public int speedMult;
    public int jumpMult;
    public int duration;

    
    //on trigger enter, if the object is the player, add stat buffs/debuffs and destroy self
    OnTriggerEnter(Collider other){
        if (other.gameObject.Tag == "Player")
        {
            other.speedMult(speedMult, duration);
            other.jumpMult(jumpMult, duration);
            Object.Destroy(this.gameObject);
        }
    }
}