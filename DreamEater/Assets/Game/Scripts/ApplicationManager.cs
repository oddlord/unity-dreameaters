using UnityEngine;

public class ApplicationManager : MonoBehaviour {

    public void ExitGame() {
        Debug.Log("Quitting game.");
        Application.Quit();
    }
}
