using UnityEngine;

public class Target : MonoBehaviour, IInteractable
{

    [SerializeField] private string _prompt;

    public string InteractionPrompt => _prompt;

    public float health = 30f;

    public void TakeDamage (float amount)
    {
        health -= amount;

        //if (health <= 0f) {
        //    Die();
        //}
    }

    void Die() {
        Destroy(gameObject);
    }

    public bool Interact(Interactor interactor) {
        Debug.Log("Shot target");
        return true;
    }
}