using System;
using System.Collections.Generic;
using System.Linq;
using Exanite.Core.Helpers;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace Exanite.Core.Editor
{
    /// <summary>
    /// Unity <see cref="EditorWindow"/> that allows the user to create any valid <see cref="ScriptableObject"/>
    /// </summary>
    public sealed class ScriptableObjectCreator : ScriptableObjectCreator<ScriptableObject>
    {
        /// <summary>
        /// Opens an <see cref="EditorWindow"/> that creates <see cref="ScriptableObject"/>s
        /// </summary>
        public static void OpenWindow()
        {
            CreateInstance<ScriptableObjectCreator>().Show();
        }
    }

    /// <summary>
    /// Abstract Unity <see cref="EditorWindow"/> that allows the user to create <see cref="ScriptableObject"/>s <para/>
    /// Note: Must be extended and made concrete to work because Unity does not support generics
    /// </summary>
    public abstract class ScriptableObjectCreator<T> : OdinEditorWindow where T : ScriptableObject
    {
        private IEnumerable<Type> validTypes;
        private Type selectedType;

        /// <summary>
        /// Called whenever this <see cref="EditorWindow"/> creates a <see cref="ScriptableObject"/>
        /// </summary>
        [OdinSerialize, HideInInspector]
        public Action<T> ScriptableObjectCreated { get; set; }
        
        /// <summary>
        /// <see cref="Type"/> of <see cref="ScriptableObject"/> to create
        /// </summary>
        [TitleGroup("$name", HorizontalLine = true)]
        [ShowInInspector, ValueDropdown(nameof(ValidTypes))]
        public Type SelectedType
        {
            get
            {
                return selectedType;
            }

            set
            {
                selectedType = value;

                DestroyPreview();
                CreatePreview();
            }
        }

        /// <summary>
        /// Preview of the object that is about to be created
        /// </summary>
        [ShowInInspector, TitleGroup("Preview", HorizontalLine = true)]
        [InlineEditor(DrawGUI = true, DrawHeader = false, DrawPreview = false, Expanded = true, ObjectFieldMode = InlineEditorObjectFieldModes.CompletelyHidden)]
        public ScriptableObject Preview { get; protected set; }

        /// <summary>
        /// The types that this <see cref="EditorWindow"/> is allowed to create
        /// </summary>
        protected IEnumerable<Type> ValidTypes
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

        protected override void Initialize()
        {
            base.Initialize();

            name = GetType().GetNiceName().SplitPascalCase();
            Reset();
        }

        /// <summary>
        /// Resets the <see cref="EditorWindow"/> to its default state
        /// </summary>
        [TitleGroup("Actions", HorizontalLine = true)]
        [HorizontalGroup("Actions/Buttons")]
        [Button(ButtonSizes.Large)]
        protected virtual void Reset()
        {
            SelectedType = null;
            DestroyPreview();
        }

        [TitleGroup("Actions", HorizontalLine = true)]
        [HorizontalGroup("Actions/Buttons")]
        [Button(ButtonSizes.Large), EnableIf(nameof(Preview))]
        protected virtual void ResetPreview()
        {
            DestroyPreview();
            CreatePreview();
        }

        /// <summary>
        /// Creates the <see cref="ScriptableObject"/>
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        [HorizontalGroup("Actions/Buttons")]
        [Button(ButtonSizes.Large), EnableIf(nameof(Preview))]
        public virtual void Create()
        {
            if (Preview == null)
            {
                throw new InvalidOperationException("Cannot create a new ScriptableObject at this time");
            }

            string path = EditorUtility.SaveFilePanel($"Save {SelectedType.Name} to folder", "Assets", $"new {SelectedType.Name}", "asset");

            if (path.IsNullOrWhitespace())
            {
                return;
            }

            try
            {
                path = FileHelper.GetAssetsRelativePath(path);
            }
            catch (ArgumentException)
            {
                EditorUtility.DisplayDialog($"Cannot create {typeof(T).Name}", $"Save location must be in the Unity Assets folder", "Ok");
                return;
            }

            AssetDatabase.CreateAsset(Preview, path);
            AssetDatabase.Refresh();
            EditorGUIUtility.PingObject(Preview);
            Selection.activeObject = Preview;
            ScriptableObjectCreated?.Invoke(Preview as T);

            CreatePreview();
        }

        /// <summary>
        /// Creates a new preview
        /// </summary>
        protected virtual void CreatePreview()
        {
            if (SelectedType != null)
            {
                Preview = CreateInstance(SelectedType);
            }
        }

        /// <summary>
        /// Destroys the currently previewed object
        /// </summary>
        protected virtual void DestroyPreview()
        {
            if (Preview)
            {
                DestroyImmediate(Preview);
            }
        }

        /// <summary>
        /// Gets all the types that this <see cref="EditorWindow"/> is allowed to create <para/>
        /// Note: Use ValidTypes instead as it is cached instead of calculating the result each time
        /// </summary>
        protected virtual IEnumerable<Type> GetValidTypes()
        {
            var result = AssemblyUtilities.GetTypes(AssemblyTypeFlags.CustomTypes)
                .Where(x =>
                    x.IsClass &&
                    x.IsConcrete() &&
                    typeof(T).IsAssignableFrom(x) &&
                    !typeof(UnityEditor.Editor).IsAssignableFrom(x) &&
                    !typeof(EditorWindow).IsAssignableFrom(x));

            return result.ToList();
        }
    }
}
