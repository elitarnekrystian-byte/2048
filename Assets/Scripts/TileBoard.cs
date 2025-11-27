using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    public GameManager gameManager;

    private TileGrid grid;

    public Tile tilePrefab;
    private List<Tile> tiles;

    public TileState[] tileStates;

    private bool waiting;

    // --- DOPAMINOWE T£O RGB ---
    private float hue;
    private float rgbSpeed = 2000f; // ultra szybkie

    // --- CAMERA SHAKE ---
    public Camera mainCamera; // przypisz w inspectorze
    private Vector3 originalCamPos;
    public float shakeDuration = 0.3f;
    public float shakeMagnitude = 2.5f;

    private void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();
        tiles = new List<Tile>(16);
    }

    private void Start()
    {
        if (mainCamera != null)
            originalCamPos = mainCamera.transform.position;
    }

    public void CreateTile()
    {
        Tile newTile = Instantiate(tilePrefab, grid.transform);
        newTile.SetState(tileStates[0], 2);
        TileCell cell = grid.GetRandomEmptyCell();
        newTile.Spawn(cell);
        tiles.Add(newTile);
    }

    public void ClearBoard()
    {
        foreach (var cell in grid.cells)
            cell.tile = null;

        foreach (Tile tile in tiles)
            Destroy(tile.gameObject);

        tiles.Clear();
    }

    private void Update()
    {
        // --- EKSTREMALNIE SZYBKIE RGB T£O ---
        hue += Time.deltaTime * rgbSpeed / 100f;
        if (hue > 1f) hue -= 1f;

        Camera.main.backgroundColor = Color.HSVToRGB(hue, 1f, 1f);

        if (waiting)
            return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            MoveTiles(Vector2Int.up, 0, 1, 1, 1);
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            MoveTiles(Vector2Int.down, 0, 1, grid.height - 2, -1);
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            MoveTiles(Vector2Int.left, 1, 1, 0, 1);
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            MoveTiles(Vector2Int.right, grid.width - 2, -1, 0, 1);
    }

    private void MoveTiles(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        bool changed = false;
        for (int x = startX; x < grid.width && x >= 0; x += incrementX)
        {
            for (int y = startY; y < grid.height && y >= 0; y += incrementY)
            {
                TileCell cell = grid.GetCell(x, y);
                if (cell.occupied)
                    changed |= MoveTile(cell.tile, direction);
            }
        }

        if (changed)
        {
            StartCoroutine(WaitForTileMovement());
            if (mainCamera != null)
                StartCoroutine(CameraShake()); // trzêsie ekran tylko jeœli coœ siê ruszy³o
        }
    }

    private bool MoveTile(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdjacentCell(tile.cell, direction);

        while (adjacent != null)
        {
            if (adjacent.occupied)
            {
                if (CanMerge(tile, adjacent.tile))
                {
                    Merge(tile, adjacent.tile);
                    return true;
                }
                break;
            }

            newCell = adjacent;
            adjacent = grid.GetAdjacentCell(adjacent, direction);
        }

        if (newCell != null)
        {
            tile.MoveTo(newCell);
            return true;
        }
        return false;
    }

    private IEnumerator WaitForTileMovement()
    {
        waiting = true;
        yield return new WaitForSeconds(0.1f);
        waiting = false;

        foreach (var tile in tiles)
            tile.locked = false;

        if (tiles.Count < grid.size)
            CreateTile();

        if (CheckForGameOver())
            gameManager.GameOver();
    }

    private bool CanMerge(Tile a, Tile b)
    {
        return a.number == b.number && !b.locked;
    }

    private void Merge(Tile a, Tile b)
    {
        tiles.Remove(a);
        a.Merge(b.cell);

        int index = Mathf.Clamp(IndexOf(b.state) + 1, 0, tileStates.Length - 1);
        int newNumber = b.number * 2;

        b.SetState(tileStates[index], newNumber);
        gameManager.UpdateScore(newNumber);

        // kamera trzêsie siê te¿ przy merge
        if (mainCamera != null)
            StartCoroutine(CameraShake());
    }

    private int IndexOf(TileState state)
    {
        for (int i = 0; i < tileStates.Length; i++)
        {
            if (tileStates[i] == state)
                return i;
        }
        return -1;
    }

    private bool CheckForGameOver()
    {
        if (tiles.Count != grid.size)
            return false;

        foreach (Tile tile in tiles)
        {
            TileCell adjacent = grid.GetAdjacentCell(tile.cell, Vector2Int.up);
            if (adjacent != null && adjacent.occupied && CanMerge(tile, adjacent.tile))
                return false;
            adjacent = grid.GetAdjacentCell(tile.cell, Vector2Int.down);
            if (adjacent != null && adjacent.occupied && CanMerge(tile, adjacent.tile))
                return false;
            adjacent = grid.GetAdjacentCell(tile.cell, Vector2Int.left);
            if (adjacent != null && adjacent.occupied && CanMerge(tile, adjacent.tile))
                return false;
            adjacent = grid.GetAdjacentCell(tile.cell, Vector2Int.right);
            if (adjacent != null && adjacent.occupied && CanMerge(tile, adjacent.tile))
                return false;
        }

        return true;
    }

    // --- CAMERA SHAKE COROUTINE ---
    private IEnumerator CameraShake()
    {
        Vector3 originalPos = mainCamera.transform.position;
        float elapsed = 0f;
        float speed = 50f;

        while (elapsed < shakeDuration)
        {
            float x = Mathf.Sin(elapsed * speed) * shakeMagnitude;
            float y = Mathf.Sin(elapsed * speed * 0.5f) * (shakeMagnitude / 2);
            //mainCamera.transform.position = originalPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

      //  mainCamera.transform.position = originalPos;
    }
}
