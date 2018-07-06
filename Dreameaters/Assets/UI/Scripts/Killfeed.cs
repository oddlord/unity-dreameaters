using System.Collections;
using UnityEngine;

public class Killfeed : MonoBehaviour {

    [SerializeField]
    private GameObject killfeedItemPrefab;

    [SerializeField]
    private AnimationClip killfeedItemFadeoutAnim;
    
	void Start () {
        GameManager.instance.onPlayerKilledCallback += OnKill;
	}

    public void OnKill(string player, string source) {
        GameObject killfeedItemGO = Instantiate(killfeedItemPrefab, this.transform);
        killfeedItemGO.GetComponent<KillfeedItem>().Setup(player, source);

        if (killfeedItemFadeoutAnim != null) {
            StartCoroutine(DestroyKillfeedItem(killfeedItemGO, killfeedItemFadeoutAnim.length));
        } else {
            Destroy(killfeedItemGO, 4f);
        }
    }

    private IEnumerator DestroyKillfeedItem(GameObject killfeedItemGO, float fadeoutDuration) {
        yield return new WaitForSeconds(fadeoutDuration);

        Destroy(killfeedItemGO);
    }
}
