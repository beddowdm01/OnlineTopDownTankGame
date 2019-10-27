using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField]
    private bool overlapped = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        overlapped = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        overlapped = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        overlapped = false;
    }
    
    public bool GetOverlapped()
    {
        return overlapped;
    }
}
