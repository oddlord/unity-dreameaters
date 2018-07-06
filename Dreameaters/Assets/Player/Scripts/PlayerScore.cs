using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerScore : MonoBehaviour {

    private int lastSyncedKills = 0;
    private int lastSyncedDeaths = 0;

    Player player;

	void Start () {
        player = GetComponent<Player>();
        StartCoroutine(SyncScoreLoop());
	}

    void OnDestroy() {
        if (player != null) {
            SyncNow();
        }
    }

    IEnumerator SyncScoreLoop() {
        while (true) {
            yield return new WaitForSeconds(5f);
            SyncNow();
        }
    }

    void SyncNow() {
        if (UserAccountManager.IsLoggedIn) {
            UserAccountManager.instance.GetData(OnDataReceived);
        }
    }

    void OnDataReceived(string data) {
        if (player.kills <= lastSyncedKills && player.deaths <= lastSyncedDeaths) {
            return;
        }

        int killsSinceLastSync = player.kills - lastSyncedKills;
        int deathsSinceLastSync = player.deaths - lastSyncedDeaths;

        if(killsSinceLastSync == 0 && deathsSinceLastSync == 0) {
            return;
        }

        int kills = DataTranslator.DataToKills(data);
        int deaths = DataTranslator.DataToDeaths(data);

        int newKills = killsSinceLastSync + kills;
        int newDeaths = deathsSinceLastSync + deaths;

        string newData = DataTranslator.ValuesToData(newKills, newDeaths);

        Debug.Log("Syncing: " + newData);

        lastSyncedKills = player.kills;
        lastSyncedDeaths = player.deaths;

        UserAccountManager.instance.SendData(newData);
    }
}
