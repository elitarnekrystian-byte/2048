using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public TileRow[] rows { get; private set; }
    public TileCell[] cells { get; private set; }

    public int size => cells.Length;
    public int height => rows.Length;
    public int width => size/height;

    private void Awake()
    {
        rows = GetComponentsInChildren<TileRow>();
        cells = GetComponentsInChildren<TileCell>();
    }

    private void Start()
    {
        for (int y = 0; y < rows.Length; y++)
        {
            for (int x = 0; x < rows[y].cells.Length; x++)
            {
                rows[y].cells[x].coordinates = new Vector2Int(x, y);
            }
        }
    }

    public TileCell GetRandomEmptyCell()
    {
        int randomIndex = Random.Range(0, cells.Length);
        int startIndex = randomIndex;

        while (cells[randomIndex].occupied)
        {
            randomIndex++;
            if (randomIndex >= cells.Length)
            {
                randomIndex = 0; 
            }
            
            if (randomIndex == startIndex)
            {
                return null; 
            }
        }

        return cells[randomIndex];
    }

    public TileCell GetCell(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
        {
            return null; 
        }
        return cells[y * width + x]; 
    }

    public TileCell GetAdjacentCell(TileCell cell, Vector2Int direction)
    {
        if (cell == null) return null;
        int newX = cell.coordinates.x + direction.x;
        int newY = cell.coordinates.y - direction.y;
        return GetCell(newX, newY);
    }
}
