using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CharacterPlacer : MonoBehaviour
{
    private Camera mainCamera;
    public LayerMask targetMask;
    private Vector3 targetPos;

    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private GameObject characterImage;
    // Start is called before the first frame update
    void Start()
    {
        characterImage.transform.position = new Vector3(-10000, -10000);
        mainCamera = this.GetComponent<Camera>();
    }

    GameObject character;
    bool isPlaying = false;
    GameObject Hilight;
    // Update is called once per frame
    void Update()
    {
   
        if (GameManager.Instance.currentState == GameState.CharacterPlacement  ) {


            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetMask) )
            {
                SplineComputer splineComp = hit.collider.gameObject.GetComponent<SplineComputer>();


                if (Hilight == null)
                {
                    Hilight = Instantiate(characterPrefab, targetPos, Quaternion.identity);
                }
                if (Hilight != null && Hilight.name== characterImage.name)
                {

                    Hilight.transform.position = new Vector3(-10000,-10000);
                    Hilight = Instantiate(characterPrefab, targetPos, Quaternion.identity);
                }
                Hilight.GetComponent<SplineFollower>().enabled= false;
                Hilight.transform.position = splineComp.Evaluate(splineComp.Project(hit.point).percent).position;
                targetPos = splineComp.Evaluate(splineComp.Project(hit.point).percent).position;
            }
            else
            {
               
                if (Hilight != null &&  Hilight.name!=characterImage.name)
                {
                    Destroy(Hilight);
                    Hilight = characterImage;
                }
                if (Hilight == null)
                {
                    Hilight = characterImage;
                }

                Hilight.transform.position = Input.mousePosition;
            }

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetMask) && Input.GetMouseButtonDown(0) )
            {
                SplineComputer splineComp = hit.collider.gameObject.GetComponent<SplineComputer>();



                targetPos = splineComp.Evaluate( splineComp.Project(hit.point).percent).position;
                character = Instantiate(characterPrefab, targetPos, Quaternion.identity);
                character.GetComponent<SplineFollower>().spline = splineComp;
                print(splineComp.Project(hit.point).percent);
                character.GetComponent<SplineFollower>().SetPercent(splineComp.Project(hit.point).percent);
           
                character.GetComponent<SplineFollower>().RebuildImmediate();
                character.GetComponent<SplineFollower>().SetPercent(splineComp.Project(hit.point).percent);
                ItemSpawner.instace.SpawnItems();
                GameManager.Instance.ChangeState(GameState.GodView);
                isPlaying=true;

                if (Hilight != null && Hilight.name != characterPrefab.name)
                {

                    Hilight.transform.position = new Vector3(-10000, -10000);
                 
                }
                if (Hilight != null && Hilight.name != characterImage.name)
                {
                    Destroy(Hilight);
          
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(targetPos, 0.5f);
    }
}
