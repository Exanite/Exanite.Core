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
    [DefaultExecutionOrder(-12000)]
    public class DiBinding : SerializedMonoBehaviour
    {
        [ListDrawerSettings(Expanded = true, CustomAddFunction = nameof(AddBinding))]
        [OdinSerialize] private List<ComponentBinding> bindings = new();

        [SerializeField] private bool useSceneContext = false;
        [DisableIf(nameof(useSceneContext))] [SerializeField]
        private Context context = null;

        private void Awake()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            if (!context)
            {
                context = FindContext();

                if (!context)
                {
                    throw new DiBindingException("Failed to find context to bind to");
                }
            }

            foreach (var binding in bindings)
            {
                binding.Install(context.Container);
            }
        }

        public void Start()
        {
            // Define this method so we expose the enabled check box
        }

        private Context FindContext()
        {
            if (useSceneContext)
            {
                return FindSceneContext();
            }

            var context = FindContextInParent();
            if (context)
            {
                return context;
            }

            return FindSceneContext();
        }

        private Context FindContextInParent()
        {
            return GetComponentInParent<Context>();
        }

        private SceneContext FindSceneContext()
        {
            foreach (var rootGameObject in gameObject.scene.GetRootGameObjects())
            {
                var sceneContext = rootGameObject.GetComponentInChildren<SceneContext>();
                if (sceneContext)
                {
                    return sceneContext;
                }
            }

            return null;
        }

        private void AddBinding()
        {
            bindings.Add(new ComponentBinding());
        }

        public enum BindType
        {
            Self,
            AllInterfaces,
            AllInterfacesAndSelf,
            Custom,
        }

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
}
#endif
