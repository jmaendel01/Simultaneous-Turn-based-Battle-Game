using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This class holds information about the player
public class Player : MonoBehaviour
{

    private Text healthField;
    private string playerName;
    public string getPlayerName()
    {
        return playerName;
    }

    public bool isPlayer;

    public static readonly int maxHealth = 15;
    private int health;
    public int getHealth()
    {
        return health;
    }

    public void dealDamage(int amount)
    {
        health -= amount;
        healthField.text = "Health: " + health.ToString();
    }

    public void Setup(string name, bool isPlayer)
    {
        health = maxHealth;
        this.isPlayer = isPlayer;
        playerName = name;
        Text[] textFields = GetComponentsInChildren<Text>();
        textFields[0].text = playerName;
        healthField = textFields[1];
        healthField.text = "Health: " + health.ToString();
    }
}
