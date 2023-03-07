using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Cache;
using UnityEngine;
using UnityEngine.UI;

public class InputData : MonoBehaviour // ������ �Է� Ŭ���� 
{
    private void Start()
    {
        DataManager.Instance.LoadGameData();
        WarmingUpLoad();
    }

    public void WarmingUpLoad() // �ҷ����� �� �غ�ܰ�
    {

        GameObject.FindGameObjectWithTag("ClientIP").GetComponent<InputField>().text = PlayerPrefs.GetString("InputClientIP");
        GameObject.FindGameObjectWithTag("Port").GetComponent<InputField>().text = PlayerPrefs.GetInt("InputPort").ToString();
        GameObject.FindGameObjectWithTag("ServerIP").GetComponent<InputField>().text = PlayerPrefs.GetString("ServerClientIP");
        // PlayerPrefs�� ������ ������ �����ؼ� �� �ҷ����� 
    }

    public void WarmingUpSave() // �����ϱ��� �غ�ܰ�
    {
            PlayerPrefs.DeleteKey("InputClientIP");
            PlayerPrefs.DeleteKey("InputPort");
            PlayerPrefs.DeleteKey("ServerClientIP");
        // ������ �ִ� �����͵� ����

            PlayerPrefs.SetString("InputClientIP", GameObject.FindGameObjectWithTag("ClientIP").GetComponent<InputField>().text);
            PlayerPrefs.SetInt("InputPort", int.Parse(GameObject.FindGameObjectWithTag("Port").GetComponent<InputField>().text));
            PlayerPrefs.SetString("ServerClientIP", GameObject.FindGameObjectWithTag("ServerIP").GetComponent<InputField>().text);
     // PlayperPrefs ������ �����ϱ� (1)
    }

    public void GameDataSaveKey()  // �����͸� key ������ ��ȯ 
    {
        DataManager.Instance.data.ClientIP = PlayerPrefs.GetString("InputClientIP");
        DataManager.Instance.data.Port = PlayerPrefs.GetInt("InputPort").ToString();
        DataManager.Instance.data.ServerIP = PlayerPrefs.GetString("ServerClientIP");
        // �����͵��� PlayerPrefs ������ ��ȯ���Ѽ� �����Ͽ� �ҷ����⸦ ���� �ϱ� ����
    }

    public void SaveBtnClik() // ���� �޼���
    {
        WarmingUpSave();
        GameDataSaveKey();
        DataManager.Instance.SaveGameData();
    }

}
