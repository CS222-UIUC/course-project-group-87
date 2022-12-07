using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtificialIntelligence : MonoBehaviour
{
    [SerializeField] public float speed = 1.0f;
    [SerializeField] public float damage = 1.0f;
    [SerializeField] public float health = 2.0f;
    [SerializeField] public static float strength = 1.0f;
    [SerializeField] private GameObject pc;
    
    // Start is called before the first frame update
    void Start()
    {
        health *= strength;
        damage *= strength;
        health *= strength;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(pc.transform.position);
        Debug.Log(this.transform.position);
        Vector3 dir = (pc.transform.position - this.transform.position);
        dir.Normalize();
        this.transform.forward = dir;
        Vector3 moveDir = dir;
        moveDir.y = 0;
        MoveTowards(moveDir);


    }

    void MoveTowards(Vector3 moveDir)
    {
        Vector3 pos = this.transform.position;
        pos += 100.0f * Time.deltaTime * speed * moveDir;

    }

    void JumpRandom()
    {
        
    }

    void DealtDamage()
    {
        
    }
    
    public void TakeDamage (float amount)
    {
        health -= amount;

        if (health <= 0f) {
            Die();
        }
    }

    void Die()
    {
        strength *= 1.2f;
        //Destroy(gameObject);

        //gameObject.transform.position = new Vector3(-66, 0, 10);
        health = 50f;
    }
}
