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
    bool inInterSection;
    private void OnEnable()
    {
        //onNode is called every time the follower passes by a Node
    }
    private void OnDisable()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            MoveOnIntersection(0);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            MoveOnIntersection(1);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            GoBack();
        }
    }
    void GoBack()
    {
        if (follower.direction == Spline.Direction.Forward)
            follower.direction = Spline.Direction.Backward;
        else
            follower.direction = Spline.Direction.Forward;
    }
    void SwitchSpline(Node.Connection from, Node.Connection to,bool flipDirection)
    {
        //See how much units we have travelled past that Node in the last frame
        float excessDistance = follower.spline.CalculateLength(follower.spline.GetPointPercent(from.pointIndex), follower.UnclipPercent(follower.result.percent));
       excessDistance = 0;
        //Set the spline to the follower
        follower.spline = to.spline;
        if (!to.spline.isClosed)
        {
            follower.wrapMode = SplineFollower.Wrap.PingPong;
        }
        else
        {
            follower.wrapMode = SplineFollower.Wrap.Loop;
        }
        follower.RebuildImmediate();
        //Get the location of the junction point in percent along the new spline
        double startpercent = follower.ClipPercent(to.spline.GetPointPercent(to.pointIndex));
        if (follower.direction == Spline.Direction.Forward)
        {
            if (Vector3.Dot(from.spline.Evaluate(from.pointIndex).right, to.spline.Evaluate(to.pointIndex).forward) < 0f)
            {
                if (!flipDirection)
                {
                    if (follower.direction == Spline.Direction.Forward)
                        follower.direction = Spline.Direction.Backward;
                    else
                        follower.direction = Spline.Direction.Forward;
                }


            }
            else
            {
                if (flipDirection)
                {
                    if (follower.direction == Spline.Direction.Forward)
                        follower.direction = Spline.Direction.Backward;
                    else
                        follower.direction = Spline.Direction.Forward;
                }
            }
        }
        else
        {
            if (Vector3.Dot(from.spline.Evaluate(from.pointIndex).right, to.spline.Evaluate(to.pointIndex).forward) < 0f)
            {
                if (!flipDirection)
                {
                    if (follower.direction == Spline.Direction.Forward)
                        follower.direction = Spline.Direction.Backward;
                    else
                        follower.direction = Spline.Direction.Forward;
                }


            }
            else
            {
                if (flipDirection)
                {
                    if (follower.direction == Spline.Direction.Forward)
                        follower.direction = Spline.Direction.Backward;
                    else
                        follower.direction = Spline.Direction.Forward;
                }
            }
        }
       
       
        //Position the follower at the new location and travel excessDistance along the new spline
        follower.SetPercent(follower.Travel(startpercent, excessDistance, follower.direction));

    }
    Vector3 intersectionPos;

    public void MoveOnIntersection(int direction)
    {

        if (!inInterSection) return;    
        int current = intersection.GetCurrentConnection(follower);
      
        SwitchSpline(intersection.GetConnectionByIndex(current), intersection.GetNextDirection(current), direction>0);
    }
    Intersection intersection;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Intersection") && !inInterSection)
        {
            print("Intersect");
            intersectionPos=other.transform.position ;
            intersection = other.GetComponent<Intersection>();
           
              inInterSection =true;
    
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Intersection") && inInterSection)
        {
          // other.transform.position= intersectionPos;
            inInterSection = false;
        }
    }
}


