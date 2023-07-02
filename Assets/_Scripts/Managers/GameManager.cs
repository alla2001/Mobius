using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum GameState
{
    GodView,
    RewardMode,
    ShapePlacement,
    CharacterPlacement,
    BridgeBuildMode,
    CharacterView
}


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Game variables
    public GameState currentState;
    [HideInInspector] public float gameTimeScaleCharacter = 1f;
    [HideInInspector] public float gameTimeScaleGodmode = 0.5f;

    [HideInInspector] public List<Character> allCharacters;

    [HideInInspector] public List<GameObject> allWalls;
    private List<Vector3> allWallPositions = new List<Vector3>();
     public Vector3 averageCenterPointPosition;


    public float speedForCharacterMode = 2f;
    public float speedForGodMode = 0.5f;

    [HideInInspector] public float timeLeft;
    int minutes;
    int seconds;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentState = GameState.BridgeBuildMode;
        timeLeft = 300;

        allCharacters = new List<Character>(FindObjectsOfType<Character>());
        allWalls = new List<GameObject>(GameObject.FindGameObjectsWithTag("wall"));
        UpdateAveragePosition();
    }

    public void UpdateAveragePosition()
    {
        for (int i = 0; i < allWalls.Count; i++)
        {
            allWallPositions.Add(allWalls[i].gameObject.transform.position);
        }

        averageCenterPointPosition = GetMeanVector(allWallPositions);
    }

    // Update is called once per frame
    void Update()
    {
        print(allWalls.Count);
        // Determine what time to use
        /*if (currentState == GameState.CharacterView)
            timeLeft -= Time.deltaTime * gameTimeScaleCharacter;
        else
            timeLeft -= Time.deltaTime * gameTimeScaleGodmode;*/

        /* if (timeLeft <= 0)
             Death();*/

        if (allCharacters.Count <= 0)
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
        currentState = GameState.BridgeBuildMode;
    }
}
