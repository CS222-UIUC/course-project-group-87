using UnityEngine;
using TMPro;


public class InteractionPromptUI : MonoBehaviour
{
    //private Camera mainCam;
    [SerializeField] private Transform playerCamera = null;

    [SerializeField] private GameObject UIPanel;
    [SerializeField] private TextMeshProUGUI promptText;

    private void Start() {
        UIPanel.SetActive(false);
    }

    private void LateUpdate() {
        var rotation = playerCamera.transform.rotation;
        transform.LookAt(transform.position + rotation * Vector3.forward, rotation * Vector3.up);
    }

    public bool IsDisplayed = false;

    public void SetUp(string prompt_Text) {
        promptText.text = prompt_Text;
        UIPanel.SetActive(true);
        IsDisplayed = true;
    }

    public void Close() {
        UIPanel.SetActive(false);
        IsDisplayed = false;
    }

}
