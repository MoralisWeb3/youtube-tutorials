using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyPrefab : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TextMeshProUGUI levelLabel;

    public void Initialize(string newName, int newLevel)
    {
        nameLabel.text = newName;
        levelLabel.text = newLevel.ToString();
    }
}
