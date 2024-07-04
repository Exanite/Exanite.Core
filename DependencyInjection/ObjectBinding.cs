#if EXANITE_UNIDI && ODIN_INSPECTOR
using System;
using System.Collections.Generic;
using System.Linq;
using Exanite.Core.Types;
using Sirenix.OdinInspector;
using UniDi;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using SerializationUtility = Exanite.Core.Utilities.SerializationUtility;

namespace Exanite.Core.DependencyInjection
{
    [Serializable]
    public class ObjectBinding : ISerializationCallbackReceiver
    {
        /// <summary>
        /// Types ignored by <see cref="BindTypeFilter.Smart"/>.
        /// </summary>
        public static IReadOnlyCollection<Type> IgnoredTypes { get; } = new HashSet<Type>
        {
            typeof(MonoBehaviour),
            typeof(Behaviour),
            typeof(Component),
            typeof(ScriptableObject),
            typeof(Object),
            typeof(UIBehaviour),
            typeof(Graphic),
            typeof(MaskableGraphic),
        };

        [PropertyOrder(0)]
        [LabelText("Object")]
        [SerializeField] private Object instance = new();

        [EnumToggleButtons]
        [SerializeField] private BindTypeFilter filter = BindTypeFilter.Smart;

        [HideInInspector]
        [SerializeField] private List<string> serializedCustomTypes = new();

        [PropertyOrder(2)]
        [ShowInInspector]
        [ShowIf(nameof(IsCustomBindTypeEnabled))]
        [EnableIf(nameof(instance))]
        [ValidateInput(nameof(ValidateCustomBindTypes))]
        [ValueDropdown(nameof(GetBindTypesToDisplay), ExcludeExistingValuesInList = true)]
        private List<Type> customTypes = new();

        [PropertyOrder(3)]
        [ShowInInspector]
        [ReadOnly]
        [ShowIf(nameof(HasUnknownBindTypes))]
        [DelayedProperty]
        [InfoBox("Some types were not found. Please update or remove the unknown types (this requires manually editing Unity serialized data).", InfoMessageType.Error)]
        private List<string> unknownCustomTypes = new();

#if UNITY_EDITOR
        [PropertyOrder(3)]
        [ShowInInspector]
        [ReadOnly]
        private List<Type> TypesToBind
        {
            get
            {
                var results = GetTypesToBind().ToList();
                results.Sort((a, b) => string.Compare(a.ToString(), b.ToString(), StringComparison.Ordinal));

                return results;
            }
        }
#endif

        public Object Instance => instance;

        public void Install(DiContainer container)
        {
            if (!instance)
            {
                return;
            }

            container.Bind(GetTypesToBind()).FromInstance(instance);
        }

        private IEnumerable<Type> GetTypesToBindNonCustom()
        {
            var typeFilter = new TypeFilter();

            if ((filter & BindTypeFilter.Smart) != 0)
            {
                typeFilter.Add(new InheritanceHierarchyTypeFilter(IgnoredTypes, false, false));
                typeFilter.Interfaces();
            }

            if ((filter & BindTypeFilter.Self) != 0)
            {
                typeFilter.Self();
            }

            if ((filter & BindTypeFilter.Interfaces) != 0)
            {
                typeFilter.Interfaces();
            }

            return typeFilter.Filter(instance.GetType());
        }

        private IEnumerable<Type> GetTypesToBind()
        {
            var types = new HashSet<Type>(GetTypesToBindNonCustom());

            if ((filter & BindTypeFilter.Custom) != 0)
            {
                types.UnionWith(customTypes);
            }

            types.Remove(null!);

            return types;
        }

        private IEnumerable<Type> GetValidBindTypes()
        {
            var typeFilter = new TypeFilter();
            typeFilter.Interfaces();
            typeFilter.InheritanceHierarchy(null, false, false);

            return typeFilter.Filter(instance.GetType());
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
            return (filter & BindTypeFilter.Custom) != 0;
        }

        private bool HasUnknownBindTypes()
        {
            return unknownCustomTypes.Count > 0;
        }

        private bool ValidateCustomBindTypes(List<Type> customBindTypes, ref string errorMessage, ref InfoMessageType? messageType)
        {
            var validCustomBindTypes = new HashSet<Type>(GetValidBindTypes());
            foreach (var customBindType in customBindTypes)
            {
                if (!validCustomBindTypes.Contains(customBindType))
                {
                    var typeName = customBindType == null ? "Null" : customBindType.ToString();

                    errorMessage = $"{typeName} is not a valid bind type for {instance.GetType()}";
                    messageType = InfoMessageType.Error;

                    return false;
                }
            }

            return true;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            serializedCustomTypes.Clear();

            if ((filter & BindTypeFilter.Custom) != 0)
            {
                var typesToBindNonCustom = new HashSet<Type>(GetTypesToBindNonCustom());
                foreach (var customBindType in customTypes)
                {
                    if (typesToBindNonCustom.Contains(customBindType))
                    {
                        continue;
                    }

                    serializedCustomTypes.Add(SerializationUtility.SerializeType(customBindType));
                }

                serializedCustomTypes.AddRange(unknownCustomTypes);
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            customTypes.Clear();
            unknownCustomTypes.Clear();

            foreach (var serializedBindType in serializedCustomTypes)
            {
                try
                {
                    customTypes.Add(SerializationUtility.DeserializeType(serializedBindType)!);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);

                    unknownCustomTypes.Add(serializedBindType);
                }
            }
        }
    }
}
#endif
