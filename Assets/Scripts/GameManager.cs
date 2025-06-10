using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public UserData playerData;

    public string currentUserId;        //      현재 로그인한 사용자 Id

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
        string json = JsonUtility.ToJson(playerData);       //      Json은 txt 저장 / 보통 중요한 내용을 저장하는데 쓰임 / 플레이어보단 아이템 정보를 저장
        PlayerPrefs.SetString(userDataKey, json);
        PlayerPrefs.Save();     //      유니티가 제공하는 저장 공간 / 보통 중요하지 않은 저장내역 저장

        Debug.Log("[GM] 데이터 저장 완료");

        // 유저 Id가 없거나 공백일 시 출력되는 오류
        if (string.IsNullOrEmpty(currentUserId))
        {
            Debug.LogWarning("현재 유저 ID가 설정되지 않았습니다");
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
            Debug.Log("[GM] 데이터 불러오기 완료");
        }
        else
        {
            // 저장 값 없고 첫 시작시 기본값
            playerData = new UserData("최동혁", 50000, 100000);
            SaveUserData();
            Debug.Log("[GM] 기본 데이터 생성");
        }

        // UI 갱신
        FindObjectOfType<UIManager>()?.UpdateUserUI();
    }

}
