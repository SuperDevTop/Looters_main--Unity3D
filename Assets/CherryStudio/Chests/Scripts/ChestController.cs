using UnityEngine;
using UnityEngine.Events;

public class ChestController : MonoBehaviour
{
    public bool printDebugInformation = false;
    public UnityEvent onOpen;
    public UnityEvent onClose;

    private bool _isOpen = true;

    private string LogIdentifier => $"Chest {gameObject.name}";

    private void OnMouseDown()
    {
        if (printDebugInformation)
        {
            print($"{LogIdentifier}: Toggling chest state from {GetStateString(_isOpen)} to {GetStateString(!_isOpen)} and executing actions");
        }

        _isOpen = !_isOpen;

        if (!_isOpen)
        {
            print($"{LogIdentifier}: Executing close actions");
            onClose.Invoke();
        }
        else
        {
            print($"{LogIdentifier}: Executing open actions");
            onOpen.Invoke();
        }
    }

    private string GetStateString(bool isOpen)
    {
        return isOpen
            ? "opened"
            : "closed";
    }
}
