using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareMovement : MonoBehaviour
{
   
    [SerializeField]Camera cam;
    Vector2 movement;
    public float speed = 5f;
    Health hp;
    Rigidbody2D rb;
   
    void Start()
    {
        rb = transform.GetComponent<Rigidbody2D>();
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
        movement.y = Input.GetAxisRaw("Vertical");

        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }
    
}
