using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public enum GameState
{
    GodView,
    RewardMode,
    ShapePlacement,
    CharacterPlacement,
    CharacterView,
    GameOver
}


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int Score;

    // Game variables
    public  GameState currentState { get; private set; }    
    [HideInInspector] public float gameTimeScaleCharacter = 1f;
    [HideInInspector] public float gameTimeScaleGodmode = 0.5f;
    public UnityEvent<GameState> onStateChange = new UnityEvent<GameState>();
    [HideInInspector] public List<Character> allCharacters;

    [HideInInspector] public List<GameObject> allWalls;
    private List<Vector3> allWallPositions = new List<Vector3>();
     public Vector3 averageCenterPointPosition;


    public float speedForCharacterMode = 2f;
    public float speedForGodMode = 0.5f;

    [SerializeField] private GameObject gameOverPanel;

    public CharacterMovement currentControlledCharacter;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeState(GameState.GodView);

        gameOverPanel.SetActive(false);
        allCharacters = new List<Character>(FindObjectsOfType<Character>());
        allWalls = new List<GameObject>(GameObject.FindGameObjectsWithTag("wall"));
        UpdateAveragePosition();
    }

    public void ChangeState(GameState newState) 
    {
        onStateChange.Invoke(newState);
        currentState = newState;
       

        if (newState == GameState.CharacterView)
        {
            Time.timeScale = 1f;
        }
        else
        {
            Time.timeScale = 0.5f;
          
        }

        if(newState == GameState.ShapePlacement)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible=false;

        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true   ;

        }
    }
    public void UpdateAveragePosition()
    {
        for (int i = 0; i < allWalls.Count; i++)
        {
            if (allWalls[i] != null) 
            {
                allWallPositions.Add(allWalls[i].gameObject.transform.position);
            }
            
        }

        averageCenterPointPosition = GetMeanVector(allWallPositions);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == GameState.GameOver)
        {
            gameOverPanel.SetActive(true);
        }

        if (CharacterMovement.characters.Count() == 0)
        {
            Death();
        }
    }

    private Vector3 GetMeanVector(List<Vector3> positions)
    {
        if (positions.Count == 0)
            return Vector3.zero;

        float x = 0f;
        float y = 0f;
        float z = 0f;

        foreach (Vector3 pos in positions)
        {
            x += pos.x;
            y += pos.y;
            z += pos.z;
        }
        return new Vector3(x / positions.Count, y / positions.Count, z / positions.Count);
    }

    void Death()
    {
        currentState = GameState.GameOver;
    }

    public void StartCharacterPlacement()
    {
        currentState = GameState.CharacterPlacement;
    }

}
