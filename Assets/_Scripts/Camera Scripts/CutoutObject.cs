using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutoutObject : MonoBehaviour
{
    [SerializeField]
    private Transform targetObject;
    public float falloff;
    public float size;
    [SerializeField]
    private LayerMask wallMask;

    private Camera mainCamera;
   
    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        Vector2 cutoutPos = mainCamera.WorldToViewportPoint(targetObject.position);
        cutoutPos.y /= (Screen.width / Screen.height);

        Vector3 offset = targetObject.position - transform.position;
        RaycastHit[] hitObjects = Physics.SphereCastAll(transform.position,0.5f, offset, offset.magnitude-0.52f, wallMask);
        GameObject[] objectsInLayer = GameObject.FindGameObjectsWithTag("wall");
        foreach (GameObject obj in objectsInLayer)
        {
            Material[] materials = obj.transform.GetComponent<Renderer>().materials;

            foreach (Material mat in materials)
            {
                print(obj.name);
               
                mat.SetFloat("_CutoutSize", 0);
                mat.SetFloat("_FalloffSize", 0);
            }
        }
        for (int i = 0; i < hitObjects.Length; ++i)
        {
            Material[] materials = hitObjects[i].transform.GetComponent<Renderer>().materials;

            foreach (Material mat in materials)
            {
                mat.SetVector("_CutoutPos", cutoutPos);
                mat.SetFloat("_CutoutSize", size);
                mat.SetFloat("_FalloffSize", falloff);
            }
        }

        
        //Debug.Log(cutoutPos);
    }
}
