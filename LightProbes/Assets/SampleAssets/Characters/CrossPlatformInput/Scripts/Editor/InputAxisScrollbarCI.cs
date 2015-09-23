using System;
using UnityEditor;
using UnityEditor.UI;

namespace UnitySampleAssets.CrossPlatformInput.Inspector
{
    [CustomEditor(typeof (InputAxisScrollbar))]
    public class InputAxisScrollbarCI : ScrollbarEditor
    {
        public override void OnInspectorGUI()
        {
            InputAxisScrollbar axisScrollbar = target as InputAxisScrollbar;
            axisScrollbar.axis = EditorGUILayout.TextField("Input Axis", axisScrollbar.axis);
            base.OnInspectorGUI();
        }
    }
}
