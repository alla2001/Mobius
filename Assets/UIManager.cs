using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] TMP_Text timerText;
    int minutes;
    int seconds;
    string timerString;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Update timer from game manager
        minutes = (int)GameManager.Instance.timeLeft / 60;
        seconds = (int)GameManager.Instance.timeLeft % 60;

        timerString = $"{minutes}:{seconds}";
        timerText.text = timerString;
    }
}
