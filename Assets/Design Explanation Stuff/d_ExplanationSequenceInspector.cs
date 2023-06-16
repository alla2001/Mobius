using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(D_ExplanationSequence))]
public class d_ExplanationSequenceInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        D_ExplanationSequence explanationSequence = (D_ExplanationSequence)target;
        if (GUILayout.Button("nextStep"))
        {
            explanationSequence.currentState = Mathf.Clamp((explanationSequence.currentState + 1), 0, explanationSequence.Length);
        }
        if (GUILayout.Button("previousStep"))
        {
            explanationSequence.currentState = Mathf.Clamp((explanationSequence.currentState - 1), 0, explanationSequence.Length);
        }
    }
}
