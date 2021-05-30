using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    
    public GameObject projectile;
    public Transform nose;
    public float fireRate = 1f;
    public float bulletForce = 10f;
    float time;
    Camera cam;
    void Start()
    {
        time = 0f;
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0) && time >= fireRate)
        {
            Shoot();
            time = 0;
        }
        else if(Input.GetMouseButtonUp(0))
            time = fireRate;
        time += Time.deltaTime;
    }

    void Shoot()
    {
        Vector3 MousePos = Input.mousePosition;
        MousePos = cam.ScreenToWorldPoint(MousePos);
        Vector2 Direction = (MousePos - transform.position);
        float angle = Mathf.Atan2(Direction.y,Direction.x)*Mathf.Rad2Deg;
        
        GameObject pro = Instantiate(projectile,nose.position,nose.rotation);
        pro.AddComponent<Bullet>();

        Rigidbody2D rb = pro.GetComponent<Rigidbody2D>();
        if(rb == null)
        {
            rb = pro.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 0f;
        rb.AddForce(nose.right*bulletForce,ForceMode2D.Impulse);
     
    }

}
