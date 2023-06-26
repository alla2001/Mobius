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
    GameState currentState;
    const float gameTimeScaleCharacter = 1f;
    const float gameTimeScaleGodmode = 0.5f;
    [HideInInspector]public float timeLeft;
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
    }

    // Update is called once per frame
    void Update()
    {
        // Determine what time to use
        if (currentState == GameState.CharacterPlacement)
            timeLeft -= Time.deltaTime * gameTimeScaleCharacter;
        else
            timeLeft -= Time.deltaTime * gameTimeScaleGodmode;

        if (timeLeft <= 0)
            Death();
    }

    void Death()
    {
        currentState = GameState.BridgeBuildMode;
    }
}
