using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.NiceVibrations;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    public Text mapID;


    [Header("Status")]
    public int levelGame;
    private int levelFixed;
    public bool isComplete;
    private bool isChangeColor;
    

    [Header("Level Controller")]
    public int count;
    public int maxCount;
    public bool isMoving;
    public bool isScored;
    public List<Ball> ballList;
    private List<Transform> brickList = new List<Transform>();
    public Texture2D[] textures;
    private Vector4 Yellow = new Vector4(255, 255, 0, 255);
    private Vector4 Red = new Vector4(255, 0, 0, 255);
    private Vector4 Black = new Vector4(0, 0, 0, 255);
    private Vector4 White = new Vector4(255, 255, 255, 255);
    private Vector4 Green = new Vector4(0, 255, 0, 255);
    public bool isBlack;
    

    [Header("Score Controller")]
    public int totalMoney;
    public int combo;
    public int swipeAmount;
    public int point;
    public List<int> scores = new List<int>();

    [Header("Camera Controller")]
    public GameObject pivotCamera;
    private Vector3 camPos;
    private float cameraMinX;
    private float cameraMaxX;
    private float cameraMinY;
    private float cameraMaxY;
    public float _horizontal;

    [Header("Effects")]
    public ParticleSystem[] fireWork;

    [Header("UI")]
    public Text currentLevelText;
    public Text nextLevelText;
    public Text swipeAmountText;
    public Text score;
    public Text comboText;
    public Animator comboAnimator;
    public Animator scoreAnimatior;
    public GameObject combo01;
    public Text combo01Text;
    public string[] congratulations;
    public Image levelFill;

    public delegate void DisableCollider();
    public static event TouchSwipe disableCollider;

    public delegate void EnableCollider();
    public static event TouchSwipe enableCollider;

    public delegate void TouchSwipe();
    public static event TouchSwipe touchSwipe;

    public Vector3 directionSwipe;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        MMVibrationManager.iOSInitializeHaptics();

        instance = this;
        for (int i = 0; i < fireWork.Length; i++)
        {
            fireWork[i].Stop();
        }
    }

    private void Start()
    {
        totalMoney = PlayerPrefs.GetInt("TotalMoney", 0);
        levelGame = PlayerPrefs.GetInt("levelGame", 0);
        currentLevelText.text = "" + (levelGame + 1);
        nextLevelText.text = "" + (levelGame + 2);
        levelFixed = levelGame;
        if (levelFixed >= textures.Length)
        {
            levelFixed = Random.Range(0, textures.Length);
        }

        //GenerateLevel();
        // StartCoroutine(C_Score());

        GenerateLevel();
    }


    private void LevelUp()
    {
        var random = Random.value;
        if (random < 0.65)
        {
            random = 0.65f;
        }

        if (swipeAmount != 0)
            totalMoney += (((int)(point * maxCount * 1.5 * random)) / swipeAmount);

        levelGame++;

        PlayerPrefs.SetInt("levelGame", levelGame);

        levelFixed = levelGame;

        if (levelFixed >= textures.Length)
        {
            //levelFixed = Random.Range(0, textures.Length);
            levelFixed = levelGame % textures.Length;
        }

        isChangeColor = true;
    }

    public int _d;
    public int _maxD;

    public void ActiveColliderBrick()
    {
        _d++;
        if (_d >= _maxD)
        {
            enableCollider();
        }
    }


    public void Swipe(Vector3 direction)
    {
        swipeAmount++;

        if (_d != _maxD) return;

        if (isComplete || isMoving)
        {
            return;
        }
        isMoving = true;

        for (int i = 0; i < ballList.Count; i++)
        {
        //    ballList[i].Move(direction);
        }


        _d = 0;
        _maxD = 0;
        directionSwipe = direction;
        disableCollider();
        touchSwipe();
        //StartCoroutine(EnableMoving());
    }

    private bool CheckBallMoving()
    {
        for (int i = 0; i < ballList.Count; i++)
        {
            if (ballList[i].isMoving)
            {
                return true;
            }
        }
        return false;
    }

    private int _curCombo;
    private float _timeCombo;

    private void Update()
    {
        if(_timeCombo > 0)
        {
            _timeCombo -= Time.deltaTime;

            if(_timeCombo <= 0)
            {
                combo01.SetActive(true);
                _timeCombo = 0;
                combo01Text.text = "Combo x" + _curCombo;
                _curCombo = 0;
            }
        }

        if (!CheckBallMoving())
        {
            isMoving = false;
            combo = 0;
        }
    }

    private IEnumerator EnableMoving()
    {
        yield return new WaitForSeconds(0.25f);
        isMoving = false;
        combo = 0;

    }
    #region Map
    private void ClearMap()
    {
        _d = _maxD = 0;
        levelFill.fillAmount = 0;
        blankObjects.Clear();
        ballList.Clear();
        brickList.Clear();
        swipeAmount = 0;
        swipeAmountText.text = swipeAmount.ToString();
        isComplete = false;
        count = maxCount = point = 0;
        for (int i = 0; i < fireWork.Length; i++)
        {
            fireWork[i].Stop();
        }
        Time.timeScale = 1;
        isComplete = false;
        PoolManager.instance.RefreshItem(PoolManager.NameObject.brick);
        PoolManager.instance.RefreshItem(PoolManager.NameObject.tile);
        PoolManager.instance.RefreshItem(PoolManager.NameObject.redball);
        PoolManager.instance.RefreshItem(PoolManager.NameObject.yellowball);
        PoolManager.instance.RefreshItem(PoolManager.NameObject.hole);
        PoolManager.instance.RefreshItem(PoolManager.NameObject.obstacle);
        PoolManager.instance.RefreshItem(PoolManager.NameObject.blank);
    }

    public void GenerateLevel()
    {
        //AnalyticsManager.instance.CallEvent(AnalyticsManager.EventType.StartEvent);

        ClearMap();

        if (isChangeColor)
        {
            isChangeColor = false;
            ColorManager.instance.ChangeColor();
        }

        levelFixed = levelGame % textures.Length;

        UIManager.instance.Show_InGameUI();
        //currentLevelText.text = "" + (levelGame + 1);
        //nextLevelText.text = "" + (levelGame + 2);
        currentLevelText.text = "" + (levelFixed + 1);
        nextLevelText.text = "" + (levelFixed + 2);
        GenerateMap(textures[levelFixed]);
        Enviroment.Instance.RandomTransform();
    }

    public void ResetLevel()
    {
        levelGame = 0;
        PlayerPrefs.SetInt("levelGame", levelGame);
        GenerateLevel();

    }

    private void GenerateMap(Texture2D texture)
    {
        mapID.text = texture.name;
        if (texture.height % 2 == 0)
        {
            for (int x = 0; x < texture.width; x++)
            {
                isBlack = !isBlack;
                for (int y = 0; y < texture.height; y++)
                {
                    GenerateTile(texture, x, y);
                    isBlack = !isBlack;
                }
            }

        }
        else if (texture.height % 2 != 0)
        {

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    GenerateTile(texture, x, y);
                    isBlack = !isBlack;
                }
            }
        }

        SetCameraPos(texture);
    }


    public IEnumerator ResetTimeScale()
    {
        yield return new WaitForSeconds(0.25f);
        Time.timeScale = 1f;
    }
    private void GenerateTile(Texture2D texture, int x, int y)
    {
        Color32 pixelColor = texture.GetPixel(x, y);
        Vector4 color32 = new Vector4(pixelColor.r, pixelColor.g, pixelColor.b, pixelColor.a);
        Vector3 pos = new Vector3(x, 0, y);

        if (color32 == Red)
        {
            GenerateRedBall(pos);
            GenerateBrick(pos);
        }
        else if (color32 == Yellow)
        {
            GenerateYellowBall(pos);
            GenerateBrick(pos);
        }
        // else if (color32 == White)
        // {
        //     GenerateTile(pos);
        // }
        else if (color32 == Black)
        {
            GenerateObstacle(pos);
        }

        else if (color32 == Green)
        {
            GenerateDropOut(pos);
        }

        SetCamera();
    }


    private void GenerateYellowBall(Vector3 _position)
    {
        GameObject yellowBallObject = PoolManager.instance.GetObject(PoolManager.NameObject.yellowball);
        var ball = yellowBallObject.GetComponent<Ball>();
        ball.Init(_position);
        ballList.Add(ball);
        maxCount++;
        yellowBallObject.SetActive(true);
    }

    private void GenerateRedBall(Vector3 _position)
    {
        GameObject redBallObject = PoolManager.instance.GetObject(PoolManager.NameObject.redball);
        var ball = redBallObject.GetComponent<Ball>();
        ball.Init(_position);
        ballList.Add(ball);
        redBallObject.SetActive(true);
    }

    private void GenerateDropOut(Vector3 _position)
    {
        GameObject holeObject = PoolManager.instance.GetObject(PoolManager.NameObject.hole);
        _position.y = 0.5f;
        holeObject.transform.position = _position;
    //    holeObject.layer = 8;
        holeObject.tag = "Hole";
        var hole = holeObject.GetComponent<Hole>();
        hole.SetBrickColor(isBlack);
        holeObject.SetActive(true);
    }
    private void GenerateTile(Vector3 _position)
    {
        GameObject tileObject = PoolManager.instance.GetObject(PoolManager.NameObject.tile);
        _position.y = 0.1f;
        tileObject.transform.position = _position;
        tileObject.SetActive(true);
    }

    private void GenerateObstacle(Vector3 _position)
    {
        GameObject obstacleObject = PoolManager.instance.GetObject(PoolManager.NameObject.obstacle);
        _position.y = 1f;
        obstacleObject.transform.position = _position;
        obstacleObject.SetActive(true);
    }

    private void GenerateBrick(Vector3 _position)
    {
        GameObject brickObject = PoolManager.instance.GetObject(PoolManager.NameObject.brick);
        _position.y = 1f;
        brickObject.transform.position = _position;
        var brick = brickObject.GetComponent<Brick>();
        brick.SetBrickColor(isBlack);
       // brickObject.layer = 10;

        brickList.Add(brickObject.transform);
        brickObject.SetActive(true);
    }
    #endregion

    public void SetPoint(int ballPoint)
    {
        point += ballPoint;
        swipeAmountText.text = point.ToString();
    }
    public void CheckComplete()
    {
        levelFill.fillAmount = ((float)count / (float)maxCount);
        if (count >= maxCount)
        {
            Complete();
        }
    }


    public void Complete()
    {
        if (isComplete) return;

        isComplete = true;
        StartCoroutine(C_Complete());
    }

    private IEnumerator C_Complete()
    {
        //AnalyticsManager.instance.CallEvent(AnalyticsManager.EventType.EndEvent);

        for (int i = 0; i < fireWork.Length; i++)
        {
            fireWork[i].Play();
        }
        LevelUp();
        score.text = totalMoney.ToString();
        Debug.Log("Complete");
        yield return new WaitForSeconds(1.25f);

        UIManager.instance.Show_CompleteUI();

    }

    public void Fail()
    {
        if (isComplete) return;

        isComplete = true;
        StartCoroutine(C_Fail());
    }

    private IEnumerator C_Fail()
    {
        //AnalyticsManager.instance.CallEvent(AnalyticsManager.EventType.EndEvent);

        Debug.Log("Fail");

        yield return new WaitForSeconds(1.25f);

        UIManager.instance.Show_FailUI();
    }

    public void RedExplosion(Vector3 _pos)
    {
        return;

        GameObject obj = PoolManager.instance.GetObject(PoolManager.NameObject.redExplosion);
        obj.transform.position = _pos;
        obj.SetActive(true);
    }

    public void YellowExplosion(Vector3 _pos)
    {
        return;

        GameObject obj = PoolManager.instance.GetObject(PoolManager.NameObject.yellowExplosion);
        obj.transform.position = _pos;
        obj.SetActive(true);
    }

    private bool isVibrating;

    public void Vibration()
    {
        if (isVibrating) return;

        StartCoroutine(C_Vibration());
    }

    private IEnumerator C_Vibration()
    {
        Debug.Log("vibration");

        MMVibrationManager.iOSTriggerHaptics(HapticTypes.HeavyImpact);

        isVibrating = true;

        yield return new WaitForSeconds(0.01f);

        isVibrating = false;
    }


    private void SetCameraPos(Texture2D texture)
    {
   //     camPos.z = 9;
 //       Camera.main.fieldOfView = 60;
   //     if (texture.width > 7) Camera.main.fieldOfView = 65;
     //   if (texture.width > 8 && texture.height > 7) Camera.main.fieldOfView = 70;
   //     camPos.x = (texture.width - 5) * 0.5f + 8f;
   //     camPos.z = (texture.height - 3) * 0.5f + 7f;

      //  fireWork[0].transform.parent.transform.position = camPos - new Vector3(9, 0, 9);
     //   pivotCamera.transform.position = camPos;
    }

    public void InHoleEffect(Vector3 _pos)
    {
        Vibration();

        GameObject _obj = PoolManager.instance.GetObject(PoolManager.NameObject.inHoleEffect);
        _obj.transform.position = _pos;
        _obj.SetActive(true);
    }

    public void DisplayCombo(Vector3 _pos)
    {
        _curCombo = combo;

        if (combo % 2 == 2 || combo < 2 || isComboAnimation)
        {
            return;
        }

        _timeCombo = 0.5f;

        comboAnimator.GetComponent<Text>().text = congratulations[Random.Range(0, congratulations.Length)];
        comboAnimator.SetTrigger("ShowUp");
        StartCoroutine(C_DelayCombo());
        //GameObject _obj = PoolManager.instance.GetObject(PoolManager.NameObject.combo);
        //_obj.transform.position = _pos;
        //_obj.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = "Combo x" + combo;
        //_obj.SetActive(true);
        //if (combo >= 5)
        //{
        //    comboText.SetText("Perfect");
        //    comboText.fontSize += Time.deltaTime * 10f;
        //}
        //else
        //    comboText.SetText("Combo x" + combo);

    }

   

    bool isComboAnimation;

    private IEnumerator C_DelayCombo()
    {
        isComboAnimation = true;
        yield return new WaitForSeconds(1.25f);
        isComboAnimation = false;
    }

    public IEnumerator HideCombo()
    {
        yield return new WaitForSeconds(0.25f);
        comboText.text = "";
    }

    public List<GameObject> blankObjects;
    public Vector3[][] blankArray;
    public void GenerateRandomMap(Texture2D texture)
    {
        int blankObjectsIndex = 0;
        for (int i = 0; i < texture.width - 2; i++)
        {
            for (int j = 0; j < texture.height - 2; j++)
            {
                blankArray[i][j] = blankObjects[blankObjectsIndex].transform.position;
                blankObjectsIndex++;
            }
        }

        var iHole = Random.Range(0, texture.width - 2);
        var jHole = Random.Range(0, texture.height - 2);

        int iPlayer = 0;
        int jPlayer = 0;
        bool isSame = true;
        while (isSame)
        {
            iPlayer = Random.Range(0, texture.width - 2);
            jPlayer = Random.Range(0, texture.height - 2);

            if (iPlayer == iHole || jPlayer == jHole) isSame = true;
            else
            {
                isSame = false;
            }
        }

        for (int i = 0; i < texture.width - 2; i++)
        {
            int obstaclePos = Random.Range(0, texture.height - 2);
            for (int j = 0; j < texture.height - 2; j++)
            {
                if (i == iHole && j == jHole)
                {
                    GenerateDropOut(blankArray[i][j]);
                    GenerateBrick(blankArray[i][j]);
                }
                else if (i == iPlayer && j == jPlayer)
                {
                    GenerateRedBall(blankArray[i][j]);
                    GenerateBrick(blankArray[i][j]);
                }
                else if (j == obstaclePos)
                {
                    GenerateObstacle(blankArray[i][j]);
                    GenerateBrick(blankArray[i][j]);
                }
                else
                {
                    GenerateYellowBall(blankArray[i][j]);
                    GenerateBrick(blankArray[i][j]);
                }
                isBlack = !isBlack;
            }
        }
    }

    private void SetCamera()
    {
        cameraMinX = 100;
        cameraMaxX = -100;
        cameraMinY = 100;
        cameraMaxY = -100;

        for(int i = 0; i < brickList.Count; i++)
        {
            if(cameraMinX > brickList[i].position.x)
            {
                cameraMinX = brickList[i].position.x;
            }

            if(cameraMaxX < brickList[i].position.x)
            {
                cameraMaxX = brickList[i].position.x;
            }

            if (cameraMinY > brickList[i].position.z)
            {
                cameraMinY = brickList[i].position.z;
            }

            if (cameraMaxY< brickList[i].position.z)
            {
                cameraMaxY = brickList[i].position.z;
            }
        }

        float _posX = (cameraMinX + cameraMaxX) / 2.0f;
        float _posY = (cameraMinY + cameraMaxY) / 2.0f;
        pivotCamera.transform.position = new Vector3(_posX, 0.0f, _posY);

        _horizontal = cameraMaxX - cameraMinX + 1;
        CameraControl.instance.SetFOVSizeMap();
    }

    private IEnumerator C_Score()
    {
        bool isLoop = true;

        while (isLoop)
        {
            if (scores.Count > 0)
            {
                //if (isComplete)
                //{
                //    scores.Clear();
                //}
                //else
                //{
                scoreAnimatior.SetTrigger("Awake");

                // SpectatorManager.instance.SpectatorAnimation();

                GameObject _score = PoolManager.instance.GetObject(PoolManager.NameObject.scoreup);
                _score.transform.GetChild(0).GetComponent<Text>().text = "+" + scores[0].ToString();
                scores.RemoveAt(0);
                _score.SetActive(true);
                //}
                yield return new WaitForSeconds(0.1f);
            }

            yield return null;
        }
    }
}
