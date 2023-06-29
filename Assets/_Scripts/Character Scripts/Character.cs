using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class Character : MonoBehaviour
{
    [SerializeField] private int lifeTime;
    private SplineFollower splineFollower;
    [HideInInspector] public bool isDead;
    [SerializeField] private float secondsLeft;

    private bool changesApplied;

    // Start is called before the first frame update
    void Start()
    {
        splineFollower = GetComponent<SplineFollower>();
        secondsLeft = lifeTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.currentState == GameState.CharacterView)
        {
            secondsLeft -= Time.deltaTime * GameManager.Instance.gameTimeScaleCharacter;
            splineFollower.followSpeed = GameManager.Instance.speedForCharacterMode;
        }

        else
        {
            secondsLeft -= Time.deltaTime * GameManager.Instance.gameTimeScaleGodmode;
            splineFollower.followSpeed = GameManager.Instance.speedForGodMode;
        }

        if (secondsLeft <= 0)
        {
            isDead = true;
            GameManager.Instance.allCharacters.Remove(this);
            this.gameObject.SetActive(false);
        }
    }
}
