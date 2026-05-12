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

        if (hit.collider != null && hit.collider.CompareTag ("Enemy")) {
            LogShot ("FOUND ONE!");
        } else {
            LogShot ("MISSED!");
        }
    }

    void LogShot (string message) {
        if (logShots) {
            Debug.Log (message);
        }
    }
}
