using UnityEngine;
using System.Collections;

/*
 * A script that causes the attached transform to smoothly follow the "target" GameObject in the scene. 
 */
public class FollowSmoothly : MonoBehaviour {

    public GameObject target;
    public bool lockedRotation = false;
    [Range(0.1f, 50)]
    public float rotationalSpeed = 5f;
    [Range(0.1f, 50)]
    public float positionalSpeed = 1.5f;

    private Vector3 velocity = Vector3.zero;

    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, target.transform.position, ref velocity, 100/positionalSpeed * Time.deltaTime);
        if (lockedRotation)
        {
            transform.rotation = target.transform.rotation;
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, target.transform.rotation, rotationalSpeed * Time.deltaTime);
        }
    }
}
