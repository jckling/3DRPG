using UnityEngine;

public class TransitionDestination : MonoBehaviour
{
    public enum DestinationTag
    {
        ENTER,
        A,
        B,
        C
    }
    
    public DestinationTag destinationTag;
}
