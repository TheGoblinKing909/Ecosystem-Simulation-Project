using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Movement : MonoBehaviour
{
    public Rigidbody2D body;
    public Attributes attributes;

    float horizontal;
    float vertical;
    float runSpeed;
    int collisionCount;
    public int currentLayer;
    public int waterLevel;
    public Grid grid = null;
    public List<Tilemap> tilemaps = new List<Tilemap>();
    public List<GameObject> collisions = new List<GameObject>();

    public HumanAgent agent = null;

    // Start is called before the first frame update
    public void OnInstantiate()
    {

        ObjectSpawner entityManager = transform.parent.GetComponent<ObjectSpawner>();
        agent = GetComponent<HumanAgent>();

        if ( entityManager != null ) {
            grid = entityManager.grid;
            tilemaps = entityManager.tilemaps;
            body = GetComponent<Rigidbody2D>();
            currentLayer = gameObject.layer - 6;
            attributes = GetComponent<Attributes>();
            runSpeed = attributes.agility;
        }
        else {
            Debug.LogError("EntityManager not found!");
        }

    }

    public void SetMovement(float x, float y)
    {
        horizontal = x;
        vertical = y;
        body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);

        Vector2 currentPosition = body.position; // Assuming 'body' is a Rigidbody2D
        float deltaTime = Time.deltaTime; // Get the time since the last frame update
        Vector2 newPosition = currentPosition + body.velocity * deltaTime;

        if(newPosition.x > 50 || newPosition.x < -50)
        {
            agent.AddReward(-10000000);
        }
        if (newPosition.y > 50 || newPosition.y < -50)
        {
            agent.AddReward(-10000000);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        collisions.Add(collision.gameObject);
        if (collision.gameObject.CompareTag("Tilemap"))
        {
            collisionCount = 0;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Tilemap"))
        {
            collisionCount++;
            if (collisionCount == 5)
            {
                Vector3 currentPos = new Vector3(transform.position.x, transform.position.y, 0);
                Vector3Int currentCell = grid.WorldToCell(currentPos);
                Vector3 direction = new Vector3(horizontal, vertical, 0);
                direction = Quaternion.Euler(0, 0, -45) * direction;
                Vector3Int directionInt = new Vector3Int(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y), 0);
                
                bool stillMoving = true;
                if (currentLayer > 0)
                {
                    Vector3Int cellDown = currentCell;
                    if (currentLayer > 2)
                    {
                    cellDown.x--;
                    cellDown.y--;
                    }
                    cellDown += directionInt;
                    if (tilemaps[currentLayer - 1].HasTile(cellDown))
                    {
                        gameObject.layer = tilemaps[currentLayer - 1].gameObject.layer;
                        currentLayer--;
                        Vector3 newPos = grid.CellToWorld(cellDown);
                        newPos.y += 0.2885f;
                        newPos.z = 25;
                        transform.position = newPos;
                        stillMoving = false;
                    }
                }

                if (stillMoving && currentLayer < tilemaps.Count - 1)
                {
                    Vector3Int cellUp = currentCell;
                    if (currentLayer > 1)
                    {
                    cellUp.x++;
                    cellUp.y++;
                    }
                    cellUp += directionInt;
                    if (tilemaps[currentLayer + 1].HasTile(cellUp))
                    {
                        gameObject.layer = tilemaps[currentLayer + 1].gameObject.layer;
                        currentLayer++;
                        Vector3 newPos = grid.CellToWorld(cellUp);
                        newPos.y += 0.2885f;
                        newPos.z = 25;
                        transform.position = newPos;
                    }
                }

                collisionCount = 0;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        collisions.Remove(collision.gameObject);
    }
}
