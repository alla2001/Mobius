using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static Unity.Burst.Intrinsics.X86.Avx;

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
    public UnityEvent<GameState> onStateChange = new UnityEvent<GameState>();
    [HideInInspector] public List<Character> allCharacters;
    [SerializeField] private List<GameObject> startingPositionPrefabs; 

    [HideInInspector] public List<GameObject> allWalls;
    private List<Vector3> allWallPositions = new List<Vector3>();
     public Vector3 averageCenterPointPosition;

    [SerializeField] GameObject placeCharacterRewardButton;
    [SerializeField] GameObject addBridgeEnergyRewardButton;
    [SerializeField] GameObject addCharactersLifeRewardButton;
    [Range(0, 100)]
    [SerializeField] int addCharacterSpawnChance = 100;

    [SerializeField]
    private float godModeTimeMultiplier;

    [SerializeField] private GameObject gameOverPanel;

    private CharacterMovement p_currentControlledCharacter;

    [SerializeField] private float[] timeModes;

    [Header("Black Hole Effect")]
    public GameObject blackHolePrefab;
    public float lerpSpeed;
    public float maxDistance;
    public float waitBlackHole;
    public CharacterMovement currentControlledCharacter
    {
        get 
        { 
            return p_currentControlledCharacter; 
        }
        set
        {
            if (p_currentControlledCharacter != null) { p_currentControlledCharacter.releaseControl(); }

            p_currentControlledCharacter = value;
            
            if (value != null) { p_currentControlledCharacter.takeControl(); }
        }
    }
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
        SpawnRandomStartingPosition();
        SwitchToTimeMode(0);
        Shader.SetGlobalFloat("_HolePull", 0);
    }

    private void SpawnRandomStartingPosition()
    {
        Instantiate(startingPositionPrefabs.GetRandomElement()); 
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
            SwitchToTimeMode(2); 
        }

        if(newState == GameState.ShapePlacement)
        {
            SwitchToTimeMode(0);  
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible=false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    public Vector3 UpdateAveragePosition()
    {
        for (int i = 0; i < allWalls.Count; i++)
        {
            if (allWalls[i] != null) 
            {
                allWallPositions.Add(allWalls[i].gameObject.transform.position);
            }
            
        }

        return averageCenterPointPosition = GetMeanVector(allWallPositions);
    }
    bool dead;
    // Update is called once per frame
    void Update()
    {
        if (CharacterInfo.characters.Count() == 0)
        {
            ChangeState(GameState.GameOver);
        }
        if (blackHoleActivated)
        {
            Shader.SetGlobalFloat("_HolePull",Mathf.Lerp(Shader.GetGlobalFloat("_HolePull"),1,lerpSpeed*Time.deltaTime));
            blackholeInstance.transform.localScale = Vector3.Lerp(blackholeInstance.transform.localScale,Vector3.one*8,6f*Time.deltaTime);
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

    void Death() //zypernKatze02 check this
    {
        ChangeState(GameState.GameOver);
        dead = true;
        StartCoroutine(WaitBlackHole());
    }

    public IEnumerator WaitBlackHole()
    {
        yield return new WaitForSeconds(waitBlackHole);
        TriggerBlackHole();
    }
    public void StartCharacterPlacement()
    {
        ChangeState(GameState.CharacterPlacement); 
    }

    public void SwitchToTimeMode(int timeMode)
    {
        Time.timeScale = timeModes[timeMode]; 
    }

    public void NextTimeMode()
    {
        for (int i = 0; i < timeModes.Length - 1; i++)
        {
            if (Time.timeScale == timeModes[i])
            {
                Time.timeScale = timeModes[i + 1];
                return;
            }
        }
        Time.timeScale = timeModes[0];
    }

    public void DetermineReward()
    {
        int probability = UnityEngine.Random.Range(0, 101);

        Debug.Log("Chance:" + probability + ", CHance: " + addCharacterSpawnChance);

        if (probability <= addCharacterSpawnChance)
        {
            Debug.Log("Lucky");
            addBridgeEnergyRewardButton.SetActive(true);
            placeCharacterRewardButton.SetActive(true);
            addCharactersLifeRewardButton.SetActive(false);
        }
        else
        {
            Debug.Log("Baza");
            addBridgeEnergyRewardButton.SetActive(true);
            addCharactersLifeRewardButton.SetActive(true);
            placeCharacterRewardButton.SetActive(false);
        }
    }
    
    bool blackHoleActivated;
    GameObject blackholeInstance;
    public void TriggerBlackHole()
    {
        Shader.SetGlobalFloat("_MaxDis", maxDistance);
        blackholeInstance=Instantiate(blackHolePrefab, Vector3.zero, Quaternion.identity);
        blackholeInstance.transform.localScale = Vector3.zero;
        blackHoleActivated = true;

    }
}
