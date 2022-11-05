using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateCollisionOnExit : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        GameObject obj = other.gameObject;

        SphereCollider sphere = obj.GetComponent<SphereCollider>();

        if (sphere)
        {
            sphere.enabled = true;
        }
    }
}
