using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(RectTransform))]
public class TrackEditorGrid : MonoBehaviour
{
    public GameObject tile;
    public GameObject selectedTrackTile;
    public Quaternion tilePlaceRotation;

    // Start is called before the first frame update
    void Start()
    {
        int tileSize = (int)tile.GetComponent<RectTransform>().rect.width;
        RectTransform canvas = GetComponent<RectTransform>();

        float width = (int)canvas.rect.width / tileSize * tileSize;
        float height = (int)canvas.rect.height / tileSize * tileSize;
        canvas.sizeDelta = new Vector2(width, height);

        for (int i = 0; i < height; i += tileSize)
        {
            for (int j = 0; j < width; j += tileSize)
            {
                GameObject newTile = Instantiate(tile, transform);
                RectTransform newTileTransform = newTile.GetComponent<RectTransform>();
                newTileTransform.SetParent(canvas);
                newTileTransform.anchoredPosition = new Vector2((-width / 2) + (tileSize / 2.0f) + j, (-height / 2) + (tileSize / 2.0f) + i);
            }
        }
    }

    public void HandleTileClick(TrackEditorTile tile)
    {
        tile.ReplaceTileContents(selectedTrackTile, tilePlaceRotation);
    }
}
