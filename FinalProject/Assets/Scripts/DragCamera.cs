using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragCamera : MonoBehaviour
{
    public float dragSpeed;
    Vector3 mousePos;

    private void Start()
    {
        mousePos = Input.mousePosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            Vector3 dragDir = Vector3.zero;
            Vector3 mouseDelta = Input.mousePosition - mousePos;
            if (mouseDelta.x > 0)
            {
                dragDir.x = -1;
            }
            else if (mouseDelta.x < 0)
            {
                dragDir.x = 1;
            }

            if (mouseDelta.y > 0)
            {
                dragDir.z = -1;
            }
            else if (mouseDelta.y < 0)
            {
                dragDir.z = 1;
            }

            dragDir = Vector3.Normalize(dragDir);
            dragDir.x *= Mathf.Abs(mouseDelta.x);
            dragDir.z *= Mathf.Abs(mouseDelta.y);
            dragDir *= dragSpeed;
            transform.position += dragDir;
        }

        mousePos = Input.mousePosition;
    }
}
