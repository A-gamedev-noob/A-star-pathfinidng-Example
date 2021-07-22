using UnityEngine;

public class Bullet : MonoBehaviour {
    
    public Vector2 direction;
    float time;
    public float damage = 40f;

    private void Update() 
    {
        if(time > 4f)
            Destroy(this.gameObject);
        time += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.transform.CompareTag("Enemy") || other.gameObject.GetComponent<Health>()!=null)
            other.gameObject.GetComponent<Health>().DamageHealth(damage);
        Destroy(this.gameObject);
    }

}