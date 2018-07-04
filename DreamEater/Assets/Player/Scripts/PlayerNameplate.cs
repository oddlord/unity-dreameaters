using UnityEngine;
using UnityEngine.UI;

public class PlayerNameplate : MonoBehaviour {

    [SerializeField]
    private Text usernameText;

    [SerializeField]
    private RectTransform healthbarFill;

    [SerializeField]
    private Player player;
	
	void Update () {
        usernameText.text = player.username;
        healthbarFill.localScale = new Vector3(player.GetHealthPct(), 1f, 1f);
	}
}
