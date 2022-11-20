using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class TrackEditorTile : MonoBehaviour
{
    GameObject trackTile;

    private void Start()
    {
        trackTile = null;
    }

    public void ReplaceTileContents(GameObject content, Quaternion rotation)
    {
        if (content != null)
        {
            if (trackTile != null)
            {
                Object.Destroy(trackTile);
            }

            trackTile = Instantiate(content, transform.position, rotation);
        }
    }
}
