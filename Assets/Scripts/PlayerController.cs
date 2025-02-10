using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 6.0f;
    public float jumpForce = 10.0f;
    public ConnectionManager manager;

    private Animator animator;
    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;
    // private Quaternion initialRotation;
    private bool inAir = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Cursor.visible = false;
        // initialRotation = transform.rotation;
    }
  
    void FixedUpdate()
    {
        if (Global.viewingScene)
        {
            animator.SetBool("Running", false);
            body.velocity = new Vector2(0, 0);
            return;
        }

        float axis = Input.GetAxis("Horizontal");

        body.velocity = new Vector2(axis * speed, body.velocity.y);

        if (axis == 0.0F)
        {
            animator.SetBool("Running", false);
        }
        else
        {
            animator.SetBool("Running", true);

            if (axis < 0)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && !inAir)
        {
            body.AddForce(new Vector2(jumpForce, jumpForce), ForceMode2D.Impulse);
        }

        transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, Math.Min(Math.Max(transform.rotation.z, -30), 40), transform.rotation.w);

        if (Input.GetKeyDown(KeyCode.E))
        {
            MemoryInstance instance = manager.GetCurrentInstance();
            if (instance != null)
            {
                instance.Interact();
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            MemoryInstance instance = manager.GetCurrentInstance();
            if (instance == null)
            {
                if (manager.IsHolding())
                {
                    manager.DropCurrentConnection();
                }
                return;
            }

            if (!manager.IsHolding())
            {
                Connection connection = manager.Create(instance.id, System.Guid.Empty, transform);
                manager.Add(connection);
            }
            else
            {
                Connection connection = manager.GetCurrentConnection();
                manager.DropCurrentConnection();
                if (connection.from != instance.id)
                {
                    manager.Add(manager.Create(connection.from, instance.id, transform));
                }
            }
        }


        if (Input.GetKeyDown(KeyCode.X))
        {
            MemoryInstance instance = manager.GetCurrentInstance();
            if (instance == null)
            {
                return;
            }
            if (manager.HasCompleteConnection(instance.id))
            {
                Connection[] connections = manager.GetConnectionsOf(instance.id);
                manager.DropConnection(connections[0]);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            inAir = false;
        }
    }

    void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            inAir = true;
        }
    }
}
