using UnityEngine;
using TMPro;

public class BattleHUD : MonoBehaviour
{
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI enemyText;

    public void SetAmmo(int value)
    {
        ammoText.text = "Attacks: " + value;
    }

    public void SetEnemies(int value)
    {
        enemyText.text = "Enemies: " + value;
    }
}
