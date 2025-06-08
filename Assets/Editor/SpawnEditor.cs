using Demon;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(StateControl))]
    public sealed class SpawnEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Spawn"))
            {
                var control = (target as StateControl);
                control.Start();
            }
        }
    }
}