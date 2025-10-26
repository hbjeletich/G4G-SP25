using UnityEditor;
using UnityEngine;

namespace CameraSnap.EditorTools
{
    public class AnimalCreatorWindow : EditorWindow
    {
        private string animalName = "NewAnimal";
        private Sprite sprite;
        private RuntimeAnimatorController animatorController;
        private AudioClip captureSound;

        private bool canWalk = true;
        private bool canHide = false;
        private float moveSpeed = 1f;
        private float patrolDistance = 2f;

        [MenuItem("Tools/CameraSnap/Animal Creator")]
        public static void ShowWindow()
        {
            GetWindow<AnimalCreatorWindow>("Animal Creator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Create New Animal", EditorStyles.boldLabel);

            animalName = EditorGUILayout.TextField("Animal Name", animalName);
            sprite = (Sprite)EditorGUILayout.ObjectField("Main Sprite", sprite, typeof(Sprite), false);
            animatorController = (RuntimeAnimatorController)EditorGUILayout.ObjectField("Animator Controller", animatorController, typeof(RuntimeAnimatorController), false);
            captureSound = (AudioClip)EditorGUILayout.ObjectField("Capture Sound", captureSound, typeof(AudioClip), false);

            GUILayout.Space(10);
            GUILayout.Label("Behavior Settings", EditorStyles.boldLabel);
            canWalk = EditorGUILayout.Toggle("Can Walk", canWalk);
            canHide = EditorGUILayout.Toggle("Can Hide in Bush", canHide);
            moveSpeed = EditorGUILayout.FloatField("Move Speed", moveSpeed);
            patrolDistance = EditorGUILayout.FloatField("Patrol Distance", patrolDistance);

            GUILayout.Space(20);
            if (GUILayout.Button("Generate Animal"))
            {
                CreateAnimalAssetAndPrefab();
            }
        }

        private void CreateAnimalAssetAndPrefab()
        {
            string folderPath = "Assets/CameraSnap/Animals/";
            if (!AssetDatabase.IsValidFolder(folderPath))
                AssetDatabase.CreateFolder("Assets/CameraSnap", "Animals");

            // Create AnimalData asset
            AnimalData data = ScriptableObject.CreateInstance<AnimalData>();
            data.animalName = animalName;
            data.canWalk = canWalk;
            data.canHideInBush = canHide;
            data.moveSpeed = moveSpeed;
            data.patrolDistance = patrolDistance;
            data.captureSound = captureSound;

            string dataPath = folderPath + animalName + "_Data.asset";
            AssetDatabase.CreateAsset(data, dataPath);
            AssetDatabase.SaveAssets();

            // Create prefab object hierarchy
            GameObject root = new GameObject(animalName);
            GameObject visual = new GameObject("Visual");
            visual.transform.SetParent(root.transform, false);

            // Sprite Renderer
            var sr = visual.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;

            // Animator
            var animator = visual.AddComponent<Animator>();
            animator.runtimeAnimatorController = animatorController;

            // Animal Behavior on root
            var behavior = root.AddComponent<AnimalBehavior>();
            behavior.animator = animator;
            behavior.animalData = data;

            // Add 3D collider to the ROOT object
            var box = root.AddComponent<BoxCollider>();
            if (sprite != null)
            {
                // Match sprite's physical dimensions
                box.size = sprite.bounds.size;
                // Position so feet rest on ground
                box.center = new Vector3(0, box.size.y / 2f, 0);
                visual.transform.localPosition = new Vector3(0, box.size.y / 2f, 0);
            }

            // Assign Animal layer
            int animalLayer = LayerMask.NameToLayer("Animal");
            if (animalLayer != -1)
            {
                root.layer = animalLayer;
            }
            else
            {
                Debug.LogWarning("Layer 'Animal' not found. Add it in Project Settings → Tags & Layers.");
            }

            // Save prefab
            string prefabPath = folderPath + animalName + ".prefab";
            GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            data.animalPrefab = savedPrefab;
            EditorUtility.SetDirty(data);

            AssetDatabase.SaveAssets();
            DestroyImmediate(root);

            Debug.Log($"Created Animal '{animalName}' with 3D collider + proper alignment!");
        }
    }
}
