using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    // 로그인
    [SerializeField]
    private GameObject loginGroup;
    [SerializeField]
    private TMP_InputField idInputField;
    [SerializeField]
    private TMP_InputField passwordInputField;
    [SerializeField]
    private Button loginButton;
    private TextMeshProUGUI loginButtonText;

    private bool loginDone;

    private void Awake()
    {
        loginButtonText = loginButton.GetComponentInChildren<TextMeshProUGUI>();
    }

    private IEnumerator Start()
    {
        Client.instance.ConnectToServer(304);

        while (!loginDone)
        {
            yield return null;
        }

        Loading.instance.LoadScene(Scenes.Lobby);
    }

    public void ActiveLoginGroup(int serverId)
    {
        loginButtonText.text = "로그인";

        loginButton.onClick.RemoveAllListeners();
        loginButton.onClick.AddListener(OnLoginButtonClick);
        loginGroup.SetActive(true);
    }

    public void OnLoginButtonClick()
    {
        Client.instance.SendData(Command.CMD_C2S_LOGIN, $"{idInputField.text}|{passwordInputField.text}");
        Client.instance.RecvData((data) =>
        {
            switch (Client.instance.GetCommandByData(data))
            {
                case Command.CMD_S2C_LOGIN_SUCCESS:
                    Debug.Log("Login success!");

                    Client.instance.UID = uint.TryParse(data.Split('|')[1], out uint result) ? result : 0;

                    loginDone = true;

                    break;
                case Command.CMD_S2C_LOGIN_FAILED:
                    Debug.Log("Login failed!");

                    break;
            }

            OnGoBacklButtonClick();
        });
    }

    public void OnCreateAccountButtonClick()
    {
        loginButtonText.text = "계정 생성";

        loginButton.onClick.RemoveAllListeners();
        loginButton.onClick.AddListener(OnCreateAccountConfirmButtonClick);
        loginGroup.SetActive(true);
    }

    public void OnCreateAccountConfirmButtonClick()
    {
        Client.instance.SendData(Command.CMD_C2S_REGIST, $"{idInputField.text}|{passwordInputField.text}");
        Client.instance.RecvData((data) =>
        {
            switch (Client.instance.GetCommandByData(data))
            {
                case Command.CMD_S2C_REGIST_SUCCESS:
                    Debug.Log("Regist success!");

                    break;
                case Command.CMD_S2C_REGIST_FAILED:
                    Debug.Log("Regist failed!");

                    break;
            }

            OnGoBacklButtonClick();
        });
    }

    public void OnGoBacklButtonClick()
    {
        loginGroup.SetActive(false);

        idInputField.text = string.Empty;
        passwordInputField.text = string.Empty;
    }
}
