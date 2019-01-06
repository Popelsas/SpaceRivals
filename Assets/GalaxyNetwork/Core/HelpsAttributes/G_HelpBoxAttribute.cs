/////////////////////////////////////////////////////////////////////////////////
//
//	GalaxyNetwork tools
//
/////////////////////////////////////////////////////////////////////////////////

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;




/// <summary>
/// реализация справочной системы, которая может быть использована без специального редактора
/// </summary>
public class G_HelpBoxAttribute : PropertyAttribute
{

    public string Message;          // сообщение, которое будет показано в справочной системе
    public string ManualURL;        // ссылка на ручную главу. если установлено, эта ссылка откроется в веб-браузере, когда окно будет двойным щелчком

    public readonly bool ForceHeight;   // если это правда, справочная служба всегда будет вынуждена «Высота»
    public readonly bool ExtraHeight;   // если это правда, справочная служба всегда будет вынуждена «Высота»

    public readonly MessageType Type;   // determines which icon to show: 'Info', 'Warning', 'Error' or 'None'
                                        // NOTE: 'None' is drawn without an icon and with !GUI.enabled,
                                        // providing an option for a very subtle helpbox

    public Type TargetType;             // if set, the helpbox will only display on components of this type
    public Type RequireComponent;       // if set, the helpbox will only display if the gameobject has a component of this type

    public G_PropertyDrawerUtility.Space Bottom;   // what to add immediately below the helpbox (nothing, an empty line or a separator)


    /// <summary>
    /// 
    /// </summary>
    public G_HelpBoxAttribute(string message, MessageType type = MessageType.Info, Type targetType = null, Type requireComponent = null,/* int initialHeight = 50, */bool extraHeight = false, G_PropertyDrawerUtility.Space bottom = G_PropertyDrawerUtility.Space.Separator)
    {
        Message = message;
        Type = type;
        Bottom = bottom;
        ExtraHeight = extraHeight;
        TargetType = targetType;
        RequireComponent = requireComponent;
    }


    /// <summary>
    /// 
    /// </summary>
    //public G_HelpBoxAttribute(System.Type objectType, MessageType type = MessageType.Info, Type targetType = null, Type requireComponent = null,/* int initialHeight = 50, */bool extraHeight = false, G_PropertyDrawerUtility.Space bottom = G_PropertyDrawerUtility.Space.Separator)
    //{
    //    Message = G_Help.GetText(objectType);
    //    ManualURL = G_Help.GetURL(objectType);
    //    Type = type;
    //    Bottom = bottom;
    //    ExtraHeight = extraHeight;
    //    TargetType = targetType;
    //    RequireComponent = requireComponent;
    //}
    
}


/// <summary>
/// property drawer with editor rendering functions
/// </summary>
[CustomPropertyDrawer(typeof(G_HelpBoxAttribute))]
public class HelpBoxDrawer : PropertyDrawer
{

    private G_HelpBoxAttribute helpAttribute { get { return ((G_HelpBoxAttribute)attribute); } }

    private bool m_Enabled = true;

    private Type m_TargetType = null;
    private Component m_RequiredComponent = null;


    /// <summary>
    /// override to adjust with our own height. called by Unity
    /// </summary>
    public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
    {

        if (!m_Enabled)
            return 0;

        if (prop.propertyType != SerializedPropertyType.Float)
        {
            Debug.LogError("Error (HelpBox -> \"" + prop.name + "\") HelpBoxes can only be used on 'float' properties (needed to serialize helpbox height).");
            return 0;
        }

        if (prop.floatValue == -1)
            return 0;

        return prop.floatValue;

    }


    /// <summary>
    /// 
    /// </summary>
    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
    {

        if (string.IsNullOrEmpty(helpAttribute.Message))
            m_Enabled = false;

        if (!m_Enabled)
            return;

        if (prop.propertyType != SerializedPropertyType.Float)
            return;

        if (helpAttribute.TargetType != null)
        {
            if (m_TargetType == null)
            {
                m_TargetType = prop.serializedObject.targetObject.GetType();
                //Debug.Log(prop.serializedObject.targetObject.GetType().BaseType);	// uncomment to print currently drawn base type
            }

            if (helpAttribute.TargetType != m_TargetType &&
                helpAttribute.TargetType != m_TargetType.BaseType)  // TODO: evaluate. if this is too inclusive, add new attribute parameter
            {
                m_Enabled = false;
                return;
            }
        }

        if (helpAttribute.RequireComponent != null)
        {

            if (m_RequiredComponent == null)
                m_RequiredComponent = ((Component)prop.serializedObject.targetObject).GetComponent(helpAttribute.RequireComponent.ToString());

            if (m_RequiredComponent == null)
            {
                m_Enabled = false;
                return;
            }

        }

        if (prop.floatValue == -1)
            return;

        Rect rect = pos;
        rect.x += 16;
        rect.y += 10;
        rect.width -= 30;

        G_PropertyDrawerUtility.HelpBox(rect, helpAttribute.Message, helpAttribute.Type, helpAttribute.ManualURL);

        if (!helpAttribute.ForceHeight)
            prop.floatValue = G_PropertyDrawerUtility.CalcHelpBoxHeight(rect.width, helpAttribute.Message) +
                ((helpAttribute.Bottom != G_PropertyDrawerUtility.Space.Nothing) ? 16 : 0) +
                20;

        pos.y += prop.floatValue - 20;
        if (helpAttribute.Bottom == G_PropertyDrawerUtility.Space.Separator)
            G_PropertyDrawerUtility.Separator(pos);

    }

}


#endif


