using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class PopupBank : MonoBehaviour
{
    // 입금용으로 만원, 3만원, 5만원 버튼 필요 (O)
    // 직접 입력해서 반응하는 버튼도 필요 (O)
    // 출금용으로 만원, 3만원, 5만원 버튼 필요 (O)

    // 돈 부족하면 나오는 팝업
    public GameObject okButton;

    public TMP_InputField inputFieldDes;
    public TMP_InputField inputFieldWit;
    public TMP_InputField inputFieldTarget;
    public TMP_InputField inputFieldAmount;
    public GameObject remittanceWarning;
    public TextMeshProUGUI warningText;

    public void Deposite(int amount)
    {
        if (GameManager.Instance.userData.cash >= amount)
        {
            GameManager.Instance.userData.balance += (ulong)amount;
            GameManager.Instance.userData.cash -= amount;
            GameManager.Instance.Refresh();
            GameManager.Instance.SaveUserData();
        }
        else
        {
            okButton.SetActive(true);
        }
    }

    public void CustomDeposite()
    {
        int amount = 0;
        if (int.TryParse(inputFieldDes.text, out amount) && amount > 0)
        {
            Deposite(amount);
            GameManager.Instance.SaveUserData();
        }
        else
        {
            okButton.SetActive(true);
        }
    }

    public void Withdrawal(int amount)
    {
        if (GameManager.Instance.userData.balance >= (ulong)amount)
        {
            GameManager.Instance.userData.cash += amount;
            GameManager.Instance.userData.balance -= (ulong)amount;
            GameManager.Instance.Refresh();
            GameManager.Instance.SaveUserData();
        }
        else
        {
            okButton.SetActive(true);
        }
    }

    public void CustomWithdrawl()
    {
        int amount = 0;
        if (int.TryParse(inputFieldWit.text, out amount) && amount > 0)
        {
            Withdrawal(amount);
            GameManager.Instance.SaveUserData();
        }
        else
        {
            okButton.SetActive(true);
        }
    }

    public void TransferButton()
    {
        string targetId = inputFieldTarget.text.Trim();
        string amountText = inputFieldAmount.text.Trim();

        // 입력값 체크
        if (string.IsNullOrEmpty(targetId))
        {
            remittanceWarning.SetActive(true);
            warningText.text = "송금 대상을 입력해주세요.";
            return;
        }
        if (string.IsNullOrEmpty(amountText))
        {
            remittanceWarning.SetActive(true);
            warningText.text = "송금 금액을 입력해주세요.";
            return;
        }

        if (amountText.Contains("-"))
        {
            remittanceWarning.SetActive(true);
            warningText.text = "올바른 금액을 입력해주세요.";
            return;
        }

        ulong amount;
        if (!ulong.TryParse(amountText, out amount))
        {
            remittanceWarning.SetActive(true);
            warningText.text = "송금 한도를 초과했습니다.";
            return;
        }
        if (amount == 0)
        {
            remittanceWarning.SetActive(true);
            warningText.text = "0원 이상 입력해주세요.";
            return;
        }

        // (본인 아이디와 같은지 체크도 추가 가능!)
        string senderId = GameManager.Instance.userData.id;

        if (senderId == targetId)
        {
            remittanceWarning.SetActive(true);
            warningText.text = "본인에게는 송금할 수 없습니다.";
            return;
        }

        string errorMsg;
        // 송금 함수 호출
        bool success = GameManager.Instance.Transfer(senderId, targetId, amount, out errorMsg);

        if (success)
        {
            remittanceWarning.SetActive(true);
            warningText.text = "송금 성공";
            GameManager.Instance.LoadUserData(); // 잔액 갱신!
        }
        else
        {
            remittanceWarning.SetActive(true);
            warningText.text = errorMsg;
        }
    }
}
