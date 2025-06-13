using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranking : MonoBehaviour
{
    private const int MaxRankCount = 10;
    private const string RankKeyPrefix = "Rank_";

    public static void SaveNewScore(int newScore)
    {
        List<int> scores = LoadScores();

        // �� ���� �߰�
        scores.Add(newScore);
        scores.Sort((a, b) => b.CompareTo(a)); // �������� ����

        // ���� 10���� ����
        if (scores.Count > MaxRankCount)
            scores.RemoveRange(MaxRankCount, scores.Count - MaxRankCount);

        // ����
        for (int i = 0; i < scores.Count; i++)
        {
            PlayerPrefs.SetInt(RankKeyPrefix + i, scores[i]);
        }

        PlayerPrefs.Save();
    }

    public static List<int> LoadScores()
    {
        List<int> scores = new List<int>();

        for (int i = 0; i < MaxRankCount; i++)
        {
            if (PlayerPrefs.HasKey(RankKeyPrefix + i))
            {
                scores.Add(PlayerPrefs.GetInt(RankKeyPrefix + i));
            }
        }

        return scores;
    }

    public static void ResetScores()  // (����) ����׿� �ʱ�ȭ
    {
        for (int i = 0; i < MaxRankCount; i++)
        {
            PlayerPrefs.DeleteKey(RankKeyPrefix + i);
        }

        PlayerPrefs.Save();
    }
}
