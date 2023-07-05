using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject panelRefernce;

    // Start is called before the first frame update
    void Start()
    {
        panelRefernce.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.currentState == GameState.RewardMode)
        {
            panelRefernce.gameObject.SetActive(true);
        }
        else
        {
            panelRefernce.gameObject.SetActive(false);
        }
    }

}
