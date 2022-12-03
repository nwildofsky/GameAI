using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent (typeof(Image))]
public class TileStencil : MonoBehaviour
{
    public GameObject tile;
    public int width;
    public int height;
    public Sprite normalSprite;
    public Sprite selectedSprite;
    public bool selected = false;

    public void Rotate()
    {
        float newRot = GetComponent<RectTransform>().rotation.eulerAngles.z - 90;
        GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, newRot);

        int temp = width;
        width = height;
        height = temp;
    }

    public void Select()
    {
        selected = true;
        GetComponent<Image>().sprite = selectedSprite;
    }

    public void Deselect()
    {
        selected = false;
        GetComponent<Image>().sprite = normalSprite;
    }
}
