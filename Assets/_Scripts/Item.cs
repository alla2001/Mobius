using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public float timeBeforeDespawn;
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

    }
}
