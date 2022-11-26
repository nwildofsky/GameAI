using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class TileStencil : MonoBehaviour
{
    public GameObject tile;
    public int width;
    public int height;

    public void Rotate()
    {
        float newRot = GetComponent<RectTransform>().rotation.eulerAngles.z - 90;
        GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, newRot);

        int temp = width;
        width = height;
        height = temp;
    }
}
