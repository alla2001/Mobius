using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
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
        if (placingShape)
        {
            
            placingShape = true;
            //tmp = null;
            rotatingShape = true;
       

            if (Input.GetMouseButton(0))
            {
                Vector2 pastMouse = mouseInput;
                mouseInput.x += Input.GetAxis("Mouse X")*2f;
                mouseInput.y += Input.GetAxis("Mouse Y") * 2f;

                Vector2 delta = mouseInput - pastMouse;

                tmp.transform.Rotate(tmp.transform.InverseTransformDirection( Camera.main.transform.up), -delta.x);
                tmp.transform.Rotate(tmp.transform.InverseTransformDirection(Camera.main.transform.right), delta.y);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (tmp.GetComponent<Shape>().canBePlaced)
                {
                    tmp.transform.parent = levelHolder;
                    placingShape = false;
                    rotatingShape = false;
                    tmp.GetComponent<MeshCollider>().enabled = true;
                    foreach (var item in tmp.GetComponentsInChildren<SplineComputer>())
                    {
                        item.RebuildImmediate();
                    }
                    GameManager.Instance.allWalls.Add(tmp);
                    GameManager.Instance.UpdateAveragePosition();
                    GameManager.Instance.ChangeState(GameState.RewardMode);
                    ItemSpawner.instace.shapes.Add(tmp);
                 
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
