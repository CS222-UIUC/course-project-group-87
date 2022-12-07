using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

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
    public TextMeshProUGUI foodText;
    public TextMeshProUGUI interactUIText;

    public CharacterController cc;
    public Canvas c1;
    public Canvas c2;
    public Canvas c3;
    public Canvas winScreen;
    public float restartDelay = 10f;

    private GameObject pc;
    
    private int cleanCount = 0;
    private bool cleaned = false;

    private bool foodReady = false;
    private bool foodCooking = false;
    private bool foodTaken = false;

    private bool buttonPressed = false;

    private bool scanned = false;

    private bool shotTarget = false;

    private bool Won = false;

    void Win() {
        Debug.Log("YOU WIN");
        Won = true;
    }

    void Start() {
        pc = GameObject.FindGameObjectsWithTag("Player")[0];
    }

    private void Update() {

        if (Won && Input.GetKeyDown("r")) {
            Won = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (cleaned && foodTaken && scanned && shotTarget && buttonPressed) {
            Invoke("Win", restartDelay);
            cc.enabled = false;
            c1.enabled = false;
            c2.enabled = false;
            c3.enabled = false;
            winScreen.enabled = true;
            pc.transform.GetComponent<PlayerController>().health = 1000000000f;
        }

        _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRadius, _colliders, _interactableMask);

        if (_numFound > 0) {
            interactable = _colliders[0].GetComponent<IInteractable>();
            

            if (interactable != null) {
                if (!InteractionPromptUI.IsDisplayed) {
                    if (!(interactable is Whiteboard && cleaned) && !(interactable is Ball && buttonPressed) && !(interactable is Scanner && scanned) && !(interactable is Food && foodTaken) && !(interactable is Target && ((Target)interactable).health != 30 && shotTarget)) {
                        InteractionPromptUI.SetUp("Interact!");
                    }
                    
                }

                if (interactable is Ball && !buttonPressed) {
                    if (!buttonPressed) {
                        interactUIText.SetText("Destroy power unit");
                        changeText = ballText;

                        if (Input.GetKeyDown("e")) {
                            interactable.Interact(this);
                            changeTextColor();
                            buttonPressed = true;
                            InteractionPromptUI.Close();
                        }
                    }
                    
                } else if (interactable is Scanner && !scanned) {
                    if (!scanned) {
                        changeText = scanText;
                        interactUIText.SetText("Scan");
                        
                        if (Input.GetKeyDown("e")) {
                            StartCoroutine(ScanWait());
                            
                        }
                    }
                    
                } else if (interactable is Whiteboard && !cleaned) {
                    if (!cleaned) {
                        changeText = whiteboardText;
                        interactUIText.SetText("Erase evidence");

                        if (Input.GetKeyDown("e")) {
                            cleanCount++;
                            Debug.Log(cleanCount);

                            if (cleanCount == 3) {
                                interactable.Interact(this);
                                changeTextColor();
                                cleaned = true;
                                InteractionPromptUI.Close();
                            }
                        }
                    }
                    
                } else if (interactable is Food && !foodTaken) {
                    changeText = foodText;

                    if (foodCooking) {
                        interactUIText.SetText("Decoding");
                    } else if (!foodReady) {
                        interactUIText.SetText("Start decoder");
                    } else if (foodReady) {
                        interactUIText.SetText("Steal password");
                    }

                    if (Input.GetKeyDown("e") && !foodReady) {
                        foodText.text = "   Decoding";
                        StartCoroutine(FoodWait());
                    } else if (Input.GetKeyDown("e") && foodReady) {
                        interactable.Interact(this);
                        changeTextColor();
                        foodTaken = true;
                        InteractionPromptUI.Close();
                    }
                } else if (interactable is Target && !shotTarget) {

                    Target t = (Target) interactable;
                    changeText = targetText;
                    interactUIText.SetText("Shoot calibration panel");

                    if (t.health < 30f) {
                        InteractionPromptUI.Close();
                        interactable.Interact(this);
                        changeTextColor();
                        shotTarget = true;
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
            scanned = false;
            yield return null;
        } else {
            interactable.Interact(this);
            changeTextColor();
            scanned = true;
            InteractionPromptUI.Close();
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

    IEnumerator FoodWait() {
        foodCooking = true;
        yield return new WaitForSeconds(20);
        foodCooking = false;
        foodReady = true;
        Debug.Log("Food is ready");
        foodText.text = "   Decoded!";
    }
}
