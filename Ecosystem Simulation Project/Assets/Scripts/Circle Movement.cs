using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Movement : MonoBehaviour
{
    Rigidbody2D body;

    float horizontal;
    float vertical;
    public float runSpeed = 5.0f;
    int collisionCount;
    int currentLayer;
    public Grid grid = null;
    public List<Tilemap> tilemaps = new List<Tilemap>();

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        currentLayer = gameObject.layer - 6;
    }

    // Update is called once per frame
    // void Update()
    // {
        
    // }

    private void FixedUpdate()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        collisionCount = 0;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        collisionCount++;
        if (collisionCount == 10)
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

    // void OnCollisionExit2D(Collision2D collision)
    // {
        
    // }
}
