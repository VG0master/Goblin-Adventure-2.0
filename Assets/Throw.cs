using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour
{
    public GameObject prefab;
    public Camera MainCamera;
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray r = MainCamera.ScreenPointToRay(Input.mousePosition);

            Vector3 dir = r.GetPoint(1) - r.GetPoint(0);

            // position of spanwed object could be 'GetPoint(0).. 1.. 2' half random choice ;)
            GameObject Torch = Instantiate(prefab, r.GetPoint(2), Quaternion.LookRotation(dir));

            Torch.GetComponent<Rigidbody>().velocity = Torch.transform.forward * 20;
            Destroy(Torch, 3);
        }
    }

}
