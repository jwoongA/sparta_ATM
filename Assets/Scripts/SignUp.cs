using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using System.IO;

public class SignUp : MonoBehaviour
{
    [SerializeField] private TMP_InputField idInputField;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_InputField psInputField;
    [SerializeField] private TMP_InputField psConfirmInputField;
    [SerializeField] private TextMeshProUGUI noticeText;
    [SerializeField] private GameObject popupBank;
    [SerializeField] private GameObject popupLogin;
    [SerializeField] private TMP_InputField logIDInput;
    [SerializeField] private TMP_InputField logPSInput;
    [SerializeField] private GameObject warning;

    public void SignUpButton()
    {
        if (string.IsNullOrEmpty(idInputField.text))
        {
            noticeText.text = "아이디를 입력해주세요.";
            return;
        }

        if (string.IsNullOrEmpty(nameInputField.text))
        {
            noticeText.text = "이름을 입력해주세요.";
            return;
        }

        if (string.IsNullOrEmpty(psInputField.text))
        {
            noticeText.text = "비밀번호를 입력해주세요.";
            return;
        }

        if (string.IsNullOrEmpty(psConfirmInputField.text))
        {
            noticeText.text = "비밀번호 확인을 입력해주세요.";
            return;
        }

        if (psInputField.text != psConfirmInputField.text)
        {
            noticeText.text = "비밀번호가 일치하지 않습니다.";
            return;
        }

        GameManager.Instance.userData.name = nameInputField.text;
        GameManager.Instance.userData.id = idInputField.text;
        GameManager.Instance.userData.ps = psInputField.text;
        GameManager.Instance.userData.cash = 100000;
        GameManager.Instance.userData.balance = 50000;
        GameManager.Instance.SaveUserData();

        noticeText.text = "회원가입이 완료되었습니다.";
    }

    public bool Login(string idInput, string psInput, out UserData loadedData)
    {
        string fileName = $"UserData_{idInput}.json";
        string path = Application.dataPath + "/UserData/" + fileName;

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            loadedData = JsonUtility.FromJson<UserData>(json);

            if (loadedData.ps == psInput)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            loadedData = null;
            return false;
        }
    }

    public void LoginButton()
    {
        UserData loadedUser;
        if (Login(logIDInput.text, logPSInput.text, out loadedUser))
        {
            GameManager.Instance.userData = loadedUser;
            popupLogin.SetActive(false);
            popupBank.SetActive(true);
            GameManager.Instance.Refresh();
        }
        else
        {
            Debug.Log("로그인 실패");
            warning.SetActive(true);
        }
    }
}
