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
using UnityEngine.Windows;
using UnityEngine.SceneManagement;

public class Client : MonoBehaviour
{
    string clientName;

    bool socketReady; // ���� �غ�Ǿ�����

    TcpClient socket;
    NetworkStream stream; // ��Ʈ�� ����
    StreamWriter writer;
    StreamReader reader;

    // private string SendStr = "%1POWR";
    private IEnumerator coroutine;
    private bool isCoroutine = false;

    private void Start()
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
        string ip = DataManager.Instance.data.ServerIP;
        int port = int.Parse(DataManager.Instance.data.Port);

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
            coroutine = countTime(300f); // 300�� , 5�и��� �ݺ�
            StartCoroutine(coroutine);
        }
    }

     IEnumerator countTime(float delayTime) // �ڷ�ƾ ���鼭 �� �ҷ����� 
     {
        isCoroutine = true;
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene(0);
        isCoroutine = false;
     }

     void OnIncomingData(string data)
     {
        if (data == "%NAME") //�г��� ǥ��
        {
            clientName = DataManager.Instance.data.ClientIP; // Ŭ���̾�Ʈ ip�� pc���� �����ϴ� �г������� ��� 
            Send($"&NAME|{clientName}");

            return;
        }

            else if (data == DataManager.Instance.data.ClientIP) // �������� ��ε�ĳ��Ʈ�� �����Ͱ� Ŭ���̾�Ʈip�� pc off
            {
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
                Chat.instance.ShowMessage("���ϴٽû���"+e);
                CloseSocket();
                ConnectToServer();
            }
        }

       /* void OnSendButton(string SendInput)
        {
            if (SendInput.Trim() == "") return;
            SendInput = clientName;
            string message = SendInput;

            Send(message);

        }*/

        void CloseSocket() // ���� �ݱ� 
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
            pro.StandardInput.Write(@"shutdown -s -t 0" + Environment.NewLine); // cmd ���� �� shutdown ��ɾ� �Է� 
            pro.StandardInput.Close();


            pro.WaitForExit();
            pro.Close();

        }
   
}