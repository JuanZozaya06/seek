using UnityEngine;

public class TargetScript : MonoBehaviour {
    public Camera targetCamera;
    [SerializeField] float maxDistance = 100f;
    [SerializeField] LayerMask hitMask = ~0;
    [SerializeField] bool logShots = true;

    Transform targetCameraTransform;

    void Awake () {
        CacheCamera ();
    }

    void Update () {
        if (Input.GetButtonDown ("Fire1")) {
            Shoot ();
        }
    }

    void CacheCamera () {
        if (targetCamera == null) {
            targetCamera = Camera.main;
        }

        targetCameraTransform = targetCamera != null ? targetCamera.transform : null;
    }

    void Shoot () {
        if (targetCameraTransform == null) {
            CacheCamera ();

            if (targetCameraTransform == null) {
                if (logShots) {
                    Debug.LogWarning ("TargetScript needs a camera assigned.");
                }

                return;
            }
        }

        RaycastHit hit;
        if (!Physics.Raycast (targetCameraTransform.position, targetCameraTransform.forward, out hit, maxDistance, hitMask, QueryTriggerInteraction.Ignore)) {
            LogShot ("MISSED!");
            return;
        }

        GameManager gameManager = GameManager.Instance;

        if (gameManager != null && gameManager.State != GameManager.RoundState.Running) {
            return;
        }

        if (gameManager != null) {
            if (gameManager.TryResolveSeekerSelection (hit.collider)) {
                return;
            }

            LogShot ("MISSED!");
            return;
        }

        LogShot (IsEnemyFallback (hit.collider) ? "found one" : "MISSED!");
    }

    void LogShot (string message) {
        if (logShots) {
            Debug.Log (message);
        }
    }

    bool IsEnemyFallback (Collider hitCollider) {
        if (hitCollider == null) {
            return false;
        }

        return HasTag (hitCollider.gameObject, "Enemy") || HasTag (hitCollider.transform.root.gameObject, "Enemy");
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
}
