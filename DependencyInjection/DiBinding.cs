#if EXANITE_UNIDI && ODIN_INSPECTOR
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UniDi;
using UnityEngine;

namespace Exanite.Core.DependencyInjection
{
    [DefaultExecutionOrder(-12000)]
    public class DiBinding : SerializedMonoBehaviour
    {
        [ListDrawerSettings(DefaultExpandedState = true, CustomAddFunction = nameof(AddBinding))]
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

            context.AddNormalInstaller(new DiBindingInstaller(bindings));
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
    }
}
#endif
