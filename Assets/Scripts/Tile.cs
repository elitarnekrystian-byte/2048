using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public TileState state { get; private set; }
    public TileCell cell { get; private set; } 
    public int number { get; private set; }
    public bool locked { get; set; }

    private Image background;
    private TextMeshProUGUI text;

    private void Awake()
    {
        background = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetState(TileState state, int number)
    {
        this.state = state;
        this.number = number;

        background.color = state.backgroundColor;
        text.color = state.textColor;
        text.text = number.ToString();
    }

    public void Spawn(TileCell cell)
    {
        if (this.cell != null)
        {
            return;
        }
        this.cell = cell;
        this.cell.tile = this;
        transform.position = cell.transform.position;
    }

    public void MoveTo(TileCell cell)
    {
        // Update the current cell reference
        if (this.cell != null)
        {
            this.cell.tile = null; // Clear the tile reference in the old cell
        }
        // Move to the new cell
        this.cell = cell;
        this.cell.tile = this; // Set the tile reference in the new cell
        StartCoroutine(Animate(cell.transform.position, false)); // Start the animation coroutine
    }

    private IEnumerator Animate(Vector3 to, bool merging)
    {

        float elapsedTime = 0f; // Start the animation timer
        float duration = 0.1f; // Duration of the animation

        Vector3 startPosition = transform.position; // Store the starting position
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration; // Calculate the interpolation factor
            transform.position = Vector3.Lerp(startPosition, to, t); // Interpolate position
            elapsedTime += Time.deltaTime; // Increment the elapsed time
            yield return null; // Wait for the next frame
        }

        transform.position = to; // Ensure the final position is set

        if (merging)
        {
            Destroy(gameObject); // Destroy the tile if it was merging
        }
    }

    public void Merge(TileCell cell)
    { 
        // Update the current cell reference
        if (this.cell != null)
        {
            this.cell.tile = null; // Clear the tile reference in the old cell
        }
        this.cell = null;
        cell.tile.locked = true; // Lock the cell to prevent further merges

        StartCoroutine(Animate(cell.transform.position, true)); // Start the animation coroutine
    }
}
