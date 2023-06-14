using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchUI : MonoBehaviour
{
    public Image playerFlag;
    public Text enemyNameText;
    public Image enemyFlag;
    public GameObject matchCharactor;

    public void OnEnable()
    {
        matchCharactor.SetActive(true);
        Match();
    }

    public void Match()
    {
        StartCoroutine(C_Match());
    }

    private IEnumerator C_Match()
    {
        playerFlag.sprite = UIManager.Instance.flagPlayer;

        // random name - flag enemy
        string enemyname = UIManager.Instance.listName[Random.Range(0, UIManager.Instance.listName.Length)];
        UIManager.Instance.nameEnemy = enemyname;
        enemyNameText.text = enemyname;
        Debug.LogError(UIManager.Instance.name.Length);

        Sprite enemyflag = UIManager.Instance.flagsSpr[Random.Range(0, UIManager.Instance.flagsSpr.Length)];
        UIManager.Instance.flagEnemy = enemyflag;
        enemyFlag.sprite = enemyflag;


        yield return new WaitForSeconds(2.0f);

        UIManager.Instance.Show_Play_UI();
    }

    public void OnDisable()
    {
        matchCharactor.SetActive(false);
    }
}
