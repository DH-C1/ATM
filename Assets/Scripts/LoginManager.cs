using System.Collections;
using TMPro;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    public GameObject login;
    public GameObject signUp;
    public GameObject PopupBank;

    public TMP_InputField loginIdInput;     //      ���̵� �Է�ĭ ��������
    public TMP_InputField loginPwInput;     //      ��й�ȣ �Է�ĭ ��������

    public TMP_InputField registerIdInput;      //      ȸ������ ���̵� �Է�ĭ ��������
    public TMP_InputField registerNameInput;    //      ȸ������ �̸� �Է�ĭ ��������
    public TMP_InputField registerPwInput;      //      ȸ������ ��й�ȣ �Է�ĭ ��������
    public TMP_InputField registerPwConfirmInput;//     ȸ������ ��й�ȣ Ȯ�� �Է�ĭ ��������

    public TextMeshProUGUI loginErrorText;      //      �α��� ���� �ؽ�Ʈ ��������
    public TextMeshProUGUI signUpErrorText;   //      ȸ������ ���� �ؽ�Ʈ ��������


    public void TryLogin()
    {
        string inputId = loginIdInput.text;
        string inputPw = loginPwInput.text;

        string savedPw = PlayerPrefs.GetString("UserPW_" + inputId, "");      //      ����� PW ������ ��������

        if (string.IsNullOrEmpty(savedPw))
        {
            ShowLoginError("���̵� �Ǵ� ��й�ȣ�� Ʋ�Ƚ��ϴ�");
            return;
        }

        if (inputPw != savedPw)
        {
            ShowLoginError("���̵� �Ǵ� ��й�ȣ�� Ʋ�Ƚ��ϴ�");
            return;
        }

        //      �α��� ���� �� �ش� ���� ������ ��������
        GameManager.Instance.LoadUserData(loginIdInput.text);
        Debug.Log("�α��� ����");
        ShowSignUpError("");
        login.SetActive(false);
        PopupBank.SetActive(true);
    }

    // ȸ������
    public void TryRegister()
    {
        string id = registerIdInput.text;
        string name = registerNameInput.text;
        string pw = registerPwInput.text;
        string cf = registerPwConfirmInput.text;

        // ȸ������ �� �Է�ĭ�� ����� ��
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(name)
            || string.IsNullOrEmpty(pw) || string.IsNullOrEmpty(cf))
        {
            ShowSignUpError("��� �Է����ּ���");
            return;
        }

        // �ߺ�üũ��
        if (PlayerPrefs.HasKey("UserPW_" + id))
        {
            ShowSignUpError("�̹� �����ϴ� ���̵��Դϴ�.");
            return;
        }

        // ��й�ȣ Ȯ��
        if (pw != cf)
        {
            ShowSignUpError("��й�ȣ�� ��ġ���� �ʽ��ϴ�");
            return;
        }

        // PW ����
        PlayerPrefs.SetString("UserPW_" + id, pw);

        // ���� ������ ���� �� ����
        UserData newUser = new UserData(name, 10000, 50000);
        string key = $"UserData_{id}";
        PlayerPrefs.SetString(key, JsonUtility.ToJson(newUser));
        PlayerPrefs.Save();

        ShowSignUpError("ȸ������ �Ϸ�");
        Debug.Log("ȸ������ ����");

        ShowLoginUI();      //      �α��� ȭ������ �̵�
    }

    public void ShowLoginUI()
    {
        login.SetActive(true);
        signUp.SetActive(false);
        ShowLoginError("");
    }

    public void ShowSignUpUI()
    {
        login.SetActive(false);
        signUp.SetActive(true);
        ShowSignUpError("");
    }

    public void ShowLoginError(string message)
    {
        StopAllCoroutines();
        StartCoroutine(LoginErrorBlink(message, 1f));       //      1�� �� �޼��� �����
    }

    public void ShowSignUpError(string message)
    {
        StopAllCoroutines();
        StartCoroutine(SignUpErrorBlink(message, 1f));       //      1�� �� �޼��� �����
    }

    // �α��� ���� �޼����� ��Ÿ���� ������� �ϱ� ����
    private IEnumerator LoginErrorBlink(string message, float delay)
    {
        loginErrorText.text = message;
        yield return new WaitForSeconds(delay);
        loginErrorText.text = "";
    }

    private IEnumerator SignUpErrorBlink(string message, float delay)
    {
        signUpErrorText.text = message;
        yield return new WaitForSeconds(delay);
        signUpErrorText.text = "";
    }
}
