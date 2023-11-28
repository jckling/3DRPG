using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>, IEndGameObserver
{
    public GameObject playerPrefab;
    public SceneFader fadeCanvasPrefab;
    private GameObject player;
    private NavMeshAgent playerAgent;
    private bool fadeFinished;

    #region Event Functions

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        GameManager.Instance.AddObserver(this);
        fadeFinished = true;
    }

    #endregion

    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.transitionType)
        {
            case TransitionPoint.TransitionType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;
            case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                break;
        }
    }

    IEnumerator Transition(string sceneName, TransitionDestination.DestinationTag destinationTag)
    {
        SaveGameData();

        if (SceneManager.GetActiveScene().name != sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return player = Instantiate(playerPrefab, GetDestination(destinationTag).transform.position,
                GetDestination(destinationTag).transform.rotation);

            SaveManager.Instance.LoadPlayerData();
        }
        else
        {
            player = GameManager.Instance.playerStats.gameObject;
            playerAgent = player.GetComponent<NavMeshAgent>();
            playerAgent.enabled = false;

            var destination = GetDestination(destinationTag).transform;
            player.transform.SetPositionAndRotation(destination.position, destination.rotation);
            playerAgent.enabled = true;
            yield return null;
        }
    }

    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)
    {
        var entrances = FindObjectsOfType<TransitionDestination>();
        foreach (var entrance in entrances)
        {
            if (entrance.destinationTag == destinationTag)
            {
                return entrance;
            }
        }

        return null;
    }

    #region Main Menu

    public void TransitionToMain()
    {
        StartCoroutine(LoadMain());
    }

    public void TransitionToLoadGame()
    {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }

    public void TransitionToNewGame()
    {
        StartCoroutine(LoadLevel("Ground"));
    }

    IEnumerator LoadLevel(string scene)
    {
        SceneFader fadeCanvas = Instantiate(fadeCanvasPrefab);
        if (!string.IsNullOrEmpty(scene))
        {
            yield return StartCoroutine(fadeCanvas.FadeOut(3f));
            yield return SceneManager.LoadSceneAsync(scene);
            yield return player = Instantiate(playerPrefab, GameManager.Instance.GetEntrance().position,
                GameManager.Instance.GetEntrance().rotation);

            SaveGameData();
            yield return StartCoroutine(fadeCanvas.FadeIn(2f));
        }
    }

    IEnumerator LoadMain()
    {
        SaveGameData();
        SceneFader fadeCanvas = Instantiate(fadeCanvasPrefab);
        yield return StartCoroutine(fadeCanvas.FadeOut(3f));
        yield return SceneManager.LoadSceneAsync("Main");
        yield return StartCoroutine(fadeCanvas.FadeIn(2f));
    }

    #endregion

    private void SaveGameData()
    {
        SaveManager.Instance.SavePlayerData();
        InventoryManager.Instance.SaveData();
        QuestManager.Instance.SaveQuestData();
    }

    public void EndNotify()
    {
        if (fadeFinished)
        {
            fadeFinished = false;
            StartCoroutine(LoadMain());
        }
    }
}