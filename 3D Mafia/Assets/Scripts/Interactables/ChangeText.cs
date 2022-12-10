using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChangeText : MonoBehaviour
{

    public TextMeshProUGUI changeText;

    // Update is called once per frame
    public void changeTextColor()
    {
        changeText.color = new Color32(255, 128, 0, 255);
    }
}
