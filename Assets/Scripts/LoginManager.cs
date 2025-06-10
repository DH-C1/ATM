using System.Collections;
using TMPro;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    public GameObject login;
    public GameObject signUp;
    public GameObject PopupBank;

    public TMP_InputField loginIdInput;     //      아이디 입력칸 가져오기
    public TMP_InputField loginPwInput;     //      비밀번호 입력칸 가져오기

    public TMP_InputField registerIdInput;      //      회원가입 아이디 입력칸 가져오기
    public TMP_InputField registerNameInput;    //      회원가입 이름 입력칸 가져오기
    public TMP_InputField registerPwInput;      //      회원가입 비밀번호 입력칸 가져오기
    public TMP_InputField registerPwConfirmInput;//     회원가입 비밀번호 확인 입력칸 가져오기

    public TextMeshProUGUI loginErrorText;      //      로그인 오류 텍스트 가져오기
    public TextMeshProUGUI signUpErrorText;   //      회원가입 오류 텍스트 가져오기


    public void TryLogin()
    {
        string inputId = loginIdInput.text;
        string inputPw = loginPwInput.text;

        string savedPw = PlayerPrefs.GetString("UserPW_" + inputId, "");      //      저장된 PW 데이터 가져오기

        if (string.IsNullOrEmpty(savedPw))
        {
            ShowLoginError("아이디 또는 비밀번호가 틀렸습니다");
            return;
        }

        if (inputPw != savedPw)
        {
            ShowLoginError("아이디 또는 비밀번호가 틀렸습니다");
            return;
        }

        //      로그인 성공 시 해당 유저 데이터 가져오기
        GameManager.Instance.LoadUserData(loginIdInput.text);
        Debug.Log("로그인 성공");
        ShowSignUpError("");
        login.SetActive(false);
        PopupBank.SetActive(true);
    }

    // 회원가입
    public void TryRegister()
    {
        string id = registerIdInput.text;
        string name = registerNameInput.text;
        string pw = registerPwInput.text;
        string cf = registerPwConfirmInput.text;

        // 회원가입 시 입력칸이 비었을 시
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(name)
            || string.IsNullOrEmpty(pw) || string.IsNullOrEmpty(cf))
        {
            ShowSignUpError("모두 입력해주세요");
            return;
        }

        // 중복체크용
        if (PlayerPrefs.HasKey("UserPW_" + id))
        {
            ShowSignUpError("이미 존재하는 아이디입니다.");
            return;
        }

        // 비밀번호 확인
        if (pw != cf)
        {
            ShowSignUpError("비밀번호가 일치하지 않습니다");
            return;
        }

        // PW 저장
        PlayerPrefs.SetString("UserPW_" + id, pw);

        // 유저 데이터 생성 및 저장
        UserData newUser = new UserData(name, 10000, 50000);
        string key = $"UserData_{id}";
        PlayerPrefs.SetString(key, JsonUtility.ToJson(newUser));
        PlayerPrefs.Save();

        ShowSignUpError("회원가입 완료");
        Debug.Log("회원가입 성공");

        ShowLoginUI();      //      로그인 화면으로 이동
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
        StartCoroutine(LoginErrorBlink(message, 1f));       //      1초 후 메세지 사라짐
    }

    public void ShowSignUpError(string message)
    {
        StopAllCoroutines();
        StartCoroutine(SignUpErrorBlink(message, 1f));       //      1초 후 메세지 사라짐
    }

    // 로그인 오류 메세지가 나타났다 사라지게 하기 위해
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
