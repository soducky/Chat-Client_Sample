using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideMonitor : MonoBehaviour
{

    public Image IsLock; // 기본 이미지
    public Sprite IsUnlock; // 열린 자물쇠
    public Sprite IsLockSprite; // 닫힌 자물쇠

    public GameObject HideImg; // 화면을 가릴 가림막

    int i = 1; // 초기값 1 
    void Awake ()
    {
        Application.runInBackground = true; 
    }

    void Update()
    {
        if (DataManager.Instance.data.Security == true) // start문에서 인식 안돼서 update문으로 옮김
        {
            IsLock.sprite = IsLockSprite;
            HideImg.SetActive(true);
        }

        else if (DataManager.Instance.data.Security == false)
        {
            IsLock.sprite = IsUnlock;
            HideImg.SetActive(false);
            i = 2; // i를 2로 바꿔줘야 그 다음 버튼을 누를때 홀수가 됨.
        }
    }

    public void LockClick()
    {
        i++; // 누를떄마다 i값 증가

        if (i % 2 == 0) // 짝수일때 가림막 비활성화, 자물쇠 열기
        {
            IsLock.sprite = IsUnlock;
            HideImg.SetActive(false);
            DataManager.Instance.data.Security = false;
        }

        else  // 홀수일때 가림막 활성화, 자물쇠 닫기
        {
            IsLock.sprite = IsLockSprite;
            HideImg.SetActive(true);
            DataManager.Instance.data.Security = true;
        }
    }
}
