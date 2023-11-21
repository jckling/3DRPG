using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public CharacterStats playerStats;
    private List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();
    private CinemachineFreeLook freeLookCamera;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public void RegisterPlayer(CharacterStats player)
    {
        playerStats = player;
        freeLookCamera = FindObjectOfType<CinemachineFreeLook>();
        if (freeLookCamera != null)
        {
            freeLookCamera.Follow = playerStats.transform.GetChild(2);
            freeLookCamera.LookAt = playerStats.transform.GetChild(2);
        }
    }

    public void AddObserver(IEndGameObserver observer)
    {
        endGameObservers.Add(observer);
    }

    public void RemoveObserver(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
    }

    public void NotifyObservers()
    {
        foreach (var observer in endGameObservers)
        {
            observer.EndNotify();
        }
    }

    public Transform GetEntrance()
    {
        foreach (var item in FindObjectsOfType<TransitionDestination>())
        {
            if (item.destinationTag == TransitionDestination.DestinationTag.ENTER)
            {
                return item.transform;
            }
        }

        return null;
    }
}