#if UNITY_EDITOR
#if ODIN_INSPECTOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Exanite.Core.Utilities;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace Exanite.Core.Editor
{
    /// <summary>
    /// Unity <see cref="EditorWindow"/> that allows the user to create
    /// <see cref="ScriptableObject"/>s
    /// <para/>
    /// Note: Must be extended and made concrete to work because Unity
    /// does not support generics
    /// </summary>
    public class ScriptableObjectCreator : OdinEditorWindow
    {
        private List<Type> validTypes;
        private List<Assembly> validAssemblies;

        private Type selectedType;
        private Assembly selectedAssembly;

        private List<Type> filteredTypes;

        /// <summary>
        /// Editor window name.
        /// </summary>
        public virtual string Name { get; } = "Scriptable Object Creator";

        /// <summary>
        /// Selected assembly to filter the resulting types by.
        /// </summary>
        [TitleGroup("$name", HorizontalLine = true)]
        [ShowInInspector]
        [ValueDropdown(nameof(ValidAssemblies), SortDropdownItems = true, CopyValues = false)]
        public Assembly SelectedAssembly
        {
            get => selectedAssembly;

            set
            {
                selectedAssembly = value;
                selectedType = null;

                UpdateFilteredTypes();
            }
        }

        /// <summary>
        /// <see cref="Type"/> of <see cref="ScriptableObject"/> to create.
        /// </summary>
        [ShowInInspector]
        [ValueDropdown(nameof(FilteredTypes), SortDropdownItems = true, CopyValues = false)]
        public Type SelectedType
        {
            get => selectedType;

            set
            {
                selectedType = value;

                DestroyPreview();
                CreatePreview();
            }
        }

        /// <summary>
        /// Preview of the object that is about to be created.
        /// </summary>
        [ShowInInspector]
        [TitleGroup("Preview", HorizontalLine = true)]
        [InlineEditor(DrawGUI = true, DrawHeader = false, DrawPreview = false, Expanded = true,
            ObjectFieldMode = InlineEditorObjectFieldModes.CompletelyHidden)]
        public ScriptableObject Preview { get; protected set; }

        /// <summary>
        /// The types that this <see cref="EditorWindow"/> is allowed to
        /// create.
        /// </summary>
        protected List<Type> ValidTypes
        {
            get
            {
                if (validTypes == null)
                {
                    validTypes = GetValidTypes();
                }

                return validTypes;
            }
        }

        /// <summary>
        /// Assemblies containing <see cref="ValidTypes"/>.
        /// </summary>
        protected List<Assembly> ValidAssemblies
        {
            get
            {
                if (validAssemblies == null)
                {
                    validAssemblies = GetValidAssemblies();
                }

                return validAssemblies;
            }
        }

        /// <summary>
        /// List of types shown to the user in the editor window.
        /// </summary>
        protected List<Type> FilteredTypes
        {
            get
            {
                if (filteredTypes == null)
                {
                    UpdateFilteredTypes();
                }

                return filteredTypes;
            }
        }

        protected override void Initialize()
        {
            base.Initialize();

            name = Name;
            titleContent.text = Name;
            Reset();
        }

        /// <summary>
        /// Resets the <see cref="EditorWindow"/> to its default state.
        /// </summary>
        [TitleGroup("Actions", HorizontalLine = true)]
        [HorizontalGroup("Actions/Buttons")]
        [Button(ButtonSizes.Large)]
        protected virtual void Reset()
        {
            SelectedType = null;
            SelectedAssembly = null;

            DestroyPreview();
        }

        /// <summary>
        /// Resets the <see cref="Preview"/> to its default state.
        /// </summary>
        [TitleGroup("Actions", HorizontalLine = true)]
        [HorizontalGroup("Actions/Buttons")]
        [Button(ButtonSizes.Large)]
        [EnableIf(nameof(Preview))]
        protected virtual void ResetPreview()
        {
            DestroyPreview();
            CreatePreview();
        }

        /// <summary>
        /// Creates the <see cref="ScriptableObject"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        [HorizontalGroup("Actions/Buttons")]
        [Button(ButtonSizes.Large)]
        [EnableIf(nameof(Preview))]
        public virtual void Create()
        {
            if (Preview == null)
            {
                throw new InvalidOperationException("Cannot create a new ScriptableObject at this time");
            }

            var path = EditorUtility.SaveFilePanel($"Save {SelectedType.Name} to folder", "Assets", $"new {SelectedType.Name}", "asset");

            if (path.IsNullOrWhitespace())
            {
                return;
            }

            try
            {
                path = FileUtility.GetAssetsRelativePath(path);
            }
            catch (ArgumentException)
            {
                EditorUtility.DisplayDialog($"Cannot create {typeof(ScriptableObject).Name}", "Save location must be in the Unity Assets folder",
                    "Ok");

                return;
            }

            AssetDatabase.CreateAsset(Preview, path);
            AssetDatabase.Refresh();
            EditorGUIUtility.PingObject(Preview);
            Selection.activeObject = Preview;

            CreatePreview();
        }

        /// <summary>
        /// Creates a new preview.
        /// </summary>
        protected virtual void CreatePreview()
        {
            if (SelectedType != null)
            {
                Preview = CreateInstance(SelectedType);
            }
        }

        /// <summary>
        /// Destroys the currently previewed object.
        /// </summary>
        protected virtual void DestroyPreview()
        {
            if (Preview)
            {
                DestroyImmediate(Preview);
            }
        }

        /// <summary>
        /// Gets all the types that this <see cref="EditorWindow"/> is
        /// allowed to create.
        /// </summary>
        protected virtual List<Type> GetValidTypes()
        {
            return AssemblyUtilities.GetTypes(AssemblyTypeFlags.CustomTypes)
                .Where(x =>
                    x.IsClass
                    && x.IsConcrete()
                    && typeof(ScriptableObject).IsAssignableFrom(x)
                    && !typeof(UnityEditor.Editor).IsAssignableFrom(x)
                    && !typeof(EditorWindow).IsAssignableFrom(x))
                .ToList();
        }

        /// <summary>
        /// Gets all the assemblies containing the <see cref="validTypes"/>.
        /// </summary>
        protected virtual List<Assembly> GetValidAssemblies()
        {
            return ValidTypes
                .Select(x => x.Assembly)
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Returns the types that this window can create after filtering.
        /// </summary>
        protected virtual List<Type> GetFilteredTypes()
        {
            var results = (IEnumerable<Type>)ValidTypes;

            if (SelectedAssembly != null)
            {
                results = results.Where(x => x.Assembly == SelectedAssembly);
            }

            return results.ToList();
        }

        protected void UpdateFilteredTypes()
        {
            filteredTypes = GetFilteredTypes();
        }

        public static void OpenWindow<T>() where T : ScriptableObjectCreator
        {
            CreateInstance<T>().Show();
        }
    }
}
#endif
#endif
