using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 50f;
    public Transform transform;

    public void TakeDamage (float amount)
    {
        health -= amount;

        if (health <= 0f) {
            Die();
        }
    }

    void Die() {
        //Destroy(gameObject);
        //Transform TeleportGoal = new Transform;
        //TeleportGoal.position = new Vector3(-66, 0, 10);
        //gameObject.transform.position = TeleportGoal.position;
        //gameObject.NetworkTransform.position = new Vector3(-66, 0, 10);

        //CharacterController _controller = GetComponent<CharacterController>();
        //Vector3 velocity = new Vector3(0, 10, 0);
        //_controller.Move(velocity);

        transform.position = new Vector3(-66, 0, 10);

        health = 50f;
    }
}
