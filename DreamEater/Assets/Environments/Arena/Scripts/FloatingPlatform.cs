using UnityEngine;

public class FloatingPlatform : MonoBehaviour {

    private const float PI = Mathf.PI;

    [SerializeField]
    private float speed = 0.5f;
    [SerializeField]
    private float maxOffset = 1f;
    [SerializeField]
    private bool startUpwards = true;

    [SerializeField]
    private string fireTag = "Fire";

    private Vector3 originalPosition;
    private Transform fireComplex;

    private bool fireEnabled;

    void Start() {
        originalPosition = transform.position;

        foreach (Transform child in transform) {
            if (child.tag == fireTag) {
                fireComplex = child;
                break;
            }
        }
        if (fireComplex == null) {
            Debug.LogError("Error: no GameObject with tag \"" + fireTag + "\" found under the floating platform.");
        }

        fireEnabled = true;
    }

    void SetEffectsAndLight(bool enabled) {
        foreach (Transform child in fireComplex) {
            GameObject childGO = child.gameObject;
            ParticleSystem particleSystem = childGO.GetComponent<ParticleSystem>();

            if (particleSystem != null) {
                ParticleSystem.EmissionModule em = particleSystem.emission;
                em.enabled = enabled;
            } else {
                childGO.SetActive(enabled);
            }
        }

        fireEnabled = enabled;
    }

    void Update () {
        float t = Time.time;
        float sign = startUpwards ? 1f : -1f;

        float derivate = sign * Mathf.Cos(PI * t * speed) * maxOffset;
        if (derivate > 0 && !fireEnabled) {
            SetEffectsAndLight(true);
        } else if (derivate < 0 && fireEnabled) {
            SetEffectsAndLight(false);
        }

        float offsetY = sign * Mathf.Sin(PI * t * speed) * maxOffset;
        transform.position = originalPosition + new Vector3(0f, offsetY, 0f);
	}
}
