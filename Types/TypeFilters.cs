using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Exanite.Core.Types
{
    public static class TypeFilters
    {
        public static ITypeFilter Self { get; } = new SelfTypeFilter();
        public static ITypeFilter Interface { get; } = new InterfaceTypeFilter();
        public static ITypeFilter InheritanceHierarchy { get; } = new InheritanceHierarchyTypeFilter();
        public static ITypeFilter Smart { get; } = new TypeFilter().InheritanceHierarchy().Interfaces();

#if UNITY_2021_3_OR_NEWER
        /// <summary>
        /// Types ignored by the <see cref="UnitySmart"/> <see cref="ITypeFilter"/>.
        /// </summary>
        public static IReadOnlyCollection<Type> UnityIgnoredTypes { get; }

        public static ITypeFilter UnitySmart { get; }

        static TypeFilters()
        {
            UnityIgnoredTypes = new HashSet<Type>
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

            UnitySmart = new TypeFilter()
                .Add(new InheritanceHierarchyTypeFilter(UnityIgnoredTypes, false, false))
                .Interfaces();
        }
#endif
    }
}
