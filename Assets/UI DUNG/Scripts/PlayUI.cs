using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayUI : MonoBehaviour
{
    public Image playerFlag;
    public Image enemyFlag;
    public Text enemyText;
    public Text enemyText1;
    public Text scoreText;

    public GameObject[] keys;

    private void OnEnable()
    {
        playerFlag.sprite = UIManager.Instance.flagPlayer;
        enemyFlag.sprite = UIManager.Instance.flagEnemy;
        enemyText.text = UIManager.Instance.nameEnemy;
        enemyText1.text = UIManager.Instance.nameEnemy;
        scoreText.text = MyScore + " - " + EnemyScore;
    }

    public int Key
    {
        get
        {
            return PlayerPrefs.GetInt("Key");
        }
        set
        {
            PlayerPrefs.SetInt("Key", value);

            for(int i = 0; i < keys.Length; i++)
            {
                keys[i].SetActive(false);

                if (value > i)
                {
                    keys[i].SetActive(true);
                }
            }
        }
    }

    public int MyScore
    {
        get
        {
            return PlayerPrefs.GetInt("MyScore");
        }
        set
        {
            PlayerPrefs.SetInt("MyScore", value);
        }
    }

    public int EnemyScore
    {
        get
        {
            return PlayerPrefs.GetInt("EnemyScore");
        }
        set
        {
            PlayerPrefs.SetInt("EnemyScore", value);
        }
    }
}
