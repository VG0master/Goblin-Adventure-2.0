using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBurn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Wall")
            Destroy(collision.gameObject);
    }
}
