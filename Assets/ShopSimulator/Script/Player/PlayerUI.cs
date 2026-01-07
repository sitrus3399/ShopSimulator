using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Mobile UI")]
    [SerializeField] private GameObject mobileUI;
    [SerializeField] private PlayerEvents playerEvent;

    [SerializeField] private Joystick moveJoystick;
    [SerializeField] private Joystick lookJoystick;

    [SerializeField] private Button interactButton;

    [SerializeField] private TMP_Text currencyText;

    public Joystick MoveJoystick { get { return moveJoystick; } }
    public Joystick LookJoystick { get { return lookJoystick; } }
    public Button InteractButton { get { return interactButton; } }

    private void Awake()
    {
#if UNITY_ANDROID || UNITY_IOS
        mobileUI.SetActive(true);
#else
        mobileUI.SetActive(false);
#endif
    }

    private void Start()
    {
        interactButton.onClick.AddListener(() => { playerEvent.OnInteract?.Invoke(); });
        playerEvent.OnUpdateCurrencyUI += UpdateCurrencyUI;
    }

    private void Update()
    {
        Vector3 moveInput = new Vector3(moveJoystick.Horizontal, 0, moveJoystick.Vertical);
        playerEvent.Move(moveInput);

        Vector2 lookInput = new Vector2(lookJoystick.Horizontal, lookJoystick.Vertical);
        playerEvent.Rotate(lookInput);
    }

    void UpdateCurrencyUI(float value)
    {
        currencyText.text = $"${value:F2}";
    }
}