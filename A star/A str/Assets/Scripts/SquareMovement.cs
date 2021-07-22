using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareMovement : MonoBehaviour
{
   
    [SerializeField]Camera cam;
    Vector2 movement;
    public float speed = 5f;
    public float jumpHieght = 5f;
    Health hp;
    public Rigidbody2D rb;
    Vector2 velocity;

    public Transform groundCheck;
    public LayerMask groundLayer;
   
    void Start()
    {
        hp = GetComponent<Health>();
        if(rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        FaceLook();
        if(hp.health<=0)
            Destroy(gameObject);
        if(Input.GetKeyDown(KeyCode.Space)){
            velocity.y = Mathf.Sqrt(jumpHieght*-2f*Physics2D.gravity.y);
            rb.velocity = velocity;
        }else{
            velocity.y = rb.velocity.y;
        }
    }
    private void FixedUpdate() 
    {
        Movement();
    }
    void FaceLook()
    {
        Vector3 MousePos = Input.mousePosition;
        MousePos = cam.ScreenToWorldPoint(MousePos);
        Vector2 Direction = (MousePos - transform.position).normalized;

        transform.right = Direction;
    }

    void Movement()
    {
        movement.x = Input.GetAxisRaw("Horizontal");

        velocity.x =  movement.x * speed;
        
        rb.velocity = velocity;
        //rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    public bool isGrounded()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f, groundLayer);
    }

}
