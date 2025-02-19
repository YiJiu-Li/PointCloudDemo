using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
public class ConditionalFieldDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ConditionalFieldAttribute attribute = (ConditionalFieldAttribute)base.attribute;

        // Check if the condition field exists
        SerializedProperty conditionProp = property.serializedObject.FindProperty(attribute.ConditionField);

        if (conditionProp != null && conditionProp.boolValue)
        {
            // Draw the field if the condition is met
            EditorGUI.PropertyField(position, property, label);
        }
    }
}