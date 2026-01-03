using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private PlayerEvents playerEvent;

    [SerializeField] private Joystick moveJoystick;
    [SerializeField] private Joystick lookJoystick;

    [SerializeField] private Button grabButton;
    [SerializeField] private Button scanButton;

    public Joystick MoveJoystick { get { return moveJoystick; } }
    public Joystick LookJoystick { get { return lookJoystick; } }
    public Button GrabButton { get { return grabButton; } }
    public Button ScanButton { get { return scanButton; } }

    private void Start()
    {
        grabButton.onClick.AddListener(() => { playerEvent.OnGrab?.Invoke(); });
        scanButton.onClick.AddListener(() => { playerEvent.OnScan?.Invoke(); });
    }

    private void Update()
    {
        Vector3 moveInput = new Vector3(moveJoystick.Horizontal, 0, moveJoystick.Vertical);
        playerEvent.Move(moveInput);

        Vector2 lookInput = new Vector2(lookJoystick.Horizontal, lookJoystick.Vertical);
        playerEvent.Rotate(lookInput);
    }
}