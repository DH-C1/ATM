using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public UserData playerData;

    public string currentUserId;        //      ���� �α����� ����� Id

    private void Awake()
    {   
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

    }

    public void SaveUserData()
    {
        string userDataKey = $"UserData_{currentUserId}";
        string json = JsonUtility.ToJson(playerData);       //      Json�� txt ���� / ���� �߿��� ������ �����ϴµ� ���� / �÷��̾�� ������ ������ ����
        PlayerPrefs.SetString(userDataKey, json);
        PlayerPrefs.Save();     //      ����Ƽ�� �����ϴ� ���� ���� / ���� �߿����� ���� ���峻�� ����

        Debug.Log("[GM] ������ ���� �Ϸ�");

        // ���� Id�� ���ų� ������ �� ��µǴ� ����
        if (string.IsNullOrEmpty(currentUserId))
        {
            Debug.LogWarning("���� ���� ID�� �������� �ʾҽ��ϴ�");
            return;
        }
    }

    public void LoadUserData(string userId)
    {
        currentUserId = userId;
        string userDataKey = $"UserData_{userId}";


        if (PlayerPrefs.HasKey(userDataKey))
        {
            string json = PlayerPrefs.GetString(userDataKey);
            playerData = JsonUtility.FromJson<UserData>(json);
            Debug.Log("[GM] ������ �ҷ����� �Ϸ�");
        }
        else
        {
            // ���� �� ���� ù ���۽� �⺻��
            playerData = new UserData("�ֵ���", 50000, 100000);
            SaveUserData();
            Debug.Log("[GM] �⺻ ������ ����");
        }

        // UI ����
        FindObjectOfType<UIManager>()?.UpdateUserUI();
    }

}
