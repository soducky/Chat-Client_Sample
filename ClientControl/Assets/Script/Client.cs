using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System.IO;
using System;
using Unity.VisualScripting;
using System.Diagnostics;
using TMPro;
using UnityEngine.UIElements;

public class Client : MonoBehaviour
{
    public InputField IpInput, PortInput;
    string clientName;

    bool socketReady; // ���� �غ�Ǿ�����
    TcpClient socket;
    NetworkStream stream; // ��Ʈ�� ����
    StreamWriter writer;
    StreamReader reader;

    public string ip;
    public int port;

    private string SendStr = "%1POWR";
    private IEnumerator coroutine;
    private bool isCoroutine = false;

    /* private void Start()
     {
         ConnectToServer();
     }*/

    public void StartBtn()
    {
        ConnectToServer();
    }

    public void ConnectToServer()
    {
        // �̹� ����Ǿ��ٸ� �Լ� ����
        if (socketReady)
        {
            return;
        }

        // �⺻ ȣ��Ʈ/ ��Ʈ��ȣ
        ip = IpInput.text == "" ? "192.168.10.27" : IpInput.text;
        port = PortInput.text == "" ? 3040 : int.Parse(PortInput.text);

        // ���� ����
        try
        {
            socket = new TcpClient(ip, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            socketReady = true;
        }
        catch (Exception e)
        {
            Chat.instance.ShowMessage($"���Ͽ��� : {e.Message}");
        }
    }

    void Update()
    {
        if (socketReady && stream.DataAvailable) // ���� �� ���� 
        {
            string data = reader.ReadLine();

            if (data != null)
                OnIncomingData(data);
        }

        if (!isCoroutine)
        {
            coroutine = countTime(30f);
            StartCoroutine(coroutine);
        }
    }

    IEnumerator countTime(float delayTime)
    {
        isCoroutine = true;
        yield return new WaitForSeconds(delayTime);
        OnSendButton(SendStr);
        isCoroutine = false;

    }

    void OnIncomingData(string data)
    {
        if (data == "%NAME") //�г��� ǥ��
        {
            clientName = "PC1";
            Send($"&NAME|{clientName}");
            return;
        }

        else if (data == "s")
        {
            Chat.instance.ShowMessage("offcomputer");
            OffComputer();
        }

        Chat.instance.ShowMessage(data);
    }

    void Send(string data)
    {
        if (!socketReady) return;

        try
        {
            writer.WriteLine(data);
            writer.Flush();
        }

        catch (Exception e)
        {
            Chat.instance.ShowMessage("���ϴٽû���");
            CloseSocket();
            ConnectToServer();
        }
    }

    public void OnSendButton(string SendInput)
    {
        if (SendInput.Trim() == "") return;
        SendInput = "c";
        string message = SendInput;

        Send(message);

    }
    void OnApplicationQuit()
    {
        CloseSocket();
    }

    void CloseSocket()
    {
        if (!socketReady) return;

        writer.Close();
        reader.Close();
        socket.Close();
        socketReady = false;
    }

    void OffComputer()
    {
        ProcessStartInfo proInfo = new ProcessStartInfo();
        Process pro = new Process();


        proInfo.FileName = @"cmd"; // ������ ���ϸ� �Է�

        proInfo.CreateNoWindow = false; // cmd â ���� true(������ʱ�), false(����)
        proInfo.UseShellExecute = false;
        proInfo.RedirectStandardOutput = true; // cmd �����͹ޱ�
        proInfo.RedirectStandardInput = true; // cmd ������ ������
        proInfo.RedirectStandardError = true; // �������� �ޱ�


        pro.StartInfo = proInfo;
        pro.Start();
        pro.StandardInput.Write(@"shutdown -s -t 0" + Environment.NewLine);
        pro.StandardInput.Close();


        pro.WaitForExit();
        pro.Close();

    }
}