using UnityEngine;

public class Inventory : MonoBehaviour
{
    public bool HasKey = false;

    public void Update() {
        if (Input.GetKeyDown("q")) {
            HasKey = !HasKey;
        }
    }
}
