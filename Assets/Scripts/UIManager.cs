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

    public TMP_InputField DepositInputField;       //      �����Է��ʵ�
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
        UpdateUserUI();     //      ���� ���� �ҷ�����
        ShowMainUI();       //      ���� UI �ҷ�����
    }
    public void UpdateUserUI()
    {
        if (GameManager.Instance == null || GameManager.Instance.playerData == null)
        {
            Debug.LogWarning("GM Ȥ�� playerData�� ���� �ʱ�ȭ���� ����");
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
        previousUI = UIType.Deposit;    //      ���
    }

    public void Deposit(int amount)
    {
        var user = GameManager.Instance.playerData;

        if (user.cash >= amount)
        {
            user.cash -= amount;
            user.bankBalance += amount;

            GameManager.Instance.SaveUserData();        //      �ݾ� ������ �ڵ� ����
            UpdateUserUI();

        }
        else
        {
            previousUI = UIType.Deposit;    //      �������� ���
            error.SetActive(true);
        }
    }

    public void DepositFromInput()
    {
        if (int.TryParse(DepositInputField.text, out int inputAmount))
        {
            Deposit(inputAmount);
            DepositInputField.text = "";       //      �Է� �ʱ�ȭ
        }
        else
        {
            Debug.Log("���ڸ� �Է����ּ���");
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
            WithdrawInputField.text = "";       //      �Է� �ʱ�ȭ
        }
        else
        {
            Debug.Log("���ڸ� �Է����ּ���");
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

        // �Է�ĭ�� ������� �� ��µǴ� ����UI
        if (string.IsNullOrEmpty(targetId) || string.IsNullOrEmpty(amountStr))
        {
            previousUI = UIType.Remittance;
            nullError.SetActive(true);
            return;
        }

        // �ݾ��� �������� Ȯ��
        if (!int.TryParse(amountStr, out int amount) || amount <= 0)
        {
            previousUI = UIType.Remittance;
            nullError.SetActive(true);
            return;
        }

        string recipientKey = $"UserData_{targetId}";   //      �����ϴ� ���� ID ��������

        // ������ ID�� �������� ���� �� ��µǴ� ����UI
        if (!PlayerPrefs.HasKey(recipientKey))
        {
            previousUI = UIType.Remittance;
            idError.SetActive(true);
            return;
        }

        var sender = GameManager.Instance.playerData;       //      �ݾ� ���� �� ������ ����� �ݾ� ������ ��������

        // �ܾ��� ������ �� ��µǴ� ����UI
        if (sender.bankBalance < amount)
        {
            previousUI = UIType.Remittance;
            error.SetActive(true);
            return;
        }

        // �ڽſ��� �۱� ��� X
        if (targetId == GameManager.Instance.currentUserId)
        {
            previousUI = UIType.Remittance;
            nullError.SetActive(true);
            return;
        }

        // ������ ���� ������ ��������
        string json = PlayerPrefs.GetString(recipientKey);
        Debug.Log("���� ����� ������: " + PlayerPrefs.GetString(recipientKey));       //      ���� �� ���������� Ȯ�ο� Log
        UserData recipient = JsonUtility.FromJson<UserData>(json);      //      ���� ������(json)�� ������ ��������

        // �۱� ó��
        sender.bankBalance -= amount;
        recipient.bankBalance += amount;

        // ����
        GameManager.Instance.SaveUserData();        //      �ڽ��� �����͸� ����
        PlayerPrefs.SetString(recipientKey, JsonUtility.ToJson(recipient));
        PlayerPrefs.Save();

        // UI ������Ʈ
        UpdateUserUI();
        RemitIdInputField.text = "";
        RemitAmountInputField.text = "";
        remittance.SetActive(false);
        atm.SetActive(true);        //        �۱� ���� �� ���� ȭ������

    }

    // ������ �� ���ư� �� ù ȭ���� �ƴ� ���� ȭ������ ����
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
                atm.SetActive(true);    //     Ȥ�ó� ���� ���ܿ� ���� ó��
                break;
        }
    }
}
