using UnityEngine;

public class StackSlot : MonoBehaviour
{
    public bool IsOccupied { get; private set; }

    public void SetOccupied(bool isOccupied)
    {
        IsOccupied = isOccupied;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = IsOccupied ?  Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}
