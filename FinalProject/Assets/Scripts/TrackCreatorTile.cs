using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class TrackCreatorTile : MonoBehaviour
{
    GameObject trackTile;
    Transform trackParent;

    private void Start()
    {
        trackTile = null;
        trackParent = GameObject.Find("Racetrack").transform;
    }

    public void ReplaceTileContents(GameObject content, Quaternion rotation, Vector3 offset)
    {
        if (content == null)
        {
            RemoveTileContents();
        }
        else if (content.GetComponent<TrackCreatorTile>())
        {
            RemoveTileContents();
            TrackCreatorTile tileRef = content.GetComponent<TrackCreatorTile>();
            trackTile = tileRef.trackTile;
        }
        else if (content != null)
        {
            RemoveTileContents();
            trackTile = Instantiate(content, transform.position + offset, rotation, trackParent);
        }
    }

    void RemoveTileContents()
    {
        if (trackTile != null)
        {
            Object.Destroy(trackTile);
        }
    }
}
