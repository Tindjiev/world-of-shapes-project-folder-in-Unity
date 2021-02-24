using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseComponent : MonoBehaviour
{

    private static Transform _mouse;
    public static Transform Mouse => (_mouse != null) ? _mouse : _mouse = GameObject.Find("mouse").transform;

    private void LateUpdate()
    {
        Vector3 tempPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        tempPosition.z = 0f;
        if ((tempPosition - transform.position).sqrMagnitude > 30f * 30f)
        {
            transform.position = tempPosition;
        }
        else
        {
            transform.position += (Vector3)MoveComponent.Follow(tempPosition, transform.position, Camera.main.orthographicSize / 3f);
            // at original camera size (15) its 5 meters per frame
        }
    }
}
