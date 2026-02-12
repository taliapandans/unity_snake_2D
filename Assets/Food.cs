using UnityEngine;


public class Food : MonoBehaviour
{
    public BoxCollider2D gridArea;

    private void Start()
    {
        RandomizePosition();
    }

    // Randomize food position
    private void RandomizePosition()
    {
        Bounds bounds = this.gridArea.bounds;

        // Create boundaries of grid and set x and y position within the grid
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        
        // Randomize the point using grid boundaries
        this.transform.position = new Vector2(Mathf.Round(x), Mathf.Round(y));
    }

    // Create trigger response everytime food got eaten (food collides with snake)
    private void OnTriggerEnter2D(Collider2D other)
    {
        // set Snake's tag as Player
        if(other.tag == "Player")
        {
            RandomizePosition();
        }

    }
}