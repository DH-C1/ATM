using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject atm;
    public GameObject deposit;
    public GameObject withdraw;
    public GameObject remittance;
    public GameObject error;
    public GameObject nullError;
    public GameObject idError;

    public TextMeshProUGUI userNameText;
    public TextMeshProUGUI cashText;
    public TextMeshProUGUI bankText;
    public TMP_InputField RemitIdInputField;
    public TMP_InputField RemitAmountInputField;

    public TMP_InputField DepositInputField;       //      직접입력필드
    public TMP_InputField WithdrawInputField;

    private UIType previousUI;

    public enum UIType
    {
        Main,
        Deposit,
        Withdraw,
        Remittance
    }

    private void Start()
    {
        UpdateUserUI();     //      유저 정보 불러오기
        ShowMainUI();       //      메인 UI 불러오기
    }
    public void UpdateUserUI()
    {
        if (GameManager.Instance == null || GameManager.Instance.playerData == null)
        {
            Debug.LogWarning("GM 혹은 playerData가 아직 초기화되지 않음");
            return;
        }

        UserData user = GameManager.Instance.playerData;

        userNameText.text = $"{user.userName}";
        cashText.text = $"{user.cash}";
        bankText.text = $"{user.bankBalance}";
    }

    public void ShowMainUI()
    {
        atm.SetActive(true);
        deposit.SetActive(false);
        withdraw.SetActive(false);
        remittance.SetActive(false);
    }

    public void ShowDepositUI()
    {
        atm.SetActive(false);
        error.SetActive(false);
        deposit.SetActive(true);
        previousUI = UIType.Deposit;    //      기록
    }

    public void Deposit(int amount)
    {
        var user = GameManager.Instance.playerData;

        if (user.cash >= amount)
        {
            user.cash -= amount;
            user.bankBalance += amount;

            GameManager.Instance.SaveUserData();        //      금액 변동에 자동 저장
            UpdateUserUI();

        }
        else
        {
            previousUI = UIType.Deposit;    //      에러나도 기록
            error.SetActive(true);
        }
    }

    public void DepositFromInput()
    {
        if (int.TryParse(DepositInputField.text, out int inputAmount))
        {
            Deposit(inputAmount);
            DepositInputField.text = "";       //      입력 초기화
        }
        else
        {
            Debug.Log("숫자를 입력해주세요");
        }
    }

    public void ShowWithdrawUI()
    {
        atm.SetActive(false);
        error.SetActive(false);
        withdraw.SetActive(true);
        previousUI = UIType.Withdraw;
    }

    public void Withdraw(int amount)
    {
        var user = GameManager.Instance.playerData;

        if (user.bankBalance >= amount)
        {
            user.bankBalance -= amount;
            user.cash += amount;

            GameManager.Instance.SaveUserData();
            UpdateUserUI();
        }
        else
        {
            previousUI = UIType.Withdraw;   
            error.SetActive(true);
        }
    }

    public void WithdrawFromInput()
    {
        if (int.TryParse(WithdrawInputField.text, out int inputAmount))
        {
            Withdraw(inputAmount);
            WithdrawInputField.text = "";       //      입력 초기화
        }
        else
        {
            Debug.Log("숫자를 입력해주세요");
        }
    }

    public void ShowRemittanceUI()
    {
        atm.SetActive(false);
        error.SetActive(false);
        nullError.SetActive(false);
        idError.SetActive(false);
        remittance.SetActive(true);
    }

    public void Remittance()
    {
        string targetId = RemitIdInputField.text;
        string amountStr = RemitAmountInputField.text;

        // 입력칸이 비어있을 시 출력되는 오류UI
        if (string.IsNullOrEmpty(targetId) || string.IsNullOrEmpty(amountStr))
        {
            previousUI = UIType.Remittance;
            nullError.SetActive(true);
            return;
        }

        // 금액이 숫자인지 확인
        if (!int.TryParse(amountStr, out int amount) || amount <= 0)
        {
            previousUI = UIType.Remittance;
            nullError.SetActive(true);
            return;
        }

        string recipientKey = $"UserData_{targetId}";   //      존재하는 유저 ID 가져오기

        // 수신자 ID가 존재하지 않을 시 출력되는 오류UI
        if (!PlayerPrefs.HasKey(recipientKey))
        {
            previousUI = UIType.Remittance;
            idError.SetActive(true);
            return;
        }

        var sender = GameManager.Instance.playerData;       //      금액 보낼 때 보내는 사람의 금액 데이터 가져오기

        // 잔액이 부족할 시 출력되는 오류UI
        if (sender.bankBalance < amount)
        {
            previousUI = UIType.Remittance;
            error.SetActive(true);
            return;
        }

        // 자신에게 송금 기능 X
        if (targetId == GameManager.Instance.currentUserId)
        {
            previousUI = UIType.Remittance;
            nullError.SetActive(true);
            return;
        }

        // 수신자 저장 데이터 가져오기
        string json = PlayerPrefs.GetString(recipientKey);
        Debug.Log("상대방 저장된 데이터: " + PlayerPrefs.GetString(recipientKey));       //      돈이 잘 보내지는지 확인용 Log
        UserData recipient = JsonUtility.FromJson<UserData>(json);      //      유저 데이터(json)를 변수로 가져오기

        // 송금 처리
        sender.bankBalance -= amount;
        recipient.bankBalance += amount;

        // 저장
        GameManager.Instance.SaveUserData();        //      자신의 데이터를 저장
        PlayerPrefs.SetString(recipientKey, JsonUtility.ToJson(recipient));
        PlayerPrefs.Save();

        // UI 업데이트
        UpdateUserUI();
        RemitIdInputField.text = "";
        RemitAmountInputField.text = "";
        remittance.SetActive(false);
        atm.SetActive(true);        //        송금 성공 후 메인 화면으로

    }

    // 에러날 시 돌아갈 때 첫 화면이 아닌 이전 화면으로 복귀
    public void ReturnFromError()
    {
        error.SetActive(false);
        idError.SetActive(false);
        nullError.SetActive(false);

        switch (previousUI)
        {
            case UIType.Deposit:
                deposit.SetActive(true);
                break;
            case UIType.Withdraw:
                withdraw.SetActive(true);
                break;
            case UIType.Remittance:
                remittance.SetActive(true);
                break;
            default:
                atm.SetActive(true);    //     혹시나 나올 예외에 대한 처리
                break;
        }
    }
}
