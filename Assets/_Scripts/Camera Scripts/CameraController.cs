using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{   
    private float rotationY;
    private float rotationX;
    private Transform centerPoint;
    private GameObject playerRef;

    [Header("Camera Settings")]
    [SerializeField] float mouseSensitivity = 3.0f;
    [SerializeField] private float distanceToTarget = 10.0f;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float zoomSpeed = 3.0f;

    [Header("Camera Modes")]
    [SerializeField]
    private bool rotationIsInLocal = false;
    [SerializeField]
    private bool moveAfterPlayer = false;

    // Start is called before the first frame update
    void Start()
    {
        centerPoint = transform.parent;
        playerRef = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (moveAfterPlayer)
        {
            targetTransform = playerRef.transform;
            centerPoint.rotation = playerRef.transform.rotation;
        }

        centerPoint.position = targetTransform.position;
        distanceToTarget = Mathf.Clamp(distanceToTarget, 5, 15);
        transform.position = centerPoint.position - transform.forward * distanceToTarget;

        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            rotationX += mouseY * -1f;
            rotationY += mouseX;

            rotationX = Mathf.Clamp(rotationX, -90, 90);

            Vector3 rotationVector = new Vector3(rotationX, rotationY);

            Vector3 rotationUpdateVector = new Vector3(mouseY * -1f, mouseX, 0);

            if (rotationIsInLocal)
                centerPoint.Rotate(rotationUpdateVector);
            else
                centerPoint.eulerAngles = rotationVector;
        }

        float scrollInput = Input.mouseScrollDelta.y;
        if (scrollInput > 0)
            distanceToTarget -= zoomSpeed * Time.deltaTime;
        if (scrollInput < 0)
            distanceToTarget += zoomSpeed * Time.deltaTime;
    }
}
