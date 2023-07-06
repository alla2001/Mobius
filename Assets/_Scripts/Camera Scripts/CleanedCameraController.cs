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
    public Transform godModeCenterPoint;

    public CharacterMovement currentPlayer;
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
                    transform.SetParent(godModeCenterPoint);
                    GameManager.Instance.UpdateAveragePosition(); 
                    godModeCenterPoint.transform.position = GameManager.Instance.averageCenterPointPosition; 
                    break;
                case CleanedCameraMode.FollowCharacter:
                    transform.SetParent(GameManager.Instance.currentControlledCharacter.transform); 
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

    //PUBLIC STATIC METHODS

    //MONOBEHAVIOUR METHODS
    private void Start()
    {
        godModeCenterPoint = transform.parent;
        
        cameraMode = CleanedCameraMode.GodModeLocal;

        GameManager.Instance.onStateChange.AddListener((state) => {

            if (state == GameState.CharacterView)
            {
                cameraMode = CleanedCameraMode.FollowCharacter;
            }
            else
            {
                cameraMode = CleanedCameraMode.GodModeLocal;
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //if (cameraMode == CleanedCameraMode.GodModeGlobal || cameraMode == CleanedCameraMode.GodModeLocal) 
            //{
            //    isGodModeLocal = !isGodModeLocal; //zypernKatze is not sure whether this might break stuff (cause a global cam can't ever be rotated in roll direction)
            //}
            //cameraMode = isGodModeLocal ? CleanedCameraMode.GodModeLocal : CleanedCameraMode.GodModeGlobal;
            cameraMode = CleanedCameraMode.GodModeLocal;

            GameManager.Instance.ChangeState(GameState.GodView);
            GameManager.Instance.currentControlledCharacter = null;
        }
        if (currentPlayer==null)
        {
            cameraMode= CleanedCameraMode.GodModeLocal;
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
                currentPlayer = hit.collider.gameObject.GetComponent<CharacterMovement>();
                GameManager.Instance.currentControlledCharacter = currentPlayer;
                cameraMode = CleanedCameraMode.FollowCharacter;
            }
        }
    }

    private void UpdateCameraBasedOnMode()
    {
        switch (cameraMode)
        {
            case CleanedCameraMode.GodModeGlobal:
                //GameManager.Instance.currentState = GameState.GodView;
                UpdateCameraGodmodeGlobal(); 
                break;
            case CleanedCameraMode.GodModeLocal:
                //GameManager.Instance.currentState = GameState.GodView;
                UpdateCameraGodModeLocal();
                break; 
            case CleanedCameraMode.FollowCharacter:
                GameManager.Instance.ChangeState(GameState.CharacterView);
                UpdateCameraFollowCharacter();
                break;
            default:
                Debug.LogError("CameraMode not found");
                break; 
        }
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
        transform.localPosition = characterModeCameraLocalPosition;
        transform.localEulerAngles = characterModeLocalEulerAngles; 


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

        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0))
        {
            mouseLeftEdge = Input.mousePosition.x < Screen.width * 1 / 5; 
            mouseRightEdge = Input.mousePosition.x > Screen.width * 4 / 5;
        }
        //When the player grabs the screen at the edge, they can do a roll-rotation

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

    private void OnDisable() //zypernKatze very much not the clean method
    {
        if (GameManager.Instance.currentState != GameState.CharacterView)
        {
            cameraMode = CleanedCameraMode.GodModeLocal; 
        }
    }
}