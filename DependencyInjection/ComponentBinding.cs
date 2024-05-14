#if EXANITE_UNIDI && ODIN_INSPECTOR
using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UniDi;
using UnityEngine;
using UnityEngine.Serialization;
using SerializationUtility = Exanite.Core.Utilities.SerializationUtility;

namespace Exanite.Core.DependencyInjection
{
    [Serializable]
    public class ComponentBinding : ISerializationCallbackReceiver
    {
        [PropertyOrder(0)]
        [SerializeField] private Component component = new();

        [FormerlySerializedAs("newBindType")]
        [EnumToggleButtons]
        [SerializeField] private BindTypes bindTypes = BindTypes.Self;

        [HideInInspector]
        [FormerlySerializedAs("unitySerializedCustomBindTypes")]
        [SerializeField] private List<string> serializedCustomBindTypes = new();

        [ShowInInspector]
        [PropertyOrder(2)]
        [EnableIf(nameof(component))]
        [ShowIf(nameof(IsCustomBindTypeEnabled))]
        [ValidateInput(nameof(ValidateCustomBindTypes))]
        [ValueDropdown(nameof(GetValidBindTypes), ExcludeExistingValuesInList = true)]
        private List<Type> customBindTypes = new();

        public Component Component => component;

        public void Install(DiContainer container)
        {
            if (!component)
            {
                return;
            }

            container.Bind(GetTypesToBind()).FromInstance(component);
        }

        private IEnumerable<Type> GetTypesToBind()
        {
            var types = new HashSet<Type>();

            // Consider using bit masks here
            if ((bindTypes & BindTypes.Self) != 0)
            {
                types.Add(component.GetType());
            }

            if ((bindTypes & BindTypes.Interfaces) != 0)
            {
                foreach (var type in GetValidBindTypes().Where(t => t.IsInterface))
                {
                    types.Add(type);
                }
            }

            if ((bindTypes & BindTypes.Custom) != 0)
            {
                foreach (var customBindType in customBindTypes)
                {
                    types.Add(customBindType);
                }
            }

            types.Remove(null);

            return types;
        }

        private IEnumerable<Type> GetValidBindTypes()
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

        private bool IsCustomBindTypeEnabled()
        {
            return (bindTypes & BindTypes.Custom) != 0;
        }

        private bool ValidateCustomBindTypes(List<Type> customBindTypes, ref string errorMessage, ref InfoMessageType? messageType)
        {
            var validCustomBindTypes = new HashSet<Type>(GetValidBindTypes());
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

        public void OnBeforeSerialize()
        {
            serializedCustomBindTypes.Clear();
            foreach (var customBindType in customBindTypes)
            {
                serializedCustomBindTypes.Add(SerializationUtility.SerializeType(customBindType));
            }
        }

        public void OnAfterDeserialize()
        {
            customBindTypes.Clear();
            foreach (var serializedBindType in serializedCustomBindTypes)
            {
                customBindTypes.Add(SerializationUtility.DeserializeType(serializedBindType));
            }
        }
    }
}
#endif
