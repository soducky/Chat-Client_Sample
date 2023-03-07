using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Cache;
using UnityEngine;
using UnityEngine.UI;

public class InputData : MonoBehaviour // 데이터 입력 클래스 
{
    private void Start()
    {
        DataManager.Instance.LoadGameData();
        WarmingUpLoad();
    }

    public void WarmingUpLoad() // 불러오기 전 준비단계
    {

        GameObject.FindGameObjectWithTag("ClientIP").GetComponent<InputField>().text = PlayerPrefs.GetString("InputClientIP");
        GameObject.FindGameObjectWithTag("Port").GetComponent<InputField>().text = PlayerPrefs.GetInt("InputPort").ToString();
        GameObject.FindGameObjectWithTag("ServerIP").GetComponent<InputField>().text = PlayerPrefs.GetString("ServerClientIP");
        // PlayerPrefs로 저장한 값들을 대입해서 값 불러오기 
    }

    public void WarmingUpSave() // 저장하기전 준비단계
    {
            PlayerPrefs.DeleteKey("InputClientIP");
            PlayerPrefs.DeleteKey("InputPort");
            PlayerPrefs.DeleteKey("ServerClientIP");
        // 기존에 있던 데이터들 삭제

            PlayerPrefs.SetString("InputClientIP", GameObject.FindGameObjectWithTag("ClientIP").GetComponent<InputField>().text);
            PlayerPrefs.SetInt("InputPort", int.Parse(GameObject.FindGameObjectWithTag("Port").GetComponent<InputField>().text));
            PlayerPrefs.SetString("ServerClientIP", GameObject.FindGameObjectWithTag("ServerIP").GetComponent<InputField>().text);
     // PlayperPrefs 값으로 저장하기 (1)
    }

    public void GameDataSaveKey()  // 데이터를 key 값으로 변환 
    {
        DataManager.Instance.data.ClientIP = PlayerPrefs.GetString("InputClientIP");
        DataManager.Instance.data.Port = PlayerPrefs.GetInt("InputPort").ToString();
        DataManager.Instance.data.ServerIP = PlayerPrefs.GetString("ServerClientIP");
        // 데이터들을 PlayerPrefs 값으로 변환시켜서 저장하여 불러오기를 쉽게 하기 위해
    }

    public void SaveBtnClik() // 저장 메서드
    {
        WarmingUpSave();
        GameDataSaveKey();
        DataManager.Instance.SaveGameData();
    }

}
