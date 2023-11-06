using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Movement : MonoBehaviour
{
    Rigidbody2D body;
    Attributes attributes;

    float horizontal;
    float vertical;
    float runSpeed;
    int collisionCount;
    int currentLayer;
    public int waterLevel;
    public Grid grid = null;
    public List<Tilemap> tilemaps = new List<Tilemap>();

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        currentLayer = gameObject.layer - 6;
        attributes = GetComponent<Attributes>();
        runSpeed = attributes.agility;
    }

    void FixedUpdate()
    {
        HandleWater();
    }

    public void SetMovement(float x, float y)
    {
        horizontal = x;
        vertical = y;
        body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //colliding with resource
        GameObject collidedObject = collision.gameObject;
        Debug.Log("Collision enter ", collidedObject);
        if(collidedObject.CompareTag("Resource"))
        {
            HandleResourceCollision(collidedObject);
        }
        //colliding with entity
        else if (collidedObject.CompareTag("Entity"))
        {
            attributes.Attack(collidedObject);
        }
        //colliding with tilemap
        else if (collidedObject.CompareTag("Tilemap"))
        {
            collisionCount = 0;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        GameObject collidedObject = collision.gameObject;
        if (collidedObject.CompareTag("Tilemap"))
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
        Debug.Log("collision exited");
    }

    private void HandleResourceCollision(GameObject resource)
    {
        Debug.Log("Collided with resource", resource);
        if(attributes.currentStamina >= 10)
        {
            attributes.ModifyStamina(-5);
            Resource harvestItem = resource.GetComponent<Resource>();
            if(harvestItem == null)
            {
                Debug.Log("Resource does not have resource script");
            }
            
            int harvestAmount = harvestItem.Harvest();
            Debug.Log("harvested " + harvestAmount);
            attributes.Eat(harvestAmount);
        }
    }

    private void HandleWater()
    {
        if (currentLayer <= waterLevel) 
        {
            attributes.ModifyStamina(-5 * Time.deltaTime);
            attributes.Drink(10 * Time.deltaTime);
        }
    }
}
