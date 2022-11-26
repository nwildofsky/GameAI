using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(RectTransform))]
public class TrackEditorGrid : MonoBehaviour
{
    public GameObject tile;
    int tileSize;
    static TileStencil selectedStencil;
    static Quaternion tilePlaceRotation;
    List<Transform> grid;
    int width;
    int height;

    // Start is called before the first frame update
    void Start()
    {
        selectedStencil = GameObject.FindGameObjectWithTag("DefaultStencil").GetComponent<TileStencil>();
        tilePlaceRotation = Quaternion.identity;
        grid = new List<Transform>();

        tileSize = (int)tile.GetComponent<RectTransform>().rect.width;
        RectTransform canvas = GetComponent<RectTransform>();

        width = (int)canvas.rect.width / tileSize * tileSize;
        height = (int)canvas.rect.height / tileSize * tileSize;
        canvas.sizeDelta = new Vector2(width, height);

        for (int i = 0; i < height; i += tileSize)
        {
            for (int j = 0; j < width; j += tileSize)
            {
                GameObject newTile = Instantiate(tile, transform);
                RectTransform newTileTransform = newTile.GetComponent<RectTransform>();
                newTileTransform.SetParent(canvas);
                newTileTransform.anchoredPosition = new Vector2((-width / 2.0f) + (tileSize / 2.0f - 1) + j, (-height / 2.0f) + (tileSize / 2.0f - 1) + i);
                grid.Add(newTile.transform);
            }
        }
    }

    private void Update()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        for (int i = 0; i < grid.Count; i++)
        {
            float xMin = grid[i].position.x - tileSize / 2.0f;
            float xMax = grid[i].position.x + tileSize / 2.0f;
            float zMin = grid[i].position.z - tileSize / 2.0f;
            float zMax = grid[i].position.z + tileSize / 2.0f;

            if (mouseWorldPosition.x > xMin && mouseWorldPosition.x < xMax
                && mouseWorldPosition.z > zMin && mouseWorldPosition.z < zMax)
            {
                grid[i].GetComponent<Button>().enabled = true;
                grid[i].GetComponent<Button>().interactable = true;
                grid[i].GetComponent<Button>().Select();

                if (mouseWorldPosition.x < grid[i].position.x && selectedStencil.width > 1)
                {
                    if (i != 0)
                    {
                        grid[i - 1].GetComponent<Button>().interactable = false;
                    }
                    else
                    {
                        grid[i].GetComponent<Button>().enabled = false;
                    }
                }
                else if (mouseWorldPosition.x > grid[i].position.x && selectedStencil.width > 1)
                {
                    if (i != grid.Count - 1)
                    {
                        grid[i + 1].GetComponent<Button>().interactable = false;
                    }
                    else
                    {
                        grid[i].GetComponent<Button>().enabled = false;
                    }
                }

                if (mouseWorldPosition.z < grid[i].position.z && selectedStencil.height > 1)
                {
                    if (i - width / tileSize >= 0)
                    {
                        grid[i - width / tileSize].GetComponent<Button>().interactable = false;
                    }
                    else
                    {
                        grid[i].GetComponent<Button>().enabled = false;
                    }
                }
                else if (mouseWorldPosition.z > grid[i].position.z && selectedStencil.height > 1)
                {
                    if (i + width / tileSize < grid.Count)
                    {
                        grid[i + width / tileSize].GetComponent<Button>().interactable = false;
                    }
                    else
                    {
                        grid[i].GetComponent<Button>().enabled = false;
                    }
                }
            }
            else
            {
                grid[i].GetComponent<Button>().enabled = true;
                grid[i].GetComponent<Button>().interactable = true;
            }
        }
    }

    public void HandleTileClick(TrackEditorTile tile)
    {
        tile.ReplaceTileContents(selectedStencil.tile, tilePlaceRotation, Vector3.zero);
    }

    public void SetSelectedTrackTile(TileStencil stencil)
    {
        selectedStencil = stencil;
    }

    public void RotateStencils()
    {
        float newRot = tilePlaceRotation.eulerAngles.y + 90;
        tilePlaceRotation = Quaternion.Euler(0, newRot, 0);

        TileStencil[] allStencils = GameObject.FindObjectsOfType<TileStencil>();
        foreach (TileStencil stencil in allStencils)
        {
            if (stencil.tile != null)
            {
                stencil.Rotate();
            }
        }
    }
}
