using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(RectTransform))]
public class TrackCreatorGrid : MonoBehaviour
{
    public GameObject tile;
    int tileSize;
    List<Transform> grid;
    int width;
    int height;

    static TileStencil selectedStencil;
    static Quaternion tilePlaceRotation;
    static Vector3 tilePlaceOffset;
    static List<TrackCreatorTile> adjacentPlaceTiles;

    GraphicRaycaster hudRaycaster;
    EventSystem eventSystem;

    // Start is called before the first frame update
    void Start()
    {
        selectedStencil = GameObject.FindGameObjectWithTag("DefaultStencil").GetComponent<TileStencil>();
        tilePlaceRotation = Quaternion.identity;
        tilePlaceOffset = Vector3.zero;
        adjacentPlaceTiles = new List<TrackCreatorTile>();

        hudRaycaster = GameObject.Find("TrackCreatorHUD").GetComponent<GraphicRaycaster>();
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();

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
                newTileTransform.anchoredPosition = new Vector2(
                    (-width / 2.0f) + (tileSize / 2.0f - 1) + j,
                    (-height / 2.0f) + (tileSize / 2.0f - 1) + i
                );
                grid.Add(newTile.transform);
            }
        }
    }

    private void Update()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        adjacentPlaceTiles.Clear();

        if (selectedStencil.width <= 1)
            tilePlaceOffset.x = 0;
        if (selectedStencil.height <= 1)
            tilePlaceOffset.z = 0;

        // Loop through every tile in the grid
        for (int i = 0; i < grid.Count; i++)
        {
            float xMin = grid[i].position.x - tileSize / 2.0f;
            float xMax = grid[i].position.x + tileSize / 2.0f;
            float zMin = grid[i].position.z - tileSize / 2.0f;
            float zMax = grid[i].position.z + tileSize / 2.0f;

            // If the mouse is currently in this tile
            if (mouseWorldPosition.x > xMin && mouseWorldPosition.x < xMax
                && mouseWorldPosition.z > zMin && mouseWorldPosition.z < zMax)
            {
                grid[i].GetComponent<Button>().enabled = true;
                grid[i].GetComponent<Button>().interactable = true;
                grid[i].GetComponent<Button>().Select();

                // Check if tile placement in this current position is possible
                if (
                    (mouseWorldPosition.x < grid[i].position.x && selectedStencil.width > 1 && i % (width / tileSize) == 0) ||
                    (mouseWorldPosition.x > grid[i].position.x && selectedStencil.width > 1 && i % (width / tileSize) == (width / tileSize) - 1) ||
                    (mouseWorldPosition.z < grid[i].position.z && selectedStencil.height > 1 && i - width / tileSize < 0) ||
                    (mouseWorldPosition.z > grid[i].position.z && selectedStencil.height > 1 && i + width / tileSize >= grid.Count)
                )
                {
                    grid[i].GetComponent<Button>().enabled = false;
                    tilePlaceOffset.x = 0;
                    tilePlaceOffset.z = 0;
                }

                // Disable tile placement when the mouse is hovering over the tile stencil options
                PointerEventData p = new PointerEventData(eventSystem);
                p.position = Input.mousePosition;
                List<RaycastResult> results = new List<RaycastResult>();
                hudRaycaster.Raycast(p, results);
                if (results.Count > 0)
                {
                    grid[i].GetComponent<Button>().enabled = false;
                }

                // If tile placement in this current position is possible
                if (grid[i].GetComponent<Button>().enabled)
                {
                    // Check left tile
                    if (mouseWorldPosition.x < grid[i].position.x && selectedStencil.width > 1)
                    {
                        // If there is a tile to the left
                        if (i % (width / tileSize) != 0)
                        {
                            grid[i - 1].GetComponent<Button>().interactable = false;
                            tilePlaceOffset.x = -tileSize / 2.0f;
                            adjacentPlaceTiles.Add(grid[i - 1].GetComponent<TrackCreatorTile>());
                        }
                    }
                    // Check right tile
                    else if (mouseWorldPosition.x > grid[i].position.x && selectedStencil.width > 1)
                    {
                        // If there is a tile to the right
                        if (i % (width / tileSize) != (width / tileSize) - 1)
                        {
                            grid[i + 1].GetComponent<Button>().interactable = false;
                            tilePlaceOffset.x = tileSize / 2.0f;
                            adjacentPlaceTiles.Add(grid[i + 1].GetComponent<TrackCreatorTile>());
                        }
                    }

                    // Check bottom tile
                    if (mouseWorldPosition.z < grid[i].position.z && selectedStencil.height > 1)
                    {
                        // If there is a tile below
                        if (i - width / tileSize >= 0)
                        {
                            grid[i - width / tileSize].GetComponent<Button>().interactable = false;
                            tilePlaceOffset.z = -tileSize / 2.0f;
                            adjacentPlaceTiles.Add(grid[i - width / tileSize].GetComponent<TrackCreatorTile>());
                        }
                    }
                    // Check top tile
                    else if (mouseWorldPosition.z > grid[i].position.z && selectedStencil.height > 1)
                    {
                        // If there is a tile above
                        if (i + width / tileSize < grid.Count)
                        {
                            grid[i + width / tileSize].GetComponent<Button>().interactable = false;
                            tilePlaceOffset.z = tileSize / 2.0f;
                            adjacentPlaceTiles.Add(grid[i + width / tileSize].GetComponent<TrackCreatorTile>());
                        }
                    }

                    // Highlight the last tile in the square for the 4x4 tile stencil
                    if (tilePlaceOffset.x != 0 && tilePlaceOffset.z != 0)
                    {
                        int xDir = (int)Mathf.Sign(tilePlaceOffset.x);
                        int zDir = (int)Mathf.Sign(tilePlaceOffset.z);

                        int index = i + xDir;
                        index = index + (zDir * width) / tileSize;
                        grid[index].GetComponent<Button>().interactable = false;
                        adjacentPlaceTiles.Add(grid[index].GetComponent<TrackCreatorTile>());
                    }

                    // Place tiles when the user drags left-click on an available tile space
                    if (Input.GetMouseButton(0))
                    {
                        HandleTileClick(grid[i].GetComponent<TrackCreatorTile>());
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

    public void HandleTileClick(TrackCreatorTile tile)
    {
        tile.ReplaceTileContents(selectedStencil.tile, tilePlaceRotation, tilePlaceOffset);

        foreach (TrackCreatorTile t in adjacentPlaceTiles)
        {
            t.ReplaceTileContents(tile.gameObject, tilePlaceRotation, tilePlaceOffset);
        }
    }

    public void SetSelectedTrackTile(TileStencil stencil)
    {
        if (selectedStencil != null)
            selectedStencil.Deselect();

        selectedStencil = stencil;
        selectedStencil.Select();
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
