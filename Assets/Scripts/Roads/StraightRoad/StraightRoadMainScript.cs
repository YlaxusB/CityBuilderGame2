using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raycasts;
using StraightRoadMeshMainNamespace;
using System.Threading.Tasks;
using UnityEditor;
using System.Net;
using System;
using System.Security.Cryptography;
using UnityEditor.IMGUI.Controls;
using System.Drawing;
using System.Linq;
using Arcs;
using UnityEngine.UIElements;
using UnityEngine.XR;
using static UnityEditor.PlayerSettings;
using static UnityEngine.ParticleSystem;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
//using System.Numerics;

public class StraightRoadMainScript : MonoBehaviour
{
    // Start is called before the first frame update
    public float width = 1;
    public float lanes = 1;
    public bool oneWay = false;

    public GameObject mouse;
    public bool isMouseHoldingRoad;
    public bool isContinuing = false;
    public int clickIndex = 0;
    public int roadWidth = 2;
    public Vector3 roadStartPoint = new Vector3();
    public Vector3 roadEndPoint = new Vector3();

    public Camera camera;

    public GameObject previewObject;
    public GameObject preview;

    public GameObject reference1;
    public GameObject reference2;
    public GameObject reference3;

    public float lastRoadAngle;

    Coroutine roadPreviewCoroutine;

    public Vector3 testVector;

    public Transform startPoint;
    public Transform middlePoint;
    public Transform endPoint;
    public float curveHeight = 2.0f;
    public int numPoints = 20;
    public float arcHeight = 2.0f;
    public GameObject prefab;

    public GameObject continuationObject;

    public Transform testCube;

    public bool isMouseToRight; // Is mouse to the right of the start point
    public Material roadMaterial;

    // Suggested vector is the vector when the player mouse is over a road, this vector is the center of the road. (it should only be used for pre preview, before starting building the road)
    public Vector3 suggestedMouseVector;
    public bool isMouseOverRoad;
    public bool isBuildingWithIntersection;

    private RaycastHit mouseRaycastHit;

    public GameObject intersectingObject;
    public GameObject objectMouseOver;

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
                RaycastHit hit;
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    mouseRaycastHit = hit;
                };

                // Road Pre Preview
                if (clickIndex == 0)
                {
                    // Move Circle Preview and create its mesh
                    if (!isMouseOverRoad)
                    {
                        suggestedMouseVector = Raycast3D.RaycastPoint(camera) + new Vector3(0, 0.2f, 0);
                    }
                    roadStartPoint = suggestedMouseVector;
                    previewObject.transform.position = suggestedMouseVector; //Raycast3D.RaycastPoint(camera) + new Vector3(0, 0.1f, 0);
                    previewObject.GetComponent<MeshFilter>().mesh = StraightRoadMeshMain.CreatePrePreviewMesh(previewObject, Raycast3D.RaycastPoint(camera), 2);

                    if (Input.GetButtonDown("Fire1"))
                    {
                        intersectingObject = hit.transform.gameObject;
                        // Changed to click index 1 == road preview, with start and end
                        if (Physics.Raycast(ray))
                        {
                            //roadStartPoint = hit.point;
                            clickIndex++;
                        }
                    }
                }
                // Road Straight Preview, with startPoint already set and endPoint being set by a raycast
                else if (clickIndex == 1)
                {
                    if (isBuildingWithIntersection)
                    {
                        //Debug.Log("Building over intersection");
                        UpdatePreview();
                        // Assume the first rectangle is named "outerRectangle" and the second is named "innerRectangle"
                        // Assume the first rectangle is named "outerRectangle" and the second is named "innerRectangle"
                        Collider innerCollider = intersectingObject.GetComponent<Collider>();
                        Collider outerCollider = previewObject.GetComponent<Collider>();

                        float raycastDistance = Vector3.Distance(previewObject.transform.position, hit.point);

                        RaycastHit leftHit;
                        Ray leftRay = new Ray(previewObject.transform.position + previewObject.transform.TransformDirection(new Vector3(raycastDistance, 0, roadWidth)), -previewObject.transform.right);

                        RaycastHit rightHit;
                        Ray rightRay = new Ray(previewObject.transform.position + previewObject.transform.TransformDirection(new Vector3(raycastDistance, 0, -roadWidth)), -previewObject.transform.right);

                        if (innerCollider.Raycast(leftRay, out leftHit, raycastDistance) && innerCollider.Raycast(rightRay, out rightHit, raycastDistance))
                        {
                            testCube.transform.position = leftHit.point;
                            Mesh newMesh = previewObject.GetComponent<MeshFilter>().mesh;
                            Vector3[] vertices = newMesh.vertices;
                            vertices[0] = previewObject.transform.InverseTransformPoint(leftHit.point);
                            vertices[1] = previewObject.transform.InverseTransformPoint(rightHit.point);
                            newMesh.vertices = vertices;
                            newMesh.RecalculateBounds();
                            newMesh.RecalculateNormals();
                            previewObject.GetComponent<MeshFilter>().mesh = newMesh;
                        }



                        if (Input.GetButtonDown("Fire1"))
                        {
                            // Build the road and the arc connecting the two roads
                            roadEndPoint = hit.point;
                            CreateStraightRoad();
                            isBuildingWithIntersection = false;
                            // Reset click index
                            clickIndex = 0;
                            // Create the continuation arc
                            GameObject instantiateContinuation = Instantiate(continuationObject);
                            MeshCollider instantiateContinuationMeshCollider = instantiateContinuation.AddComponent<MeshCollider>();
                            instantiateContinuationMeshCollider.convex = true;

                            // Start the continuation road function
                            ContinueRoad();
                        }
                    }
                    else
                    {
                        if (!isContinuing)
                        {
                            // Not a continuation of a road, probably a isolated road
                            UpdatePreview();
                            // Loop to show the road preview && wait for next click
                            if (Input.GetButtonDown("Fire1"))
                            {
                                // Changed to click index 2 == build road, destroy preview, reset click index
                                if (Physics.Raycast(ray))
                                {
                                    roadEndPoint = hit.point;
                                    CreateStraightRoad();
                                    isBuildingWithIntersection = false;
                                    // Reset click index
                                    clickIndex = 0;
                                    // Start the continuation road function
                                    ContinueRoad();
                                }
                            }
                        }
                        else
                        {
                            // Continuation of a road
                            UpdatePreview();

                            if (Input.GetButtonDown("Fire1"))
                            {
                                // Build the road and the arc connecting the two roads
                                roadEndPoint = hit.point;
                                CreateStraightRoad();
                                isBuildingWithIntersection = false;
                                // Reset click index
                                clickIndex = 0;
                                // Create the continuation arc
                                GameObject instantiateContinuation = Instantiate(continuationObject);
                                MeshCollider instantiateContinuationMeshCollider = instantiateContinuation.AddComponent<MeshCollider>();
                                instantiateContinuationMeshCollider.convex = true;

                                // Start the continuation road function
                                ContinueRoad();
                            }
                        }
                    }

                }

                // Right Click decreases clickIndex by 1
                if (Input.GetButtonDown("Fire2"))
                {
                    clickIndex--;
                    isContinuing = false;
                    EnableArcPreview(false);
                    if (clickIndex <= -1)
                    {
                        clickIndex = 0;
                        isMouseHoldingRoad = false;
                        isBuildingWithIntersection = false;
                    }
                }

                // Disable or Enable the preview object if mouse is holding a road
                if (isMouseHoldingRoad == true)
                {
                    previewObject.SetActive(true);
                }
                else { previewObject.SetActive(false); }
            }
            yield return null; // wait for next frame update
        }
    }

    private void UpdatePreview()
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit);

        // Move the preview object to start position
        previewObject.transform.position = roadStartPoint;
        // Get the angle and rotate the straight road
        float angle = -Mathf.Atan2(Raycast3D.RaycastPoint(camera).z - roadStartPoint.z, Raycast3D.RaycastPoint(camera).x - roadStartPoint.x) * (180 / Mathf.PI);
        previewObject.transform.localRotation = Quaternion.Euler(0, angle, 0);
        // Create and assign the straight road mesh
        previewObject.GetComponent<MeshFilter>().mesh = StraightRoadMeshMain.BuildMeshAlongLocalPoints(new List<Vector3>() { roadStartPoint, Raycast3D.RaycastPoint(camera) + new Vector3(0, 0.1f, 0) }, 2);

        // If its a road continuation then also updates the arc
        if (isContinuing)
        {
            Transform activeReference = GetActiveReference(hit);

            // calculate the start, middle, and end positions
            Vector3 startPoint = reference1.transform.position;
            Vector3 centerPoint = activeReference.transform.position;
            Vector3 endPoint = previewObject.transform.position;
            Vector3 secondVertice = previewObject.transform.position;

            float arcAngle;
            bool reverseNormals;

            List<Vector2> points;

            // 
            if (isMouseToRight)
            {
                // Second vertice is the extremity of the second road
                secondVertice += activeReference.TransformDirection(new Vector3(0, 0, roadWidth));
                arcAngle = Math.Abs(Vector3.Angle(centerPoint - secondVertice, reference3.transform.TransformDirection(0, 0, roadWidth)) - 180);
                points = ArcScript.GetArcPoints(arcAngle, roadWidth / 2, -roadWidth, true);
                points.Reverse();
                continuationObject.transform.rotation = Quaternion.Euler(90, lastRoadAngle - 90, 180);
                continuationObject.transform.Rotate(new Vector3(0f, 180f, 0f), Space.Self);
                reverseNormals = true;
            }
            else
            {
                secondVertice += activeReference.TransformDirection(new Vector3(0, 0, -roadWidth));
                arcAngle = Vector3.Angle(centerPoint - secondVertice, reference3.transform.TransformDirection(0, 0, roadWidth * 2));

                points = ArcScript.GetArcPoints(arcAngle, roadWidth / 2, -roadWidth, false);
                continuationObject.transform.rotation = Quaternion.Euler(90, lastRoadAngle - 90, 0);
                reverseNormals = false;
            }

            continuationObject.GetComponent<MeshFilter>().mesh = StraightRoadMeshMain.CreateMeshAlongPointsB(points, roadWidth, reverseNormals);
            continuationObject.transform.position = new Vector3(activeReference.position.x, 0.2f, activeReference.position.z);

            activeReference.transform.localRotation = Quaternion.Euler(0, angle, 0);
            // Create and assign the straight road mesh
            previewObject.GetComponent<MeshFilter>().mesh = StraightRoadMeshMain.BuildMeshAlongLocalPoints(new List<Vector3>() { roadStartPoint, hit.point + new Vector3(0, 0.1f, 0) }, roadWidth);

            previewObject.transform.localRotation = Quaternion.Euler(0, angle, 0);
            previewObject.transform.position = activeReference.transform.position;
            //MoveAccordingToRotation(previewObject.transform, previewObject.transform, new Vector3(roadWidth, 0, 0));
            Vector3 moveTo = activeReference.name == "ReferencePreview1" ? new Vector3(0, 0, roadWidth) : new Vector3(0, 0, -roadWidth);
            MoveAccordingToRotation(activeReference.transform, previewObject.transform, moveTo);

            roadEndPoint = hit.point;
            // Build the road and the continuation arc, connecting the old road (first) and the new road (second)
            roadStartPoint = previewObject.transform.position;
        }
    }

    // Create the final road
    public void CreateStraightRoad()
    {
        // Creathe the object and assign its components
        GameObject newRoad = new GameObject();
        MeshRenderer newRoadMeshRenderer = newRoad.AddComponent<MeshRenderer>();
        newRoadMeshRenderer.material = roadMaterial;
        MeshFilter newRoadMeshFilter = newRoad.AddComponent<MeshFilter>();
        // Build the mesh along the given points
        Mesh newRoadMesh = StraightRoadMeshMain.BuildMeshAlongLocalPoints(new List<Vector3>() { roadStartPoint, roadEndPoint }, 2);
        newRoadMeshFilter.mesh = newRoadMesh;



        // If this road is starting from an intersection/junction
        if (isBuildingWithIntersection)
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out hit);

            float raycastDistance = Vector3.Distance(previewObject.transform.position, hit.point);
            RaycastHit leftHit;
            Ray leftRay = new Ray(previewObject.transform.position + previewObject.transform.TransformDirection(new Vector3(raycastDistance, 0, roadWidth)), -previewObject.transform.right);

            RaycastHit rightHit;
            Ray rightRay = new Ray(previewObject.transform.position + previewObject.transform.TransformDirection(new Vector3(raycastDistance, 0, -roadWidth)), -previewObject.transform.right);

            Collider intersectionCollider = intersectingObject.GetComponent<Collider>();
            if (intersectionCollider.Raycast(leftRay, out leftHit, raycastDistance) && intersectionCollider.Raycast(rightRay, out rightHit, raycastDistance))
            {
                testCube.transform.position = leftHit.point;
                Mesh newMesh = newRoadMeshFilter.mesh;
                Vector3[] vertices = newMesh.vertices;
                vertices[0] = previewObject.transform.InverseTransformPoint(leftHit.point);
                vertices[1] = previewObject.transform.InverseTransformPoint(rightHit.point);
                newMesh.vertices = vertices;
                newMesh.RecalculateBounds();
                newMesh.RecalculateNormals();
                newRoadMeshFilter.mesh = newMesh;
            }
        }

        // Positionate the road and rotate
        newRoad.transform.position = new Vector3(roadStartPoint.x, 0.2f, roadStartPoint.z);
        float newRoadAngle = -Mathf.Atan2(roadEndPoint.z - roadStartPoint.z, roadEndPoint.x - roadStartPoint.x) * (180 / Mathf.PI);
        newRoad.transform.localRotation = Quaternion.Euler(0, newRoadAngle, 0);
        newRoad.name = "Road";


        // Create a collider for the road
        RoadColliderScript newRoadColliderScript = newRoad.AddComponent<RoadColliderScript>();
        BoxCollider newRoadMeshCollider = newRoad.AddComponent<BoxCollider>();
        //newRoadMeshCollider.sharedMesh = newRoadMesh;
        //newRoadMeshCollider.convex = true;
        newRoadColliderScript.mainScript = gameObject.GetComponent<StraightRoadMainScript>();
        newRoadColliderScript.camera = camera;
        newRoadColliderScript.roadStartPoint = roadStartPoint;
        newRoadColliderScript.roadEndPoint = roadEndPoint;
        newRoadColliderScript.previewTransform = previewObject.transform;


        lastRoadAngle = newRoadAngle;
    }

    // Get the active reference for making the arc, this depends on the mouse, if the mouse is to left then return the left reference
    public Transform GetActiveReference(RaycastHit hit)
    {
        if (reference3.transform.InverseTransformPoint(hit.point).z >= 0)
        {
            isMouseToRight = false;
            return reference2.transform;
        }
        else
        {
            isMouseToRight = true;
            return reference1.transform;
        }

    }

    public void ContinueRoad()
    {
        Vector3 oldStartPoint = roadStartPoint;
        roadStartPoint = roadEndPoint;

        // Reference 1 is the left, 2 is the right, 3 is the center

        // Position references
        reference1.transform.localRotation = Quaternion.Euler(0, AngleBetweenTwo3DVectors(roadEndPoint, oldStartPoint) + 180, 0);
        reference1.transform.position = roadEndPoint;
        MoveAccordingToRotation(reference1.transform, reference1.transform, new Vector3(0, 0, -roadWidth));

        reference2.transform.localRotation = Quaternion.Euler(0, AngleBetweenTwo3DVectors(roadEndPoint, oldStartPoint) + 180, 0);
        reference2.transform.position = roadEndPoint;
        MoveAccordingToRotation(reference2.transform, reference2.transform, new Vector3(0, 0, roadWidth));

        reference3.transform.localRotation = Quaternion.Euler(0, AngleBetweenTwo3DVectors(roadEndPoint, oldStartPoint) + 180, 0);
        reference3.transform.position = roadEndPoint;

        isContinuing = true;
        EnableArcPreview(true);

        // Stop conventional road update coroutine and starts a new, for the continuation preview/road



        clickIndex = 1;
    }

    public void EnableArcPreview(bool enable)
    {
        reference1.GetComponent<MeshRenderer>().enabled = enable;
        reference2.GetComponent<MeshRenderer>().enabled = enable;
        reference3.GetComponent<MeshRenderer>().enabled = enable;
        continuationObject.GetComponent<MeshRenderer>().enabled = enable;
    }

    public void MoveAccordingToRotation(Transform reference, Transform objectToMove, Vector3 moveTo)
    {
        Vector3 worldDirection = reference.TransformDirection(moveTo);
        objectToMove.position += worldDirection;
    }

    // Angle between two vectors, returns angle in degrees
    public float AngleBetweenTwo3DVectors(Vector3 start, Vector3 end)
    {
        return -Mathf.Atan2(end.z - start.z, end.x - start.x) * (180 / Mathf.PI);
    }


}
