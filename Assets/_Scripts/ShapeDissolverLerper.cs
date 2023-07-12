using UnityEngine; 

public class ShapeDissolverLerper : MonoBehaviour
{
    //PUBLIC STATICS & EVENTS

    //REFERENCES

    //EDITORVALUES
    float dissolveTime = 2; 

    //PUBLIC VARIABLES
    public float shouldDissolveValue
    {
        get { return p_shouldDissolveValue; }
        set 
        { 
            enabled = true;
            timeSinceDissolveStart = 0; 
            p_isDissolveValue = material.GetFloat("_DissolveValue"); 
            p_shouldDissolveValue = value;
        }
    }

    //PRIVATE VARIABLES
    private Material material;
    private float p_isDissolveValue;
    private float p_shouldDissolveValue = 1f;
    private float timeSinceDissolveStart; 


    //PUBLIC STATIC METHODS

    //MONOBEHAVIOUR METHODS    
    private void Start()
    {
        material = GetComponent<Renderer>().material; 
    }

    private void Update()
    {
        timeSinceDissolveStart += Time.deltaTime;
        material.SetFloat("_DissolveValue", Mathf.Lerp(p_isDissolveValue, p_shouldDissolveValue, timeSinceDissolveStart/dissolveTime));
        if (timeSinceDissolveStart > dissolveTime)
        {
            enabled = false;  
        }
    }

    //IN SCENE METHODS (e.g. things that need to be accessed by unityEvents)

    //PUBLIC CODE METHODS

    //PRIVATE CODE METHODS
}