using UnityEngine;

public class Enemy1 : MonoBehaviour {

    Transform ply;
    [SerializeField]float speed = 2f;
    Health hp;
    public float power;
    Rigidbody2D rb;
    private void Start() 
    {
        ply = FindObjectOfType<SquareMovement>().transform;
        hp = transform.GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() 
    {
        transform.position = Vector2.MoveTowards(transform.position,ply.position,speed*Time.deltaTime);
        if(hp.health<=0)
            Destroy(this.gameObject); 
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if(other.transform.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Health>().DamageHealth(power);
            Destroy(gameObject);
        }
    }

}