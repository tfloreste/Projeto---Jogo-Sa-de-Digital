using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndlessRunnerManager : Singleton<EndlessRunnerManager>, IDataPersistence
{
    [Header("Params")]
    [SerializeField] private float startingSpeed = 5.0f;
    [SerializeField] private float maxSpeed = 20.0f;
    [SerializeField] private float increaseSpeedInterval = 10.0f;
    [SerializeField] private float increaseSpeedStep = 1.0f;

    [Header("Required References")]
    [SerializeField] private FloatVariable moveSpeed;
    [SerializeField] private TextMeshProUGUI startGameText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Image coinImage;
    [SerializeField] private GameObject player;
    [SerializeField] private BoolVariable gameCompletedCondition;
    [SerializeField] private string sceneToGoOnFinish;

    private Collider2D playerCollider;
    private Rigidbody2D playerRigidBody;
    private Camera mainCamera;
    private Plane[] planes;
    private List<GameObject> plataformList;
    private float increaseSpeedTimer;

    public bool gameStarted { get; private set; }
    public bool gameOver { get; private set; }

    private void Awake()
    {
        increaseSpeedTimer = 0;
        moveSpeed.value = 0;
        gameStarted = false;
        plataformList = new List<GameObject>();
    }

    private void Start()
    {
        ScreenEffect.Instance.onFadeCompleted += StartingFadeCompleted;
        ScreenEffect.Instance.FadeIn(false);

        startGameText.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(false); 
        coinImage.gameObject.SetActive(false);  


        mainCamera = Camera.main;
        playerCollider = player.GetComponent<Collider2D>();
        playerRigidBody = player.GetComponent<Rigidbody2D>();
        InitatePlataformList();
    }

    private void Update()
    {
        if (!gameStarted || gameOver)
            return;

        increaseSpeedTimer += Time.deltaTime;
        if(increaseSpeedTimer > increaseSpeedInterval)
        {
            float newSpeed = moveSpeed.value + increaseSpeedStep;
            moveSpeed.value = newSpeed > maxSpeed ? moveSpeed.value : newSpeed;
            increaseSpeedTimer = 0;
        }

        planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);

        if (!IsPlayerInCameraView())
        {
            gameOver = true;
            StartCoroutine(GameOverCO());
        }
    }

    private void FixedUpdate()
    {
        if (!gameStarted || gameOver)
            return;

        if (playerRigidBody.velocity.y < 0.1f && !CanPlayerSurviveFall())
        {
            gameOver = true;
            StartCoroutine(GameOverCO());
        }
    }

    public void AddPlataform(GameObject plataform)
    {
        plataformList.Add(plataform);
        Debug.Log("Added plataform: " + plataformList.Count);
    }

    public void PlataformDestroyed()
    {
        plataformList.RemoveAt(0);
        Debug.Log("Removed plataform: " + plataformList.Count);
    }

    private void StartGame()
    {
        moveSpeed.value = startingSpeed;

        startGameText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(true);
        coinImage.gameObject.SetActive(true);
        gameStarted = true;

        InputManager.OnTouchStart -= StartGame;
    }

    private void StartingFadeCompleted()
    {
        InputManager.OnTouchStart += StartGame;
        ScreenEffect.Instance.onFadeCompleted -= StartingFadeCompleted;
    }

    private void GameOverFadeCompleted()
    {
        gameCompletedCondition.value = true;
        DataPersistenceManager.instance.SaveGame();
        SceneChanger.Instance.ChangeTo(sceneToGoOnFinish);
    }

    private IEnumerator GameOverCO()
    {
        yield return new WaitForSeconds(0.5f);

        moveSpeed.value = 0;
        gameStarted = false;
        ScreenEffect.Instance.FadeOut(true);

        yield return new WaitForSeconds(1.5f);
        GameOverFadeCompleted();

    }

    private bool IsPlayerInCameraView()
    {
        return GeometryUtility.TestPlanesAABB(planes, playerCollider.bounds);
    }

    private void InitatePlataformList()
    {
        GameObject initialPlataform = GameObject.Find("Plataforma_Inicial");

        if (!initialPlataform)
            return;

        //plataformList = new List<GameObject>(initialPlataforms);
        //plataformList.Sort((obj1, obj2) => obj1.transform.position.x.CompareTo(obj2.transform.position.x));
        plataformList = new List<GameObject>();
        AddPlataform(initialPlataform);

        foreach(var plataform in plataformList)
        {
            Debug.Log("Adicionada plataforma: " + plataform.name);
        }
    }

    private bool CanPlayerSurviveFall()
    {
        Debug.Log("------------------CHECKING IF PLAYER CAN SURVIVE ---------------------");
        float playerYPos = player.transform.position.y;
        Debug.Log("playerYPos: " + playerYPos);

        foreach(var plataform in plataformList)
        {
            if (!plataform)
                continue;

            Debug.Log("Plataforma " + plataform.name + " tem " + plataform.transform.childCount + " filhos");
            for(int i = 0; i < plataform.transform.childCount; i++)
            {
                var child = plataform.transform.GetChild(i);
                if (!child.CompareTag("Ground"))
                    continue;

                Debug.Log("child " + child.name + " found on index: " + i);
                Collider2D childCollider = child.GetComponent<Collider2D>();
                if (!childCollider)
                    continue;

                Debug.Log("plataform Collider found at index: " + i);
                Debug.Log("bounds min: " + childCollider.bounds.max);
                Debug.Log("bounds max: " + childCollider.bounds.min);
                Debug.Log("position: " + child.transform.position);
                if (childCollider.bounds.min.y < playerYPos)
                    return true;

            }
        }

        Debug.Log("PlayerCannotSurvive");
        return false;
    }

    public void LoadData(GameData data)
    {
       // Nothing to load
    }

    public void SaveData(GameData data)
    {
        if (data.cutscenesConditions.ContainsKey(gameCompletedCondition.name))
        {
            data.cutscenesConditions[gameCompletedCondition.name] = gameCompletedCondition.value;
        }
        else
        {
            data.cutscenesConditions.Add(gameCompletedCondition.name, gameCompletedCondition.value);
        }
    }
}
