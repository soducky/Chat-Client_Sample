using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartBtn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonBtn()
    {
        GameObject.FindGameObjectWithTag("Client").GetComponent<Client>().ConnectToServer();
    }

}
