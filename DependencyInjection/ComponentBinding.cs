#if EXANITE_UNIDI && ODIN_INSPECTOR
using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UniDi;
using UnityEngine;
using SerializationUtility = Exanite.Core.Utilities.SerializationUtility;

namespace Exanite.Core.DependencyInjection
{
    [Serializable]
    public class ComponentBinding : ISerializationCallbackReceiver
    {
        [PropertyOrder(0)]
        [SerializeField] private Component component = new();

        [EnumToggleButtons]
        [SerializeField] private BindTypes newBindTypes = BindTypes.Smart;

        [HideInInspector]
        [SerializeField] private List<string> serializedCustomBindTypes = new();

        [PropertyOrder(2)]
        [ShowInInspector]
        [ShowIf(nameof(IsCustomBindTypeEnabled))]
        [EnableIf(nameof(component))]
        [ValidateInput(nameof(ValidateCustomBindTypes))]
        [ValueDropdown(nameof(GetBindTypesToDisplay), ExcludeExistingValuesInList = true)]
        private List<Type> customBindTypes = new();

        [PropertyOrder(3)]
        [ShowInInspector]
        [ReadOnly]
        [ShowIf(nameof(HasUnknownBindTypes))]
        [DelayedProperty]
        [InfoBox("Some types were not found. Please update or remove the unknown types (this requires manually editing Unity serialized data).", InfoMessageType.Error)]
        private List<string> unknownBindTypes = new();

        public Component Component => component;

        public void Install(DiContainer container)
        {
            if (!component)
            {
                return;
            }

            container.Bind(GetTypesToBind()).FromInstance(component);
        }

        private IEnumerable<Type> GetTypesToBindNonCustom()
        {
            var types = new HashSet<Type>();

            if ((newBindTypes & BindTypes.Self) != 0)
            {
                types.Add(component.GetType());
            }

            if ((newBindTypes & BindTypes.Interfaces) != 0)
            {
                foreach (var type in GetValidBindTypes().Where(t => t.IsInterface))
                {
                    types.Add(type);
                }
            }

            return types;
        }

        private IEnumerable<Type> GetTypesToBind()
        {
            var types = new HashSet<Type>(GetTypesToBindNonCustom());

            if ((newBindTypes & BindTypes.Custom) != 0)
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

        private IEnumerable<Type> GetBindTypesToDisplay()
        {
            var types = new HashSet<Type>(GetValidBindTypes());
            foreach (var type in GetTypesToBind())
            {
                types.Remove(type);
            }

            return types;
        }

        private bool IsCustomBindTypeEnabled()
        {
            return (newBindTypes & BindTypes.Custom) != 0;
        }

        private bool HasUnknownBindTypes()
        {
            return unknownBindTypes.Count > 0;
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

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            serializedCustomBindTypes.Clear();

            var typesToBindNonCustom = new HashSet<Type>(GetTypesToBindNonCustom());
            foreach (var customBindType in customBindTypes)
            {
                if (typesToBindNonCustom.Contains(customBindType))
                {
                    continue;
                }

                serializedCustomBindTypes.Add(SerializationUtility.SerializeType(customBindType));
            }

            serializedCustomBindTypes.AddRange(unknownBindTypes);

            serializedCustomBindTypes.Sort(StringComparer.Ordinal);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            customBindTypes.Clear();
            unknownBindTypes.Clear();

            foreach (var serializedBindType in serializedCustomBindTypes)
            {
                try
                {
                    customBindTypes.Add(SerializationUtility.DeserializeType(serializedBindType));
                }
                catch (Exception e)
                {
                    Debug.LogError(e);

                    unknownBindTypes.Add(serializedBindType);
                }
            }
        }
    }
}
#endif
