using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whiteboard : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;

    public string InteractionPrompt => _prompt;
    private Vector3 scaleChange, positionChange;

    public bool Interact(Interactor interactor) {

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        Debug.Log("Entered password!");
        return true;

    }
}
