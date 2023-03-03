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
    string clientName; // Ŭ���̾�Ʈ �̸� == Ŭ���̾�Ʈ ip

    bool socketReady; // ���� �غ�Ǿ�����

    TcpClient socket; // ���� ����
    NetworkStream stream; // ��Ʈ�� ����
    StreamWriter writer;
    StreamReader reader;

    private IEnumerator coroutine; // �ڷ�ƾ(���� ������ ����) -> �º��� ���
    private bool isCoroutine = false; // 2��¥�� �ڷ�ƾ update���� ��� 
    private bool thisCoroutine = false; // 2��¥�� �ڷ�ƾ  SceneReLoad�� ���
    private bool LoadCoroutine = false; // 1��¥�� �ڷ�ƾ ���� ����� �κп� ��� (Send�޼ҵ�)

    private void Start()
    {
        ConnectToServer(); // �ٷ� ���� ����
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
            Chat.instance.ShowMessage($"���Ͽ��� : {e.Message}"); // ���Ͽ����� �� �ٽ� �ε� 

            if (!thisCoroutine)
            {
                coroutine = SceneReLoad(120f); // 120�� , 2�и��� �ݺ�
                StartCoroutine(coroutine);
                UnityEngine.Debug.Log(thisCoroutine);
            }
        }
    }

    IEnumerator SceneReLoad(float delayTime) // �ڷ�ƾ ���鼭 ���� ������
    {
        thisCoroutine = true;
        yield return new WaitForSeconds(delayTime);

        SceneManager.LoadScene(0);  //start������ catch������ �̵��� �� �ٽ÷ε�

        thisCoroutine = false;
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
            coroutine = countTime(120f); // 120�� , 2�и��� �ݺ�
            StartCoroutine(coroutine);
        }
    }

     IEnumerator countTime(float delayTime) // �ڷ�ƾ ���鼭 ���� ������
     {
        isCoroutine = true;
        yield return new WaitForSeconds(delayTime);

        Send(DataManager.Instance.data.ClientIP + 1); //���� ���� (ip+1�� �����ٴ� �ǹ�)

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
            OnSendButton(clientName);
            OffComputer();
        }

         Chat.instance.ShowMessage(data); // if, else if �� �̿��� ������ 
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


            if (!LoadCoroutine)
            {
                coroutine = LoadSocket(60f); // 60�ʸ��� �ݺ�
                StartCoroutine(coroutine);
                UnityEngine.Debug.Log("dd"); 
            }
            //CloseSocket();
            //ConnectToServer();
        }
    }

    IEnumerator LoadSocket(float delayTime) // �ڷ�ƾ ���鼭 ���� ������
    {
        thisCoroutine = true;
        yield return new WaitForSeconds(delayTime);

        SceneManager.LoadScene(0);  //start������ catch������ �̵��� �� �ٽ÷ε�

        thisCoroutine = false;
    }

    void OnSendButton(string SendInput)
     {
         //if (SendInput.Trim() == "") return;
         SendInput = DataManager.Instance.data.ClientIP + 0; // ����ɶ� �����ȣ ������
         string message = SendInput;

         Send(message);
     }

    private void OnApplicationQuit()
    {
        OnSendButton(clientName); // ���α׷��� ����Ǹ� ���� �ݱ� 
        CloseSocket();
    }

    void CloseSocket() // ���� �ݱ� 
    {
      if (!socketReady) return;

       writer.Close();
       reader.Close();
       socket.Close();
       socketReady = false;
    }

       
    void OffComputer() // �������� ��ǻ�� off��Ű�� ��ɾ�  
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