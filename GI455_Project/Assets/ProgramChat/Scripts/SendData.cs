using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SendData : MonoBehaviour
{
    public Text iP, port, username;
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
        PlayerPrefs.SetString("Username", username.text);
        SceneManager.LoadScene("ProgramChat");
    }
}
