using Dreamteck.Splines;
using System.Collections.Generic;
using UnityEngine;

public class Intersection : MonoBehaviour
{
    public Node nodeIntersection;

    public Node.Connection GetNextDirection(int current)
    {
        Node.Connection[] connections = nodeIntersection.GetConnections();
        int index = 0;
        if (current + 1 < connections.Length) index = current + 1;
        return nodeIntersection.GetConnections()[index];
    }

    public int GetCurrentConnection(SplineFollower follower)
    {
        Node.Connection[] connections = nodeIntersection.GetConnections();

        if (connections.Length == 1) return -1;

        int currentConnection = 0;
        for (int i = 0; i < connections.Length; i++)
        {
            if (connections[i].spline == follower.spline)
            {
                currentConnection = i;
                break;
            }
        }
        return currentConnection;
    }

    public Node.Connection GetConnectionByIndex(int index)
    {
        Node.Connection[] connections = nodeIntersection.GetConnections();
        return connections[index];
    }

    private void OnNode(List<SplineTracer.NodeConnection> passed)
    {
        //Debug.Log("Reached node " + passed[0].node.name + " connected at point " + passed[0].point);
        ////Get all available connected splines
        //Node.Connection[] connections = passed[0].node.GetConnections();
        ////If this node does not have other connected splines, skip everything - there is no junction
        //if (connections.Length == 1) return;
        ////get the connected splines and find the index of the follower's current spline
        ////int currentConnection = 0;
        ////for (int i = 0; i < connections.Length; i++)
        ////{
        ////    if (connections[i].spline == follower.spline && connections[i].pointIndex ==
        ////   passed[0].point)
        ////    {
        ////        currentConnection = i;
        ////        break;
        ////    }
        ////}
        ////Choose a random connection to use that is not the current one
        ////This part can be replaced with any other Junction-picking logic (see TrainEngine.cs inExamples)
        //int newConnection = Random.Range(0, connections.Length);
        ////If the random index corrensponds to the current connection, change it so that it

        //if (newConnection == currentConnection)
        //{
        //    newConnection++;
        //    if (newConnection >= connections.Length) newConnection = 0;
        //}
        ////A good method to use which takes into account spline directions and travel distances
        ////and adds compensation so that no twitching occurs
        //SwitchSpline(connections[currentConnection], connections[newConnection]);
    }
}