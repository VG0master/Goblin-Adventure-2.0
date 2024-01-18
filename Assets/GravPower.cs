using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GravPower : MonoBehaviour
{
    public float timeRemaining = 10;
    public bool start = false;
    
    // Start is called before the first frame update
    void Start()
    {

    }
    void Update()
    {
        if (start == true)
        {

            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                EndGrav();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Grav"))
        {
            Destroy(other.gameObject);
            Physics.gravity = new Vector3(0, -1, 0);
            start = true;
            
           

        }
    }
    void EndGrav()
    {
        Physics.gravity = new Vector3(0, -9.83f, 0);
    }
}
