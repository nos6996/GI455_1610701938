using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class textCheck : MonoBehaviour
{
    public Text textIn, textOut;
    string[] textList = {"Bear", "Lion", "Giraffe", "Kangaroo", "Monkey"};

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnClickButton()
    {
        for (int i = 0; i < 5; i++)
        {
            if (textIn.text == textList[i])
            {
                textOut.color = Color.blue;
                textOut.text = "<color=#000000ff>[</color> " + textIn.text + " <color=#000000ff>] is found.</color>";
                break;
            }
            else
            {
                textOut.color = Color.red;
                textOut.text = "<color=#000000ff>[</color> " + textIn.text + " <color=#000000ff>] is not found.</color>";
            }
        }
    }
}
