using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapePlacer : MonoBehaviour
{
    private Transform levelHolder;
    private bool placingShape;
    private bool rotatingShape;
    GameObject tmp;
    private bool canPlace;
    Vector2 mouseInput;
    [SerializeField]
    private Vector3 OffsetToCamera; 

    private void Start()
    {
        GameManager.Instance.onStateChange.AddListener((state) => { if (state == GameState.ShapePlacement) { placingShape = true; } });

        placingShape = false;
        rotatingShape = false;
    }
    private void Update()
    {
        if (GameManager.Instance.currentState == GameState.ShapePlacement)
        {
            placingShape = true;
        }

        if (placingShape && tmp==null) 
        {
            GameObject shape = ShapeDatabase.instance.GenerateRandomShape(); 
            tmp = Instantiate(shape);
            tmp.GetComponent<MeshCollider>().enabled = false;
            tmp.transform.parent = transform;
            tmp.transform.localPosition = OffsetToCamera;

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
            Vector2 pastMouse = mouseInput;
            mouseInput.x += Input.GetAxis("Mouse X")*2f;
            mouseInput.y += Input.GetAxis("Mouse Y") * 2f;

            Vector2 delta = mouseInput - pastMouse;

            tmp.transform.Rotate(tmp.transform.InverseTransformDirection( Camera.main.transform.up), -delta.x);
            tmp.transform.Rotate(tmp.transform.InverseTransformDirection(Camera.main.transform.right), delta.y);
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (tmp.GetComponent<Shape>().canBePlaced)
                {
                    placingShape = false;
                    rotatingShape = false;
                    tmp.GetComponent<MeshCollider>().enabled = true;
                    GameManager.Instance.allWalls.Add(tmp);
                    GameManager.Instance.UpdateAveragePosition();
                    tmp = null;
                }
                else
                {
                    Debug.Log("Cant place the shape");
                }
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
