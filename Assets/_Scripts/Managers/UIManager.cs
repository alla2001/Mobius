using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [Header("Icons")]
    [SerializeField] GameObject clockIcon; 
    [SerializeField] GameObject energyIcon; 
    [SerializeField] TextField energyText;

    [Header("Explanations")]
    [SerializeField] GameObject godViewControls; 
    [SerializeField] GameObject characterViewControls; 
    [SerializeField] GameObject rewardSelectionPanel;
    [SerializeField] GameObject shapePlacementSpacebar; 
    [SerializeField] GameObject characterIcon;

    private GameState gameState; 

    void Start()
    {
        GameManager.Instance.onStateChange.AddListener(ListenToGameState);

        clockIcon.SetActive(true);
        energyIcon.SetActive(true);

        DeactivateUI(GameState.GodView); 
        DeactivateUI(GameState.CharacterView); 
        DeactivateUI(GameState.ShapePlacement);  
        DeactivateUI(GameState.RewardMode);
        DeactivateUI(GameState.CharacterPlacement);

        ActivateUI(gameState); 
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
        DeactivateUI(this.gameState);
        this.gameState = gameState; 
        ActivateUI(this.gameState); 
    }
    

    private void DeactivateUI(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.GodView:
                godViewControls.SetActive(false); 

                break;
            case GameState.CharacterView:
                characterViewControls.SetActive(false); 

                break;
            case GameState.RewardMode:
                rewardSelectionPanel.SetActive(false); 

                break;
            case GameState.CharacterPlacement:
                godViewControls.SetActive(false);
                characterIcon.SetActive(false); 

                break;
            case GameState.ShapePlacement:
                godViewControls.SetActive(false);
                shapePlacementSpacebar.SetActive(false); 

                break; 
        }
    }

    private void ActivateUI(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.GodView:
                godViewControls.SetActive(true);

                break;
            case GameState.CharacterView:
                characterViewControls.SetActive(true);

                break;
            case GameState.RewardMode:
                rewardSelectionPanel.SetActive(true);

                break;
            case GameState.CharacterPlacement:
                godViewControls.SetActive(true);
                characterIcon.SetActive(true);

                break;
            case GameState.ShapePlacement:
                godViewControls.SetActive(true);
                shapePlacementSpacebar.SetActive(true);

                break;
        }
    }
}
