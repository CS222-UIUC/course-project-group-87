using UnityEngine;

public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;

    public Camera fpsCam;
    public ParticleSystem flash;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1")) {
            Shoot();
        }
    }

    void Shoot()
    {

        flash.Play();

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);
            

            ArtificialIntelligence enemy = hit.transform.GetComponent<ArtificialIntelligence>();
            if (enemy != null) {
                enemy.TakeDamage(damage);
                Debug.Log(enemy.health);
            }

            Target target = hit.transform.GetComponent<Target>();
            if (target != null) {
                target.TakeDamage(damage);
                
            }
        }
    }

}
