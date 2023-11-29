using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LassoGrappling : MonoBehaviour
{
    private LineRenderer Lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform gunTip, camera, Player;
    private float maxDistance = 100f;
    private SpringJoint joint;

    private void Awake()
    {
        Lr = GetComponent<LineRenderer>();
    }
    private void Update()
    {
       

        if (Input.GetMouseButtonDown(0))
         {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopGrapple();
        }
    }
    private void LateUpdate()
    {
        DrawRope();
    }

    void StartGrapple()
    {
        RaycastHit hit;
        if(Physics.Raycast(origin: camera.position, direction: camera.forward, out hit, maxDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            joint = Player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(a: Player.position, b: grapplePoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.15f;

            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;
            Lr.positionCount = 2;
        }
    }


    void DrawRope()
    {
        if (!joint) return;

        Lr.SetPosition(index: 0, gunTip.position);
        Lr.SetPosition(index: 1, grapplePoint);
    }
    void StopGrapple()
    {
        Lr.positionCount = 0;
        Destroy(joint);
    }
}
