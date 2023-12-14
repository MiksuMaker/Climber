using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;

public class OnChangedCallAttribute : PropertyAttribute
{
    public string MethodName;
    public OnChangedCallAttribute(string methodname)
    {
        MethodName = methodname;
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(OnChangedCallAttribute))]
public class OnChangedAttributePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginChangeCheck();
        EditorGUI.PropertyField(position, property, label);
        if (EditorGUI.EndChangeCheck())
        {
            OnChangedCallAttribute at = attribute as OnChangedCallAttribute;
            MethodInfo method = property.serializedObject.targetObject.GetType().GetMethods().Where(m => m.Name == at.MethodName).First();

            if (method != null && method.GetParameters().Count() == 0)// Only instantiate methods with 0 parameters
                method.Invoke(property.serializedObject.targetObject, null);
        }
    }
}

#endif