using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raycasts
{
    public class Raycast3D
    {
        public static Vector3 RaycastPoint(Camera camera)
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            Physics.Raycast(ray, out hit);
            return hit.point;
        }
    }

}
