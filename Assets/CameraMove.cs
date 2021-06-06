using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public GameObject Target;

    Vector3 nextPosition;
    [SerializeField]
    float offsetX, offsetY, offsetZ;

    private void FixedUpdate()
    {
        nextPosition  = Target.transform.position + new Vector3(offsetX, offsetY, offsetZ);
        transform.position = Vector3.Slerp(transform.position, nextPosition, 0.5f);
    }
}
