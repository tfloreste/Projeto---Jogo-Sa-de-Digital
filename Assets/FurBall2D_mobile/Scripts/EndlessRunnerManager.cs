using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class EndlessRunnerManager : Singleton<EndlessRunnerManager>, IDataPersistence
{
    [Header("Params")]
    [SerializeField] private float startingSpeed = 5.0f;
    [SerializeField] private float maxSpeed = 20.0f;
    [SerializeField] private float increaseSpeedInterval = 10.0f;
    [SerializeField] private float increaseSpeedStep = 1.0f;
    [SerializeField] private float notInCameraViewTotalTime = 1.0f;

    [Header("Required References")]
    [SerializeField] private FloatVariable moveSpeed;
    [SerializeField] private TextMeshProUGUI startGameText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Image coinImage;
    [SerializeField] private GameObject player;
    [SerializeField] private BoolVariable gameCompletedCondition;
    [SerializeField] private string sceneToGoOnFinish;

    public UnityAction onGameOver;

    private Collider2D playerCollider;
    private Rigidbody2D playerRigidBody;
    private Camera mainCamera;
    private Plane[] planes;
    private List<GameObject> plataformList;
    private float increaseSpeedTimer;
    private float currentTimeNotInCameraView = 0.0f;

    private const string pointsToWinVariableName = "endless_runner_points_to_win";
    private const string playerPointsVariableName = "endless_runner_points";
    private const string playerWonVariableName = "player_won_endless_runner_game";
    
    public int pointsToWin { get; private set; } = 30;
    public bool gameStarted { get; private set; }
    public bool gameOver { get; private set; }

    private void Awake()
    {
        Debug.Log("EndlessRunnerManager awake fired");
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

        pointsToWin = DialogueManager.Instance.GetDialogueVariable<int>(pointsToWinVariableName);
        currentTimeNotInCameraView = 0.0f;
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
            currentTimeNotInCameraView += Time.deltaTime;

            if(currentTimeNotInCameraView > notInCameraViewTotalTime)
            {
                gameOver = true;
                StartCoroutine(GameOverCO());
            }
        }
        else
        {
            currentTimeNotInCameraView = 0.0f;
        }
    }

    private void FixedUpdate()
    {
        if (!gameStarted || gameOver)
            return;

        if (playerRigidBody.velocity.y < 0.15f && !CanPlayerSurviveFall())
        {
            gameOver = true;
            StartCoroutine(GameOverCO());
        }
    }

    public void AddPlataform(GameObject plataform)
    {
        plataformList.Add(plataform);
    }

    public void PlataformDestroyed()
    {
        plataformList.RemoveAt(0);
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
        gameCompletedCondition.Value = true;
        SceneChanger.Instance.ChangeTo(sceneToGoOnFinish);
    }

    private IEnumerator GameOverCO()
    {
        onGameOver?.Invoke();

        yield return new WaitForSeconds(0.5f);

        moveSpeed.value = 0;
        gameStarted = false;
        ScreenEffect.Instance.FadeOut(true);

        int playerScore = ScoreManager.GetInstance().GetCurrentScore();
        bool playerWon = playerScore >= pointsToWin;


        DialogueManager.Instance.SetDialogueVariable(playerPointsVariableName, playerScore);
        DialogueManager.Instance.SetDialogueVariable(playerWonVariableName, playerWon);
        DataPersistenceManager.Instance.SaveGame();

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
        float playerYPos = player.transform.position.y;

        foreach(var plataform in plataformList)
        {
            if (!plataform)
                continue;

            for(int i = 0; i < plataform.transform.childCount; i++)
            {
                var child = plataform.transform.GetChild(i);
                if (!child.CompareTag("Ground"))
                    continue;

                Collider2D childCollider = child.GetComponent<Collider2D>();
                if (!childCollider)
                    continue; 

                child.GetComponent<SpriteRenderer>().color = Color.red;
                if (childCollider.bounds.min.y < playerYPos)
                    return true;
            }
        }

        return false;
    }

    public void LoadData(GameData data)
    {
       // Nothing to load
    }

    public void SaveData(GameData data)
    {
        if (data == null || data.conditions == null)
            return;

        if (data.conditions.ContainsKey(gameCompletedCondition.name))
        {
            data.conditions[gameCompletedCondition.name] = gameCompletedCondition.Value;
        }
        else
        {
            data.conditions.Add(gameCompletedCondition.name, gameCompletedCondition.Value);
        }
    }
}
