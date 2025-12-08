using UnityEngine;
using UnityEngine.EventSystems;

public class enlargeOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector3 normalScale = Vector3.one;
    public Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1.2f);
    public float speed = 10f;

    private Vector3 targetScale;

    void Start()
    {
        targetScale = normalScale;       // initial target scale
        transform.localScale = normalScale;
    }

    void Update()
    {
        // Scaling towards target scale every frame
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * speed);
    }

    // When the mouse pointer hovers over the object
    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = hoverScale;    // set target to hover scale
    }

    // When the mouse pointer stops hovering over the object
    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = normalScale;    // set target back to normal scale
    }
}
