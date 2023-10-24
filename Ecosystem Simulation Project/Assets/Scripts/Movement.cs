using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement : MonoBehaviour
{
    Rigidbody2D body;
    Attributes attributes;

    float horizontal;
    float vertical;
    public float runSpeed = 5.0f;
    int collisionCount;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        attributes = GetComponent<Attributes>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        collisionCount = 0;
        Debug.Log("collision entered");

        //colliding with resource
        GameObject gameObject = collision.gameObject;
        Debug.Log("Collision enter ", gameObject);
        if(gameObject.CompareTag("Resource"))
        {
            HandleResourceCollision(gameObject);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        collisionCount++;
        Debug.Log("collision count = " + collisionCount);
        if (collisionCount == 50)
        {
            transform.position = transform.position + new Vector3(horizontal / 2, vertical / 2, 0);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("collision exited");
    }

    private void HandleResourceCollision(GameObject resource)
    {
        Debug.Log("Collided with resource", resource);

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
