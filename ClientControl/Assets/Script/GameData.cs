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

[Serializable] // ����ȭ

public class Data // �� �̵��� ������ ������ �͵�
{
    public string ClientIP; // Ŭ�� ip
    public int Port;
    public string ServerIP; // ���� ip

    public bool Security = true; // �ڹ��� true, false ���� �����Ϳ� ����
}

