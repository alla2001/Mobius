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

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = this.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.currentState == GameState.CharacterPlacement) {

            Debug.Log("Can Place Character");

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetMask) && Input.GetMouseButtonDown(0))
            {
                SplineComputer splineComp = hit.collider.gameObject.GetComponent<SplineComputer>();
                int index = splineComp.PercentToPointIndex(splineComp.Project(hit.point).percent);
                SplineSample result = splineComp.Evaluate(index);

                targetPos = result.position;
                Debug.Log(targetPos);

                GameObject character = Instantiate(characterPrefab, targetPos, Quaternion.identity);
                character.GetComponent<SplineFollower>().spline = splineComp;
                GameManager.Instance.ChangeState(GameState.GodView);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(targetPos, 0.2f);
    }
}
