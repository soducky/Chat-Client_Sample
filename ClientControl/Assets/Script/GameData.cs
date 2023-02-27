using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable] // 직렬화

public class Data // 씬 이동시 데이터 저장할 것들
{
    public string ClientIP; // 클라 ip
    public int Port;
    public string ServerIP; // 서버 ip

    public bool Security = true; // 자물쇠 true, false 값을 데이터에 저장
}

