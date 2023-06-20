using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapePlacer : MonoBehaviour
{
    public GameObject Shape;
    public Transform levelHolder;
    public bool placingShape;
    GameObject tmp;
    public bool canPlace;
    private void Update()
    {
        if (placingShape && tmp==null) 
        {
            tmp = Instantiate(Shape);
            tmp.transform.parent = transform;
            tmp.transform.position = tmp.transform.position+transform.forward*5f;

        }
        if (Input.GetKeyDown(KeyCode.Space) && placingShape && canPlace)
        {
            tmp.transform.parent = levelHolder;


        }

    }

    private void OnTriggerEnter(Collider other)
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
    }

}
