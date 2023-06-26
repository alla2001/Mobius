using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapePlacer : MonoBehaviour
{
    [SerializeField] private List<GameObject> shapes;

    private Transform levelHolder;
    private bool placingShape;
    private bool rotatingShape;
    GameObject tmp;
    private bool canPlace;
    Vector2 mouseInput;

    private void Start()
    {
        placingShape = false;
        rotatingShape = false;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            placingShape = true;
        }

        if (placingShape && tmp==null) 
        {
            tmp = Instantiate(shapes[Random.Range(0, shapes.Count)]);
            tmp.transform.parent = transform;
            tmp.transform.position = transform.position+transform.forward*20f;

        }
        if (Input.GetKeyDown(KeyCode.Space) && placingShape)
        {
            tmp.transform.parent = levelHolder;
            placingShape = false;
            //tmp = null;
            rotatingShape = true;
        }

        if (rotatingShape)
        {
            mouseInput.x += Input.GetAxis("Mouse X");
            mouseInput.y += Input.GetAxis("Mouse Y");
            tmp.transform.localRotation = Quaternion.Euler(-mouseInput.y, mouseInput.x, 0);

            if (Input.GetKeyDown(KeyCode.F))
            {
                placingShape = false;
                rotatingShape = false;
                tmp = null;
            }
        }

    }

    /*private void OnTriggerEnter(Collider other)
    {
        canPlace = false;   
    }
    private void OnTriggerExit(Collider other)
    {

        canPlace=true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        canPlace = false;
    }
    private void OnCollisionExit(Collision collision)
    {
        canPlace = true;
    }*/

}
