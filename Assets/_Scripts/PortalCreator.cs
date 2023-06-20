using Dreamteck.Splines;
using UnityEngine;

public class PortalCreator : MonoBehaviour
{
    public bool inPortalCreationMode;
    public float maxPortalDistance;
    public GameObject hilightPrefab;
    public Camera mainCamera;
    public GameObject nodePrefab;

    public Node firstBridgePoint;
    public Node secondBridgePoint;

    private SplineComputer splineBridge;

    public SplineComputer firstSpline;
    private SplineComputer secondSpline;
    public GameObject bridge;

    private GameObject tempHilight;
    
    public LayerMask ignoreMask;
    // Start is called before the first frame update
    private void Start()
    {
    }

    private void OnDrawGizmos()
    {
       
    }

    // Update is called once per frame
    private void Update()
    {
        //print(firstSpline.GetComponent<SplineMesh>().GetChannel(0).minScale.x);
        
        if (!inPortalCreationMode) return;
        Ray r = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(r, out hit , 1000,~ignoreMask))
        {
            if (firstBridgePoint != null && Vector3.Distance(firstBridgePoint.transform.position, hit.point) > maxPortalDistance)
            {
                if (tempHilight != null)
                    Destroy(tempHilight);
                return;
            }

            if (tempHilight == null)
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
                firstSpline = hit.collider.GetComponent<SplineComputer>();

                firstBridgePoint = Instantiate(nodePrefab).GetComponent<Node>();

                firstBridgePoint.transform.position = firstSpline.GetPoint(firstSpline.PercentToPointIndex(firstSpline.Project(hit.point).percent)).position;
                firstBridgePoint.GetComponent<Node>().AddConnection(firstSpline, firstSpline.PercentToPointIndex(firstSpline.Project(hit.point).percent));
            }
            else
            {
                
                secondSpline = hit.collider.GetComponent<SplineComputer>();
                if (secondSpline == null) return;
                if (firstSpline == secondSpline) 
                {

                    Destroy(firstBridgePoint.gameObject);
                    Destroy(secondBridgePoint.gameObject);
                    return;
                }
                secondBridgePoint = Instantiate(nodePrefab).GetComponent<Node>();

                secondBridgePoint.transform.position = secondSpline.GetPoint(secondSpline.PercentToPointIndex(secondSpline.Project(hit.point).percent)).position;
                secondBridgePoint.GetComponent<Node>().AddConnection(secondSpline, secondSpline.PercentToPointIndex(secondSpline.Project(hit.point).percent));

                Vector3 rayCastPoint1;
                Vector3 rayCastPoint2;

                Vector3 direction = secondBridgePoint.transform.position - firstBridgePoint.transform.position;

                float w1 = firstSpline.GetComponent<SplineMesh>().GetChannel(0).minScale.x/2 + 0.2f;
                rayCastPoint1 = firstBridgePoint.transform.position + direction.normalized * w1;

                float w2 = secondSpline.GetComponent<SplineMesh>().GetChannel(0).minScale.x/2 + 0.2f;
                rayCastPoint2 = secondBridgePoint.transform.position + -direction.normalized * w2;
                float distance = Vector3.Distance(rayCastPoint1, rayCastPoint2);
              
                Debug.DrawLine(rayCastPoint1, rayCastPoint2,Color.green,100000);
                if (!Physics.Linecast(rayCastPoint1, rayCastPoint2))
                {
                  
                    SplinePoint[] points = new SplinePoint[2];
                    
                    points[0] = new SplinePoint();
                    points[0].position = firstBridgePoint.GetPoint(0, false).position;
                    points[0].normal = Vector3.up;
                    points[0].size = 1f;
                    points[0].color = Color.white;

                    points[1] = new SplinePoint();
                    points[1].position = secondBridgePoint.GetPoint(0, false).position;
                    points[1].normal = Vector3.up;
                    points[1].size = 1f;
                    points[1].color = Color.white;

                    splineBridge = Instantiate(bridge).GetComponent<SplineComputer>();

                    splineBridge.SetPoints(points);
                    firstBridgePoint.AddConnection(splineBridge, 0);
                    secondBridgePoint.AddConnection(splineBridge, 1);
                    splineBridge.GetComponent<SplineMesh>().RebuildImmediate();



                }
                else
                {
                    Destroy(firstBridgePoint.gameObject);
                    Destroy(secondBridgePoint.gameObject);
                }
            }
            return;
        }
        if (tempHilight != null)
            Destroy(tempHilight);
    }
}