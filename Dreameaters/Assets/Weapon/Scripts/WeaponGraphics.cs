using UnityEngine;

public class WeaponGraphics : MonoBehaviour {

    public ParticleSystem muzzleFlash;
    public GameObject hitEffectPrefab;
    public LineRenderer laserRendered;
    public float laserTime = 0.1f; // in seconds

    void Start() {
        laserRendered.material = new Material(Shader.Find("Particles/Additive"));
    }
}
