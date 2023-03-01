using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideMonitor : MonoBehaviour
{

    public Image IsLock; // �⺻ �̹���
    public Sprite IsUnlock; // ���� �ڹ���
    public Sprite IsLockSprite; // ���� �ڹ���

    public GameObject HideImg; // ȭ���� ���� ������

    int i = 1; // �ʱⰪ 1 
    void Awake ()
    {
        Application.runInBackground = true; 
    }

    void Update()
    {
        if (DataManager.Instance.data.Security == true) // start������ �ν� �ȵż� update������ �ű�
        {
            IsLock.sprite = IsLockSprite;
            HideImg.SetActive(true);
        }

        else if (DataManager.Instance.data.Security == false)
        {
            IsLock.sprite = IsUnlock;
            HideImg.SetActive(false);
            i = 2; // i�� 2�� �ٲ���� �� ���� ��ư�� ������ Ȧ���� ��.
        }
    }

    public void LockClick()
    {
        i++; // ���������� i�� ����

        if (i % 2 == 0) // ¦���϶� ������ ��Ȱ��ȭ, �ڹ��� ����
        {
            IsLock.sprite = IsUnlock;
            HideImg.SetActive(false);
            DataManager.Instance.data.Security = false;
        }

        else  // Ȧ���϶� ������ Ȱ��ȭ, �ڹ��� �ݱ�
        {
            IsLock.sprite = IsLockSprite;
            HideImg.SetActive(true);
            DataManager.Instance.data.Security = true;
        }
    }
}
