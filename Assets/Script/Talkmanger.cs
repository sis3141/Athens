using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Talkmanger : MonoBehaviour
{
    Dictionary<int, string[]> talkData;

    void Awake()
    {
        talkData = new Dictionary<int, string[]>();
        GenerateData();
    }

    // Update is called once per frame
    void GenerateData()
    {
        talkData.Add(1000, new string[] { "책을 구매하시겠습니까?", "어떤책을 구매하실건가요?", "감사합니다" });
        talkData.Add(100, new string[] { "책을 읽어드릴까요?", "어떤책을 읽어드릴까요?", "감사합니다" });
        talkData.Add(10, new string[] { "컴퓨터의 역사을 가져가실껀가요?", "감사합니다" });
    }

    public string GetTalk(int id, int talkIndex)
    {
        if (talkIndex == talkData[id].Length)
        {
            return null;
        }
        else
        {
            return talkData[id][talkIndex];
        }
    }
}
