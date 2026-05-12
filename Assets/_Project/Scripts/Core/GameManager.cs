using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public enum RoundState {
        WaitingToStart,
        Running,
        Finished
    }

    public enum RoundWinner {
        None,
        Seeker,
        Hider
    }

    public static GameManager Instance { get; private set; }

    [SerializeField] bool lockCursorOnStart = true;
    [SerializeField] KeyCode unlockKey = KeyCode.Escape;
    [SerializeField] bool autoStartRound = true;
    [SerializeField] string gameplaySceneName = "Kitchen";
    [SerializeField] float roundDurationSeconds = 300f;
    [SerializeField] bool createTimerHud = true;
    [SerializeField] Vector2 timerAnchoredPosition = new Vector2 (0f, -28f);

    public RoundState State { get; private set; } = RoundState.WaitingToStart;
    public RoundWinner Winner { get; private set; } = RoundWinner.None;
    public float RemainingRoundSeconds { get; private set; }

    bool cursorLocked;
    Text timerText;

    void Awake () {
        if (Instance != null && Instance != this) {
            enabled = false;
            return;
        }

        Instance = this;
        RemainingRoundSeconds = roundDurationSeconds;
    }

    void Start () {
        if (lockCursorOnStart) {
            SetCursorLocked (true);
        }

        if (autoStartRound && IsGameplayScene ()) {
            StartRound ();
        }
    }

    void Update () {
        if (Input.GetKeyDown (unlockKey)) {
            SetCursorLocked (false);
        } else if (!cursorLocked && Input.GetMouseButtonDown (0)) {
            SetCursorLocked (true);
        }

        if (State == RoundState.Running) {
            TickRoundTimer ();
        }
    }

    void OnApplicationFocus (bool hasFocus) {
        if (hasFocus && cursorLocked) {
            ApplyCursorState ();
        }
    }

    void OnDestroy () {
        if (Instance == this) {
            Instance = null;
        }
    }

    public void StartRound () {
        State = RoundState.Running;
        Winner = RoundWinner.None;
        RemainingRoundSeconds = Mathf.Max (0f, roundDurationSeconds);

        ConfigureSceneParticipants ();
        EnsureTimerHud ();
        UpdateTimerHud ();

        Debug.Log ("Round started: player is seeker, hidden character is hider.");
    }

    public bool TryResolveSeekerSelection (Collider selectedCollider) {
        if (State != RoundState.Running || selectedCollider == null) {
            return false;
        }

        if (!IsHider (selectedCollider)) {
            return false;
        }

        Debug.Log ("found one");
        EndRound (RoundWinner.Seeker);
        return true;
    }

    void TickRoundTimer () {
        RemainingRoundSeconds = Mathf.Max (0f, RemainingRoundSeconds - Time.deltaTime);
        UpdateTimerHud ();

        if (RemainingRoundSeconds <= 0f) {
            EndRound (RoundWinner.Hider);
        }
    }

    void EndRound (RoundWinner winner) {
        if (State == RoundState.Finished) {
            return;
        }

        State = RoundState.Finished;
        Winner = winner;
        RemainingRoundSeconds = Mathf.Max (0f, RemainingRoundSeconds);
        UpdateTimerHud ();

        Debug.Log (winner == RoundWinner.Seeker ? "Round ended: seeker wins." : "Round ended: hider wins.");
    }

    bool IsGameplayScene () {
        return SceneManager.GetActiveScene ().name == gameplaySceneName;
    }

    void ConfigureSceneParticipants () {
        AssignRoleToTaggedObjects ("Player", RoundRole.Seeker);
        AssignRoleToTaggedObjects ("Enemy", RoundRole.Hider);
    }

    void AssignRoleToTaggedObjects (string tagName, RoundRole role) {
        GameObject[] taggedObjects;

        try {
            taggedObjects = GameObject.FindGameObjectsWithTag (tagName);
        } catch (UnityException) {
            return;
        }

        foreach (GameObject taggedObject in taggedObjects) {
            RoundParticipant participant = taggedObject.GetComponent<RoundParticipant> ();

            if (participant == null) {
                participant = taggedObject.AddComponent<RoundParticipant> ();
            }

            participant.SetRole (role);
        }
    }

    bool IsHider (Collider selectedCollider) {
        RoundParticipant participant = selectedCollider.GetComponentInParent<RoundParticipant> ();

        if (participant != null) {
            return participant.Role == RoundRole.Hider;
        }

        return HasTag (selectedCollider.gameObject, "Enemy") || HasTag (selectedCollider.transform.root.gameObject, "Enemy");
    }

    bool HasTag (GameObject target, string tagName) {
        if (target == null) {
            return false;
        }

        try {
            return target.CompareTag (tagName);
        } catch (UnityException) {
            return false;
        }
    }

    void EnsureTimerHud () {
        if (!createTimerHud || timerText != null) {
            return;
        }

        GameObject canvasObject = new GameObject ("Round HUD", typeof (Canvas), typeof (CanvasScaler), typeof (GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas> ();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler canvasScaler = canvasObject.GetComponent<CanvasScaler> ();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2 (1920f, 1080f);
        canvasScaler.matchWidthOrHeight = 0.5f;

        GameObject timerObject = new GameObject ("Round Timer", typeof (Text), typeof (Shadow));
        timerObject.transform.SetParent (canvasObject.transform, false);

        timerText = timerObject.GetComponent<Text> ();
        timerText.font = Resources.GetBuiltinResource<Font> ("Arial.ttf");
        timerText.alignment = TextAnchor.MiddleCenter;
        timerText.fontSize = 42;
        timerText.fontStyle = FontStyle.Bold;
        timerText.color = Color.white;
        timerText.raycastTarget = false;

        Shadow shadow = timerObject.GetComponent<Shadow> ();
        shadow.effectColor = new Color (0f, 0f, 0f, 0.65f);
        shadow.effectDistance = new Vector2 (2f, -2f);

        RectTransform timerTransform = timerText.rectTransform;
        timerTransform.anchorMin = new Vector2 (0.5f, 1f);
        timerTransform.anchorMax = new Vector2 (0.5f, 1f);
        timerTransform.pivot = new Vector2 (0.5f, 1f);
        timerTransform.anchoredPosition = timerAnchoredPosition;
        timerTransform.sizeDelta = new Vector2 (320f, 72f);
    }

    void UpdateTimerHud () {
        if (timerText == null) {
            return;
        }

        if (State == RoundState.Finished) {
            timerText.text = Winner == RoundWinner.Seeker ? "SEEKER WINS" : "HIDER WINS";
            return;
        }

        timerText.text = FormatTime (RemainingRoundSeconds);
    }

    string FormatTime (float seconds) {
        int secondsLeft = Mathf.CeilToInt (Mathf.Max (0f, seconds));
        int minutes = secondsLeft / 60;
        int remainderSeconds = secondsLeft % 60;

        return string.Format ("{0:00}:{1:00}", minutes, remainderSeconds);
    }

    void SetCursorLocked (bool locked) {
        CursorLockMode targetLockState = locked ? CursorLockMode.Locked : CursorLockMode.None;

        if (cursorLocked == locked && Cursor.lockState == targetLockState && Cursor.visible == !locked) {
            return;
        }

        cursorLocked = locked;
        ApplyCursorState ();
    }

    void ApplyCursorState () {
        Cursor.lockState = cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !cursorLocked;
    }
}
