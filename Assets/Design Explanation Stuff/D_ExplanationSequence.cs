using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; 

public class D_ExplanationSequence : MonoBehaviour
{
    [System.Serializable]
    public class GameObjectArray
    {
        public GameObject[] gameObjects;
    }

    public List<GameObjectArray> objectsToActivate; 
    public List<GameObjectArray> objectsToDeactivate;
    private int p_currentState = -1;
    public int currentState
    {
        get { return p_currentState; }
        set
        {
            p_currentState = value;
            OnStateChange();
            Debug.Log("currentState: " + currentState); 
        }
    }
    public int Length
    {
        get { return objectsToActivate.Count;  }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            p_currentState++; 
            changeObjectsByIndex(p_currentState);
        }
    }

    void changeObjectsByIndex(int i, bool reverse = false)
    {
        activateObjectsByIndex(i, reverse);
        deactivateObjectByIndex(i, reverse);
    }

    void activateObjectsByIndex(int i, bool reverse = false)
    {
        foreach(GameObject gO in objectsToActivate[i].gameObjects)
        {
            gO.SetActive(!reverse);
        }
    }

    void deactivateObjectByIndex(int i, bool reverse = false)
    {
        foreach (GameObject gO in objectsToDeactivate[i].gameObjects)
        {
            gO.SetActive(reverse);
        }
    }

    void DeactivateAllExplanationObjects()
    {
        foreach (GameObjectArray gA in objectsToActivate)
        {
            foreach (GameObject gO in gA.gameObjects)
            {
                gO.SetActive(false); 
            }
        }
    }

    private int oldState = -1; 
    public void OnStateChange()
    {
        if (p_currentState == -1)
        {
            DeactivateAllExplanationObjects();
            oldState = p_currentState;
        }
        else if (p_currentState > -1)
        {

            if (oldState < p_currentState)
            {
                do
                {
                    oldState++;
                    changeObjectsByIndex(oldState);
                } while (oldState != p_currentState); 
            }
            else
            {
                do
                {
                    changeObjectsByIndex(oldState, true);
                    oldState--;
                } while (oldState != p_currentState);
            }

        }
    }
}
