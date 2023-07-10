using System;
using System.Collections;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEditor.SceneView;

/*The Camera has multiple modes
 *In GodMode and in CharacterMode it has a point that it follows (if the point is deleted it recreates itself at the proper position)
 *In LerpMode the Camera moves one Object to another one (these can also be moving objects)  
*/

//zypernKatze the camera can be broken by first moving offsetting it heavily and then trying to zoom into the shape

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
    public Camera camera; 

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
    [SerializeField] private float nearClipPlane; 

    [Header("LerpMode Camera Settings")]
    [SerializeField] private float lerpStartUpTime;
    [SerializeField] private float lerpTime;

    [Header("DeadMode Camera Settings")]


    //CODE VARIABLES
    //Input-Savers
    private float mouseX, mouseY, mouseZ;
    private bool isMouseOnLeftEdge, isMouseOnRightEdge;
    private float scrollInput;

    //State-Variables
    private CleanedCameraMode currentCameraMode; 

    private float distanceToTarget;
    private float distanceToTargetOverflow; 
    private Vector3 cameraOffset;

    private bool isLerpSetup; 
    private float currentLerpTime;
    private GameObject lerpFromObject, lerpToObject;
    private CleanedCameraMode lerpToMode; 


    //PUBLIC STATIC METHODS

    //MONOBEHAVIOUR METHODS
    private void Start()
    {
        SetupCameraStart();
        GameManager.Instance.onStateChange.AddListener(ListenToGameManager);
    }

    private void OnDestroy()
    {
        GameManager.Instance.onStateChange.RemoveListener(ListenToGameManager);
    }

    private void LateUpdate() //making sure the character ( & followpoint) have moved already)
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
        if (currentCameraMode == CleanedCameraMode.CHARACTERMODE)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1) || 
                (CheckCharacterClick() != null && CheckCharacterClick() == GameManager.Instance.currentControlledCharacter)
                )
            {
                SetupLerp(followObject, GetSavedGodMode(), CleanedCameraMode.GODMODE, this.lerpStartUpTime);
                ChangeCameraMode(CleanedCameraMode.LERPMODE); 
                GameManager.Instance.ChangeState(GameState.GodView); 
            }
        }
        if (currentCameraMode == CleanedCameraMode.GODMODE && GameManager.Instance.currentState == GameState.GodView)
        {
            CharacterMovement character = CheckCharacterClick(); 
            if (character != null && BridgeCreator.instance.firstSpline.IsUnityNull())
            {
                TakeCharacterPossesion(character);
            }
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
                UpdateGodMode(); 
                break;
            case CleanedCameraMode.CHARACTERMODE:
                UpdateCharacterMode(); 
                break;
            case CleanedCameraMode.LERPMODE:
                UpdateLerpMode(); 
                break;
            case CleanedCameraMode.DEAD:
                UpdateDeadMode(); 
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
                camera.nearClipPlane = 0.05f; 
                break;
            case CleanedCameraMode.CHARACTERMODE:
                SetupCharacterFollowObject();
                camera.nearClipPlane = this.nearClipPlane;
                break;
            case CleanedCameraMode.LERPMODE:
                if (!isLerpSetup) { throw new Exception("cameraLerp is not setup before switching to LerpMode"); }
                camera.nearClipPlane = this.nearClipPlane/2;
                break;
            case CleanedCameraMode.DEAD:
                SetupGodModePivotPoint(); 
                SetupDeadModeFollowObject();
                camera.nearClipPlane = 0.05f;
                break;
        }
    }

    private void UpdateGodMode()
    {
        AdjustCameraDistance();
        AdjustCameraPivotOffset();
        if (Input.GetMouseButton(1)) 
        { 
            godModePivotPoint.Rotate(new Vector3(mouseY, mouseX, mouseZ));
        }
        FollowObject(); 
    }

    private void UpdateCharacterMode()
    {
        //zypernKatze maybe add some options here
        LerpFollowObject(); 
    }

    private void UpdateLerpMode()
    {
        currentLerpTime += Time.deltaTime;

        if (currentLerpTime < 0) { return; }
        transform.position = Vector3.Lerp(lerpFromObject.transform.position, lerpToObject.transform.position, Mathf.Pow(currentLerpTime / lerpTime, 0.33f));
        transform.rotation = Quaternion.Lerp(lerpFromObject.transform.rotation, lerpToObject.transform.rotation, Mathf.Pow(currentLerpTime / lerpTime, 0.33f)); 

        //upon finishing the lerp the gamestate & therefore the cameraMode is set to the new State/Mode
        if (currentLerpTime >= lerpTime)
        {
            isLerpSetup = false;

            ChangeCameraMode(lerpToMode); 
        }
    }

    private void UpdateDeadMode()
    {
        godModePivotPoint.Rotate(Vector3.up, 20 * Time.deltaTime);
        FollowObject();
    }


    //SUPPORTING METHODS
    private void SetupCameraStart()
    {
        SetupGodModePivotPoint();
        SetupGodModeFollowObject();
        currentCameraMode = CleanedCameraMode.GODMODE;
    }

    private void ListenToGameManager(GameState state) //not listening to GodView and CharacterView because they set themselves up
    {
        switch (state)
        {
            case GameState.ShapePlacement:
                SetupLerp(followObject, GetSavedGodMode(), CleanedCameraMode.GODMODE);
                ChangeCameraMode(CleanedCameraMode.LERPMODE);
                break;
            case GameState.GameOver:
                SetupLerp(followObject, GetSavedGodMode(), CleanedCameraMode.GODMODE);
                ChangeCameraMode(CleanedCameraMode.LERPMODE);
                break; 
        }
    }

    private CharacterMovement CheckCharacterClick()
    {
        RaycastHit hit;
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0) && 
            Physics.Raycast(ray, out hit) && 
            hit.collider.GetComponent<CharacterMovement>() != null)
        {
            return hit.collider.GetComponent<CharacterMovement>(); 
        }
        return null; 
    }

    private void TakeCharacterPossesion(CharacterMovement characterMovement)
    {
        GameManager.Instance.currentControlledCharacter = characterMovement;
        GameManager.Instance.SwitchToTimeMode(2); 
        AudioManager.instance.PlayOneShot(FMODEvents.instance.characterTakeControl);
        //zypernKatze should change sound here, when the character is low on energy

        SaveGodModePosition();
        SetupLerp(GetSavedGodMode(), followObject, CleanedCameraMode.CHARACTERMODE, 0.4f);
        
        GameManager.Instance.ChangeState(GameState.CharacterView); //changing cameraMode happens via ListenToGameManager

        ChangeCameraMode(CleanedCameraMode.LERPMODE);
    }

    private void AdjustCameraPivotOffset()
    {
        if (Input.GetMouseButton(2))
        {
            cameraOffset += (transform.right * -mouseX + transform.up * mouseY) * offSetMoveSpeed;
            cameraOffset = cameraOffset.ClampVectorComponentWise(-maxOffset, maxOffset);
        }
    }

    private void AdjustCameraDistance()
    {
        distanceToTarget -= scrollInput * zoomSpeed;
        
        //zypernKatze not the best designed way to move the camera forward again, but fine
        if (distanceToTarget < minZoomGodMode)
        {
            distanceToTargetOverflow += minZoomGodMode - distanceToTarget;
            distanceToTarget = minZoomGodMode;
        }
        else
        {
            distanceToTargetOverflow -= Time.deltaTime * 4f; 
        }
        if (distanceToTarget > maxZoomGodMode)
        {
            distanceToTarget = maxZoomGodMode; 
        }
        if (distanceToTargetOverflow > 10 && scrollInput > 0)
        {
            float forceCameraForwardLength = distanceToTargetOverflow - 10;
            distanceToTargetOverflow = 10; 
            cameraOffset += transform.forward * forceCameraForwardLength/2; 
        }

        distanceToTarget = Mathf.Clamp(distanceToTarget, minZoomGodMode, maxZoomGodMode);
        godModePivotPoint.transform.position = GameManager.Instance.averageCenterPointPosition + cameraOffset; 
        followObject.transform.localPosition = Vector3.forward * -distanceToTarget;
    }

    private void FollowObject()
    {
        transform.position = followObject.transform.position;
        transform.eulerAngles = followObject.transform.eulerAngles; 
    }

    private void LerpFollowObject()
    {
        transform.position = Vector3.Lerp(transform.position, followObject.transform.position, 3f * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, followObject.transform.rotation, 3f * Time.deltaTime);
    }

    private void SetupFollowObject(Transform parent, Vector3 localPosition, Vector3 localEulerAngle)
    {
        if (followObject == null)
        {
            followObject = new GameObject(); 
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
        SetupFollowObject(godModePivotPoint.transform, GetSavedGodMode().transform.position, GetSavedGodMode().transform.localEulerAngles);
    }

    private void SetupDeadModeFollowObject()
    {
        SetupFollowObject(godModePivotPoint.transform, Vector3.back * maxZoomGodMode, GetSavedGodMode().transform.localEulerAngles); 
    }

    private void SaveGodModePosition()
    {
        if (savedGodModeGameObject == null)
        {
            savedGodModeGameObject = new GameObject(); 
        }
        savedGodModeGameObject.transform.position = transform.position;
        savedGodModeGameObject.transform.rotation = transform.rotation;

        StartCoroutine(DeleteGodModePosition()); 
    }

    private IEnumerator DeleteGodModePosition()
    {
        float deleteTimer = 0f;
        yield return null; 
        while (deleteTimer < 5.0f)
        {
            deleteTimer += Time.deltaTime;
            if (currentCameraMode != CleanedCameraMode.CHARACTERMODE)
            {
                yield break; 
            }
            yield return null; 
        }

        Destroy(savedGodModeGameObject); 
    }

    private void SetupLerp(GameObject from, GameObject to, CleanedCameraMode lerpToMode, float lerpStartUpTime = 0f)
    {
        if (currentCameraMode == CleanedCameraMode.LERPMODE)
        {
            Debug.LogWarning("trying to setup lerp, while camera is already lerping"); 
        }
        
        lerpFromObject = from;
        lerpToObject = to;
        this.lerpToMode = lerpToMode; 
        currentLerpTime = -lerpStartUpTime;

        if (lerpToMode == CleanedCameraMode.CHARACTERMODE) //changing cameraMode happens via ListenToGameManager
        {
            SetupCharacterFollowObject();
        }

        isLerpSetup = true; 
    }

    private void SetupGodModePivotPoint()
    {
        if (godModePivotPoint == null)
        {
            godModePivotPoint = (new GameObject()).transform; 
        }
        GameManager.Instance.UpdateAveragePosition(); 
        godModePivotPoint.transform.position = GameManager.Instance.averageCenterPointPosition; 
    }

    public GameObject GetSavedGodMode()
    {
        if (savedGodModeGameObject == null)
        {
            godModePivotPoint.transform.position = transform.position;
            godModePivotPoint.transform.rotation = transform.rotation; 
            cameraOffset = transform.position - GameManager.Instance.averageCenterPointPosition;
            distanceToTarget = 25.0f; 
            savedGodModeGameObject = Instantiate(emptyPrefab, transform.position + -transform.forward * distanceToTarget, transform.rotation, godModePivotPoint); 
        }
        return savedGodModeGameObject; 
    }
}