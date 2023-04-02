using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raycasts;
using System;

public class RoadColliderScript : MonoBehaviour
{
    public StraightRoadMainScript mainScript;
    public Transform mainUI;
    public Camera camera;
    public Vector3 roadStartPoint;
    public Vector3 roadEndPoint;
    public Transform previewTransform;

    private void OnMouseOver()
    {
        //mainScript = mainUI.GetComponent<StraightRoadMainScript>();
        Vector3 mouseRayCast = Raycast3D.RaycastPoint(camera);
        Vector3 mousePointInRelationToRoad = transform.InverseTransformDirection(mouseRayCast - roadStartPoint);
        Vector3 suggestedVector = roadStartPoint + transform.TransformDirection(mousePointInRelationToRoad.x, 0.2f, 0);
        mainScript.suggestedMouseVector = suggestedVector;
        mainScript.isMouseOverRoad = true;
        mainScript.objectMouseOver = gameObject;
        if (Input.GetButtonDown("Fire1"))
        {
            mainScript.isBuildingWithIntersection = true;
        }
        //GameObject.Find("cubeTesting").transform.position = suggestedVector;
    }

    private void OnMouseExit()
    {
        mainScript.isMouseOverRoad = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
