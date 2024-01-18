
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Potion : MonoBehaviour
{
    //stats
    public int speedBoost;
    public int jumpBoost;
    public int duration;


    
    //on trigger enter, if the object is the player, add stat buffs/debuffs and destroy self
    void OnTriggerEnter(Collider other){
        if (other.gameObject.tag == "Player")
        {

            other.gameObject.GetComponent<PlayerMovement>().speedMult(speedBoost, duration);
            other.gameObject.GetComponent<PlayerMovement>().jumpMult(jumpBoost, duration);
            Destroy (this.gameObject);
        }
    }
}
