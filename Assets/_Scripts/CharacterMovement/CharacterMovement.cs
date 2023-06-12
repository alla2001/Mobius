using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

[RequireComponent(typeof(SplineFollower))]
public class CharacterMovement : MonoBehaviour
{
    public SplineFollower follower { 
        get {
            if (_follower == null)
            {
                _follower = GetComponent<SplineFollower>();
                return _follower;
            }
            return _follower;
            } 
        set { _follower = value; } }

    private SplineFollower _follower;
    
    public bool hasControle=false;
    private void OnEnable()
    {
        follower.onNode += OnNode; //onNode is called every time the follower passes by a Node
    }
    private void OnDisable()
    {
        follower.onNode -= OnNode;
    }

    // Update is called once per frame
    void Update()
    {
        //if ()
        //{

        //}
    }
    private void OnNode(List<SplineTracer.NodeConnection> passed)
    {
        
 Debug.Log("Reached node " + passed[0].node.name + " connected at point " + passed[0].point);
        //Get all available connected splines
        Node.Connection[] connections = passed[0].node.GetConnections();
        //If this node does not have other connected splines, skip everything - there is no junction
        if (connections.Length == 1) return;
        //get the connected splines and find the index of the follower's current spline
        int currentConnection = 0;
        for (int i = 0; i < connections.Length; i++)
        {
            if (connections[i].spline == follower.spline && connections[i].pointIndex ==
           passed[0].point)
            {
                currentConnection = i;
                break;
            }
        }
        //Choose a random connection to use that is not the current one
        //This part can be replaced with any other Junction-picking logic (see TrainEngine.cs inExamples)
 int newConnection = Random.Range(0, connections.Length);
        //If the random index corrensponds to the current connection, change it so that it
      
 if (newConnection == currentConnection)
        {
            newConnection++;
            if (newConnection >= connections.Length) newConnection = 0;
        }
        //A good method to use which takes into account spline directions and travel distances
        //and adds compensation so that no twitching occurs
        SwitchSpline(connections[currentConnection], connections[newConnection]);
    }
    void SwitchSpline(Node.Connection from, Node.Connection to)
    {
        //See how much units we have travelled past that Node in the last frame
        float excessDistance =
       follower.spline.CalculateLength(follower.spline.GetPointPercent(from.pointIndex),
       follower.UnclipPercent(follower.result.percent));
        //Set the spline to the follower
        follower.spline = to.spline;
        follower.RebuildImmediate();
        //Get the location of the junction point in percent along the new spline
        double startpercent = follower.ClipPercent(to.spline.GetPointPercent(to.pointIndex));
        if (Vector3.Dot(from.spline.Evaluate(from.pointIndex).forward,
       to.spline.Evaluate(to.pointIndex).forward) < 0f)
        {
            if (follower.direction == Spline.Direction.Forward) follower.direction =
           Spline.Direction.Backward;
            else follower.direction = Spline.Direction.Forward;
        }
        //Position the follower at the new location and travel excessDistance along the new spline
        follower.SetPercent(follower.Travel(startpercent, excessDistance, follower.direction));
      
    }

    private void OnTriggerEnter(Collider other)
    {
        Node node = other.gameObject.GetComponent<Node>();

    }
}


