using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LocalCameraRotation : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 3.0f;

    private float rotationY;
    private float rotationX;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotationX += mouseY * -1f;
        rotationY += mouseX;

        Debug.Log("RotationX: " + rotationX);
        //Debug.Log("RotationY: " + rotationY);

        transform.RotateAround(transform.position, transform.right, mouseY);
        transform.RotateAround(transform.position, transform.up, mouseX);
    }
}
