using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;

    public string InteractionPrompt => _prompt;
    private Vector3 scaleChange, positionChange;

    public bool Interact(Interactor interactor) {
        Debug.Log("Scaling size of ball!");
        scaleChange = new Vector3(0.1f, 0.1f, 0.1f);
        this.transform.localScale += scaleChange;
        return true;
    }
}
