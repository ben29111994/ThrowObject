using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Scoring : MonoBehaviour
{
    int count = 2;
    bool isSafe = false;
    bool isCheck = false;
    Rigidbody rigid;
    // Start is called before the first frame update
    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Target") && CompareTag("Player"))
        {
            rigid.AddForce(other.transform.position * 50);
            StartCoroutine(countDown());
        }
        if (other.transform.CompareTag("Target") && CompareTag("Enemy"))
        {
            rigid.AddForce(other.transform.position * 50);
            StartCoroutine(countDown());
        }

        if (other.transform.CompareTag("Balloon") && CompareTag("Player"))
        {
            tag = "Untagged";
            var effect = other.transform.GetChild(0).transform.GetChild(0);
            effect.transform.parent = null;
            effect.GetComponent<ParticleSystem>().Play();
            Destroy(other.transform.parent.gameObject);
            var target = GameController.instance.listLevel[GameController.instance.currentLevel].transform.GetChild(0).gameObject;
            target.transform.parent = null;
            target.transform.GetChild(1).GetComponent<MeshRenderer>().enabled = true;
            target.transform.GetChild(1).GetComponent<MeshCollider>().enabled = true;
            GameController.instance.targetObject = target;
            GameController.instance.PlayerScoring();
        }
        if (other.transform.CompareTag("Balloon") && CompareTag("Enemy"))
        {
            tag = "Untagged";
            var effect = other.transform.GetChild(0).transform.GetChild(0);
            effect.transform.parent = null;
            effect.GetComponent<ParticleSystem>().Play();
            Destroy(other.transform.parent.gameObject);
            var target = GameController.instance.listLevel[GameController.instance.currentLevel].transform.GetChild(0).gameObject;
            target.transform.parent = null;
            target.transform.GetChild(1).GetComponent<MeshRenderer>().enabled = true;
            target.transform.GetChild(1).GetComponent<MeshCollider>().enabled = true;
            GameController.instance.targetObject = target;
            GameController.instance.BotScoring();
        }

        if (other.transform.CompareTag("Stick") && CompareTag("Player"))
        {
            tag = "Untagged";
            rigid.velocity = Vector3.zero;
            transform.parent = other.transform;
            rigid.isKinematic = true;
            GameController.instance.PlayerScoring();
        }
        if (other.transform.CompareTag("Stick") && CompareTag("Enemy"))
        {
            tag = "Untagged";
            rigid.velocity = Vector3.zero;
            transform.parent = other.transform;
            rigid.isKinematic = true;
            GameController.instance.BotScoring();
        }
    }

    IEnumerator countDown()
    {
        yield return new WaitForSeconds(1);
        isSafe = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("Target") && CompareTag("Player") && isSafe)
        {
            tag = "Untagged";
            GameController.instance.PlayerScoring();
        }
        if (other.transform.CompareTag("Target") && CompareTag("Enemy") && isSafe)
        {
            tag = "Untagged";
            GameController.instance.BotScoring();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Target") && CompareTag("Player"))
        {
            isSafe = false;
        }
        if (other.transform.CompareTag("Target") && CompareTag("Enemy"))
        {
            isSafe = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (!collision.transform.CompareTag("Player") && CompareTag("Player"))
        //{
        //    GameController.instance.isDrag = false;
        //    GameController.instance.isThrow = false;
        //    GameController.instance.SpawnObject();
        //    GameController.instance.isThrowable = true;
        //    isCheck = true;
        //}
        if (collision.gameObject.CompareTag("Table"))
        transform.parent = collision.transform;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Table"))
            transform.parent = null;
    }
}
