using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;   


public class Item : MonoBehaviour
{
    public float timeBeforeDespawn;
    public static UnityEvent itemCollected = new UnityEvent();
    public int addedScore=10;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public IEnumerator WaitDespawn()
    {
        yield return new WaitForSeconds(timeBeforeDespawn);
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GivePlayerPower();
            Destroy(gameObject);
        }
    }

    public void GivePlayerPower()
    {
        GameManager.Instance.Score+= addedScore;
        itemCollected?.Invoke();
        GameManager.Instance.ChangeState(GameState.ShapePlacement);
    }
}
