using UnityEngine;

public class LoadingSpinningArrow : MonoBehaviour {

    [SerializeField]
    private float degreesPerSecond = 360f;
	
	void Update () {
        float _newZ = (degreesPerSecond * Time.deltaTime) % 360;
        transform.Rotate(0f, 0f, _newZ);
    }
}
