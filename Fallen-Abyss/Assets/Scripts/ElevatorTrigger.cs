using UnityEngine;

public class ElevatorTrigger : MonoBehaviour
{
    public Elevator2D_Controller elevator;

    private bool triggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            elevator.MoveElevator();
        }
    }
}
