using Dreamteck.Splines;
using UnityEngine;

public class BridgeCreator : MonoBehaviour
{
    public bool inPortalCreationMode;
    public float maxPortalDistance;
    public GameObject hilightPrefab;
    public Camera mainCamera;
    public GameObject nodePrefab;
    public GameObject bridge;
    public GameObject shapePrefab;
    public LayerMask ignoreMask;


    private Node firstBridgePoint;
    private Node secondBridgePoint;
    private SplineComputer splineBridge;
    private SplineComputer firstSpline;
    private SplineComputer secondSpline;
    private GameObject tempHilight;
    
 
    // Start is called before the first frame update
    private void Start()
    {
    }

    private void OnDrawGizmos()
    {
       
    }
    Vector3 tanget1;
    Vector3 tanget2;

    Vector3 pointNoraml1;
    Vector3 pointNoraml2;

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

                int index = firstSpline.PercentToPointIndex(firstSpline.Project(hit.point).percent);
                SplineSample result = firstSpline.Evaluate(index);
                firstBridgePoint.transform.position = result.position;
                  firstBridgePoint.transform.rotation = result.rotation;

          
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

                 int index = secondSpline.PercentToPointIndex(secondSpline.Project(hit.point).percent);

                SplineSample result = secondSpline.Evaluate(index);

                secondBridgePoint.transform.position = result.position;
                secondBridgePoint.transform.rotation = result.rotation;

        
                secondBridgePoint.GetComponent<Node>().AddConnection(secondSpline, secondSpline.PercentToPointIndex(secondSpline.Project(hit.point).percent));

                Vector3 rayCastPoint1;
                Vector3 rayCastPoint2;

                Vector3 direction = secondBridgePoint.transform.position - firstBridgePoint.transform.position;

                float w1 = firstSpline.GetComponent<SplineMesh>().GetChannel(0).minScale.x/2 + 0.2f;
                rayCastPoint1 = firstBridgePoint.transform.position + direction.normalized * w1;

                float w2 = secondSpline.GetComponent<SplineMesh>().GetChannel(0).minScale.x/2 + 0.2f;
                rayCastPoint2 = secondBridgePoint.transform.position + -direction.normalized * w2;

                float distance = Vector3.Distance(rayCastPoint1, rayCastPoint2);

                pointNoraml1 = FindConnnectionDirection(firstBridgePoint, secondBridgePoint).normalized;

                pointNoraml2 = FindConnnectionDirection(secondBridgePoint,firstBridgePoint).normalized;

                tanget1 = FindConnnectionTangent(firstBridgePoint, secondBridgePoint).normalized;
                tanget2 = FindConnnectionTangent(secondBridgePoint, firstBridgePoint).normalized;


                Debug.DrawRay(firstBridgePoint.transform.position, pointNoraml1, Color.red, 1000);
                Debug.DrawRay(secondBridgePoint.transform.position, pointNoraml2, Color.red, 1000);
                if (!Physics.Linecast(rayCastPoint1, rayCastPoint2))
                {
                  
                    SplinePoint[] points = new SplinePoint[2];
                    
                    points[0] = new SplinePoint();
                    points[0].position = firstBridgePoint.GetPoint(0, false).position;
                    points[0].normal = pointNoraml1;
                    points[0].tangent= Vector3.zero;
              
                    points[0].size = 1f;
                    points[0].color = Color.red;

                    points[1] = new SplinePoint();
                    points[1].position = secondBridgePoint.GetPoint(0, false).position;
                    points[1].normal = pointNoraml2;
                    points[1].tangent = Vector3.zero;

              
                    points[1].size = 1f;
                    points[1].color = Color.yellow;

                   

                    splineBridge = Instantiate(bridge).GetComponent<SplineComputer>();

                    splineBridge.SetPoints(points);
                    firstBridgePoint.AddConnection(splineBridge, 0);
                    secondBridgePoint.AddConnection(splineBridge, 1);

                    splineBridge.onRebuild += OnReBuild;

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
    public void OnReBuild()
    {
        LayOutObjects(5, shapePrefab);

        splineBridge?.SetPointTangents(1, splineBridge.GetPointPosition(1)+ tanget2 , splineBridge.GetPointPosition(1)- tanget2);
        splineBridge?.SetPointTangents(0, splineBridge.GetPointPosition(0) + tanget1, splineBridge.GetPointPosition(0) - tanget1);

        splineBridge?.SetPointNormal(0,pointNoraml1);
        splineBridge?.SetPointNormal(1,pointNoraml2);
        
        firstBridgePoint = null; secondBridgePoint = null;
        splineBridge.onRebuild -= OnReBuild;
        splineBridge.GetComponent<SplineMesh>().RebuildImmediate();
    }
    public Vector3 FindConnnectionDirection(Node from,Node To)
    {
        Vector3 dir = To.transform.position - from.transform.position;
        dir.Normalize();
        dir = Vector3.ProjectOnPlane(dir, from.transform.forward);
        Debug.DrawRay(from.transform.position, dir, Color.yellow, 1000);
        float rightAngle = Vector3.Angle(dir, from.transform.right);
        float leftAngle = Vector3.Angle(dir, -from.transform.right);
        float topAngle = Vector3.Angle(dir, from.transform.up);
        float bottomAngle = Vector3.Angle(dir, -from.transform.up);
        if (rightAngle <= leftAngle && rightAngle <= topAngle && rightAngle <= bottomAngle)
        {
            return from.transform.up;
        }
        else if (leftAngle <= rightAngle && leftAngle <= topAngle && leftAngle <= bottomAngle)
        {
            return from.transform.up;
        }
        else if (topAngle <= rightAngle && topAngle <= leftAngle && leftAngle <= bottomAngle)
        {
            return from.transform.forward;
        }
        else
        {
            return from.transform.forward;
        }



    }

    public Vector3 FindConnnectionTangent(Node from, Node To)
    {
        Vector3 dir = To.transform.position - from.transform.position;
        dir.Normalize();
        dir = Vector3.ProjectOnPlane(dir,from.transform.forward );
        Debug.DrawRay(from.transform.position, dir, Color.yellow, 1000);
        float rightAngle = Vector3.Angle(dir, from.transform.right);
        float leftAngle = Vector3.Angle(dir, -from.transform.right);
        float topAngle = Vector3.Angle(dir, from.transform.up);
        if (rightAngle <= leftAngle && rightAngle <= topAngle)
        {
            return -from.transform.right ;
        }
        else if (leftAngle <= rightAngle && leftAngle <= topAngle)
        {
            return from.transform.right;
        }
        else
        {
            return from.transform.up;
        }


    }
    public void LayOutObjects( GameObject prefab)
    {

        ObjectController obc= splineBridge.gameObject.AddComponent<ObjectController>();

       

    }
        public void LayOutObjects(int steps,GameObject prefab)
    {
       
       
        //float precent = (float)1/5;
        //for (int i = 0; i < steps; i++)
        //{
           
        //    SplineSample splinesample = splineBridge.Evaluate( (i*(precent)) + 0.1f);
        //    GameObject temp =Instantiate(prefab, splinesample.position /*+ splinesample.right * Random.Range(0,2)*/,Quaternion.identity);
         
        //    temp.transform.up = splinesample.up;
        //    temp.transform.forward = splinesample.forward;
        //    temp.transform.Rotate(0, Random.Range(0, 100), 0, Space.Self);

        //    temp = Instantiate(prefab, splinesample.position/* + -splinesample.right * Random.Range(1, 2)*/, Quaternion.identity);
      
        //    temp.transform.up = splinesample.up;
        //    temp.transform.forward = splinesample.forward;
        //    temp.transform.Rotate(0, Random.Range(0, 100), 0, Space.Self);
        //}
    }
}