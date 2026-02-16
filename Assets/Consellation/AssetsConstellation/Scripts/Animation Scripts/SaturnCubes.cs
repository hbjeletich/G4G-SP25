using UnityEngine;
public class RingCubeSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private Transform ringTransform;
    [SerializeField] private int numberOfCubes = 200;
    [SerializeField] private float minRingRadius = 15f;
    [SerializeField] private float maxRingRadius = 50f;
    [SerializeField] private float heightVariation = 5f;
    [SerializeField] private float minCubeScale = 0.3f;
    [SerializeField] private float maxCubeScale = 1.2f;
    [SerializeField] private float cubeTransparency = 0.5f; // Make cubes semi-transparent
    void Start()
    {
        SpawnCubesAlongRing();
    }
    void SpawnCubesAlongRing()
    {
        Vector3 ringWorldPos = ringTransform.position;
        for (int i = 0; i < numberOfCubes; i++)
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            // Random radius to spread across all rings
            float radius = Random.Range(minRingRadius, maxRingRadius);
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            float y = Random.Range(-heightVariation, heightVariation);
            Vector3 offset = new Vector3(x, y, z);
            Vector3 spawnPosition = ringWorldPos + offset;
            // Try to snap to ring mesh surface with raycast
            RaycastHit hit;
            Vector3 rayOrigin = ringWorldPos + new Vector3(x, 50f, z);
            Vector3 rayDirection = Vector3.down;
            if (Physics.Raycast(rayOrigin, rayDirection, out hit, 100f))
            {
                spawnPosition = hit.point;
            }
            GameObject cube = Instantiate(cubePrefab, spawnPosition, Quaternion.identity);
            // Random scale - smaller cubes
            float randomScale = Random.Range(minCubeScale, maxCubeScale);
            cube.transform.localScale = Vector3.one * randomScale;
            // Make cubes semi-transparent so they don't block view
            MakeTransparent(cube);
            cube.transform.SetParent(ringTransform, true);
        }
    }
    void MakeTransparent(GameObject cube)
    {
        Renderer renderer = cube.GetComponent<Renderer>();
        if (renderer != null && renderer.material != null)
        {
            // Create a copy of the material
            Material mat = new Material(renderer.material);
            // Set to transparent mode
            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
            // Set transparency
            Color color = mat.color;
            color.a = cubeTransparency;
            mat.color = color;
            renderer.material = mat;
        }
    }
}