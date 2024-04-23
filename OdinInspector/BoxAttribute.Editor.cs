// Originally from https://gist.github.com/oxysoft/66fe16fd12f1402232e8a0c770f3a89e

#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Exanite.Core.OdinInspector
{
    [DrawerPriority(0, 99)]
    public class ColorBoxDrawer : OdinAttributeDrawer<ColorBoxAttribute>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var hashCode = Property.ValueEntry.TypeOfValue.Name.GetHashCode();
            var h = (float)((hashCode + (double)int.MaxValue) / uint.MaxValue);
            var color = Color.HSVToRGB(h, 0.95f, 0.75f);
            color.a = 0.15f;
            BoxGui.BeginBox(color);
            CallNextDrawer(label);
            BoxGui.EndBox();
        }
    }

    [DrawerPriority(0, 99)]
    public class DarkBoxDrawer : OdinAttributeDrawer<DarkBoxAttribute>
    {
        public static readonly Color Color = EditorGUIUtility.isProSkin
            ? Color.Lerp(Color.black, Color.white, 0.1f)
            : Color.gray;

        protected override void DrawPropertyLayout(GUIContent label)
        {
            BoxGui.BeginBox(new Color(0, 0, 0, 0.15f));
            CallNextDrawer(label);

            BoxGui.EndBox(Attribute.WithBorders ? Color : null);
        }
    }

    public static class BoxGui
    {
        private static Rect CurrentLayoutRect;

        public static void BeginBox(Color color)
        {
            CurrentLayoutRect = EditorGUILayout.BeginVertical(SirenixGUIStyles.None);

            // Rect currentLayoutRect = GUIHelper.GetCurrentLayoutRect();
            if (Event.current.type == EventType.Repaint)
            {
                SirenixEditorGUI.DrawSolidRect(CurrentLayoutRect, color);
            }
        }

        public static void EndBox(Color? borders = null)
        {
            EditorGUILayout.EndVertical();

            if (Event.current.type == EventType.Repaint && borders != null)
            {
                SirenixEditorGUI.DrawBorders(CurrentLayoutRect, 1, 1, 1, 1, borders.Value);
            }

            GUILayout.Space(1);
        }
    }
}
#endif
