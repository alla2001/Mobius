using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;   


public class Item : MonoBehaviour
{
    public float timeBeforeDespawn;
    //public static UnityEvent itemCollected = new UnityEvent();
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
            GivePlayerPower(other.GetComponent<CharacterInfo>());
            AudioManager.instance.PlayOneShot(FMODEvents.instance.rewardCollected);
            FindObjectOfType<ItemSpawner>().DestroyItems(); 
        }
    }

    public void GivePlayerPower(CharacterInfo character)
    {
        GameManager.Instance.Score+= addedScore;
        character.RewardTime();
        GameManager.Instance.ChangeState(GameState.ShapePlacement);
    }
}
