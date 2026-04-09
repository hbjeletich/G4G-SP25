using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class SpriteToImageAnimationConverter : EditorWindow
{
    private AnimationClip[] clips;
    private Vector2 scrollPos;
    private bool[] selected;

    [MenuItem("Tools/Sprite to Image Animation Converter")]
    static void Open() => GetWindow<SpriteToImageAnimationConverter>("Sprite→Image Converter");

    void OnGUI()
    {
        EditorGUILayout.HelpBox(
            "This tool rewrites AnimationClip keyframes that target SpriteRenderer.m_Sprite " +
            "to instead target Image.m_Sprite (UnityEngine.UI.Image).\n\n" +
            "Select clips below or drag them in. This OVERWRITES the original clips.",
            MessageType.Info);

        EditorGUILayout.Space();

        // Drag-drop area
        var dropArea = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drag AnimationClips here");
        HandleDragDrop(dropArea);

        // Also allow manual selection
        if (GUILayout.Button("Add from Selection (Project window)"))
            AddFromSelection();

        if (clips != null && clips.Length > 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Clips ({clips.Length})", EditorStyles.boldLabel);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.MaxHeight(300));
            for (int i = 0; i < clips.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                selected[i] = EditorGUILayout.Toggle(selected[i], GUILayout.Width(20));
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField(clips[i], typeof(AnimationClip), false);
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Select All")) SetAll(true);
            if (GUILayout.Button("Select None")) SetAll(false);
            if (GUILayout.Button("Clear List")) { clips = null; selected = null; }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUI.backgroundColor = new Color(1f, 0.6f, 0.6f);
            if (GUILayout.Button("Convert Selected (Overwrites!)", GUILayout.Height(30)))
                ConvertSelected();
            GUI.backgroundColor = Color.white;
        }
    }

    void HandleDragDrop(Rect area)
    {
        var evt = Event.current;
        if (!area.Contains(evt.mousePosition)) return;
        if (evt.type != EventType.DragUpdated && evt.type != EventType.DragPerform) return;

        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        if (evt.type == EventType.DragPerform)
        {
            DragAndDrop.AcceptDrag();
            var dragged = DragAndDrop.objectReferences.OfType<AnimationClip>().ToArray();
            AddClips(dragged);
        }
        evt.Use();
    }

    void AddFromSelection()
    {
        var sel = Selection.objects.OfType<AnimationClip>().ToArray();
        if (sel.Length == 0)
        {
            EditorUtility.DisplayDialog("Nothing selected", "Select one or more AnimationClips in the Project window first.", "OK");
            return;
        }
        AddClips(sel);
    }

    void AddClips(AnimationClip[] newClips)
    {
        var list = clips != null ? new List<AnimationClip>(clips) : new List<AnimationClip>();
        var selList = selected != null ? new List<bool>(selected) : new List<bool>();
        foreach (var c in newClips)
        {
            if (c != null && !list.Contains(c))
            {
                list.Add(c);
                selList.Add(true);
            }
        }
        clips = list.ToArray();
        selected = selList.ToArray();
    }

    void SetAll(bool val) { for (int i = 0; i < selected.Length; i++) selected[i] = val; }

    void ConvertSelected()
    {
        int converted = 0;
        int skipped = 0;

        for (int i = 0; i < clips.Length; i++)
        {
            if (!selected[i] || clips[i] == null) continue;
            if (ConvertClip(clips[i]))
                converted++;
            else
                skipped++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Done",
            $"Converted: {converted}\nSkipped (no SpriteRenderer bindings): {skipped}", "OK");
    }

    static bool ConvertClip(AnimationClip clip)
    {
        var bindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);
        bool changed = false;

        foreach (var binding in bindings)
        {
            // Match SpriteRenderer sprite property
            bool isSpriteRenderer =
                binding.type == typeof(SpriteRenderer) &&
                binding.propertyName == "m_Sprite";

            if (!isSpriteRenderer) continue;

            // Read existing keyframes
            var keyframes = AnimationUtility.GetObjectReferenceCurve(clip, binding);

            // Remove old curve
            AnimationUtility.SetObjectReferenceCurve(clip, binding, null);

            // Create new binding targeting Image
            var newBinding = new EditorCurveBinding
            {
                path = binding.path,
                type = typeof(Image),
                propertyName = "m_Sprite"
            };

            // Write keyframes to new binding
            AnimationUtility.SetObjectReferenceCurve(clip, newBinding, keyframes);
            changed = true;
        }

        if (changed)
        {
            EditorUtility.SetDirty(clip);
            Debug.Log($"[Sprite→Image] Converted: {clip.name}");
        }

        return changed;
    }
}