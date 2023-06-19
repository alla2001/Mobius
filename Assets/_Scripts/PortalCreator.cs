using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using Unity.VisualScripting;

public class PortalCreator : MonoBehaviour
{

    public bool inPortalCreationMode;
    public float maxPortalDistance;
    public GameObject hilightPrefab;
    public Camera mainCamera;
    public GameObject nodePrefab;
    public BridgePoint firstBridgePoint;
    public BridgePoint secondBridgePoint;
    public SplineComputer splineBridgeTop;
    public SplineComputer splineBridgeBottom;
    GameObject tempHilight;
    Ray r1;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(r1);
    }
    // Update is called once per frame
    void Update()
    {
        if (!inPortalCreationMode) return;
        Ray r = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(r, out hit))
        {
            if (firstBridgePoint != null && Vector3.Distance(firstBridgePoint.transform.position, hit.point) > maxPortalDistance)
            {
                if (tempHilight != null)
                    Destroy(tempHilight);
                return; 
            
            }

            if(tempHilight==null)
            {
                tempHilight = Instantiate(hilightPrefab);
            }
            tempHilight.transform.position = hit.point;
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }
            Destroy(tempHilight);
           
           
            if (!firstBridgePoint)
            {

             
                RaycastHit hit2;
                if (Physics.Raycast(hit.point - hit.normal*0.01f, -hit.normal, out hit2))
                {
                    r1.origin = hit.point - hit.normal * 0.01f;
                    r1.direction = -hit.normal;




                   

                    SplineComputer spline = hit.collider.GetComponent<SplineComputer>();
                    firstBridgePoint = new GameObject().AddComponent<BridgePoint>();

                    GameObject tempNode = Instantiate(nodePrefab);
                    tempNode.transform.position = hit.point;
                    tempNode.transform.position = spline.GetPoint(spline.PercentToPointIndex(spline.Project(hit.point).percent)).position;
                    tempNode.GetComponent<Node>().AddConnection(spline, spline.PercentToPointIndex(spline.Project(hit.point).percent));
                    
                    firstBridgePoint.topNode = tempNode.GetComponent<Node>();


                    spline = hit2.collider.GetComponent<SplineComputer>();

                    tempNode = Instantiate(nodePrefab);
                    tempNode.transform.position = hit2.point;
                    tempNode.transform.position = spline.GetPoint(spline.PercentToPointIndex(spline.Project(hit2.point).percent)).position;
                    tempNode.GetComponent<Node>().AddConnection(spline, spline.PercentToPointIndex(spline.Project(hit2.point).percent));
            
                    firstBridgePoint.bottomNode = tempNode.GetComponent<Node>();



                }
               

            }
            else
            {
                RaycastHit hit2;
                if (Physics.Raycast(hit.point - hit.normal * 0.01f, -hit.normal, out hit2))
                {
                    r1.origin = hit.point - hit.normal * 0.01f;
                    r1.direction = -hit.normal;
                    SplineComputer spline = hit.collider.GetComponent<SplineComputer>();
                    secondBridgePoint = new GameObject().AddComponent<BridgePoint>();

                    GameObject tempNode = Instantiate(nodePrefab);
                    tempNode.transform.position = hit.point;
                    tempNode.transform.position = spline.GetPoint(spline.PercentToPointIndex(spline.Project(hit.point).percent)).position;
                    tempNode.GetComponent<Node>().AddConnection(spline, spline.PercentToPointIndex(spline.Project(hit.point).percent));

                    secondBridgePoint.topNode = tempNode.GetComponent<Node>();


                    spline = hit2.collider.GetComponent<SplineComputer>();

                    tempNode = Instantiate(nodePrefab);
                    tempNode.transform.position = hit2.point;
                    tempNode.transform.position = spline.GetPoint(spline.PercentToPointIndex(spline.Project(hit2.point).percent)).position;
                    tempNode.GetComponent<Node>().AddConnection(spline, spline.PercentToPointIndex(spline.Project(hit2.point).percent));

                    secondBridgePoint.bottomNode = tempNode.GetComponent<Node>();

                   

                    splineBridgeTop = new GameObject().AddComponent<SplineComputer>();
                    splineBridgeBottom = new GameObject().AddComponent<SplineComputer>();
                    
                    SplinePoint[] points = new SplinePoint[2];

                    ///////////////////////////////////////////////////////////////////////////////////////
                    points[0] = new SplinePoint();
                    points[0].position = firstBridgePoint.topNode.GetPoint(0,false).position;
                    points[0].normal = Vector3.up;
                    points[0].size = 1f;
                    points[0].color = Color.white;

                    points[1] = new SplinePoint();
                    points[1].position = secondBridgePoint.topNode.GetPoint(0, false).position;
                    points[1].normal = Vector3.up;
                    points[1].size = 1f;
                    points[1].color = Color.white;

                    splineBridgeTop.SetPoints(points);
                    firstBridgePoint.topNode.AddConnection(splineBridgeTop, 0);
                    secondBridgePoint.topNode.AddConnection(splineBridgeTop, 1);
                    ///////////////////////////////////////////////////////////////////////////////////////

                    points = new SplinePoint[2];

                    points[0] = new SplinePoint();
                    points[0].position = firstBridgePoint.bottomNode.GetPoint(0, false).position;
                    points[0].normal = Vector3.up;
                    points[0].size = 1f;
                    points[0].color = Color.white;

                    points[1] = new SplinePoint();
                    points[1].position = secondBridgePoint.bottomNode.GetPoint(0, false).position;
                    points[1].normal = Vector3.up;
                    points[1].size = 1f;
                    points[1].color = Color.white;

                    splineBridgeBottom.SetPoints(points);
                    firstBridgePoint.bottomNode.AddConnection(splineBridgeBottom, 0);
                    secondBridgePoint.bottomNode.AddConnection(splineBridgeBottom, 1);

                }

            }
            return;
        }
        if (tempHilight != null)
            Destroy(tempHilight);



    }
    
}
