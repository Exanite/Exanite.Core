// Originally from https://gist.github.com/oxysoft/66fe16fd12f1402232e8a0c770f3a89e

#if UNITY_EDITOR && ODIN_INSPECTOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace Exanite.Core.OdinInspector
{
    public class InlineAttributeProcessor : OdinAttributeProcessor
    {
        public override bool CanProcessSelfAttributes(InspectorProperty property)
        {
            return property.Attributes.GetAttribute<InlineAttribute>() != null || property.Attributes.GetAttribute<SerializableAttribute>() != null;
        }

        public override bool CanProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member)
        {
            var inlineAttr = member.GetAttribute<InlineAttribute>(true) ?? member.GetReturnType().GetAttribute<InlineAttribute>(true);

            return inlineAttr != null;
        }

        public override void ProcessSelfAttributes(InspectorProperty property, List<Attribute> attributes)
        {
            var inlineAttr = attributes.OfType<InlineAttribute>().FirstOrDefault();
            if (inlineAttr == null)
            {
                return;
            }

            Set(inlineAttr, attributes, property.Info.TypeOfValue.IsAbstract);
        }

        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            Type? memberType = null;

            if (member is FieldInfo field)
            {
                memberType = field.FieldType;
            }

            if (member is PropertyInfo property)
            {
                memberType = property.PropertyType;
            }

            if (memberType == null)
            {
                return;
            }

            if (memberType.GetCustomAttributes(true).FirstOrDefault() is InlineAttribute inlineAttr)
            {
                var baseType = (member as FieldInfo)?.FieldType;
                Set(inlineAttr, attributes, baseType != null && baseType.IsAbstract);
            }
        }

        private static void Set(InlineAttribute inline, List<Attribute> attributes, bool isAbstract = false)
        {
            attributes.Add(new InlinePropertyAttribute());

            if (inline.Label == InlineLabelType.Hidden)
            {
                attributes.Add(new HideLabelAttribute());
            }

            if (!isAbstract)
            {
                // This will hide the reference picker.
                // Fields with abstract types still need this because the user must be able to choose the implementation to use.
                attributes.Add(new HideReferenceObjectPickerAttribute());
            }
        }
    }

    [DrawerPriority(0, 95)]
    public class InlineAttributeDrawer<TAttribute> : OdinAttributeDrawer<TAttribute>
        where TAttribute : InlineAttribute
    {
        protected override void DrawPropertyLayout(GUIContent? label)
        {
            if (Property.ValueEntry.WeakSmartValue == null &&
                Property.Attributes.Any(attr => attr is SerializableAttribute) &&
                !Property.Info.TypeOfValue.IsAbstract)
            {
                Property.ValueEntry.WeakSmartValue = Activator.CreateInstance(Property.Info.TypeOfValue);
            }

            switch (Attribute.Label)
            {
                case InlineLabelType.Hidden:
                {
                    label = null;

                    break;
                }
                case InlineLabelType.Above:
                {
                    SirenixEditorGUI.Title(Property.NiceName, "", TextAlignment.Left, false, false);
                    label = null;

                    break;
                }
                case InlineLabelType.BoldedAbove:
                {
                    SirenixEditorGUI.Title(Property.NiceName, "", TextAlignment.Left, false);
                    label = null;

                    break;
                }
            }

            CallNextDrawer(label);
        }
    }

    public class InlineBoxProcessor : OdinAttributeProcessor
    {
        public override bool CanProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member)
        {
            return member.GetAttribute<InlineBoxAttribute>() != null;
        }

        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            attributes.Add(new HideLabelAttribute());
            attributes.Add(new HideReferenceObjectPickerAttribute());
            attributes.Add(new InlinePropertyAttribute());
            attributes.Add(new BoxGroupAttribute(member.GetNiceName()));
        }
    }

    public class InlineFoldoutProcessor : OdinAttributeProcessor
    {
        public override bool CanProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member)
        {
            return member.GetAttribute<InlineFoldoutAttribute>() != null;
        }

        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            attributes.Add(new HideLabelAttribute());
            attributes.Add(new HideReferenceObjectPickerAttribute());
            attributes.Add(new InlinePropertyAttribute());
            attributes.Add(new FoldoutGroupAttribute(member.GetNiceName()));
        }
    }
}
#endif
