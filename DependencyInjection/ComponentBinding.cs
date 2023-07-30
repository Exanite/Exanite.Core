#if EXANITE_UNIDI && ODIN_INSPECTOR
using System;
using System.Collections.Generic;
using System.Linq;
using Exanite.Core.Utilities;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UniDi;
using UnityEngine;

namespace Exanite.Core.DependencyInjection
{
    [HideReferenceObjectPicker]
    [Serializable]
    public class ComponentBinding
    {
        [PropertyOrder(0)]
        [SerializeField] private Component component = new();

        [HideInInspector]
        [SerializeField] private BindType bindType = BindType.Self;

        [PropertyOrder(2)]
        [EnableIf(nameof(component))]
        [ShowIf(nameof(bindType), BindType.Custom)]
        [ValidateInput(nameof(ValidateCustomBindTypes))]
        [ValueDropdown(nameof(GetValidCustomBindTypes), ExcludeExistingValuesInList = true)]
        [OdinSerialize] private List<Type> customBindTypes = new();

        public Component Component => component;

        [PropertyOrder(1)]
        [ShowInInspector]
        public BindType BindType
        {
            get => bindType;
            set
            {
                if (bindType == value)
                {
                    return;
                }

                if (value == BindType.Custom)
                {
                    customBindTypes.Clear();
                    customBindTypes.AddRange(GetTypesForBindType(bindType));
                }

                bindType = value;
            }
        }

        public void Install(DiContainer container)
        {
            if (!component)
            {
                return;
            }

            switch (bindType)
            {
                case BindType.Self:
                {
                    container.Bind(component.GetType()).FromInstance(component);

                    break;
                }
                case BindType.AllInterfaces:
                {
                    container.BindInterfacesTo(component.GetType()).FromInstance(component);

                    break;
                }
                case BindType.AllInterfacesAndSelf:
                {
                    container.BindInterfacesAndSelfTo(component.GetType()).FromInstance(component);

                    break;
                }
                case BindType.Custom:
                {
                    customBindTypes.RemoveAll(t => t == null);
                    container.Bind(customBindTypes).FromInstance(component);

                    break;
                }
                default: throw ExceptionUtility.NotSupportedEnumValue(bindType);
            }
        }

        private IEnumerable<Type> GetTypesForBindType(BindType bindType)
        {
            // Consider using bit masks here
            if (bindType == BindType.Self || bindType == BindType.AllInterfacesAndSelf)
            {
                yield return component.GetType();
            }

            if (bindType == BindType.AllInterfaces || bindType == BindType.AllInterfacesAndSelf)
            {
                foreach (var type in GetValidCustomBindTypes().Where(t => t.IsInterface))
                {
                    yield return type;
                }
            }
        }

        private bool ValidateCustomBindTypes(List<Type> customBindTypes, ref string errorMessage, ref InfoMessageType? messageType)
        {
            var validCustomBindTypes = new HashSet<Type>(GetValidCustomBindTypes());
            foreach (var customBindType in customBindTypes)
            {
                if (!validCustomBindTypes.Contains(customBindType))
                {
                    var typeName = customBindType == null ? "Null" : customBindType.ToString();

                    errorMessage = $"{typeName} is not a valid bind type for {component.GetType()}";
                    messageType = InfoMessageType.Error;

                    return false;
                }
            }

            return true;
        }

        private IEnumerable<Type> GetValidCustomBindTypes()
        {
            if (!component)
            {
                yield break;
            }

            foreach (var interfaceType in component.GetType().GetInterfaces())
            {
                yield return interfaceType;
            }

            var currentType = component.GetType();
            while (currentType != null)
            {
                yield return currentType;

                currentType = currentType.BaseType;
            }
        }
    }
}
#endif
