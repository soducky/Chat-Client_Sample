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
    string clientName; // 클라이언트 이름 == 클라이언트 ip

    bool socketReady; // 소켓 준비되었는지

    TcpClient socket; // 소켓 열기
    NetworkStream stream; // 스트림 보기
    StreamWriter writer;
    StreamReader reader;

    private IEnumerator coroutine; // 코루틴(문자 보내기 위해) -> 태블릿과 통신
    private bool isCoroutine = false; // 2분짜리 코루틴 update문에 사용 
    private bool thisCoroutine = false; // 2분짜리 코루틴  SceneReLoad에 사용
    private bool LoadCoroutine = false; // 1분짜리 코루틴 소켓 재생성 부분에 사용 (Send메소드)

    private void Start()
    {
        ConnectToServer(); // 바로 서버 연결
    }

    public void ConnectToServer()
    {
        // 이미 연결되었다면 함수 무시
        if (socketReady)
        {
            return;
        }

        // 기본 호스트/ 포트번호
        string ip = DataManager.Instance.data.ServerIP;
        int port = int.Parse(DataManager.Instance.data.Port);

        // 소켓 생성
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
            Chat.instance.ShowMessage($"소켓에러 : {e.Message}"); // 소켓에러시 씬 다시 로드 

            if (!thisCoroutine)
            {
                coroutine = SceneReLoad(120f); // 120초 , 2분마다 반복
                StartCoroutine(coroutine);
                UnityEngine.Debug.Log(thisCoroutine);
            }
        }
    }

    IEnumerator SceneReLoad(float delayTime) // 코루틴 돌면서 문자 보내기
    {
        thisCoroutine = true;
        yield return new WaitForSeconds(delayTime);

        SceneManager.LoadScene(0);  //start문에서 catch문으로 이동시 씬 다시로드

        thisCoroutine = false;
    }

    void Update()
    {
        if (socketReady && stream.DataAvailable) // 읽을 수 있음 
        {
            string data = reader.ReadLine();

            if (data != null)
                OnIncomingData(data);
        }

        if (!isCoroutine)
        {
            coroutine = countTime(120f); // 120초 , 2분마다 반복
            StartCoroutine(coroutine);
        }
    }

     IEnumerator countTime(float delayTime) // 코루틴 돌면서 문자 보내기
     {
        isCoroutine = true;
        yield return new WaitForSeconds(delayTime);

        Send(DataManager.Instance.data.ClientIP + 1); //보낼 문자 (ip+1은 켜졌다는 의미)

        isCoroutine = false;
     }

     void OnIncomingData(string data)
     {
        if (data == "%NAME") //닉네임 표시
        {
            clientName = DataManager.Instance.data.ClientIP; // 클라이언트 ip를 pc끼리 구분하는 닉네임으로 사용 
            Send($"&NAME|{clientName}");

            return;
        }

        else if (data == DataManager.Instance.data.ClientIP) // 서버에서 브로드캐스트한 데이터가 클라이언트ip면 pc off        
        {
            OnSendButton(clientName);
            OffComputer();
        }

         Chat.instance.ShowMessage(data); // if, else if 문 이외의 데이터 
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
            Chat.instance.ShowMessage("소켓다시생성"+e);


            if (!LoadCoroutine)
            {
                coroutine = LoadSocket(60f); // 60초마다 반복
                StartCoroutine(coroutine);
                UnityEngine.Debug.Log("dd"); 
            }
            //CloseSocket();
            //ConnectToServer();
        }
    }

    IEnumerator LoadSocket(float delayTime) // 코루틴 돌면서 문자 보내기
    {
        thisCoroutine = true;
        yield return new WaitForSeconds(delayTime);

        SceneManager.LoadScene(0);  //start문에서 catch문으로 이동시 씬 다시로드

        thisCoroutine = false;
    }

    void OnSendButton(string SendInput)
     {
         //if (SendInput.Trim() == "") return;
         SendInput = DataManager.Instance.data.ClientIP + 0; // 종료될때 종료신호 보내기
         string message = SendInput;

         Send(message);
     }

    private void OnApplicationQuit()
    {
        OnSendButton(clientName); // 프로그램이 종료되면 소켓 닫기 
        CloseSocket();
    }

    void CloseSocket() // 소켓 닫기 
    {
      if (!socketReady) return;

       writer.Close();
       reader.Close();
       socket.Close();
       socketReady = false;
    }

       
    void OffComputer() // 원격으로 컴퓨터 off시키는 명령어  
    {  
        ProcessStartInfo proInfo = new ProcessStartInfo();
        Process pro = new Process();

        proInfo.FileName = @"cmd"; // 실행할 파일명 입력 
        proInfo.CreateNoWindow = false; // cmd 창 띄우기 true(띄우지않기), false(띄우기)
        proInfo.UseShellExecute = false;
        proInfo.RedirectStandardOutput = true; // cmd 데이터받기
        proInfo.RedirectStandardInput = true; // cmd 데이터 보내기
        proInfo.RedirectStandardError = true; // 오류내용 받기
           
        pro.StartInfo = proInfo;
        pro.Start();
        pro.StandardInput.Write(@"shutdown -s -t 0" + Environment.NewLine); // cmd 실행 후 shutdown 명령어 입력 
        pro.StandardInput.Close();
         
        pro.WaitForExit(); 
        pro.Close();
    }
   
}