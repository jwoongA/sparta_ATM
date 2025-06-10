using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public UserData userData;

    [SerializeField] private TextMeshProUGUI cashText;
    [SerializeField] private TextMeshProUGUI balanceText;
    [SerializeField] private TextMeshProUGUI nameText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // LoadUserData();
    }

    void OnApplicationQuit()
    {
        SaveUserData();
    }

    public void Refresh()
    {
        nameText.text = userData.name;
        cashText.text = string.Format("{0:N0}", userData.cash);
        balanceText.text = string.Format("{0:N0}  잔고", userData.balance);
    }

    public void SaveUserData()
    {
        string fileName = $"UserData_{userData.id}.json";
        string json = JsonUtility.ToJson(userData, true);
        string path = Application.dataPath + "/UserData/" + fileName;
        File.WriteAllText(path, json);
    }

    public void LoadUserData()
    {
        string fileName = $"UserData_{userData.id}.json";
        string path = Application.dataPath + "/UserData/" + fileName;
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            userData = JsonUtility.FromJson<UserData>(json);
        }
        else
        {
            Debug.Log("정보가 없습니다.");
        }
        Refresh();
    }

    public bool Transfer(string senderId, string receiverId, ulong amount, out string errorMsg)
    {
        // 파일 경로 생성
        string senderPath = $"{Application.dataPath}/UserData/UserData_{senderId}.json";
        string receiverPath = $"{Application.dataPath}/UserData/UserData_{receiverId}.json";

        // 파일 존재 확인
        if (!File.Exists(senderPath) || !File.Exists(receiverPath))
        {
            errorMsg = "송금 대상을 찾을 수 없습니다.";
            return false;
        }

        // JSON 읽기 → 객체 변환
        string senderJson = File.ReadAllText(senderPath);
        string receiverJson = File.ReadAllText(receiverPath);

        UserData sender = JsonUtility.FromJson<UserData>(senderJson);
        UserData receiver = JsonUtility.FromJson<UserData>(receiverJson);

        // 송금 가능 여부 확인
        if (sender.balance < amount)
        {
            errorMsg = "잔액이 부족합니다.";
            return false;
        }

        // 송금 처리
        sender.balance -= amount;
        receiver.balance += amount;

        // JSON으로 저장
        File.WriteAllText(senderPath, JsonUtility.ToJson(sender, true));
        File.WriteAllText(receiverPath, JsonUtility.ToJson(receiver, true));

        errorMsg = null;
        return true;
    }
}
