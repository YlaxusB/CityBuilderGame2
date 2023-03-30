using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonEvent : MonoBehaviour
{
    UIDocument buttonDocument;
    Button uiButton;

    private void OnEnable()
    {
        buttonDocument = GetComponent<UIDocument>();
        uiButton = buttonDocument.rootVisualElement.Q("testButton") as Button;
        uiButton.CaptureMouse();
        uiButton.RegisterCallback<MouseOverEvent>(Event => ButtonMouseOver());
    }

    private void ButtonMouseOver(){
        Debug.Log("over3");
    }

    private void OnDisable()
    {
        uiButton.UnregisterCallback<MouseOverEvent>(Event => ButtonMouseOver());
    }
}
