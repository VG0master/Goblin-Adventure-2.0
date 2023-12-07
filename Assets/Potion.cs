
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
    void OnTriggerEnter(Collider other){
        if (other.gameObject.Tag == "Player")
        {
            GameObject Object = other.gameObject;
            Object.speedMult(speedMult, duration);
            Object.jumpMult(jumpMult, duration);
            Object.Destroy(this.gameObject);
        }
    }
}
