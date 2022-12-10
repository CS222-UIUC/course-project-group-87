using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;

    public string InteractionPrompt => _prompt;
    private Vector3 scaleChange, positionChange;

    public bool Interact(Interactor interactor) {

        var inventory = interactor.GetComponent<Inventory>();

        //if (inventory == null) {
        //    return false;
        //}

        //if (inventory.HasKey) {
            Debug.Log("Scanning!");
            //scaleChange = new Vector3(0.1f, 0.1f, 0.1f);
            //this.transform.localScale += scaleChange;
            return true;
        //}

        //Debug.Log("No key found!");
        //return false;
    }
}
