using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 3.0f;

    private float rotationY;
    private float rotationX;
    private Vector3 currentRotation;
    private Vector3 smoothVelocity = Vector3.zero;

    [SerializeField]
    private Transform targetTransform;
    [SerializeField]
    private float distanceToTarget = 10.0f;
    [SerializeField]
    private float smoothTime = 3.0f;

    [SerializeField]
    private float zoomSpeed = 3.0f;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        distanceToTarget = Mathf.Clamp(distanceToTarget, 5, 15);

        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            rotationX += mouseY * -1f;
            rotationY += mouseX;

            rotationX = Mathf.Clamp(rotationX, -90, 90);

            Vector3 nextRotation = new Vector3(rotationX, rotationY);
            currentRotation = Vector3.SmoothDamp(currentRotation, nextRotation, ref smoothVelocity, smoothTime);

            transform.localEulerAngles = currentRotation;
            transform.position = targetTransform.position - transform.forward * distanceToTarget;

            float scrollInput = Input.mouseScrollDelta.y;

            //float nextDistance = 

            // Check if there was a scroll up event
            if (scrollInput > 0)
            {
                distanceToTarget -= zoomSpeed * Time.deltaTime;
            }

            // Check if there was a scroll down event
            if (scrollInput < 0)
            {
                distanceToTarget += zoomSpeed * Time.deltaTime; 
            }
        }
    }
}
