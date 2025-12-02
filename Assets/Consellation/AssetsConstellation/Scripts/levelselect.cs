using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[ExecuteAlways]
public class AutoGallery : MonoBehaviour
{
    [Header("General")]
    public int columns = 3;
    public int rows = 2;
    public int totalSlots = 6;

    [Header("References (optional, auto-created if null)")]
    public Canvas targetCanvas;        // will be found/created if null
    public Transform imageGridParent;  // will be created under canvas if null
    public GameObject infoBox;         // auto-created if null
    public Text infoText;              // auto-created if null (child of infoBox)

    [Header("Slot appearance")]
    public Vector2 slotSize = new Vector2(300, 180);
    public Sprite defaultSlotSprite;   // optional background sprite for slots
    public Font defaultFont;           // optional font for slot labels & infoText

    [Header("Hover / animation")]
    public float hoverScale = 1.12f;
    public float shrinkScale = 0.94f;
    public float smoothSpeed = 8f;
    public float infoBoxFadeSpeed = 8f;

    // internals
    private readonly List<RectTransform> slots = new List<RectTransform>();
    private readonly Dictionary<RectTransform, Vector3> targetScales = new Dictionary<RectTransform, Vector3>();
    private CanvasGroup infoBoxCanvasGroup;
    private RectTransform highlighted = null;
    private readonly Dictionary<RectTransform, string> descriptions = new Dictionary<RectTransform, string>();

    void Awake()
    {
        EnsureEventSystem();
        EnsureCanvas();
        EnsureInfoBox();
        EnsureGridAndSlots();
    }

    void Start()
    {
        // Initialize target scales
        foreach (var s in slots)
        {
            if (!targetScales.ContainsKey(s)) targetScales[s] = Vector3.one;
        }
        // Make sure info box canvas group alpha starts hidden
        if (infoBoxCanvasGroup != null) infoBoxCanvasGroup.alpha = 0f;
    }

    void Update()
    {
        // Smoothly animate slot scaling and label scaling
        foreach (var s in slots)
        {
            Vector3 cur = s.localScale;
            Vector3 tgt = targetScales[s];
            s.localScale = Vector3.Lerp(cur, tgt, Time.deltaTime * smoothSpeed);

            // scale label if present
            Text label = s.GetComponentInChildren<Text>();
            if (label != null)
            {
                Vector3 labelTarget = (tgt == Vector3.one) ? Vector3.one : Vector3.one * (tgt.x);
                label.transform.localScale = Vector3.Lerp(label.transform.localScale, labelTarget, Time.deltaTime * smoothSpeed * 1.1f);
            }
        }

        // Fade info box in/out depending on whether highlighted (or showed via click)
        if (infoBoxCanvasGroup != null)
        {
            float targetAlpha = (infoBox != null && infoBox.activeSelf) ? 1f : 0f;
            infoBoxCanvasGroup.alpha = Mathf.Lerp(infoBoxCanvasGroup.alpha, targetAlpha, Time.deltaTime * infoBoxFadeSpeed);
            // optionally disable raycast during hidden
            infoBoxCanvasGroup.blocksRaycasts = infoBoxCanvasGroup.alpha > 0.1f;
        }
    }

    // --------------------------
    // Setup helpers
    // --------------------------
    private void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            var es = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            es.transform.SetParent(transform, false);
        }
    }

    private void EnsureCanvas()
    {
        if (targetCanvas == null)
        {
            targetCanvas = FindObjectOfType<Canvas>();
            if (targetCanvas == null)
            {
                GameObject go = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                go.transform.SetParent(transform, false);
                targetCanvas = go.GetComponent<Canvas>();
                targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

                var scaler = go.GetComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
            }
        }
    }

    private void EnsureInfoBox()
    {
        if (infoBox == null)
        {
            GameObject ib = new GameObject("InfoBox", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            ib.transform.SetParent(targetCanvas.transform, false);
            RectTransform rt = ib.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.05f, 0.02f);
            rt.anchorMax = new Vector2(0.95f, 0.18f);
            rt.offsetMin = rt.offsetMax = Vector2.zero;

            Image img = ib.GetComponent<Image>();
            img.color = new Color(0f, 0f, 0f, 0.7f);

            // add CanvasGroup
            infoBoxCanvasGroup = ib.AddComponent<CanvasGroup>();

            // add info text
            GameObject t = new GameObject("InfoText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            t.transform.SetParent(ib.transform, false);
            RectTransform tr = t.GetComponent<RectTransform>();
            tr.anchorMin = new Vector2(0.04f, 0.1f);
            tr.anchorMax = new Vector2(0.96f, 0.9f);
            tr.offsetMin = tr.offsetMax = Vector2.zero;

            infoText = t.GetComponent<Text>();
            infoText.alignment = TextAnchor.MiddleLeft;
            infoText.horizontalOverflow = HorizontalWrapMode.Wrap;
            infoText.verticalOverflow = VerticalWrapMode.Truncate;
            infoText.fontSize = 20;
            infoText.color = Color.white;
            if (defaultFont != null) infoText.font = defaultFont;

            infoBox = ib;
            infoBox.SetActive(false);
            infoBoxCanvasGroup = ib.GetComponent<CanvasGroup>(); // ensure reference
        }
        else
        {
            // if provided, ensure CanvasGroup exists
            infoBoxCanvasGroup = infoBox.GetComponent<CanvasGroup>();
            if (infoBoxCanvasGroup == null) infoBoxCanvasGroup = infoBox.AddComponent<CanvasGroup>();
        }
    }

    private void EnsureGridAndSlots()
    {
        if (imageGridParent == null)
        {
            // find existing by name
            var found = targetCanvas.transform.Find("ImageGrid");
            if (found != null) imageGridParent = found;
            else
            {
                // create container with GridLayoutGroup
                GameObject grid = new GameObject("ImageGrid", typeof(RectTransform), typeof(GridLayoutGroup));
                grid.transform.SetParent(targetCanvas.transform, false);
                RectTransform gr = grid.GetComponent<RectTransform>();

                // anchor full width, top area
                gr.anchorMin = new Vector2(0.05f, 0.25f);
                gr.anchorMax = new Vector2(0.95f, 0.85f);
                gr.offsetMin = gr.offsetMax = Vector2.zero;

                GridLayoutGroup gl = grid.GetComponent<GridLayoutGroup>();
                float spacing = 12f;
                gl.padding = new RectOffset(0, 0, 0, 0);
                gl.spacing = new Vector2((int)spacing, (int)spacing);
                // calculate cell size to fit columns x rows inside rect - approximate
                float parentWidth = Screen.width * (0.95f - 0.05f);
                float parentHeight = Screen.height * (0.85f - 0.25f);
                float cellW = (parentWidth - ((columns - 1) * spacing)) / columns;
                float cellH = (parentHeight - ((rows - 1) * spacing)) / rows;
                gl.cellSize = new Vector2(Mathf.Max(100, cellW), Mathf.Max(80, cellH));

                imageGridParent = grid.transform;
            }
        }

        // find existing slots under imageGridParent, else create
        slots.Clear();
        foreach (Transform child in imageGridParent)
        {
            var r = child as RectTransform;
            if (r != null) slots.Add(r);
        }

        if (slots.Count < totalSlots)
        {
            int toCreate = totalSlots - slots.Count;
            for (int i = 0; i < toCreate; i++)
            {
                CreateSlot("Slot_" + (slots.Count + 1));
            }
        }

        // ensure we have exactly totalSlots in list (trim if extra)
        if (slots.Count > totalSlots)
        {
            while (slots.Count > totalSlots)
            {
                var last = slots[slots.Count - 1];
                slots.RemoveAt(slots.Count - 1);
                DestroyImmediate(last.gameObject);
            }
        }

        // finalize each slot: add events, defaults, descriptions
        for (int i = 0; i < slots.Count; i++)
        {
            var s = slots[i];
            if (!targetScales.ContainsKey(s)) targetScales[s] = Vector3.one;
            // Ensure it has an Image and Button
            var img = s.GetComponent<Image>();
            if (img == null) img = s.gameObject.AddComponent<Image>();
            if (defaultSlotSprite != null) img.sprite = defaultSlotSprite;
            img.type = Image.Type.Sliced;
            // Button
            var btn = s.GetComponent<Button>();
            if (btn == null) btn = s.gameObject.AddComponent<Button>();
            // Label (if none)
            Text lbl = s.GetComponentInChildren<Text>();
            if (lbl == null)
            {
                GameObject t = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
                t.transform.SetParent(s.transform, false);
                RectTransform tr = t.GetComponent<RectTransform>();
                tr.anchorMin = new Vector2(0f, 0f);
                tr.anchorMax = new Vector2(1f, 0.18f);
                tr.offsetMin = tr.offsetMax = Vector2.zero;
                lbl = t.GetComponent<Text>();
                lbl.alignment = TextAnchor.MiddleCenter;
                lbl.fontSize = 18;
                lbl.color = Color.white;
                if (defaultFont != null) lbl.font = defaultFont;
                lbl.text = "Item " + (i + 1);
            }

            // event triggers
            AddEvent(s.gameObject, EventTriggerType.PointerEnter, (e) => OnHover(s));
            AddEvent(s.gameObject, EventTriggerType.PointerExit, (e) => OnExit(s));
            AddEvent(s.gameObject, EventTriggerType.PointerClick, (e) => OnClick(s));

            // default description text
            if (!descriptions.ContainsKey(s)) descriptions[s] = $"Description for {s.name}";
        }
    }

    private RectTransform CreateSlot(string name)
    {
        GameObject slotGO = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer));
        slotGO.transform.SetParent(imageGridParent, false);
        RectTransform rt = slotGO.GetComponent<RectTransform>();
        rt.sizeDelta = slotSize;

        var img = slotGO.AddComponent<Image>();
        img.color = new Color(1f, 1f, 1f, 0.9f);
        if (defaultSlotSprite != null) img.sprite = defaultSlotSprite;

        slotGO.AddComponent<Button>();

        slots.Add(rt);
        return rt;
    }

    // --------------------------
    // Interaction handlers
    // --------------------------
    private void OnHover(RectTransform s)
    {
        highlighted = s;
        foreach (var r in slots)
        {
            targetScales[r] = (r == s) ? Vector3.one * hoverScale : Vector3.one * shrinkScale;
        }
    }

    private void OnExit(RectTransform s)
    {
        highlighted = null;
        foreach (var r in slots)
        {
            targetScales[r] = Vector3.one;
        }
    }

    private void OnClick(RectTransform s)
    {
        // set info box text and show
        if (infoText != null && descriptions.ContainsKey(s))
        {
            infoText.text = descriptions[s];
        }
        if (infoBox != null)
        {
            infoBox.SetActive(true); // fades in automatically via Update()
        }
    }

    // Helper: add dynamic EventTrigger entry
    private void AddEvent(GameObject obj, EventTriggerType type, System.Action<BaseEventData> callback)
    {
        if (obj == null) return;
        EventTrigger trig = obj.GetComponent<EventTrigger>();
        if (trig == null) trig = obj.AddComponent<EventTrigger>();

        // Avoid duplicate identical entries by checking eventID existence
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener((data) => callback(data));
        trig.triggers.Add(entry);
    }

    // --------------------------
    // Utility
    // --------------------------
    public void CloseInfoBox()
    {
        if (infoBox != null) infoBox.SetActive(false);
    }
}
