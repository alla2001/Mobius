using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Icons")]
    [SerializeField] GameObject clockIcon; 
    [SerializeField] GameObject energyIcon; 
    [SerializeField] TextField energyText;
    public GameObject Character_energy_icon;
    public Text Character_energy_text; 

    [Header("Panels")]
    [SerializeField] GameObject gameStart;

    [SerializeField] GameObject explanationPanel; 
    //[SerializeField] GameObject godViewControls; 
    //[SerializeField] GameObject characterViewControls; 
    //[SerializeField] GameObject shapePlacementSpacebar; 
    
    [SerializeField] GameObject characterIcon;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject rewardSelectionPanel;

    private GameState savedGameState; //needed in order to deactivate the old UI
    [SerializeField] private GameObject activeExplanationPanel; 

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            Debug.LogError("Secondary FMODEvents destroyed on: " + gameObject.GetNameIncludingParents() + "\nFirst FMODEvents found on: " + instance.gameObject.GetNameIncludingParents());
        }
        instance = this; 
    }

    void Start()
    {
        GameManager.Instance.onStateChange.AddListener(ListenToGameState);

        gameStart.SetActive(false); 
        clockIcon.SetActive(true);
        energyIcon.SetActive(true);

        DeactivateUI(GameState.GodView); 
        DeactivateUI(GameState.CharacterView); 
        DeactivateUI(GameState.ShapePlacement);  
        DeactivateUI(GameState.RewardMode);
        DeactivateUI(GameState.CharacterPlacement);
        SwitchExplanationPanelActive(); 

        ActivateUI(savedGameState); 
    }

    private void OnDestroy()
    {
        GameManager.Instance.onStateChange.RemoveListener(ListenToGameState);
    }

    void Update()
    {
        // zypernKatze update the energy of bridges
        // & show / don't show the bridge-energy according to whether the playing is building a bridge
    }

    private void ListenToGameState(GameState gameState)
    {
        DeactivateUI(this.savedGameState);
        this.savedGameState = gameState; 
        ActivateUI(this.savedGameState); 
    }
    

    private void DeactivateUI(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.GodView:
                //godViewControls.SetActive(false); 

                break;
            case GameState.CharacterView:
                //characterViewControls.SetActive(false); 

                break;
            case GameState.RewardMode:
                rewardSelectionPanel.SetActive(false); 

                break;
            case GameState.CharacterPlacement:
                //godViewControls.SetActive(false);
                characterIcon.SetActive(false); 

                break;
            case GameState.ShapePlacement:
                //godViewControls.SetActive(false);
                //shapePlacementSpacebar.SetActive(false);
                break; 
            case GameState.GameOver: 
                gameOverPanel.SetActive(false);

                break; 
        }
    }

    private void ActivateUI(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.GodView:
                //godViewControls.SetActive(true);

                break;
            case GameState.CharacterView:
                //characterViewControls.SetActive(true);

                break;
            case GameState.RewardMode:
                rewardSelectionPanel.SetActive(true);
                GameManager.Instance.DetermineReward();

                break;
            case GameState.CharacterPlacement:
                //godViewControls.SetActive(true);
                characterIcon.SetActive(true);

                break;
            case GameState.ShapePlacement:
                //godViewControls.SetActive(true);
                //shapePlacementSpacebar.SetActive(true);

                break;
            case GameState.GameOver:
                gameOverPanel.SetActive(true);
                gameStart.SetActive(true);
                break;
        }
    }

    public void ReLoad()
    {
        GameManager.Instance. LoadNewGame();
    }
 
    public void SwitchExplanationPanelActive()
    {
        if (explanationPanel.activeSelf)
        {
            explanationPanel.SetActive(false);
            if (GameManager.Instance.currentState == GameState.CharacterView)
            {
                GameManager.Instance.SwitchToTimeMode(2); 
            }
            else
            {
                GameManager.Instance.SwitchToTimeMode(0);
            }
        }
        else
        {
            explanationPanel.SetActive(true);
            Time.timeScale = 0; 
        }
    }

    public void SwitchControlExplanationPanel(GameObject newActivePanel)
    {
        activeExplanationPanel.SetActive(false);
        activeExplanationPanel = newActivePanel; 
        activeExplanationPanel.SetActive(true);
    }
}
