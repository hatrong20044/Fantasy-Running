
using System.Collections.Generic;
using DailyRewardSystem;
using UnityEngine;
[CreateAssetMenu (fileName = "RewardsDB", menuName = "Daily Rewards System/Rewards Database ")]
public class RewardsDatabase : ScriptableObject
{
    public List<Reward> rewards;

    public int Count
    {
        get { return rewards.Count; }
    }

    public Reward GetReward (int index)
    {
        return rewards[index];
    }
}
