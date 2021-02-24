using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EditorExtensions
{
    public class EditorTools
    {
        #region Object Tools
        [MenuItem("Tools/Object Tools/Centre Selected (Global)")]
        public static void CentreSelectedGlobal()
        {
            List<Transform> selected = Selection.transforms.ToList();

            Vector3 posAverage = selected.Aggregate(Vector3.zero, (sum, t) => sum + t.position) / selected.Count;
            foreach (Transform t in selected) { t.position -= posAverage; }
        }


        [MenuItem("Tools/Object Tools/Centre Selected (Local)")]
        public static void CentreSelectedLocal()
        {
            List<Transform> selected = Selection.transforms.ToList();

            Vector3 posAverage = selected.Aggregate(Vector3.zero, (sum, t) => sum + t.localPosition) / selected.Count;
            foreach (Transform t in selected) { t.localPosition -= posAverage; }
        }


        [MenuItem("Tools/Object Tools/Group")]
        public static void GroupObjects()
        {
            List<Transform> selected = Selection.transforms.ToList();

            Vector3 posAverage = selected.Aggregate(Vector3.zero, (sum, t) => sum + t.position) / selected.Count;
            Transform newParent = new GameObject("New Group").transform;
            newParent.position = posAverage;
            foreach (Transform t in selected) { t.SetParent(newParent); }
            Selection.SetActiveObjectWithContext(newParent, Selection.activeContext);
        }


        [MenuItem("Tools/Object Tools/Scale Child Positions")]
        public static void ScalePositions()
        {
            List<Transform> selected = Selection.transforms.ToList();

            if (selected.Count == 0)
                return;

            foreach (Transform parent in selected)
            {
                Vector3 scale = parent.localScale;

                foreach (Transform child in parent)
                {
                    Vector3 cPos = child.localPosition;
                    cPos.x *= scale.x;
                    cPos.y *= scale.y;
                    cPos.z *= scale.z;
                    child.localPosition = cPos;
                    child.localScale = scale;
                }

                parent.localScale = Vector3.one;
            }
        }


        [MenuItem("Tools/Object Tools/Apply Rotation To Children")]
        public static void RotatePositions()
        {
            List<Transform> selected = Selection.transforms.ToList();

            if (selected.Count == 0)
                return;

            foreach (Transform parent in selected)
            {
                Quaternion rotation = parent.localRotation;

                foreach (Transform child in parent)
                {
                    child.localPosition = rotation * child.localPosition;
                    child.localRotation *= rotation;
                }

                parent.localRotation = Quaternion.identity;
            }
        }
        #endregion

        #region uGUI Tools
        //http://answers.unity3d.com/questions/782478/unity-46-beta-anchor-snap-to-button-new-ui-system.html

        [MenuItem("Tools/uGUI/Anchors to Corners %[")]
        static void AnchorsToCorners()
        {
            RectTransform t = Selection.activeTransform as RectTransform;
            RectTransform pt = Selection.activeTransform.parent as RectTransform;

            if (t == null || pt == null)
                return;

            Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width,
                                                t.anchorMin.y + t.offsetMin.y / pt.rect.height);
            Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width,
                                                t.anchorMax.y + t.offsetMax.y / pt.rect.height);

            t.anchorMin = newAnchorsMin;
            t.anchorMax = newAnchorsMax;
            t.offsetMin = t.offsetMax = new Vector2(0, 0);
        }

        [MenuItem("Tools/uGUI/Corners to Anchors %]")]
        static void CornersToAnchors()
        {
            RectTransform t = Selection.activeTransform as RectTransform;

            if (t == null)
                return;

            t.offsetMin = t.offsetMax = new Vector2(0, 0);
        }
        #endregion

    }
}
