using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DatabaseControl;
using UnityEngine.EventSystems;

public class LoginMenu : MonoBehaviour {

    [Header("UI elements")]
    // These are the GameObjects which are parents of groups of UI elements. The objects are enabled and disabled to show and hide the UI elements.
    [SerializeField]
    private GameObject loginParent;
    [SerializeField]
    private GameObject registerParent;
    [SerializeField]
    private GameObject loadingParent;
    [SerializeField]
    private GameObject exitGameParent;

    // These are all the InputFields which we need in order to get the entered usernames, passwords, etc
    [SerializeField]
    private InputField login_usernameField;
    [SerializeField]
    private InputField login_passwordField;
    [SerializeField]
    private InputField register_usernameField;
    [SerializeField]
    private InputField register_passwordField;
    [SerializeField]
    private InputField register_confirmPasswordField;

    // These are the UI Texts which display errors
    [SerializeField]
    private Text login_errorText;
    [SerializeField]
    private Text register_errorText;

    private string initialData = DataTranslator.ValuesToData(0, 0);

    // Called at the very start of the game
    void Start() {
        ShowLogin(null, true);
        Util.SetCursorLock(false);
    }

    // TODO: add TAB functionality to focus other input fields/buttons
    //void Update() {
    //    if (Input.GetKeyDown(KeyCode.Tab)) {
    //        if (Input.GetKey(KeyCode.LeftShift)) {
    //            if (EventSystem.current.currentSelectedGameObject != null) {
    //                Selectable selectable = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
    //                if (selectable != null)
    //                    selectable.Select();
    //            }
    //        } else {
    //            if (EventSystem.current.currentSelectedGameObject != null) {
    //                Selectable selectable = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
    //                if (selectable != null)
    //                    selectable.Select();
    //            }
    //        }
    //    }
    //}

    void focusInputText(InputField inputToFocus) {
        inputToFocus.Select();
        inputToFocus.ActivateInputField();
    }

    void showLoginOrRegister(GameObject parentToDisable, GameObject parentToEnable, bool reset, InputField inputToFocus) {
        if (parentToDisable != null) {
            parentToDisable.SetActive(false);
        }

        parentToEnable.SetActive(true);
        exitGameParent.SetActive(true);

        if (reset) {
            ResetAllUIElements();
            focusInputText(inputToFocus);
        }
    }

    void ShowLogin(GameObject parentToDisable, bool reset) {
        showLoginOrRegister(parentToDisable, loginParent, reset, login_usernameField);
    }

    void ShowRegister(GameObject parentToDisable, bool reset) {
        showLoginOrRegister(parentToDisable, registerParent, reset, register_usernameField);
    }

    void ShowLoading(GameObject parentToDisable) {
        parentToDisable.SetActive(false);
        exitGameParent.SetActive(false);
        loadingParent.SetActive(true);
    }

    // Called by Button Pressed Methods to Reset UI Fields
    void ResetAllUIElements () {
        // This resets all of the UI elements. It clears all the strings in the input fields and any errors being displayed
        login_usernameField.text = "";
        login_passwordField.text = "";
        register_usernameField.text = "";
        register_passwordField.text = "";
        register_confirmPasswordField.text = "";
        login_errorText.text = "";
        register_errorText.text = "";
    }

    // Called by Button Pressed Methods. These use DatabaseControl namespace to communicate with server.
    IEnumerator LoginUser (string username, string password) {
        IEnumerator e = DCF.Login(username, password); // << Send request to login, providing username and password
        while (e.MoveNext()) {
            yield return e.Current;
        }
        string response = e.Current as string; // << The returned string from the request

        if (response == "Success") {
            ResetAllUIElements();
            UserAccountManager.instance.LogIn(username, password);
        } else {
            // Something went wrong logging in. Stop showing 'Loading...' and go back to LoginUI
            ShowLogin(loadingParent, false);
            if (response == "UserError") {
                // The Username was wrong so display relevent error message
                login_errorText.text = "Error: Username not Found";
            } else {
                if (response == "PassError") {
                    // The Password was wrong so display relevent error message
                    login_errorText.text = "Error: Password Incorrect";
                } else {
                    // There was another error. This error message should never appear, but is here just in case.
                    login_errorText.text = "Error: Unknown Error. Please try again later.";
                }
            }
        }
    }
    IEnumerator RegisterUser(string username, string password, string data) {
        IEnumerator e = DCF.RegisterUser(username, password, data); // << Send request to register a new user, providing submitted username and password. It also provides an initial value for the data string on the account, which is "Hello World".
        while (e.MoveNext()) {
            yield return e.Current;
        }
        string response = e.Current as string; // << The returned string from the request

        if (response == "Success") {
            ResetAllUIElements();
            UserAccountManager.instance.LogIn(username, password);
        } else {
            // Something went wrong logging in. Stop showing 'Loading...' and go back to RegisterUI
            ShowRegister(loadingParent, false);
            if (response == "UserError") {
                // The username has already been taken. Player needs to choose another. Shows error message.
                register_errorText.text = "Error: Username Already Taken";
            } else {
                // There was another error. This error message should never appear, but is here just in case.
                login_errorText.text = "Error: Unknown Error. Please try again later.";
            }
        }
    }

    // UI Button Pressed Methods
    public void Login_LoginButtonPressed () {
        // Called when player presses button to Login

        // Get the username and password the player entered
        string _username = login_usernameField.text;
        string _password = login_passwordField.text;

        // Check the lengths of the username and password. (If they are wrong, we might as well show an error now instead of waiting for the request to the server)
        if (_username.Length > 3) {
            if (_password.Length > 5) {
                // Username and password seem reasonable. Change UI to 'Loading...'. Start the Coroutine which tries to log the player in.
                ShowLoading(loginParent);
                StartCoroutine(LoginUser(_username, _password));
            } else {
                // Password too short so it must be wrong
                login_errorText.text = "Error: Password Incorrect";
            }
        } else {
            // Username too short so it must be wrong
            login_errorText.text = "Error: Username Incorrect";
        }
    }
    public void Login_RegisterButtonPressed () {
        // Called when the player hits register on the Login UI, so switches to the Register UI
        ShowRegister(loginParent, true);
    }
    public void Register_RegisterButtonPressed () {
        // Called when the player presses the button to register

        // Get the username and password and repeated password the player entered
        string _username = register_usernameField.text;
        string _password = register_passwordField.text;
        string _confirmedPassword = register_confirmPasswordField.text;

        // Make sure username and password are long enough
        if (_username.Length > 3) {
            if (_password.Length > 5) {
                // Check the two passwords entered match
                if (_password == _confirmedPassword) {
                    // Username and passwords seem reasonable. Switch to 'Loading...' and start the coroutine to try and register an account on the server 
                    ShowLoading(registerParent);
                    StartCoroutine(RegisterUser(_username, _password, initialData));
                } else {
                    // Passwords don't match, show error
                    register_errorText.text = "Error: Password's don't Match";
                }
            }
            else {
                // Password too short so show error
                register_errorText.text = "Error: Password too Short";
            }
        } else {
            // Username too short so show error
            register_errorText.text = "Error: Username too Short";
        }
    }
    public void Register_BackButtonPressed () {
        // Called when the player presses the 'Back' button on the register UI. Switches back to the Login UI
        ShowLogin(registerParent, true);
    }
}
