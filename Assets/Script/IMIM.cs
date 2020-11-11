using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IMIM : MonoBehaviour
{
    public GameObject invPanel;
    public bool isActive;

    public void Action()
    {
        isActive = true;
        invPanel.SetActive(isActive);
    }

    public void NonAction()
    {
        invPanel.SetActive(false);
    }

}
