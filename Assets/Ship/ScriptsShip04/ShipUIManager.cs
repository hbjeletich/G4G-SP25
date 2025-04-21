using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Ship
{
public class ShipUIManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI healthText;

    private void Awake()
    {
        scoreText.text = "Score: 0";
        healthText.text = "Health: 4";
    }

    public void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score;
    }

    public void UpdateHealth(float health)
    {
        healthText.text = "Health: " + health;
    }
}
}
