using UnityEngine;
using UnityEngine.UI;

public enum CleanedCameraMode
{
    GodModeGlobal,
    GodModeLocal,
    FollowCharacter
}

public class CleanedCameraController : MonoBehaviour
{
    //PUBLIC STATICS & EVENTS

    //EDITOR REFERENCES

    //CODE REFERENCES
    private Transform centerPoint;
    private GameObject characterRef;

    //EDITOR VARIABLES
    [Header("Camera Modes")]
    [SerializeField]
    private CleanedCameraMode p_cameraMode;


    //ensuring you can't change camera mode without changing the min & the max Zoom
    public CleanedCameraMode cameraMode
    {
        get { return p_cameraMode; }
        set
        {
            p_cameraMode = value;
            switch (p_cameraMode)
            {
                case CleanedCameraMode.GodModeGlobal:
                case CleanedCameraMode.GodModeLocal:
                    minZoomCurrent = minZoomGodMode;
                    maxZoomCurrent = maxZoomGodMode;
                    break;
                case CleanedCameraMode.FollowCharacter:
                    minZoomCurrent = minZoomCharacterMode;
                    maxZoomCurrent = maxZoomCharacterMode;
                    break;
                default:
                    Debug.LogError("CameraMode not found");
                    break;
            }
        }
    }

    [Header("Camera Settings")]
    [SerializeField] private float mouseSensitivity = 3.0f;
    [SerializeField] private float zoomSpeed = 3.0f;
    [SerializeField] private float minZoomGodMode = 5.0f; 
    [SerializeField] private float maxZoomGodMode = 50.0f;
    [SerializeField] private float minZoomCharacterMode = 5.0f;
    [SerializeField] private float maxZoomCharacterMode = 10.0f;

    //CODE VARIABLES
    private float rotationY;
    private float rotationX;
    private Vector3 rotationVector;
    private Vector3 rotationUpdateVector;
    private float mouseX;
    private float mouseY;
    private float distanceToTarget = 10.0f;
    private float minZoomCurrent;
    private float maxZoomCurrent;
    private float scrollInput; 
    private bool isGodModeLocal = true;

    //PUBLIC STATIC METHODS

    //MONOBEHAVIOUR METHODS
    private void Start()
    {
        centerPoint = transform.parent;
        characterRef = GameObject.FindGameObjectWithTag("Player");

        cameraMode = p_cameraMode; 
    }

    private void Update()
    {
        CheckCameraModeChange();
        GetInput();
        ExecuteInput(); 
    }

    //IN SCENE METHODS (e.g. things that need to be accessed by unityEvents)

    //PUBLIC CODE METHODS

    //PRIVATE CODE METHODS
    private void CheckCameraModeChange()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (cameraMode == CleanedCameraMode.GodModeGlobal || cameraMode == CleanedCameraMode.GodModeLocal) 
            {
                isGodModeLocal = !isGodModeLocal; //zypernKatze is not sure whether this might break stuff (cause a global cam can't ever be rotated in roll direction)
            }
            cameraMode = isGodModeLocal ? CleanedCameraMode.GodModeLocal : CleanedCameraMode.GodModeGlobal; 


        }

        if (cameraMode != CleanedCameraMode.FollowCharacter)
        {
            CheckCharacterClick(); 
        }
    }

    private void CheckCharacterClick()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.tag == "Player" && Input.GetMouseButtonDown(0))
            {
                characterRef = hit.transform.gameObject;
                cameraMode = CleanedCameraMode.FollowCharacter;
            }
        }
    }

    private void GetInput()
    {
        scrollInput = Input.mouseScrollDelta.y;
        if (Input.GetMouseButton(1))
        {
            mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * -1f;

            rotationX += mouseY;
            rotationY += mouseX;

            rotationX = Mathf.Clamp(rotationX, -90, 90);

            rotationVector = new Vector3(rotationX, rotationY);

            rotationUpdateVector = new Vector3(mouseY, mouseX, 0);

            
            }
    }

    private void ExecuteInput()
    {
        //cameraRotation
        switch (cameraMode)
        {
            case CleanedCameraMode.GodModeGlobal:
                UpdateCameraGodmodeGlobal(); 
                break;
            case CleanedCameraMode.GodModeLocal:
                UpdateCameraGodModeLocal();
                break; 
            case CleanedCameraMode.FollowCharacter:
                UpdateCameraFollowCharacter();
                break;
            default:
                Debug.LogError("CameraMode not found");
                break; 
        }
    }

    private void UpdateCameraGodmodeGlobal()
    {
        PositionCamera();
        if (Input.GetMouseButton(1))
        {
            centerPoint.eulerAngles = rotationVector;
        }
    }

    private void UpdateCameraGodModeLocal()
    {
        PositionCamera();
        if (Input.GetMouseButton(1))
        {
            centerPoint.Rotate(rotationUpdateVector);
        }
    }

    private void UpdateCameraFollowCharacter()
    {
        centerPoint.position = characterRef.transform.position;
        centerPoint.rotation = characterRef.transform.rotation;
        PositionCamera();
    }

    private void PositionCamera()
    {
        transform.position = centerPoint.position - transform.forward * distanceToTarget;
        distanceToTarget -= scrollInput * zoomSpeed;
        distanceToTarget = Mathf.Clamp(distanceToTarget, minZoomCurrent, maxZoomCurrent);
    }
}