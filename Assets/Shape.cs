using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using System.Linq;
using UnityEditor.Experimental.GraphView;

public class Shape : MonoBehaviour
{
    [SerializeField] private float detectRadius = 0.5f;
    public bool canBePlaced;
    private SplineComputer[] splCompReferences;
    private Vector3 overlapPos;
    private int numberOfPoints;
    private List<SplinePoint> splineNodePositions = new List<SplinePoint>();

    private void Awake()
    {
        splCompReferences = GetComponentsInChildren<SplineComputer>();

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        splineNodePositions.Clear();

        foreach (var splineComp in splCompReferences)
        {
            splineNodePositions.AddRange(splineComp.GetPoints());
        }

        
    }

    private void FixedUpdate()
    {
        canBePlaced = true;
        foreach (var splinePoint in splineNodePositions)
        {
            Collider[] hitColliders = Physics.OverlapSphere(splinePoint.position, detectRadius);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject.tag == "wall")
                {
                    canBePlaced = false;
                    overlapPos = splinePoint.position;
                    return;
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
       

        foreach (var node in splineNodePositions)
        {
            if (node.position == overlapPos)
            {
                continue;
            }
            Gizmos.DrawSphere(node.position, detectRadius);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(overlapPos, detectRadius);
    }
}
