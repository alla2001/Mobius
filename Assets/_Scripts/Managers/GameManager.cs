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
    }

    // Update is called once per frame
    void Update()
    {
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

    void Death()
    {
        currentState = GameState.BridgeBuildMode;
    }
}
