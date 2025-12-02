using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SlotSceneRouter : MonoBehaviour
{
    public string sceneToLoad; // e.g. "BigDipper"

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (!string.IsNullOrEmpty(sceneToLoad))
                SceneManager.LoadScene(sceneToLoad);
            else
                Debug.LogWarning($"{name} has no scene assigned!");
        });
    }
}
