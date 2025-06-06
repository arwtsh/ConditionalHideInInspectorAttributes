using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AdditionalInspectorAttributes.Editor
{
    [CustomPropertyDrawer(typeof(HideIfNullAttribute), true)]
    internal class HideIfNullDrawer : HideIfDrawer
    {
        protected override bool IsVisible(SerializedObject serializedObject)
        {
            HideIfNullAttribute conditional = (HideIfNullAttribute)attribute;

            if (System.String.IsNullOrEmpty(conditional.propertyName))
            {
                Debug.LogWarning("The string passed into " + conditional.GetType().Name + " on property " + fieldInfo.Name + " in type " + fieldInfo.DeclaringType + " was null or empty.");
                return true;
            }

            PropertyInfo propertyReference = serializedObject.targetObject.GetType().GetProperty(conditional.propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (propertyReference != null)
            {
                if (!Utilities.IsTypeNullable(propertyReference.PropertyType))
                {
                    Debug.LogWarning("The property " + propertyReference.Name + " in type " + fieldInfo.DeclaringType + " is type " + propertyReference.PropertyType.Name + ", which isn't nullable. " +
                        "The " + conditional.GetType().Name + " on property " + fieldInfo.Name + " in type " + fieldInfo.DeclaringType + " was unable to be parsed.");
                    return true;
                }

                return conditional.ApplyInvert(Utilities.IsNull(propertyReference.GetValue(serializedObject.targetObject)));
            }

            FieldInfo fieldReference = serializedObject.targetObject.GetType().GetField(conditional.propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldReference != null)
            {
                if (!Utilities.IsTypeNullable(fieldReference.FieldType))
                {
                    Debug.LogWarning("The field " + fieldReference.Name + " in type " + fieldInfo.DeclaringType + " is type " + fieldReference.FieldType.Name + ", which isn't nullable. " +
                        "The " + conditional.GetType().Name + " on property " + fieldInfo.Name + " in type " + fieldInfo.DeclaringType + " was unable to be parsed.");
                    return true;
                }

                return conditional.ApplyInvert(Utilities.IsNull(fieldReference.GetValue(serializedObject.targetObject)));
            }

            MethodInfo methodReference = serializedObject.targetObject.GetType().GetMethod(conditional.propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (methodReference != null)
            {
                if (!Utilities.IsTypeNullable(methodReference.ReturnType))
                {
                    Debug.LogWarning("The method " + methodReference.Name + " in type " + fieldInfo.DeclaringType + " has a return value of type " + methodReference.ReturnType.Name + ", which isn't nullable. " +
                        "The " + conditional.GetType().Name + " on property " + fieldInfo.Name + " in type " + fieldInfo.DeclaringType + " was unable to be parsed.");
                    return true;
                }

                return conditional.ApplyInvert(Utilities.IsNull(methodReference.Invoke(serializedObject.targetObject, null)));
            }

            //Failed to find the property, field, or method. Default to visible.
            Debug.LogWarning("The string passed into " + conditional.GetType().Name + " on property " + fieldInfo.Name + " in type " + fieldInfo.DeclaringType + " does not match any property, field, or method.");
            return true;
        }
    }
}
