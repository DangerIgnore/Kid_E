using UnityEngine;
using System.Collections;
using UnityEditor;


namespace Clap{

    [CustomEditor(typeof(InteractionObject))]
    [CanEditMultipleObjects]
    public class InteractionObject_Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            InteractionObject myScript = (InteractionObject)target;
            if (GUILayout.Button("Setup"))
            {
                myScript.EditorSetup();
            }
        }
    }

}