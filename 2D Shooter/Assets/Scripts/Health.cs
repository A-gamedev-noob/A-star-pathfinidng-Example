using UnityEngine;

public class Health : MonoBehaviour {
    
    public float health = 100f;

    public void DamageHealth(float value)
    {
        health -= value;
    } 

}