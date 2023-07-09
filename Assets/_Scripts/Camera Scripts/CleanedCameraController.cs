using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEditor.SceneView;

/*The Camera has multiple modes
 *In GodMode and in CharacterMode it has a point that it follows (if the point is deleted it recreates itself at the proper position)
 *In LerpMode the Camera moves one Object to another one (these can also be moving objects)  
*/

public enum CleanedCameraMode
{
    GODMODE,
    CHARACTERMODE,
    LERPMODE,
    DEAD
}

public class CleanedCameraController : MonoBehaviour
{
    //PUBLIC STATICS & EVENTS

    //REFERENCES


    //EDITOR VARIABLES
    public GameObject emptyPrefab;
    public GameObject savedGodModeGameObject;
    public Transform godModePivotPoint;
    public GameObject followObject;

    [Header("GodMode Camera Settings")]
    [SerializeField] private float mouseSensitivity = 3.0f;
    [SerializeField] private float zoomSpeed = 3.0f;
    [SerializeField] private float minZoomGodMode = 5.0f;
    [SerializeField] private float maxZoomGodMode = 50.0f;
    [SerializeField] private float offSetMoveSpeed = .5f;
    [SerializeField] private float maxOffset = 25f;

    [Header("CharacterMode Camera Settings")]
    [SerializeField] private Vector3 characterModeCameraLocalPosition;
    [SerializeField] private Vector3 characterModeLocalEulerAngles;

    [Header("LerpMode Camera Settings")]
    [SerializeField] private float lerpStartUpTime;
    [SerializeField] private float lerpTime;

    [Header("DeadMode Camera Settings")]


    //CODE VARIABLES
    //Inputs
    private float mouseX, mouseY, mouseZ;
    private bool isMouseOnLeftEdge, isMouseOnRightEdge;
    private float scrollInput;

    //State-Variables
    private CleanedCameraMode currentCameraMode; 

    private float rotationX, rotationY, rotationZ;
    private Vector3 rotationVector, rotationUpdateVector; 
    private float distanceToTarget;
    private Vector3 cameraLocalOffset;

    private bool isLerpSetup; 
    private float currentLerpTime;
    private GameObject lerpFromObject, lerpToObject;
    private CleanedCameraMode lerpToMode; 


    //PUBLIC STATIC METHODS

    //MONOBEHAVIOUR METHODS
    private void Start()
    {
        SetupCameraStart();
        ListenToGameManager(); 
    }

    private void Update()
    {
        CheckModeChange(); 
        TakeInput();
        UpdateCameraBasedOnMode(); 
    }

    //IN SCENE METHODS (e.g. things that need to be accessed by unityEvents)

    //PUBLIC CODE METHODS
    
    //PRIVATE CODE METHODS
    public void CheckModeChange()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetupLerp(followObject, savedGodModeGameObject, CleanedCameraMode.GODMODE);
            ChangeCameraMode(CleanedCameraMode.LERPMODE); 
        }
        if (currentCameraMode == CleanedCameraMode.GODMODE)
        {
            CheckCharacterClick();
        }
    }

    public void ChangeCameraMode(CleanedCameraMode cameraMode)
    {
        DeactivateCameraMode(currentCameraMode);
        currentCameraMode = cameraMode;
        ActivateCameraMode(currentCameraMode); 
    }

    private void TakeInput()
    {
        scrollInput = Input.mouseScrollDelta.y;

        //when clicking on the screens edge the player can do a roll rotation; therefore the input is saved into z-axis instead of y-axis
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            isMouseOnLeftEdge = Input.mousePosition.x < Screen.width * 1 / 5;
            isMouseOnRightEdge = Input.mousePosition.x > Screen.width * 4 / 5;
        }

        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;

        if (!(isMouseOnLeftEdge || isMouseOnRightEdge))
        {
            mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * -1f;
            mouseZ = 0;
        }
        else if (isMouseOnLeftEdge)
        {
            mouseY = 0;
            mouseZ = Input.GetAxis("Mouse Y") * mouseSensitivity * 1f;
        }
        else if (isMouseOnRightEdge)
        {
            mouseY = 0;
            mouseZ = Input.GetAxis("Mouse Y") * mouseSensitivity * -1f;
        }
        else
        {
            Debug.LogError("mousePosition not recognised");
        }
    }

    private void UpdateCameraBasedOnMode()
    {
        switch(currentCameraMode)
        {
            case CleanedCameraMode.GODMODE:
                UpdateCameraGodMode(); 
                break;
            case CleanedCameraMode.CHARACTERMODE:
                UpdateCameraCharacterMode(); 
                break;
            case CleanedCameraMode.LERPMODE:
                UpdateCameraLerpMode(); 
                break;
            case CleanedCameraMode.DEAD:
                UpdateCameraDeadMode(); 
                break;
        }
    }

    private void DeactivateCameraMode(CleanedCameraMode cameraMode)
    {
        switch (cameraMode)
        {
            case CleanedCameraMode.GODMODE:
                SaveGodModePosition(); 
                break;
            case CleanedCameraMode.CHARACTERMODE:
                GameManager.Instance.currentControlledCharacter = null;
                break;
            case CleanedCameraMode.LERPMODE:
                //nothing
                break;
            case CleanedCameraMode.DEAD:
                //never happens?
                break;
        }
    }

    private void ActivateCameraMode(CleanedCameraMode cameraMode)
    {
        switch (cameraMode)
        {
            case CleanedCameraMode.GODMODE:
                SetupGodModePivotPoint();
                SetupGodModeFollowObject(); 
                break;
            case CleanedCameraMode.CHARACTERMODE:
                SetupCharacterFollowObject(); 
                break;
            case CleanedCameraMode.LERPMODE:
                if (!isLerpSetup) { throw new Exception("cameraLerp is not setup before switching to LerpMode"); } 
                break;
            case CleanedCameraMode.DEAD:
                SetupGodModePivotPoint(); 
                SetupDeadModeFollowObject(); 
                break;
        }
    }

    private void UpdateCameraGodMode()
    {
        AdjustCameraPivotOffset();
        AdjustCameraDistance();
        if (Input.GetMouseButton(1)) 
        { 
            godModePivotPoint.Rotate(new Vector3(mouseY, mouseX, mouseZ));
        }
        FollowObject(); 
    }

    private void UpdateCameraCharacterMode()
    {
        //zypernKatze maybe add some options here
        FollowObject(); 
    }

    private void UpdateCameraLerpMode()
    {
        currentLerpTime += Time.deltaTime;

        if (currentLerpTime < 0) { return; }
        transform.position = Vector3.Lerp(lerpFromObject.transform.position, lerpToObject.transform.position, Mathf.Pow(currentLerpTime / lerpTime, 0.33f));
        transform.rotation = Quaternion.Lerp(lerpFromObject.transform.rotation, lerpToObject.transform.rotation, Mathf.Pow(currentLerpTime / lerpTime, 0.33f)); 

        //upon finishing the lerp the gamestate & therefore the cameraMode is set to the new State/Mode
        if (currentLerpTime >= lerpTime)
        {
            isLerpSetup = false;

            if (lerpToMode == CleanedCameraMode.CHARACTERMODE)
            {
                GameManager.Instance.ChangeState(GameState.CharacterView); //this also calls the camera to change mode
            }
            else if (lerpToMode == CleanedCameraMode.GODMODE)
            {
                GameManager.Instance.ChangeState(GameState.GodView); //this also calls the camera to change mode
            }
        }
    }

    private void UpdateCameraDeadMode()
    {
        godModePivotPoint.Rotate(Vector3.up, 20 * Time.deltaTime);
    }


    //SUPPORTING METHODS
    private void SetupCameraStart()
    {
        SetupGodModePivotPoint();
        SetupGodModeFollowObject();
        currentCameraMode = CleanedCameraMode.GODMODE;
    }

    private void ListenToGameManager()
    {
        GameManager.Instance.onStateChange.AddListener((state) => {
            switch(state)
            {
                case GameState.GodView:
                case GameState.RewardMode:
                case GameState.ShapePlacement:
                    ChangeCameraMode(CleanedCameraMode.GODMODE);
                    break;
                case GameState.CharacterView:
                    ChangeCameraMode(CleanedCameraMode.CHARACTERMODE);
                    break;
                case GameState.GameOver:
                    ChangeCameraMode(CleanedCameraMode.DEAD); 
                    break; 
            }
        });
    }

    private void CheckCharacterClick()
    {
        RaycastHit hit;
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.tag == "Player" && Input.GetMouseButtonDown(0))
            {
                GameManager.Instance.currentControlledCharacter = hit.collider.gameObject.GetComponent<CharacterMovement>();
                AudioManager.instance.PlayOneShot(FMODEvents.instance.characterTakeControl);
                
                SaveGodModePosition(); 
                SetupLerp(savedGodModeGameObject, followObject, CleanedCameraMode.CHARACTERMODE);
                
                ChangeCameraMode(CleanedCameraMode.LERPMODE); 
            }
        }
    }

    private void AdjustCameraPivotOffset()
    {
        if (Input.GetMouseButton(2))
        {
            cameraLocalOffset += (Vector3.right * -mouseX + Vector3.up * mouseY) * offSetMoveSpeed * distanceToTarget / maxZoomGodMode;
            cameraLocalOffset = cameraLocalOffset.ClampVectorComponentWise(-maxOffset, maxOffset);
        }
    }

    private void AdjustCameraDistance()
    {
        distanceToTarget -= scrollInput * zoomSpeed;
        distanceToTarget = Mathf.Clamp(distanceToTarget, minZoomGodMode, maxZoomGodMode);
        followObject.transform.localPosition = Vector3.forward * -distanceToTarget + cameraLocalOffset;
    }

    private void FollowObject()
    {
        transform.position = followObject.transform.position;
        transform.eulerAngles = followObject.transform.eulerAngles; 
    }

    private void SetupFollowObject(Transform parent, Vector3 localPosition, Vector3 localEulerAngle)
    {
        if (followObject == null)
        {
            followObject = Instantiate(emptyPrefab);
            followObject.name = "cameraFollowObject";
        }
        followObject.transform.SetParent(parent);
        followObject.transform.localPosition = localPosition;
        followObject.transform.localEulerAngles = localEulerAngle;
    }

    private void SetupCharacterFollowObject()
    {
        SetupFollowObject(GameManager.Instance.currentControlledCharacter.transform, characterModeCameraLocalPosition, characterModeLocalEulerAngles); 
    }

    private void SetupGodModeFollowObject()
    {
        SetupFollowObject(godModePivotPoint.transform, savedGodModeGameObject.transform.position, savedGodModeGameObject.transform.localEulerAngles);
    }

    private void SetupDeadModeFollowObject()
    {
        SetupFollowObject(godModePivotPoint.transform, Vector3.back * maxZoomGodMode, savedGodModeGameObject.transform.localEulerAngles); 
    }

    private void SaveGodModePosition()
    {
        if (savedGodModeGameObject == null)
        {
            savedGodModeGameObject = Instantiate(emptyPrefab); 
        }
        savedGodModeGameObject.transform.position = transform.position; 
        savedGodModeGameObject.transform.rotation = transform.rotation; 
    }

    private void SetupLerp(GameObject from, GameObject to, CleanedCameraMode lerpToMode)
    {
        if (currentCameraMode == CleanedCameraMode.LERPMODE)
        {
            throw new Exception("trying to setup lerp, while camera is already lerping"); 
        }
        
        lerpFromObject = from;
        lerpToObject = to;
        this.lerpToMode = lerpToMode; 
        currentLerpTime = -lerpStartUpTime;

        if (lerpToMode == CleanedCameraMode.CHARACTERMODE) //zypernKatze is setting the FollowObject before saving its position
        {
            SetupCharacterFollowObject();
        }

        isLerpSetup = true; 
    }

    private void SetupGodModePivotPoint()
    {
        if (godModePivotPoint == null)
        {
            godModePivotPoint = Instantiate(emptyPrefab).transform; 
        }
        GameManager.Instance.UpdateAveragePosition(); 
        godModePivotPoint.transform.position = GameManager.Instance.averageCenterPointPosition; 
    }
}