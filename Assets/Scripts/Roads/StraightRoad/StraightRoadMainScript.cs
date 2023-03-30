using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raycasts;
using StraightRoadMeshMain;
//using System.Numerics;

public class StraightRoadMainScript : MonoBehaviour
{
    // Start is called before the first frame update
    public float width = 1;
    public float lanes = 1;
    public bool oneWay = false;

    public GameObject mouse;
    public bool isMouseHoldingRoad;
    public int clickIndex = 0;
    public Vector3 roadStartPoint = new Vector3();
    public Vector3 roadEndPoint = new Vector3();

    public Camera camera;

    public GameObject previewObject;

    Coroutine roadPreviewCoroutine;
    void OnEnable()
    {
        isMouseHoldingRoad = mouse.GetComponent<MouseRoadsHandler>().isMouseHoldingRoad;
        roadPreviewCoroutine = StartCoroutine(RoadPreviewCoroutine());
    }

    IEnumerator RoadPreviewCoroutine()
    {
        while (true)
        {
            if (isMouseHoldingRoad)
            {
                // Road Pre Preview
                if (clickIndex == 0)
                {
                    // Move Circle Preview and create its mesh
                    previewObject.transform.position = Raycast3D.RaycastPoint(camera) + new Vector3(0, 0.1f, 0); 
                    previewObject.GetComponent<MeshFilter>().mesh = StraightRoadMeshMain.StraightRoadMeshMain.CreatePrePreviewMesh(previewObject, Raycast3D.RaycastPoint(camera), 2);

                    if (Input.GetButtonDown("Fire1"))
                    {
                        // Changed to click index 1 == road preview, with start and end
                        RaycastHit hit;
                        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

                        if (Physics.Raycast(ray, out hit))
                        {
                            roadStartPoint = hit.point;
                            clickIndex++;
                        }
                    }
                }
                // Road Straight Preview, with startPoint already set and endPoint being set by a raycast
                else if (clickIndex == 1)
                {
                    // Move the preview object to start position
                    previewObject.transform.position = roadStartPoint;
                    // Get the angle and rotate the straight road
                    float angle = -Mathf.Atan2(Raycast3D.RaycastPoint(camera).z - roadStartPoint.z, Raycast3D.RaycastPoint(camera).x - roadStartPoint.x) * (180 / Mathf.PI);
                    previewObject.transform.localRotation = Quaternion.Euler(0, angle, 0);
                    // Create and assign the straight road mesh
                    previewObject.GetComponent<MeshFilter>().mesh = StraightRoadMeshMain.StraightRoadMeshMain.BuildMeshAlongLocalPoints(new List<Vector3>() { roadStartPoint, Raycast3D.RaycastPoint(camera) + new Vector3(0, 0.1f, 0) }, 2);
                    // Loop to show the road preview && wait for next click
                    if (Input.GetButtonDown("Fire1"))
                    {
                        // Changed to click index 2 == build road, destroy preview, reset click index
                        RaycastHit hit;
                        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                        if (Physics.Raycast(ray, out hit))
                        {
                            roadEndPoint = hit.point;
                            // Create the final road
                            GameObject newRoad = new GameObject();
                            MeshRenderer newRoadMeshRenderer = newRoad.AddComponent<MeshRenderer>();
                            MeshFilter newRoadMeshFilter = newRoad.AddComponent<MeshFilter>();
                            Mesh newRoadMesh = StraightRoadMeshMain.StraightRoadMeshMain.BuildMeshAlongLocalPoints(new List<Vector3>() { roadStartPoint, roadEndPoint }, 2);
                            newRoadMeshFilter.mesh = newRoadMesh;
                            newRoad.transform.position = new Vector3(roadStartPoint.x, 0.2f, roadStartPoint.z);
                            float newRoadAngle = -Mathf.Atan2(roadEndPoint.z - roadStartPoint.z, roadEndPoint.x - roadStartPoint.x) * (180 / Mathf.PI);
                            newRoad.transform.localRotation = Quaternion.Euler(0, newRoadAngle, 0);
                            newRoad.name = "Road";
                            // Reset click index
                            clickIndex = 0;
                        }
                    }
                }

                // Right Click decreases clickIndex by 1
                if (Input.GetButtonDown("Fire2"))
                {
                    clickIndex--;
                    if(clickIndex <= -1)
                    {
                        clickIndex = 0;
                        isMouseHoldingRoad = false;
                    }
                }
            }
            yield return null; // wait for next frame update
        }
    }


    // Update is called once per frame
    void Update()
    {

    }
}
