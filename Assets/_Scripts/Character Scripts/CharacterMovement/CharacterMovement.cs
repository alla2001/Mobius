using Dreamteck.Splines;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; 

[RequireComponent(typeof(SplineFollower))]
public class CharacterMovement : MonoBehaviour
{
    public SplineFollower follower
    {
        get
        {
            if (_follower == null)
            {
                _follower = GetComponent<SplineFollower>();
                return _follower;
            }
            return _follower;
        }
        set { _follower = value; }
    }
    /// <summary>
    /// TODO : Make smooth turning and smooth turn in place 
    /// TODO :
    /// </summary>
    private SplineFollower _follower;

    private bool inInterSection;
    public ParticleSystem[] particleSystems;
    public Text energyText;
    private CharacterInfo characterInfo;  

    [SerializeField] private Animator animator;
    public GameObject characterEnergy_UI;

    private void Start()
    {
        foreach (ParticleSystem particleSystem in particleSystems)
        {
            var main = particleSystem.main; 
            main.useUnscaledTime = false; 
        }
        characterInfo = GetComponent<CharacterInfo>();
        animator = GetComponentInChildren<Animator>();
        characterEnergy_UI = UIManager.instance.Character_energy_icon;
        energyText = UIManager.instance.Character_energy_text; 
    }

    // Update is called once per frame
    private void Update() //gets enabled / disabled via the TakeControl() and ReleaseControl() Method
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

        energyText.text = ((int)(characterInfo.timeBeforeDeath / 10)).ToString();
    }

    private void OnMouseEnter()
    {
        Debug.Log("MouseOverCharacter");
        AudioManager.instance.PlayOneShot(FMODEvents.instance.characterHoverOver);

        if (C_RevertMouseEnterEffect != null) { StopCoroutine(C_RevertMouseEnterEffect); }

        if (GameManager.Instance.currentState == GameState.GodView)
        {
            foreach(ParticleSystem particleSystem in particleSystems)
            {
                var emission = particleSystem.emission;
                emission.rateOverTime = new ParticleSystem.MinMaxCurve(2f);
            }

            characterEnergy_UI.SetActive(true);
            energyText.text = (characterInfo.timeBeforeDeath / characterInfo.energyPerStone).ToString("0.00"); 
        }
    }

    private void OnMouseExit()
    {
        StartCoroutine(RevertMouseEnterEffect());
        if (GameManager.Instance.currentState != GameState.CharacterView)
        {
            characterEnergy_UI.SetActive(false);
        }
    }

    Coroutine C_RevertMouseEnterEffect; 
    private IEnumerator RevertMouseEnterEffect()
    {
        float time = 0;

        while (time < 1f)
        {
            time += Time.deltaTime;
            foreach (ParticleSystem particleSystem in particleSystems)
            {
                var emission = particleSystem.emission;
                emission.rateOverTime = new ParticleSystem.MinMaxCurve(Mathf.Lerp(2f, 0.5f, time));
            }
            yield return null; 
        }

    }

    private void GoBack()
    {
        if (follower.direction == Spline.Direction.Forward)
            follower.direction = Spline.Direction.Backward;
        else
            follower.direction = Spline.Direction.Forward;
    }

    private void SwitchSpline(Node.Connection from, Node.Connection to, bool flipDirection)
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

    private Vector3 intersectionPos;

    public void MoveOnIntersection(int direction)
    {
        if (!inInterSection) return;
        int current = intersection.GetCurrentConnection(follower);

        SwitchSpline(intersection.GetConnectionByIndex(current), intersection.GetNextDirection(current), direction > 0);

        animator.SetTrigger("MoveOnIntersection"); 
    }

    private Intersection intersection;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Intersection") && !inInterSection)
        {
            print("Intersect");
            intersectionPos = other.transform.position;
            intersection = other.GetComponent<Intersection>();
            if (GameManager.Instance.currentControlledCharacter == this)
            {
                intersection.SlowTime();
            }
         
            inInterSection = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Intersection") && inInterSection)
        {
            // other.transform.position= intersectionPos;
            if (GameManager.Instance.currentControlledCharacter == this)
            {
                intersection.ReturnTime();
            }
             
            inInterSection = false;
        }
    }

    public void takeControl()
    {
        characterEnergy_UI.SetActive(true); 
        tag = "Player"; 
        enabled = true; 
    }

    public void releaseControl()
    {
        characterEnergy_UI.SetActive(false);
        tag = "Untagged"; 
        enabled = false; 
    }
}