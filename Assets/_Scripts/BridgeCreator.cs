using Dreamteck.Splines;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
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
    public float energy = 100;
    public LineRenderer lineRenderer;
    private Node firstBridgePoint;
    private Node secondBridgePoint;
    private SplineComputer splineBridge;
    private SplineComputer firstSpline;
    private SplineComputer secondSpline;
    private GameObject tempHilight;
    public float energyToAdd=2;
    public List<GameObject> Bridgeparts = new List<GameObject>();   
    
        /// <summary>
        /// TODO : Implement bridge distance/length, cant place too long bridges (Based on energy)
        /// TODO : Energy based on length, more length used less energy you have
        /// TODO : Number of scattered objectects depends on legnth 
        /// TODO : 
        /// </summary>
    // Start is called before the first frame update
    private void Start()
    {
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
    }
    void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        // Put the code that you want to execute before the camera renders here
        // If you are using URP or HDRP, Unity calls this method automatically
        // If you are writing a custom SRP, you must call RenderPipeline.BeginCameraRendering
        foreach (var item in FindObjectsOfType<MeshRenderer>())
        {

        } 
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

        if (GameManager.Instance.currentState!= GameState.GodView)
        {
            Destroy(tempHilight);
            if (firstBridgePoint != null)
            {
                Destroy(firstBridgePoint.gameObject);
            }
            if (secondBridgePoint != null)
            {
                Destroy(secondBridgePoint.gameObject);
            }
       
            return;

        }
        if (!inPortalCreationMode) return;

        if (Input.GetKeyDown(KeyCode.Escape) && firstBridgePoint != null)
        {
            Destroy(firstBridgePoint);
            lineRenderer.positionCount = 0;
            return;
        }

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
                tempHilight = Instantiate(Bridgeparts[Random.Range(0,Bridgeparts.Count)]);
                tempHilight.transform.localScale = Vector3.one * 5;
            }

            tempHilight.transform.position = hit.point + hit.normal*0.5f;
            int index = hit.collider.GetComponent<SplineComputer>().PercentToPointIndex(hit.collider.GetComponent<SplineComputer>().Project(hit.point).percent);
            SplineSample result = hit.collider.GetComponent<SplineComputer>().Evaluate(index);
           


            if (firstBridgePoint && energy - Vector3.Distance(firstBridgePoint.transform.position, result.position) >=0)
            {
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(1, result.position);
            }

          
         
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }
            
            Destroy(tempHilight);

            if (!firstBridgePoint)
            {
               
                firstSpline = hit.collider.GetComponent<SplineComputer>();
                lineRenderer.positionCount = 2;
                firstBridgePoint = Instantiate(nodePrefab).GetComponent<Node>();
                index = firstSpline.PercentToPointIndex(firstSpline.Project(hit.point).percent);
                if (!IsEmptyPoint(firstSpline, index))
                {
                    Destroy(firstBridgePoint.gameObject);
                    return;
                }
                result = firstSpline.Evaluate(index);
                firstBridgePoint.transform.position = result.position;
                 firstBridgePoint.transform.rotation = result.rotation;

                lineRenderer.SetPosition(0, result.position);
                firstBridgePoint.GetComponent<Node>().AddConnection(firstSpline, firstSpline.PercentToPointIndex(firstSpline.Project(hit.point).percent));
            }
            else
            {
                
                secondSpline = hit.collider.GetComponent<SplineComputer>();
                if (secondSpline == null) return;
                if (firstSpline == secondSpline) 
                {

                    Destroy(firstBridgePoint.gameObject);
           
                    return;
                }
                secondBridgePoint = Instantiate(nodePrefab).GetComponent<Node>();

                index = secondSpline.PercentToPointIndex(secondSpline.Project(hit.point).percent);
                if (!IsEmptyPoint(secondSpline, index))
                {
                    Destroy(firstBridgePoint.gameObject);
                    Destroy(secondBridgePoint.gameObject);

                    return;
                }
                result = secondSpline.Evaluate(index);

                secondBridgePoint.transform.position = result.position;
                secondBridgePoint.transform.rotation = result.rotation;

        
                secondBridgePoint.GetComponent<Node>().AddConnection(secondSpline, secondSpline.PercentToPointIndex(secondSpline.Project(hit.point).percent));

                Vector3 rayCastPoint1;
                Vector3 rayCastPoint2;

                Vector3 direction = secondBridgePoint.transform.position - firstBridgePoint.transform.position;

           


           
                pointNoraml1 = FindConnnectionDirection(firstBridgePoint, secondBridgePoint).normalized;

                pointNoraml2 = -FindConnnectionDirection(secondBridgePoint,firstBridgePoint).normalized;

                tanget1 = FindConnnectionTangent(firstBridgePoint, secondBridgePoint).normalized;
                tanget2 = FindConnnectionTangent(secondBridgePoint, firstBridgePoint).normalized;
             
        
                float w1 = firstSpline.GetComponent<SplineMesh>().GetChannel(0).minScale.x / 2 + 0.22f;
                rayCastPoint1 = firstBridgePoint.transform.position + FindRayCastTangent(firstBridgePoint, secondBridgePoint) * w1;

                float w2 = secondSpline.GetComponent<SplineMesh>().GetChannel(0).minScale.x / 2 + 0.22f;
                rayCastPoint2 = secondBridgePoint.transform.position + FindRayCastTangent( secondBridgePoint, firstBridgePoint) * w2;

                float distance = Vector3.Distance(rayCastPoint1, rayCastPoint2);
                if (energy - distance < 0)
                {
                    return;
                }
                energy -= distance;

            
                Debug.DrawLine(rayCastPoint1, rayCastPoint2, Color.red, 1000);

                if (!Physics.SphereCast(rayCastPoint1, 0.15f, rayCastPoint2 - rayCastPoint1,out hit, Vector3.Distance(rayCastPoint2 ,rayCastPoint1)))
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
                    float length = splineBridge.CalculateLength();
                    splineBridge.GetComponent<ObjectController>().spawnCount = (int)(distance * 5);
                    splineBridge.GetComponent<ObjectController>().RebuildImmediate();
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
        else if (Input.GetMouseButtonDown(0) && firstBridgePoint != null)
        {
            Destroy(firstBridgePoint);
            lineRenderer.positionCount = 0;
            return;
        }
        if (tempHilight != null)
            Destroy(tempHilight);
        if (firstBridgePoint )
        {
            lineRenderer.positionCount=1;
        }
    }
    public void OnReBuild()
    {
        LayOutObjects(5, shapePrefab);

        splineBridge?.SetPointTangents(1, splineBridge.GetPointPosition(1)  -tanget2 , splineBridge.GetPointPosition(1)+ tanget2);
        splineBridge?.SetPointTangents(0, splineBridge.GetPointPosition(0) + tanget1, splineBridge.GetPointPosition(0) - tanget1);

        splineBridge?.SetPointNormal(0,pointNoraml1);
        splineBridge?.SetPointNormal(1,pointNoraml2);
        
        firstBridgePoint = null; secondBridgePoint = null;
        splineBridge.onRebuild -= OnReBuild;
        splineBridge.GetComponent<SplineMesh>().RebuildImmediate();
        lineRenderer.positionCount= 0;
    }
    public Vector3 FindConnnectionDirection(Node from,Node To)
    {
        Vector3 dir = To.transform.position - from.transform.position;
        dir.Normalize();
        dir = Vector3.ProjectOnPlane(dir, from.transform.forward);
    
        float rightAngle = Vector3.Angle(dir, from.transform.right);
        float leftAngle = Vector3.Angle(dir, -from.transform.right);
        float topAngle = Vector3.Angle(dir, from.transform.up);
        float bottomAngle = Vector3.Angle(dir, -from.transform.up);
        if (rightAngle <= leftAngle && rightAngle <= topAngle )
        {
            if(rightAngle <= bottomAngle)
            {
                return from.transform.up;
            }
            return -from.transform.up;
        }
        else if (leftAngle <= rightAngle && leftAngle <= topAngle)
        {
            if (leftAngle <= bottomAngle)
            {
                return from.transform.up;
            }
            return -from.transform.up;
        }
        else if (topAngle <= rightAngle && topAngle <= leftAngle && leftAngle <= bottomAngle)
        {
            Vector3 dir2 = Vector3.ProjectOnPlane(dir, from.transform.up);
             float frontAngle = Vector3.Angle(dir2, from.transform.forward);
             float backAngle = Vector3.Angle(dir2, -from.transform.forward);
            if (frontAngle <= backAngle)
            {
                return from.transform.forward;
            }
            else
            {
                return -from.transform.forward;
            }
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
            return -from.transform.up;
        }


    }
    public Vector3 FindRayCastTangent(Node from, Node To)
    {
        Vector3 dir = To.transform.position - from.transform.position;
        dir.Normalize();
        dir = Vector3.ProjectOnPlane(dir, from.transform.forward);

        float rightAngle = Vector3.Angle(dir, from.transform.right);
        float leftAngle = Vector3.Angle(dir, -from.transform.right);
        float topAngle = Vector3.Angle(dir, from.transform.up);
        if (rightAngle <= leftAngle && rightAngle <= topAngle)
        {
            return from.transform.right;
        }
        else if (leftAngle <= rightAngle && leftAngle <= topAngle)
        {
            return -from.transform.right;
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

    public bool IsEmptyPoint(SplineComputer spline, int index)
    {


        foreach (Intersection intersection in Intersection.intersections)
        {
            if (intersection.nodeIntersection.HasConnection(spline, index))
            {
                return false;
            }

        }
        return true;
    }

    public void AddBridgeEnergy()
    {
        energy += energyToAdd;
        ItemSpawner.instace.SpawnItems();
        GameManager.Instance.ChangeState(GameState.GodView);
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