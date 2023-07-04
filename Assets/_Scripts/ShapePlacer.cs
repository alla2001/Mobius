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
            tmp = Instantiate(shapes[Random.Range(0, shapes.Count)]);
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
            mouseInput.x += Input.GetAxis("Mouse X");
            mouseInput.y += Input.GetAxis("Mouse Y");
            tmp.transform.localRotation = Quaternion.Euler(-mouseInput.y, mouseInput.x, 0);

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
