using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseRoadsHandler : MonoBehaviour
{
    public bool isMouseHoldingRoad = true;
    public Camera camera;
    public GameObject UIMain;
    // Start is called before the first frame update
    void Start()
    {

    }

    private Coroutine myCoroutine;
    // Update is called once per frame
/*    void Update()
    {
        if (isMouseHoldingRoad && Input.GetButtonDown("Fire1"))
        {
            StraightRoadMainScript straightScript = UIMain.GetComponent<StraightRoadMainScript>();
            int clickIndex = straightScript.clickIndex;
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                // Check if this is the first or second click, so it can start the road preview or build the road
                if (clickIndex == 0) { straightScript.roadStartPoint = hit.point; } else if (clickIndex == 1) { straightScript.roadEndPoint = hit.point; }
                straightScript.clickIndex = clickIndex >= 1 ? 0 : clickIndex + 1;

                if (clickIndex >= 1)
                {
                    // Stop the coroutine using the reference
                    if (myCoroutine != null)
                    {
                        StopCoroutine(myCoroutine);
                    }
                }
                else
                {
                    // Start the coroutine and store the reference
                    myCoroutine = StartCoroutine(MyCoroutine(straightScript));
                }
                // Loop to roadEndPoint follow mouse when clickIndex == 1
                //StartCoroutine(MyCoroutine(straightScript.roadEndPoint));
            }
        }
    }*/

    IEnumerator MyCoroutine(StraightRoadMainScript straightScript)
    {
        while (true)
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out hit);
            straightScript.roadEndPoint = hit.point;
            yield return null; // wait for next frame update
        }
    }

    private void MouseRoadClicked()
    {
        Debug.Log("Mouse Road Clicked");
    }
}


