using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutoutObject : MonoBehaviour
{
    [SerializeField]
    private Transform targetObject;

    [SerializeField]
    private LayerMask wallMask;

    private Camera mainCamera;
    public List<Material> materials;
    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        Vector2 cutoutPos = mainCamera.WorldToViewportPoint(targetObject.position);
        cutoutPos.y /= (Screen.width / Screen.height);

        Vector3 offset = targetObject.position - transform.position;
        RaycastHit[] hitObjects = Physics.RaycastAll(transform.position, offset, offset.magnitude, wallMask);

        //for (int i = 0; i < hitObjects.Length; ++i)
        //{
            //Material[] materials = hitObjects[i].transform.GetComponent<Renderer>().materials;

           foreach(Material mat in materials)
            {
                mat.SetVector("_CutoutPos", cutoutPos);
                mat.SetFloat("_CutoutSize", 0.08f);
                mat.SetFloat("_FalloffSize", 0.05f);
            }

        Debug.Log(cutoutPos);
    }
}
