using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartBtn : MonoBehaviour
{
    public void ButtonBtn()
    {
        SceneManager.LoadScene(0); // restart ��ư ������ �� �ٽ� �ҷ�����
    }
}
