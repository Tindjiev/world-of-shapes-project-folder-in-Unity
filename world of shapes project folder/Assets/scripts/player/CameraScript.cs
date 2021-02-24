using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteAlways]
public class CameraScript : MonoBehaviour
{
    public static bool showPlayerInterface = true;

    [SerializeField]
    protected Transform _followtr;
    public Transform FollowTransform
    {
        get => _followtr;
        set => _followtr = value;
    }

    public const float _REGULAR_ORTHOGRAPHIC_SIZE = 15f;
    public const float MAX_ORTHOGRAPHIC_SIZE = 25f;
    public const float MIN_ORTHOGRAPHIC_SIZE = 5f;
    public const float BIRDSEYE_VIEW = 320f;

    protected void Awake()
    {
        if (gameObject.CompareTag("MainCamera"))
        {
            MyCameraLib.Camera = this;
            Application.targetFrameRate = 60;
        }
    }

    protected void Start()
    {
        _style.alignment = TextAnchor.UpperLeft;
        _style.normal.textColor = new Color(0.0f, 0.0f, 0.5f);
    }

    protected void LateUpdate()
    {
        if (FollowTransform != null)
        {
            transform.position = new Vector3(FollowTransform.position.x, FollowTransform.position.y, -10f);
        }

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            Camera.main.orthographicSize -= 2f * Input.GetAxis("Mouse ScrollWheel");
            if (FollowTransform != null)
            {
                if (Camera.main.orthographicSize > MAX_ORTHOGRAPHIC_SIZE)
                {
                    Camera.main.orthographicSize = MAX_ORTHOGRAPHIC_SIZE;
                }
                else if (Camera.main.orthographicSize < MIN_ORTHOGRAPHIC_SIZE)
                {
                    Camera.main.orthographicSize = MIN_ORTHOGRAPHIC_SIZE;
                }
            }
        }
    }


    protected GUIStyle _style = new GUIStyle();
    protected float _avg0 = 0f, _avg1 = 0f;

    protected void OnGUI()
    {
        _style.fontSize = Screen.width / 100;
        _avg0 += ((Time.deltaTime / Time.timeScale) - _avg0) * 0.03f;
        _avg1 += (_avg0 - _avg1) * 0.03f;
        GUI.Label(new Rect(0f, 0f, 0f, 0f), string.Format("{0:0.0} ms ({1} fps)", _avg1 * 1000f, Mathf.Ceil(1f / _avg1)), _style);
    }

}

public static class MyCameraLib
{
    public static CameraScript Camera;

    public static Vector3 Position => Camera.transform.position;

    public static void SetCameraPosition(Vector2 position)
    {
        Camera.transform.position = new Vector3(position.x, position.y, -10f);
    }

    public static void SetCameraFollow(Transform transform, float size)
    {
        UnityEngine.Camera.main.GetComponent<CameraScript>().FollowTransform = transform;
        UnityEngine.Camera.main.orthographicSize = size;
    }

    public static void SetCameraStaticPosition(in Vector2 position, float size)
    {
        Camera.transform.position = new Vector3(position.x, position.y, -10f);
        MyCameraLib.Camera.FollowTransform = null;
        UnityEngine.Camera.main.orthographicSize = size;
    }

}