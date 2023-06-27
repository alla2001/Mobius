using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public enum CleanedCameraMode
{
    GodModeGlobal,
    GodModeLocal,
    FollowCharacter
}

public class CleanedCameraController : MonoBehaviour
{
    //DEBUG
    public KeyCode isThisTheMouseWheel;

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
    [SerializeField] private float moveSpeed = .3f; 

    //CODE VARIABLES
    private float rotationZ;
    private float rotationY;
    private float rotationX;
    private Vector3 rotationVector;
    private Vector3 rotationUpdateVector;
    private float mouseX;
    private float mouseY;
    private float mouseZ;
    private bool mouseDownAtEdge; 
    private bool mouseRollMode => mouseDownAtEdge; 
    private float distanceToTarget = 10.0f;
    private float minZoomCurrent;
    private float maxZoomCurrent;
    private float scrollInput;
    private Vector3 cameraOffSet; 
    private bool isGodModeLocal = true;

    //PUBLIC STATIC METHODS

    //MONOBEHAVIOUR METHODS
    private void Start()
    {
        centerPoint = transform.parent;
        characterRef = GameObject.FindGameObjectWithTag("Player");

        cameraMode = CleanedCameraMode.GodModeLocal; 
    }

    private void Update()
    {
        CheckCameraModeChange();
        UpdateCameraBasedOnMode(); 
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

    private void UpdateCameraBasedOnMode()
    {
        switch (cameraMode)
        {
            case CleanedCameraMode.GodModeGlobal:
                GameManager.Instance.currentState = GameState.GodView;
                UpdateCameraGodmodeGlobal(); 
                break;
            case CleanedCameraMode.GodModeLocal:
                GameManager.Instance.currentState = GameState.GodView;
                UpdateCameraGodModeLocal();
                break; 
            case CleanedCameraMode.FollowCharacter:
                GameManager.Instance.currentState = GameState.CharacterView;
                UpdateCameraFollowCharacter();
                break;
            default:
                Debug.LogError("CameraMode not found");
                break; 
        }
    }

    private void UpdateCameraGodmodeGlobal()
    {
        GetMouseInput();

        MoveCameraPivot();
        SetCameraPosition();

        //RotateGlobal
        if (Input.GetMouseButton(1))
        {
            centerPoint.eulerAngles = rotationVector;
        }
    }

    private void UpdateCameraGodModeLocal()
    {
        GetMouseInput();

        MoveCameraPivot();
        SetCameraPosition();

        //RotateLocal
        if (Input.GetMouseButton(1))
        {
            centerPoint.Rotate(rotationUpdateVector);
        }
    }

    private void UpdateCameraFollowCharacter()
    {
        centerPoint.position = characterRef.transform.position;
        centerPoint.rotation = characterRef.transform.rotation;
        
        SetCameraPosition();
    }
    private void SetCameraPosition()
    {
        transform.position = centerPoint.position - transform.forward * distanceToTarget + cameraOffSet;
        distanceToTarget -= scrollInput * zoomSpeed;
        distanceToTarget = Mathf.Clamp(distanceToTarget, minZoomCurrent, maxZoomCurrent);
    }

    private void GetMouseInput()
    {
        scrollInput = Input.mouseScrollDelta.y;

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("mouseButtonDown"); 
                mouseDownAtEdge = 
                    Input.mousePosition.x < Screen.width * 2 / 5 ||
                    Input.mousePosition.x > Screen.width * 4 / 5;
        }

        //When the player grabs the screen at the edge, they can do a roll-rotation
        mouseX = !mouseRollMode ? Input.GetAxis("Mouse X") * mouseSensitivity : Input.GetAxis("Mouse X") * mouseSensitivity;
        mouseY = !mouseRollMode ? Input.GetAxis("Mouse Y") * mouseSensitivity * -1f : 0;
        mouseZ = mouseRollMode ? Input.GetAxis("Mouse Y") * mouseSensitivity * -1f : 0;

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

    private void MoveCameraPivot()
    { 
        if (Input.GetMouseButton(2))
        {
            cameraOffSet += (-centerPoint.right * mouseX + centerPoint.up * mouseY) * moveSpeed;
            Debug.Log("CameraOffset" + cameraOffSet); 
        }
    }
}