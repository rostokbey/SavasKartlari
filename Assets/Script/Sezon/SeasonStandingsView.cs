using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SeasonStandingsView : MonoBehaviour
{
    public static SeasonStandingsView Instance { get; private set; }

    // config
    const int TOP_N = 10;
    const int SORTING_ORDER = 60; // HUD'dan biraz yukarý
    readonly Color32 colPanel = new Color32(26, 26, 26, 220);
    readonly Color32 colRow = new Color32(38, 38, 38, 220);
    readonly Color32 colSelf = new Color32(60, 88, 40, 230);
    readonly Color32 colText = new Color32(235, 235, 235, 255);
    readonly Color32 colSub = new Color32(200, 200, 200, 220);

    // ui
    Canvas canvas;
    RectTransform root;
    RectTransform listRoot;
    TextMeshProUGUI lblTitle, lblHint;
    readonly List<Row> rows = new List<Row>();

    class Row
    {
        public RectTransform rt;
        public Image bg;
        public TextMeshProUGUI txt;
    }

    // ---------------------------------------------------------------------
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoBoot()
    {
        if (FindObjectOfType<SeasonStandingsView>() == null)
        {
            var go = new GameObject("SeasonStandings_Runtime");
            go.AddComponent<SeasonStandingsView>();
            DontDestroyOnLoad(go);
        }
    }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        BuildUI();
        StartCoroutine(RefreshLoop());
    }

    void OnDestroy() { if (Instance == this) Instance = null; }

    // ---------------------------------------------------------------------
    void BuildUI()
    {
        // Canvas
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = SORTING_ORDER;

        var scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        gameObject.AddComponent<GraphicRaycaster>();

        // root panel (sað üst, HUD'un altýnda)
        root = CreateRect(transform, "Root");
        root.anchorMin = root.anchorMax = root.pivot = new Vector2(1, 1);
        root.anchoredPosition = new Vector2(-18, -150);
        root.sizeDelta = new Vector2(300, 260);

        var bg = root.gameObject.AddComponent<Image>();
        bg.color = colPanel;

        var v = root.gameObject.AddComponent<VerticalLayoutGroup>();
        v.padding = new RectOffset(10, 10, 10, 10);
        v.spacing = 4;
        v.childAlignment = TextAnchor.UpperRight;
        v.childControlWidth = v.childControlHeight = true;

        // üst strip (toggle etiketi)
        var top = CreateRect(root, "Top", sizeY: 22);
        var topH = top.gameObject.AddComponent<HorizontalLayoutGroup>();
        topH.childAlignment = TextAnchor.MiddleRight;
        topH.childControlWidth = topH.childControlHeight = true;

        var rankBtn = CreateLabel(top, "RANK", 14, FontStyles.Bold);
        rankBtn.color = colSub;
        rankBtn.alignment = TextAlignmentOptions.MidlineRight;
        var b = rankBtn.gameObject.AddComponent<Button>();
        b.onClick.AddListener(() => Toggle());

        // baþlýk
        lblTitle = CreateLabel(root, "Season Top 10", 18, FontStyles.Bold);
        lblTitle.color = colText;
        lblTitle.alignment = TextAlignmentOptions.MidlineRight;

        // satýrlar container
        listRoot = CreateRect(root, "List");
        var v2 = listRoot.gameObject.AddComponent<VerticalLayoutGroup>();
        v2.spacing = 2;
        v2.childControlWidth = v2.childControlHeight = true;
        v2.childAlignment = TextAnchor.UpperRight;

        for (int i = 0; i < TOP_N; i++)
        {
            rows.Add(CreateRow(listRoot));
        }

        // alt bilgi (kendi sýran)
        lblHint = CreateLabel(root, "", 14, FontStyles.Italic);
        lblHint.color = colSub;
        lblHint.alignment = TextAlignmentOptions.MidlineRight;

        UpdateNow();
    }

    IEnumerator RefreshLoop()
    {
        var wait = new WaitForSeconds(2f);
        while (true) { UpdateNow(); yield return wait; }
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetKeyDown(KeyCode.F3)) Toggle();
#endif
    }

    void Toggle() => gameObject.SetActive(!gameObject.activeSelf);

    // ---------------------------------------------------------------------
    void UpdateNow()
    {
        // güvenli eriþim
        string me = GetProfileId() ?? "DEFAULT";

        List<SeasonEntry> top = null;
        int myRank = -1;

        try
        {
            top = SeasonRepository.GetTop(TOP_N);
            myRank = SeasonRepository.GetRank(me);
        }
        catch { /* repo henüz yoksa sessiz geç */ }

        // baþlýk
        lblTitle.text = "Season Top 10";

        // satýrlarý yaz
        for (int i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            if (top != null && i < top.Count && top[i] != null)
            {
                var e = top[i];
                string name = string.IsNullOrEmpty(e.displayName) ? e.profileId : e.displayName;
                row.txt.text = $"{i + 1,2}. <b>{name}</b>  •  Pts:{e.points}  •  W:{e.wins} / L:{e.losses}";
                bool self = e.profileId == me;
                row.bg.color = self ? colSelf : colRow;
                row.rt.gameObject.SetActive(true);
            }
            else
            {
                row.txt.text = $"{i + 1,2}. —";
                row.bg.color = colRow;
                row.rt.gameObject.SetActive(true);
            }
        }

        // alt bilgi
        lblHint.text = myRank > 0 ? $"Your rank: <b>{myRank}</b>" : "Your rank: –";
    }

    // ---------------------------------------------------------------------
    // helpers
    static RectTransform CreateRect(Transform parent, string name, float sizeY = -1)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(1, 1);
        if (sizeY >= 0) rt.sizeDelta = new Vector2(0, sizeY);
        return rt;
    }

    static TextMeshProUGUI CreateLabel(Transform parent, string text, float size, FontStyles style)
    {
        var go = new GameObject(text, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var t = go.AddComponent<TextMeshProUGUI>();
        t.font = TMP_Settings.defaultFontAsset;
        t.text = text;
        t.fontSize = size;
        t.fontStyle = style;
        t.enableWordWrapping = false;
        t.color = Color.white;
        t.alignment = TextAlignmentOptions.MidlineRight;
        return t;
    }

    static Row CreateRow(Transform parent)
    {
        var rt = CreateRect(parent, "Row", 22);
        var img = rt.gameObject.AddComponent<Image>(); img.color = new Color32(38, 38, 38, 220);
        var l = CreateLabel(rt, "row", 15, FontStyles.Normal);
        l.rectTransform.anchorMin = new Vector2(0, 0);
        l.rectTransform.anchorMax = new Vector2(1, 1);
        l.rectTransform.pivot = new Vector2(1, 1);
        l.rectTransform.offsetMin = new Vector2(8, 2);
        l.rectTransform.offsetMax = new Vector2(-8, -2);
        return new Row { rt = rt, bg = img, txt = l };
    }

    static string GetProfileId()
    {
        var inv = PlayerInventory.Instance ?? Object.FindObjectOfType<PlayerInventory>();
        return inv != null ? inv.CurrentProfileId : null;
    }

    static void Ensure()
    {
        if (Instance != null) return;
        var go = new GameObject("SeasonStandings_Runtime");
        go.AddComponent<SeasonStandingsView>();
        DontDestroyOnLoad(go);
    }

    /// <summary> Eski alýþkanlýk uyumu için: paneli göster. </summary>
    public static void Show()
    {
        Ensure();
        if (Instance != null)
        {
            Instance.gameObject.SetActive(true);
            Instance.UpdateNow();
        }
    }

    /// <summary> Eski alýþkanlýk uyumu için: paneli gizle. </summary>
    public static void Hide()
    {
        if (Instance != null) Instance.gameObject.SetActive(false);
    }

    /// <summary> Butonlar için pratik toggle. </summary>
    public static void ToggleStatic()
    {
        if (Instance == null) { Show(); return; }
        Instance.Toggle();
    }
}
