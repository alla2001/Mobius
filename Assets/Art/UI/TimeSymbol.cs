using UnityEngine;
using UnityEngine.UIElements;

public class TimeSymbol : MonoBehaviour
{
    public Image image;
    public float RotationSpeed; 

    private void Update()
    {
        transform.Rotate(transform.forward, RotationSpeed * Time.deltaTime); 
    }
}