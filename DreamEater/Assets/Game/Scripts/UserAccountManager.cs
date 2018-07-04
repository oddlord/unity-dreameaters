using System.Collections;
using UnityEngine;
using DatabaseControl;
using UnityEngine.SceneManagement;

public class UserAccountManager : MonoBehaviour {

    public static UserAccountManager instance;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
    }

    public static string LoggedIn_Username { get; protected set; } // stores username once logged in
    private static string LoggedIn_Password = ""; // stores password once logged in

    public static bool IsLoggedIn { get; protected set; }

    public string loggedInSceneName = "Lobby";
    public string loggedOutSceneName = "LoginMenu";

    public delegate void OnDataReceivedCallback(string data);

    public void LogOut() {
        string username = LoggedIn_Username;

        LoggedIn_Username = "";
        LoggedIn_Password = "";

        IsLoggedIn = false;

        Debug.Log("User " + username + " logged out.");

        SceneManager.LoadScene(loggedOutSceneName);
    }

    public void LogIn(string username, string password) {
        LoggedIn_Username = username;
        LoggedIn_Password = password;

        IsLoggedIn = true;

        Debug.Log("Logged in as " + LoggedIn_Username + ".");

        SceneManager.LoadScene(loggedInSceneName);
    }

    IEnumerator sendGetDataRequest(string username, string password, OnDataReceivedCallback onDataReceived) {
        IEnumerator e = DCF.GetUserData(username, password); // << Send request to get the player's data string. Provides the username and password
        while (e.MoveNext()) {
            yield return e.Current;
        }
        string response = e.Current as string; // << The returned string from the request

        if (response == "Error") {
            //There was another error. This error message should never appear, but is here just in case.
            Debug.LogError("Error: Unknown Error. Please try again later.");
        } else {
            //The player's data was retrieved.
            Debug.Log("Data Retrieval Successful.");
        }

        if (onDataReceived != null) {
            onDataReceived.Invoke(response);
        }
    }

    IEnumerator sendSendDataRequest(string username, string password, string data) {
        IEnumerator e = DCF.SetUserData(username, password, data); // << Send request to set the player's data string. Provides the username, password and new data string
        while (e.MoveNext()) {
            yield return e.Current;
        }
        string response = e.Current as string; // << The returned string from the request

        if (response == "Success") {
            //The data string was set correctly.
            Debug.Log("Data Upload Successful.");
        } else {
            //There was another error. This error message should never appear, but is here just in case.
            Debug.LogError("Data Upload Error: Unknown Error. Please try again later.");
        }
    }

    public void GetData(OnDataReceivedCallback onDataReceived) {
        if (IsLoggedIn) {
            StartCoroutine(sendGetDataRequest(LoggedIn_Username, LoggedIn_Password, onDataReceived));
        }
    }

    public void SendData(string _data) {
        if (IsLoggedIn) {
            StartCoroutine(sendSendDataRequest(LoggedIn_Username, LoggedIn_Password, _data));
        }
    }
}
