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
    private bool rotationIsInLocal = false;

    [SerializeField]
    private float zoomSpeed = 3.0f;

    Transform centerPoint;

    // Start is called before the first frame update
    void Start()
    {
        centerPoint = transform.parent;
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

            Vector3 rotationVector = new Vector3(rotationX, rotationY);

            if (rotationIsInLocal)
                centerPoint.Rotate(mouseY, mouseX, 0);
            else
                centerPoint.localEulerAngles = rotationVector;
            //Vector3 nextRotation = new Vector3(rotationX, rotationY);
            //centerPoint.localEulerAngles = nextRotation;



            /*currentRotation = Vector3.SmoothDamp(currentRotation, nextRotation, ref smoothVelocity, smoothTime);

            transform.localEulerAngles = currentRotation;
            transform.position = centerPoint.position - transform.forward * distanceToTarget;*/

            float scrollInput = Input.mouseScrollDelta.y;

            if (scrollInput > 0)
                distanceToTarget -= zoomSpeed * Time.deltaTime;

            if (scrollInput < 0)
                distanceToTarget += zoomSpeed * Time.deltaTime; 
        }
    }
}
