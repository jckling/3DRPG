using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private Button newGameButton;
    private Button continueGameButton;
    private Button exitGameButton;
    private PlayableDirector director;

    private void Awake()
    {
        newGameButton = transform.GetChild(1).GetComponent<Button>();
        continueGameButton = transform.GetChild(2).GetComponent<Button>();
        exitGameButton = transform.GetChild(3).GetComponent<Button>();

        newGameButton.onClick.AddListener(PlayTimeline);
        continueGameButton.onClick.AddListener(ContinueGame);
        exitGameButton.onClick.AddListener(ExitGame);

        director = FindObjectOfType<PlayableDirector>();
        director.stopped += NewGame;
    }

    private void PlayTimeline()
    {
        director.Play();
    }

    private void NewGame(PlayableDirector playableDirector)
    {
        PlayerPrefs.DeleteAll();
        SceneController.Instance.TransitionToNewGame();
    }

    private void ContinueGame()
    {
        SceneController.Instance.TransitionToLoadGame();
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}