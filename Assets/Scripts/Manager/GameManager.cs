using System.Collections.Generic;

public class GameManager : Singleton<GameManager>
{
    public CharacterStats playerStats;
    private List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();

    public void RegisterPlayer(CharacterStats player)
    {
        playerStats = player;
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
}