using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SendData : MonoBehaviour
{
    public Text iP, port;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Connect()
    {
        PlayerPrefs.SetString("IP", iP.text);
        PlayerPrefs.SetString("Port", port.text);
        SceneManager.LoadScene("ProgramChat");
    }
}
