using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using UnityEngine.EventSystems;
using System.Linq;
using VisCircle;
using QuickPool;
[RequireComponent(typeof(PoolsManager))]

public class GameController : MonoBehaviour
{
    [Header("Variable")]
    public static GameController instance;
    public int maxLevel;
    public bool isStartGame = false;
    public bool isControl = false;
    public bool isThrowable = true;
    bool isVibrate = false;
    Vector3 firstPos, lastPos;
    public bool isDrag = false;
    public GameObject playerBall;
    public GameObject playerBallPrefab;
    public GameObject botBall;
    public GameObject botBallPrefab;
    public GameObject targetObject;
    GameObject oldBall;
    float h;
    float v;
    Vector3 dir;
    public float speed;
    public int currentLevel;
    Vector3 originPos;
    Quaternion originRot;
    public bool isThrow = false;
    public int playerScore = 0;
    public int botScore = 0;

    [Header("UI")]
    public GameObject winPanel;
    public GameObject losePanel;
    public Canvas canvas;
    public GameObject startGameMenu;
    public Text title;
    static int currentBG = 0;
    public InputField levelInput;
    public Text currentLevelText;
    public Text playerScoreText;
    public Text botScoreText;
    public Text complimentText;
    public GameObject names;

    [Header("Objects")]
    public GameObject plusVarPrefab;
    public GameObject conffeti;
    GameObject conffetiSpawn;
    public List<GameObject> listLevel = new List<GameObject>();
    public List<GameObject> listPlayerBall = new List<GameObject>();
    public List<GameObject> listBotBall = new List<GameObject>();
    public List<Color> listBGColor = new List<Color>();
    public GameObject BG;
    public GameObject blast;
    public GameObject dancingMan;

    private void OnEnable()
    {
        Application.targetFrameRate = 60;
        instance = this;
        maxLevel = listLevel.Count;
        StartCoroutine(delayStart());
    }

    IEnumerator delayStart()
    {
        Camera.main.transform.DOMoveX(20, 0);
        Camera.main.transform.DOMoveX(0, 1);
        var colorID = Random.Range(0, 5);
        BG.GetComponent<Renderer>().material.color = listBGColor[currentBG];
        //Camera.main.backgroundColor = listBGColor[currentBG];
        currentBG++;
        if (currentBG > listBGColor.Count - 1)
        {
            currentBG = 0;
        }
        yield return new WaitForSeconds(0.1f);
        currentLevel = PlayerPrefs.GetInt("currentLevel");
        currentLevelText.text = "Lv." + (currentLevel + 1).ToString();
        listLevel[currentLevel].SetActive(true);
        playerBallPrefab = listPlayerBall[currentLevel];
        botBallPrefab = listBotBall[currentLevel];
        targetObject = listLevel[currentLevel].transform.GetChild(0).gameObject;
        targetObject.transform.parent = null;
        //startGameMenu.SetActive(true);
        title.DOColor(new Color32(255, 255, 255, 0), 3);
        isControl = true;
        originPos = playerBallPrefab.transform.position;
        originRot = playerBallPrefab.transform.rotation;
        playerBall = playerBallPrefab.Spawn(originPos, originRot);
        botBall = botBallPrefab.Spawn(new Vector3(-originPos.x, originPos.y, originPos.z), originRot);
        botBall.tag = "Enemy";
        playerBall.SetActive(false);
        botBall.SetActive(false);
        targetObject.SetActive(false);
    }

    Vector3 firstP;
    Vector3 lastP;
    private void Update()
    {
        if (isStartGame && isControl && isThrowable)
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnMouseDown();
            }

            if (Input.GetMouseButton(0))
            {
                OnMouseDrag();
            }

            if (Input.GetMouseButtonUp(0))
            {
                OnMouseUp();
            }
        }
        else if (!isStartGame && isControl)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ButtonStartGame();
                //OnMouseDown();
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            PlayerPrefs.DeleteAll();
        }
    }

    void OnMouseDown()
    {
        if (!isDrag)
        {
            firstP = Input.mousePosition;
            isDrag = true;
        }
    }

    void OnMouseDrag()
    {
        if (isDrag)
        {
            lastP = Input.mousePosition;
            if (Vector3.Distance(firstP, lastP) > 0.5f)
            {
#if UNITY_EDITOR
                h = Input.GetAxis("Mouse X");
                v = Input.GetAxis("Mouse Y");
#endif
#if UNITY_IOS
                if (Input.touchCount > 0)
                {
                    h = Input.touches[0].deltaPosition.x / 9;
                    v = Input.touches[0].deltaPosition.y / 9;
                }
#endif
                dir = new Vector3(h, v, v);

                var rigid = playerBall.GetComponent<Rigidbody>();
                //dir *= 25;
                //rigid.AddForce(new Vector3(dir.x, Mathf.Clamp(dir.y, 30, 40), Mathf.Clamp(dir.z, 30, 40)));
                var angle = 50;
                if (rigid.mass == 1.1f)
                {
                    rigid.AddTorque(Vector3.right * 10f);
                }
                else if (rigid.mass == 1f)
                {
                    angle = 25;
                    rigid.AddTorque(Vector3.up * 10f);
                }
                else
                {
                    angle = 15;
                }
                if (!isThrow)
                {
                    isThrow = true;
                    rigid.useGravity = true;
                    dir = calcBallisticVelocityVector(playerBall.transform.position, targetObject.transform.position, angle) * 55f;
                    rigid.AddForce(new Vector3(0, dir.y, dir.z));
                    StartCoroutine(limitDrag());
                }
                if (Mathf.Abs(targetObject.transform.position.x - h) > 0.1f)
                {
                    if (dir != Vector3.zero)
                    {
                        rigid.AddForce(new Vector3(Mathf.Clamp(h * 50, -30, 30), 0, 0));
                    }
                }
                else
                {
                    rigid.AddForce(new Vector3(dir.x, 0, 0));
                }
            }
        }
    }

    void OnMouseUp()
    {
        if (isDrag)
        {
            isDrag = false;
            isThrow = false;
            isThrowable = false;
            StartCoroutine(delaySpawn());
        }
    }

    IEnumerator limitDrag()
    {
        yield return new WaitForSeconds(0.075f);
        if (isDrag)
        {
            isDrag = false;
            isThrow = false;
            isThrowable = false;
            StartCoroutine(delaySpawn());
        }
    }

    IEnumerator delaySpawn()
    {
        yield return new WaitForSeconds(1);
        isThrowable = true;
        SpawnObject();
    }

    public void SpawnObject()
    {
        if (playerBall.GetComponent<Rigidbody>().useGravity == true)
        {
            StartCoroutine(delayRemove(playerBall));
            playerBall = playerBallPrefab.Spawn(originPos, originRot);
            playerBall.SetActive(true);
        }
    }

    IEnumerator delayRemove(GameObject target)
    {
        yield return new WaitForSeconds(5);
        if (!target.CompareTag("Untagged"))
        {
            target.GetComponent<Rigidbody>().useGravity = false;
            target.GetComponent<Rigidbody>().velocity = Vector3.zero;
            target.Despawn();
        }
        else
        {
            target.GetComponent<Rigidbody>().useGravity = false;
            target.GetComponent<Rigidbody>().velocity = Vector3.zero;
            target.transform.DOScale(Vector3.zero, 0.5f);
        }
    }

    IEnumerator BotPlay()
    {
        yield return new WaitForSeconds(Random.Range(1f,1.5f));
        Vector3 botDir = Vector3.zero;
        if (targetObject != null)
        {
            botDir = new Vector3(targetObject.transform.position.x + Random.Range(-0.3f, 0.3f), targetObject.transform.position.y + Random.Range(10, 15), targetObject.transform.position.z + Random.Range(10, 15));
            var botRigid = botBall.GetComponent<Rigidbody>();
            botRigid.useGravity = true;
            botDir *= 25;
            botRigid.AddForce(botDir);
            if (botRigid.mass == 1.1f)
            {
                botRigid.AddTorque(Vector3.right * 10f);
            }
            else if (botRigid.mass == 1f)
            {
                botRigid.AddTorque(Vector3.up * 10f);
            }
            yield return new WaitForSeconds(1f);
            if (botBall.GetComponent<Rigidbody>().useGravity == true)
            {
                StartCoroutine(delayRemove(botBall));
                botBall = botBallPrefab.Spawn(new Vector3(-originPos.x, originPos.y, originPos.z), originRot);
                botBall.tag = "Enemy";
                botBall.SetActive(true);
            }
        }
        if (isStartGame)
        {
            StartCoroutine(BotPlay());
        }
    }

    Vector3 calcBallisticVelocityVector(Vector3 source, Vector3 target, float angle)
    {
        Vector3 direction = target - source;
        float h = direction.y;
        direction.y = 0;
        float distance = direction.magnitude;
        float a = angle * Mathf.Deg2Rad;
        direction.y = distance * Mathf.Tan(a);
        distance += h / Mathf.Tan(a);

        // calculate velocity
        float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * a));
        return velocity * direction.normalized;
    }

    public Vector3 worldToUISpace(Canvas parentCanvas, Vector3 worldPos)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        Vector2 movePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, screenPos, parentCanvas.worldCamera, out movePos);
        return parentCanvas.transform.TransformPoint(movePos);
    }

    public void ButtonStartGame()
    {
        isControl = false;
        //startGameMenu.SetActive(false);
        UIManager.Instance.Show_Match_UI();
        StartCoroutine(delayPlayable());
    }

    IEnumerator delayPlayable()
    {
        playerBall.SetActive(true);
        botBall.SetActive(true);
        targetObject.SetActive(true);
        yield return new WaitForSeconds(3.5f);
        isStartGame = true;
        isControl = true;
        StartCoroutine(BotPlay());
        try
        {
            if (targetObject != null)
            {
                Destroy(targetObject.transform.GetChild(1).gameObject);
            }
        }
        catch { }
        yield return new WaitForSeconds(1.5f);
        names.transform.DOLocalMoveY(-500, 1);
        //names.SetActive(false);
    }

    public void PlayerScoring()
    {
        if (!UnityEngine.iOS.Device.generation.ToString().Contains("5"))
        {
            MMVibrationManager.Haptic(HapticTypes.MediumImpact);
        }
        targetObject.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        var random = Random.Range(0, 10);
        if (random > 5)
        {
            complimentText.text = "PERFECT";
        }
        else
        {
            complimentText.text = "AWESOME";
            complimentText.color = Color.yellow;
        }
        complimentText.gameObject.SetActive(true);
        complimentText.transform.DOScale(Vector3.one * 0.1f, 0);
        complimentText.transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutBounce).OnComplete(() => { complimentText.gameObject.SetActive(false); });
        playerScore++;
        if(playerScore > 3)
        {
            playerScore = 3;
        }
        playerScoreText.text = botScore.ToString() + " - " + playerScore.ToString();
        if (currentLevel == 15)
        {
            targetObject.transform.DOMoveX(Random.Range(-1, 1), 0);
        }
        if (playerScore >= 3)
        {
            WinMethod();
        }
    }

    public void BotScoring()
    {
        botScore++;
        if(botScore > 3)
        {
            botScore = 3;
        }
        playerScoreText.text = botScore.ToString() + " - " + playerScore.ToString();
        if (currentLevel == 6)
        {
            targetObject.transform.DOMoveX(Random.Range(-1f, 1f), 0);
        }
        if (botScore >= 3)
        {
            Lose();
        }
    }

    IEnumerator Win()
    {
        Debug.Log("Win");
        losePanel.SetActive(false);

        float ratio = Camera.main.aspect;

        if (ratio >= 0.74) // 3:4
        {
            dancingMan.transform.DOMoveZ(0, 0);
        }
        else if (ratio >= 0.56) // 9:16
        {
            dancingMan.transform.DOMoveZ(0, 0);
        }
        else if (ratio >= 0.45) // 9:19
        {
            dancingMan.transform.DOMoveZ(0, 0);
        }

        UIManager.Instance.FireWorkEffect();
        conffetiSpawn = Instantiate(conffeti);
        yield return new WaitForSeconds(2);
        //winPanel.SetActive(true);
        var star = playerScore - botScore;
        if(star > 3)
        {
            star = 3;
        }
        UIManager.Instance.Show_Win_UI(star);
        currentLevel++;
        if (currentLevel < maxLevel)
        {
            PlayerPrefs.SetInt("currentLevel", currentLevel);
        }
        else
        {
            currentLevel = Random.Range(0, maxLevel - 1);
            PlayerPrefs.SetInt("currentLevel", currentLevel);
        }
    }

    public void WinMethod()
    {
        if (isStartGame)
        {
            isControl = false;
            isStartGame = false;
            StartCoroutine(Win());
        }
    }

    public void Lose()
    {
        if (isStartGame)
        {
            dancingMan.SetActive(false);
            isControl = false;
            Debug.Log("Lose");
            isStartGame = false;
            StartCoroutine(delayLose());
        }
    }

    IEnumerator delayLose()
    {
        yield return new WaitForSeconds(1);
        //losePanel.SetActive(true);
        UIManager.Instance.Show_Lose_UI(1);
    }

    public void LoadScene()
    {
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        StartCoroutine(delayLoadScene());
    }

    IEnumerator delayLoadScene()
    {
        Camera.main.transform.DOMoveX(-20, 1);
        yield return new WaitForSeconds(1);
        var temp = conffetiSpawn;
        Destroy(temp);
        SceneManager.LoadScene(0);
    }

    public void OnChangeMap()
    {
        if (levelInput != null)
        {
            int level = int.Parse(levelInput.text.ToString());
            Debug.Log(level);
            if (level < maxLevel)
            {
                PlayerPrefs.SetInt("currentLevel", level);
                SceneManager.LoadScene(0);
            }
        }
    }

    public void ButtonNextLevel()
    {
        title.DOKill();
        isStartGame = true;
        SceneManager.LoadScene(0);
    }
}
