using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityModels;

[CustomEditor(typeof(MonoBehaviour), true)]
public class GenericModelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var method = target.GetType().GetMethod("GetModel");

        if (method == null)
            return;

        object model = method.Invoke(target, null);

        if (model == null)
        {
            EditorGUILayout.HelpBox("Model is null", MessageType.Warning);
            return;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Model", EditorStyles.boldLabel);

        DrawModelFields(model);
    }

    private void DrawModelFields(object model)
    {
        var type = model.GetType();

        // Fields
        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
        {
            DrawField(field, model);
        }

        // Properties (optional)
        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!prop.CanRead || !prop.CanWrite) continue;
            if (prop.GetIndexParameters().Length > 0) continue;

            DrawProperty(prop, model);
        }
    }

    private void DrawField(FieldInfo field, object model)
    {
        object value = field.GetValue(model);
        object newValue = DrawByType(field.Name, value, field.FieldType);

        if (!Equals(value, newValue))
        {
            Undo.RecordObject(target, $"Modify {field.Name}");
            field.SetValue(model, newValue);
            EditorUtility.SetDirty(target);
        }
    }

    private void DrawProperty(PropertyInfo prop, object model)
    {
        object value = prop.GetValue(model);
        object newValue = DrawByType(prop.Name, value, prop.PropertyType);

        if (!Equals(value, newValue))
        {
            Undo.RecordObject(target, $"Modify {prop.Name}");
            prop.SetValue(model, newValue);
            EditorUtility.SetDirty(target);
        }
    }

    private object DrawByType(string label, object value, System.Type type)
    {
        if (type == typeof(int))
            return EditorGUILayout.IntField(label, (int)value);

        if (type == typeof(float))
            return EditorGUILayout.FloatField(label, (float)value);

        if (type == typeof(string))
            return EditorGUILayout.TextField(label, (string)value);

        if (type == typeof(bool))
            return EditorGUILayout.Toggle(label, (bool)value);

        if (type.IsEnum)
            return EditorGUILayout.EnumPopup(label, (System.Enum)value);

        EditorGUILayout.LabelField(label, $"Unsupported type: {type.Name}");
        return value;
    }
}