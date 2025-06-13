using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainRanking : MonoBehaviour
{
    public TMP_Text rankingText;

    void Start()
    {
        ShowRanking();
    }

    void ShowRanking()
    {
        List<int> scores = Ranking.LoadScores();
        rankingText.text = "<b><size=100> 랭킹</size></b>\n\n";

        for (int i = 0; i < scores.Count; i++)
        {
            rankingText.text += $"{i + 1}등 : {scores[i]}점\n";
        }

        if (scores.Count == 0)
            rankingText.text += "기록 없음";
    }
}
