using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// These are the empty tiles that make up the tile grid
//
// These tiles can be filled by a tile stencil but the data held in these
// tiles is never permanent until the user switches to simulation mode
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
        // A replace call with nothing to replace with becomes a remove call
        if (content == null)
        {
            RemoveTileContents();
        }
        // If the tile stencil being placed takes up more than 1 tile,
        // this tile holds a reference to the actual GameObject that was placed in an adjacent tile
        else if (content.GetComponent<TrackCreatorTile>())
        {
            RemoveTileContents();
            TrackCreatorTile tileRef = content.GetComponent<TrackCreatorTile>();
            trackTile = tileRef.trackTile;
        }
        // Place the selected tile stencil at this empty tile's location
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
