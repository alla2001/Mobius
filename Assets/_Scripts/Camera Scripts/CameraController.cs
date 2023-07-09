using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public enum CameraMode
{
    GodModeGlobal,
    GodModeLocal,
    FollowCharacter,
    LerpMode,
    DeathCamera
}

public class CameraController : MonoBehaviour
{
    /*
    public Transform godModeCenterPoint;

    public CharacterMovement currentPlayer;
    public GameObject characterModeCameraFollowObject;
    public GameObject lastGodPosition;

    //EDITOR VARIABLES
    [Header("Camera Modes")]
    [SerializeField]
    private CleanedCameraMode cameraMode;

    [Header("Camera Settings")]
    [SerializeField] private float mouseSensitivity = 3.0f;
    [SerializeField] private float zoomSpeed = 3.0f;
    [SerializeField] private float minZoomGodMode = 5.0f;
    [SerializeField] private float maxZoomGodMode = 50.0f;
    [SerializeField] private float offSetMoveSpeed = .3f;
    [SerializeField] private float maxOffset = 25f;
    [SerializeField] private Vector3 characterModeCameraLocalPosition;
    [SerializeField] private Vector3 characterModeLocalEulerAngles;

    //CODE VARIABLES
    private float rotationZ;
    private float rotationY;
    private float rotationX;
    private Vector3 rotationVector;
    private Vector3 rotationUpdateVector;
    private float mouseX;
    private float mouseY;
    private float mouseZ;
    private bool mouseLeftEdge;
    private bool mouseRightEdge;

    private float distanceToTarget = 10.0f;
    private float scrollInput;
    private Vector3 cameraOffSet;

    private float lerpStartupTime;
    private float currentLerpTime;
    private float fullLerpTime;
    private CleanedCameraMode lerpToMode;
    private GameObject lerpFromObject;
    private GameObject lerpToObject;

    //PUBLIC STATIC METHODS

    //MONOBEHAVIOUR METHODS
    private void Start()
    {
        godModeCenterPoint = transform.parent;

        ChangeCameraMode(CleanedCameraMode.GODMODELOCAL);
        transform.localRotation = Quaternion.identity;

        GameManager.Instance.onStateChange.AddListener((state) => {

            if (state == GameState.CharacterView)
            {
                ChangeCameraMode(CleanedCameraMode.CHARACTERMODE);
            }
            else
            {
                ChangeCameraMode(CleanedCameraMode.GODMODELOCAL);

                transform.localPosition = lastGodPosition.transform.position;
                transform.localEulerAngles = lastGodPosition.transform.eulerAngles;
            }

        });
    }

    private void Update()
    {
        GetMouseInput();
        CheckCameraModeChange();
        UpdateCameraBasedOnMode();
    }

    //IN SCENE METHODS (e.g. things that need to be accessed by unityEvents)

    //PUBLIC CODE METHODS

    //PRIVATE CODE METHODS
    private void CheckCameraModeChange()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) //zypernKatze it makes no sense to change the gameMode form the camera
        {
            ChangeCameraMode(CleanedCameraMode.GODMODELOCAL);
            GameManager.Instance.ChangeState(GameState.GodView);
            GameManager.Instance.currentControlledCharacter = null;
        }
        if (currentPlayer == null && cameraMode == CleanedCameraMode.CHARACTERMODE)
        {
            ChangeCameraMode(CleanedCameraMode.GODMODELOCAL);
        }

        if (GameManager.Instance.currentState == GameState.GameOver)
        {
            ChangeCameraMode(CleanedCameraMode.DeathCamera);
        }

        if (cameraMode != CleanedCameraMode.CHARACTERMODE)
        {
            CheckCharacterClick();
        }
    }

    private void CheckCharacterClick()
    {
        RaycastHit hit;
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {

            if (hit.transform.gameObject.tag == "Player" && Input.GetMouseButtonDown(0))
            {
                currentPlayer = hit.collider.gameObject.GetComponent<CharacterMovement>();
                GameManager.Instance.currentControlledCharacter = currentPlayer;

                AudioManager.instance.PlayOneShot(FMODEvents.instance.characterTakeControl);

                ChangeCameraMode(CleanedCameraMode.LERPMODE);
            }
        }
    }

    public void ChangeCameraMode(CleanedCameraMode newMode)
    {
        //stuff to do for leaving a cameraMode
        switch (cameraMode)
        {
            case CleanedCameraMode.GodModeGlobal:
            case CleanedCameraMode.GODMODELOCAL:
                lastGodPosition.transform.localPosition = transform.localPosition;
                lastGodPosition.transform.localEulerAngles = transform.localEulerAngles;
                break;
            case CleanedCameraMode.CHARACTERMODE:
                break;
            case CleanedCameraMode.DeathCamera:
                transform.SetParent(godModeCenterPoint);
                GameManager.Instance.UpdateAveragePosition();
                godModeCenterPoint.transform.position = GameManager.Instance.averageCenterPointPosition;
                distanceToTarget = maxZoomGodMode;
                SetCameraPosition();
                break;
            case CleanedCameraMode.LERPMODE:
                Debug.LogError("can't change to LerpMode");
                break;
            default:
                Debug.LogError("CameraMode not found");
                break;
        }

        cameraMode = newMode;

        //stuff to do when entering the new camera Mode
        switch (cameraMode)
        {
            case CleanedCameraMode.GodModeGlobal:
            case CleanedCameraMode.GODMODELOCAL:
                godModeCenterPoint.transform.position = GameManager.Instance.UpdateAveragePosition();

                DettachCharacterModeFollowPoint();
                SetupLerpMode(characterModeCameraFollowObject, lastGodPosition, cameraMode);

                break;
            case CleanedCameraMode.CHARACTERMODE:
                GameManager.Instance.ChangeState(GameState.CharacterView);

                InstantiateCharacterModeFollowPoint();
                SetupLerpMode(lastGodPosition, characterModeCameraFollowObject, cameraMode);

                break;
            case CleanedCameraMode.DeathCamera:
                transform.SetParent(godModeCenterPoint);
                GameManager.Instance.UpdateAveragePosition();
                godModeCenterPoint.transform.position = GameManager.Instance.averageCenterPointPosition;
                distanceToTarget = maxZoomGodMode;
                SetCameraPosition();
                break;
            default:
                Debug.LogError("CameraMode not found");
                break;
        }
    }

    private GameObject InstantiateCharacterModeFollowPoint()
    {
        characterModeCameraFollowObject = Instantiate(new GameObject(), GameManager.Instance.currentControlledCharacter.transform);
        characterModeCameraFollowObject.transform.position = characterModeCameraLocalPosition;
        characterModeCameraFollowObject.transform.eulerAngles = characterModeLocalEulerAngles;
        characterModeCameraFollowObject.name = "cameraPosition";
        return characterModeCameraFollowObject;
    }

    private GameObject DettachCharacterModeFollowPoint()
    {
        characterModeCameraFollowObject.transform.SetParent(null);
        return characterModeCameraFollowObject;
    }

    private void UpdateCameraBasedOnMode()
    {
        switch (cameraMode)
        {
            case CleanedCameraMode.GodModeGlobal:
                UpdateCameraGodmodeGlobal();
                break;
            case CleanedCameraMode.GODMODELOCAL:
                UpdateCameraGodModeLocal();
                break;
            case CleanedCameraMode.CHARACTERMODE:
                UpdateCameraFollowCharacter();
                break;
            case CleanedCameraMode.DeathCamera:
                UpdateCameraDeath();
                break;
            default:
                Debug.LogError("CameraMode not found");
                break;
        }
    }

    private void UpdateCameraDeath()
    {
        distanceToTarget = maxZoomGodMode;
        godModeCenterPoint.Rotate(Vector3.up, 20 * Time.deltaTime);
    }

    private void UpdateCameraGodmodeGlobal()
    {
        AdjustCameraPivotOffset();
        SetCameraPosition();

        //RotateGlobalCameraView
        if (Input.GetMouseButton(1))
        {
            godModeCenterPoint.localEulerAngles = rotationVector;
        }
    }

    private void UpdateCameraGodModeLocal()
    {
        AdjustCameraPivotOffset();
        SetCameraPosition();

        if (Input.GetMouseButton(1))
        {
            godModeCenterPoint.Rotate(rotationUpdateVector);
        }
    }

    private void UpdateCameraFollowCharacter()
    {
        AdjustCameraPivotOffset();
        transform.position = characterModeCameraFollowObject.transform.position;
        transform.eulerAngles = characterModeCameraFollowObject.transform.eulerAngles;
    }

    private void UpdateCameraLerpMode()
    {
        transform.position = Vector3.Lerp(lerpFromObject.transform.position, lerpToObject.transform.position, currentLerpTime / (lerpStartupTime + fullLerpTime));
        transform.eulerAngles = Vector3.Lerp(lerpFromObject.transform.eulerAngles, lerpToObject.transform.eulerAngles, currentLerpTime / (lerpStartupTime + fullLerpTime));

        if (currentLerpTime >= fullLerpTime)
        {
            cameraMode = lerpToMode;

            if (cameraMode == CleanedCameraMode.GODMODELOCAL)
            {
                transform.SetParent(godModeCenterPoint);
                transform.localPosition = lastGodPosition.transform.localPosition;
                transform.eulerAngles = lastGodPosition.transform.eulerAngles;
            }
        }
    }

    private void SetCameraPosition()
    {
        distanceToTarget -= scrollInput * zoomSpeed;
        distanceToTarget = Mathf.Clamp(distanceToTarget, minZoomGodMode, maxZoomGodMode);
        transform.localPosition = Vector3.forward * -distanceToTarget + cameraOffSet;
    }

    private void GetMouseInput()
    {
        scrollInput = Input.mouseScrollDelta.y;

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            mouseLeftEdge = Input.mousePosition.x < Screen.width * 1 / 5;
            mouseRightEdge = Input.mousePosition.x > Screen.width * 4 / 5;
        }

        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        if (!(mouseLeftEdge || mouseRightEdge))
        {
            mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * -1f;
            mouseZ = 0;
        }
        else if (mouseLeftEdge)
        {
            mouseY = 0;
            mouseZ = Input.GetAxis("Mouse Y") * mouseSensitivity * 1f;
        }
        else if (mouseRightEdge)
        {
            mouseY = 0;
            mouseZ = Input.GetAxis("Mouse Y") * mouseSensitivity * -1f;
        }
        else
        {
            Debug.LogError("mousePosition not recognised");
        }

        if (Input.GetMouseButton(1))
        {
            CalcRotationValues();
        }
    }

    private void CalcRotationValues()
    {
        rotationX += mouseY;
        rotationY += mouseX;
        rotationZ += mouseZ;

        rotationX = Mathf.Clamp(rotationX, -90, 90);

        rotationVector = new Vector3(rotationX, rotationY, rotationZ);

        rotationUpdateVector = new Vector3(mouseY, mouseX, mouseZ);
    }

    private void AdjustCameraPivotOffset()
    {
        if (Input.GetMouseButton(2))
        {
            cameraOffSet += (Vector3.right * -mouseX + Vector3.up * mouseY) * offSetMoveSpeed * distanceToTarget / maxZoomGodMode;
            cameraOffSet = cameraOffSet.ClampVectorComponentWise(-maxOffset, maxOffset);
        }
    }

    private void SetupLerpMode(GameObject lerpFromObject, GameObject lerpToObject, CleanedCameraMode lerpToMode)
    {
        this.lerpFromObject = lerpFromObject;
        this.lerpToObject = lerpToObject;
        this.currentLerpTime = lerpStartupTime;
        this.lerpToMode = lerpToMode;
    }
    */
}