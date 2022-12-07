using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtificialIntelligence : MonoBehaviour
{
    [SerializeField] public float speed = 10.0f;
    [SerializeField] public float damage = 1.0f;
    [SerializeField] public float health = 15.0f;
    [SerializeField] public static float strength = 1.0f;
    [SerializeField] public Rigidbody rb;
    [SerializeField] public GameObject cube;

    [SerializeField] public int Operations = 5;

    private GameObject pc;
    private int teleport = 0;
    private float maxHealth;

    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log(health);
        maxHealth = health;
        pc = GameObject.FindGameObjectsWithTag("Player")[0];
        health *= strength;
        damage *= strength;
        health *= strength;
        InvokeRepeating("LaunchTowards", 2.0f, 1.0f);
        InvokeRepeating("Damage", 0.5f, 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(pc.transform.position);
        // Debug.Log(this.transform.position);
        // Vector3 dir = (pc.transform.position - this.transform.position);
        // Debug.Log(dir);
        // dir.Normalize();
        // this.transform.forward = dir;
        // Vector3 moveDir = dir;
        // moveDir.y = 0;
        // MoveTowards(moveDir);


    }

    void Damage()
    {
        Vector3 dir = (pc.transform.position - this.transform.position);
        if (dir.magnitude < 2.0f)
        {
            DealtDamage();
            FlyAway();
        }
    }

    void FlyAway()
    {
        Vector3 dir = (pc.transform.position - this.transform.position);
        rb.velocity = -1 * dir * 14f;

    }

    void LaunchTowards()
    {
        Operations--;
        if (Operations == 0)
        {
            Die();
        }
        Vector3 dir = (pc.transform.position - this.transform.position);
        //Debug.Log(dir.magnitude);
        if (dir.magnitude < 2.0f)
        {
            DealtDamage();
            dir *= -1.0f;
        }
        dir.Normalize();
        this.transform.forward = dir;
        RaycastHit hit;
        
        // Logic

        if (Physics.Raycast(transform.position, dir, out hit))
        {
            // Debug.Log(hit.transform);
            // Debug.Log(pc.transform);
            // Debug.DrawRay(transform.position, dir * hit.distance, Color.yellow);
            if (hit.transform == pc.transform)
            {
                var r = Random.value;
                if (r < 0.1)
                {
                    rb.velocity = Random.insideUnitSphere * 15;
                }
                else
                {
                    rb.velocity = this.transform.forward * 10 * Mathf.Log(speed, 10);
                }
                // enemy can see the player!
            }
            else
            {
                // Debug.Log("HIT GROUND");
                Vector3 temp = Random.insideUnitSphere * 7;
                temp.y = Mathf.Abs(temp.y);
                transform.position = pc.transform.position + temp;
            }
        }


        }



        void DealtDamage()
        {
            pc.transform.GetComponent<PlayerController>().TakeDamage(damage);
            Debug.Log("Helath is " + pc.transform.GetComponent<PlayerController>().health);
        }

        public void TakeDamage(float amount)
        {
            health -= amount;

            if (health <= 0f)
            {
                Die();
            }
        }

        void Die()
        {
            strength *= 1.01f;
            // Destroy(gameObject);
            Vector3 p1 = Random.insideUnitSphere * 3;
            Vector3 p2 = Random.insideUnitSphere * 3;
            var temp = Random.value;
            health = maxHealth;

            Instantiate(cube, transform.position + p1, Quaternion.identity);
            if (temp < 0.8)
            {
                Instantiate(cube, transform.position + p2, Quaternion.identity);
            }
            

            //gameObject.transform.position = new Vector3(-66, 0, 10);
            // health = 50f;
            Destroy(gameObject);
            pc.transform.GetComponent<PlayerController>().score += 5;

        }
}