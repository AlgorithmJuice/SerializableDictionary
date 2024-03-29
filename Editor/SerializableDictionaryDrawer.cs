using System;
using System.Reflection;

using UnityEngine;

using UnityEditor;
using UnityEditorInternal;

[CustomPropertyDrawer(typeof(SerializableDictionary), true)]
public class SerializableDictionaryDrawer : PropertyDrawer {

    private ReorderableList list;
    private Func<Rect> VisibleRect;

    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
        if (list == null) {
            var listProp = property.FindPropertyRelative("list");
            list = new ReorderableList(property.serializedObject, listProp, true, false, true, true);
            list.drawElementCallback = DrawListItems;
            list.elementHeightCallback = elementHeightCallback;
        }

        var firstLine = position;
        firstLine.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(firstLine, property, label);

        if (property.isExpanded) {
            position.y += firstLine.height;

            if (VisibleRect == null) {
                 var tyGUIClip = System.Type.GetType("UnityEngine.GUIClip,UnityEngine");
                 if (tyGUIClip != null) {
                    var piVisibleRect = tyGUIClip.GetProperty("visibleRect", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    if (piVisibleRect != null) {
                        var getMethod = piVisibleRect.GetGetMethod(true) ?? piVisibleRect.GetGetMethod(false);
                        VisibleRect = (Func<Rect>)Delegate.CreateDelegate(typeof(Func<Rect>), getMethod);
                    }
                 }
            }

            var vRect = VisibleRect();
            vRect.y -= position.y;

            if (elementIndex == null)
                elementIndex = new GUIContent();

            list.DoList(position, vRect);
        }
    }

    private static GUIContent[] pairElementLabels => s_pairElementLabels ?? (s_pairElementLabels = new[] {new GUIContent("Key"), new GUIContent ("=>")});
    private static GUIContent[] s_pairElementLabels;

    private static GUIContent elementIndex;

    void DrawListItems(Rect rect, int index, bool isActive, bool isFocused) {
        SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index); // The element in the list

        var keyProp   = element.FindPropertyRelative("Key");
        var valueProp = element.FindPropertyRelative("Value");

        elementIndex.text = $"Element {index}";

        /*var label =*/ EditorGUI.BeginProperty(rect, elementIndex, element);

        var prevLabelWidth = EditorGUIUtility.labelWidth;

        var thirdWidth = rect.width / 3f;

        var keyRect = rect; //EditorGUI.PrefixLabel(rect, GUIUtility.GetControlID(FocusType.Passive), label);
        keyRect.width = thirdWidth;
        keyRect.y += 1f;
        keyRect.height = EditorGUIUtility.singleLineHeight;

        EditorGUIUtility.labelWidth = 30;

        EditorGUI.BeginChangeCheck();
        EditorGUI.PropertyField(keyRect, keyProp);

        var valueRect = rect; //EditorGUI.PrefixLabel(rect, GUIUtility.GetControlID(FocusType.Passive), label);
        valueRect.width = 2 * thirdWidth;
        valueRect.y += 1f;
        valueRect.height -= 2f;
        valueRect.x += thirdWidth + 8f;
        valueRect.xMin += 5f;

        EditorGUI.PropertyField(valueRect, valueProp);

        EditorGUIUtility.labelWidth = prevLabelWidth;

        EditorGUI.EndProperty();
  
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        float height;
        if (property.isExpanded) {
            var listProp = property.FindPropertyRelative("list");
            if (listProp.arraySize < 1){
                height = EditorGUIUtility.singleLineHeight + 70f;
            }
            else{
                int i = 0;
                int lineCount = 0;
                for(i = 0; i < listProp.arraySize; i++){
                    if(listProp.GetArrayElementAtIndex(i).FindPropertyRelative("Value").isExpanded){
                        lineCount += listProp.GetArrayElementAtIndex(i).FindPropertyRelative("Value").CountInProperty();
                    }
                    else{
                    lineCount++;
                    }
                }
            height = (EditorGUIUtility.singleLineHeight + 1.0f) * lineCount + 70f;
            }
        }
        else{
            height = EditorGUIUtility.singleLineHeight;
        }
        return height;
    }

    public float elementHeightCallback(int index){
        SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
        var valueProp = element.FindPropertyRelative("Value");
        return EditorGUIUtility.singleLineHeight * valueProp.CountInProperty();
    }
}
