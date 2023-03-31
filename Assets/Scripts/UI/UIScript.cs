using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class UIScript : MonoBehaviour
{
    VisualElement root;
    public StraightRoadMainScript straightRoadMainScript;
    private void OnEnable()
    {
        root = gameObject.GetComponent<UIDocument>().rootVisualElement;

        Button roadButton = root.Q<Button>("roads-container-button");
        roadButton.RegisterCallback<ClickEvent>(Evt => { RoadButtonClicked(); });

        Button backButton = root.Q<Button>("back");
        backButton.RegisterCallback<ClickEvent>(Evt => { RoadButtonClicked(); });

        Button road1 = root.Q<Button>("road1");
        road1.RegisterCallback<ClickEvent>(Evt => {road1Clicked(); });

    }

    void clicked()
    {

    }

    void RoadButtonClicked()
    {
        VisualElement categories = root.Q<VisualElement>("categories");
        categories.visible = categories.visible == true ? false : true;

        categories.Q<Button>("roads-container-button").visible = categories.visible;

        VisualElement roadCategory = root.Q<VisualElement>("roadCategory");
        roadCategory.visible = roadCategory.visible == true ? false : true;
    }

    void road1Clicked()
    {
        straightRoadMainScript.isMouseHoldingRoad = true;
    }

}
