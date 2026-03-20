using UnityEngine;

public class AnimTest : MonoBehaviour
{
    private Animator animator;

    [Header("Reference Only (clips are driven by Animator Controller)")]
    [SerializeField] private string anim1Name = "lateral weight shift";
    [SerializeField] private string anim2Name = "hip abduction";
    [SerializeField] private string anim3Name = "leg lift";
    [SerializeField] private string anim4Name = "squat";

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("AnimTest: No Animator found on " + gameObject.name);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayAnimation(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayAnimation(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PlayAnimation(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            PlayAnimation(4);
        }
    }

    private void PlayAnimation(int index)
    {
        animator.SetInteger("AnimIndex", index);

        // Force the transition even if the same key is pressed again
        animator.Play("", 0, 0f);

        Debug.Log("Playing animation " + index);
    }
}