#if EXANITE_UNIDI && ODIN_INSPECTOR
using System;
using System.Collections.Generic;
using Exanite.Core.Utilities;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UniDi;
using UnityEngine;

namespace Plugins.Exanite.Core.DependencyInjection
{
    [HideReferenceObjectPicker]
    [Serializable]
    public class ComponentBinding
    {
        [SerializeField] private Component component = new();

        [SerializeField] private BindType bindType = BindType.Self;

        [EnableIf(nameof(component))]
        [ShowIf(nameof(bindType), BindType.Custom)]
        [ValueDropdown(nameof(GetValidCustomBindTypes))]
        [OdinSerialize] private List<Type> customBindTypes = new();

        public Component Component => component;

        public BindType BindType => bindType;

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
                    container.Bind(customBindTypes).FromInstance(component);

                    break;
                }
                default: throw ExceptionUtility.NotSupportedEnumValue(bindType);
            }
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
