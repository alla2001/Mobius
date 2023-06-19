using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public enum CameraMode
{
    GodModeGlobal,
    GodModeLocal,
    FollowCharacter
}

public class CameraController : MonoBehaviour
{   
    private float rotationY;
    private float rotationX;
    private Transform centerPoint;
    private GameObject playerRef;
    Vector3 rotationVector;
    Vector3 rotationUpdateVector;
    float mouseX;
    float mouseY;

    [Header("Camera Settings")]
    [SerializeField] float mouseSensitivity = 3.0f;
    [SerializeField] private float distanceToTarget = 10.0f;
    [SerializeField] private float zoomSpeed = 3.0f;

    [Header("Camera Modes")]
    [SerializeField]
    private CameraMode cameraMode;

    // Start is called before the first frame update
    void Start()
    {
        centerPoint = transform.parent;
        playerRef = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //centerPoint.position = targetTransform.position;
        distanceToTarget = Mathf.Clamp(distanceToTarget, 5, 15);
        transform.position = centerPoint.position - transform.forward * distanceToTarget;


        

        if (Input.GetMouseButton(1) && cameraMode != CameraMode.FollowCharacter)
        {
            mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * -1f;

            rotationX += mouseY;
            rotationY += mouseX;

            rotationX = Mathf.Clamp(rotationX, -90, 90);

            rotationVector = new Vector3(rotationX, rotationY);

            rotationUpdateVector = new Vector3(mouseY, mouseX, 0);
        }

        if (cameraMode == CameraMode.FollowCharacter)
        {
            centerPoint.position = playerRef.transform.position;
            centerPoint.rotation = playerRef.transform.rotation;
        }
        else if (cameraMode == CameraMode.GodModeLocal)
        {
            centerPoint.position = Vector3.zero;
            if (Input.GetMouseButton(1))
            {
                centerPoint.Rotate(rotationUpdateVector);
            }
        }
        else if (cameraMode == CameraMode.GodModeGlobal)
        {
            centerPoint.position = Vector3.zero;
            centerPoint.eulerAngles = rotationVector;
        }
        float scrollInput = Input.mouseScrollDelta.y;

        //Debug.Log(scrollInput);
        if (scrollInput > 0)
            distanceToTarget -= zoomSpeed * Time.deltaTime;
        if (scrollInput < 0)
            distanceToTarget += zoomSpeed * Time.deltaTime;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.tag == "Player")
            {
                cameraMode = CameraMode.FollowCharacter;
            }
            Transform objectHit = hit.transform;
        }

        //Debug.Log(centerPoint.position);
    }
}
