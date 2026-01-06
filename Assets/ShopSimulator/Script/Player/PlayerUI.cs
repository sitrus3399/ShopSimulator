using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private PlayerEvents playerEvent;

    [SerializeField] private Joystick moveJoystick;
    [SerializeField] private Joystick lookJoystick;

    [SerializeField] private Button interactButton;

    public Joystick MoveJoystick { get { return moveJoystick; } }
    public Joystick LookJoystick { get { return lookJoystick; } }
    public Button InteractButton { get { return interactButton; } }

    private void Start()
    {
        interactButton.onClick.AddListener(() => { playerEvent.OnInteract?.Invoke(); });
    }

    private void Update()
    {
        Vector3 moveInput = new Vector3(moveJoystick.Horizontal, 0, moveJoystick.Vertical);
        playerEvent.Move(moveInput);

        Vector2 lookInput = new Vector2(lookJoystick.Horizontal, lookJoystick.Vertical);
        playerEvent.Rotate(lookInput);

    }
}