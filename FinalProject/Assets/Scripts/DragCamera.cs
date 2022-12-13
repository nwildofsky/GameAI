using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Camera controller for both Track Creator ans Simulation scenes
// Scroll wheel to zoom in and out, Right click and drag to move
public class DragCamera : MonoBehaviour
{
    public float minSize;
    public float maxSize;
    public float dragSpeed;
    public float minDragSpeed;
    public float scrollSpeed;
    Vector3 mousePos;

    private void Start()
    {
        mousePos = Input.mousePosition;
    }

    // Update is called once per frame
    void Update()
    {
        // While holding the right mouse button
        if (Input.GetMouseButton(1))
        {
            // Get the direction to move the camera
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

            // Scale the move speec by how zoomed in the camera is
            float zoomPercent = (GetComponent<Camera>().orthographicSize - minSize) / (maxSize - minSize);
            zoomPercent = Mathf.Max(zoomPercent, minDragSpeed);
            dragDir *= dragSpeed * zoomPercent;
            
            transform.position += dragDir;
        }
        mousePos = Input.mousePosition;

        // The scroll wheel will increase and decrease the size of the camera clamped to a set max and min
        if (Input.mouseScrollDelta.y > 0)
        {
            GetComponent<Camera>().orthographicSize -= scrollSpeed;
            GetComponent<Camera>().orthographicSize = Mathf.Max(GetComponent<Camera>().orthographicSize, minSize);
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            GetComponent<Camera>().orthographicSize += scrollSpeed;
            GetComponent<Camera>().orthographicSize = Mathf.Min(GetComponent<Camera>().orthographicSize, maxSize);
        }
    }
}
