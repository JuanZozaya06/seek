using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] bool lockCursorOnStart = true;
    [SerializeField] KeyCode unlockKey = KeyCode.Escape;

    bool cursorLocked;

    void Start () {
        if (lockCursorOnStart) {
            SetCursorLocked (true);
        }
    }

    void Update () {
        if (Input.GetKeyDown (unlockKey)) {
            SetCursorLocked (false);
        } else if (!cursorLocked && Input.GetMouseButtonDown (0)) {
            SetCursorLocked (true);
        }
    }

    void OnApplicationFocus (bool hasFocus) {
        if (hasFocus && cursorLocked) {
            ApplyCursorState ();
        }
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
