using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whiteboard : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;

    public string InteractionPrompt => _prompt;
    private Vector3 scaleChange, positionChange;
    public Material Material1;

    public bool Interact(Interactor interactor) {

        GetComponent<MeshRenderer>().material = Material1;

        Debug.Log("Entered password!");
        return true;

    }
}
