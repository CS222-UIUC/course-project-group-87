using UnityEngine;
using TMPro;
using System.Collections;

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactionPointRadius = 0.5f;
    [SerializeField] private LayerMask _interactableMask;
    [SerializeField] private InteractionPromptUI InteractionPromptUI;

    private readonly Collider[] _colliders = new Collider[3];
    [SerializeField] private int _numFound;

    private IInteractable interactable;
    private TextMeshProUGUI changeText;
    public TextMeshProUGUI ballText;
    public TextMeshProUGUI scanText;
    public TextMeshProUGUI whiteboardText;
    public TextMeshProUGUI targetText;
    public TextMeshProUGUI interactUIText;

    private int cleanCount = 0;

    private void Update() {
        _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRadius, _colliders, _interactableMask);

        if (_numFound > 0) {
            interactable = _colliders[0].GetComponent<IInteractable>();
            

            if (interactable != null) {
                if (!InteractionPromptUI.IsDisplayed) {
                    InteractionPromptUI.SetUp("Interact!");
                }

                if (interactable is Ball) {
                    interactUIText.SetText("Interact");
                    changeText = ballText;

                    if (Input.GetKeyDown("e")) {
                        interactable.Interact(this);
                        changeTextColor();
                    }
                } else if (interactable is Scanner) {
                    changeText = scanText;
                    interactUIText.SetText("Hold to interact");
                    
                    if (Input.GetKeyDown("e")) {
                        StartCoroutine(ScanWait());
                    }
                } else if (interactable is Whiteboard) {
                    changeText = whiteboardText;
                    interactUIText.SetText("Press E 3 times to interact");

                    if (Input.GetKeyDown("e")) {
                        cleanCount++;
                        Debug.Log(cleanCount);

                        if (cleanCount == 3) {
                            interactable.Interact(this);
                            changeTextColor();
                        }
                    }
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

    public void changeTextColor()
    {
        changeText.color = new Color32(0, 255, 0, 255);
    }

    IEnumerator ScanWait ()
    {
        yield return new WaitForSeconds(3);
        
        if (interactable == null || !(interactable is Scanner)) {
            Debug.Log("Failed to scan!");
            yield return null;
        } else {
            interactable.Interact(this);
            changeTextColor();
        }
        
    }

    IEnumerator WhiteboardWait() {
        yield return new WaitForSeconds(2);

        if (Input.GetKeyDown("e")) {
            yield return new WaitForSeconds(2);

            if (Input.GetKeyDown("e")) {
                interactable.Interact(this);
                changeTextColor();
            } else {
                Debug.Log("Failed to clean whiteboard!");
                yield return null;
            }
        } else {
            Debug.Log("Failed to clean whiteboard!");
            yield return null;
        }
    }
}
