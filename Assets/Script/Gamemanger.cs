using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class Gamemanger : MonoBehaviour
{
    public GameObject talkPanel;
    public Text talkText;
    public GameObject scanObject;
    public bool isAction;
    public Talkmanger talkManager;
    public int talkIndex;

    public void Action(GameObject scanObj)
    {
        scanObject = scanObj;
        if (scanObject.name.Equals("borrowcat"))
        {
            Talk(1000);
              
        }
        else if (scanObject.name.Equals("reading cat"))
        {
            Talk(100);                      
        }
        else if (scanObject.name.Equals("book_1_0"))
        {
            Talk(10);
        }
        else
        {
            isAction = false;
        }
        talkPanel.SetActive(isAction);            
    }
    public void NonAction()
    {
        talkPanel.SetActive(false);
    }

    void Talk(int id)
    {
        string talkData = talkManager.GetTalk(id, talkIndex);

        if(talkData == null)
        {
            isAction = false;
            talkIndex = 0;
            return;
        }

        talkText.text = talkData;

        isAction = true;
        talkIndex++;

    }

}
