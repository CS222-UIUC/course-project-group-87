using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactionPointRadius = 0.5f;
    [SerializeField] private LayerMask _interactableMask;
    [SerializeField] private InteractionPromptUI InteractionPromptUI;

    private readonly Collider[] _colliders = new Collider[3];
    [SerializeField] private int _numFound;

    private IInteractable interactable;

    private void Update() {
        _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRadius, _colliders, _interactableMask);

        if (_numFound > 0) {
            interactable = _colliders[0].GetComponent<IInteractable>();

            if (interactable != null) {
                if (!InteractionPromptUI.IsDisplayed) {
                    InteractionPromptUI.SetUp("Interact!");
                }

                if (Input.GetKeyDown("e")) {
                    interactable.Interact(this);
                }
            } 

            //if (interactable != null && Input.GetKeyDown("e")) {
            //   interactable.Interact(this);
            //}
        } else {
            if (interactable != null) {
                interactable = null;
            }

            if (InteractionPromptUI.IsDisplayed) {
                InteractionPromptUI.Close();
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);
    }
}
