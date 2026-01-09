using System.ComponentModel;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private PlayerEvents playerEvent;
    [SerializeField] private StoreEvents storeEvent;

    private Rigidbody rb;
    private PlayerState playerState;

    [Header("Movement Settings")]
    private Vector3 currentMoveInput;
    private Vector2 currentLookInput;
    private float xRotation = 0f;

    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private float verticalLookLimitTop = 80f;
    [SerializeField] private float verticalLookLimitBottom = 80f;
    
    [Header("Raycast Settings")]
    [SerializeField] private float raycastDistance = 10f; // Jarak raycast
    [SerializeField] private LayerMask allLayer; // Layer untuk objek Item
    [SerializeField] private GameObject whiteCirclePrefab; // Prefab lingkaran merah
    [SerializeField] private GameObject redCirclePrefab; // Prefab lingkaran merah

    [Header("Item")]
    [SerializeField] private GameObject currentItem;
    [SerializeField] private GameObject currentTarget;
    private Item itemOnHand;
    [SerializeField] private Transform handPoint;
    [SerializeField] private Transform frontPoint;

    public Transform FrontPoint { get { return frontPoint; } }

    [Header("Payment")]
    [SerializeField] private Transform edcPoint;
    [SerializeField] private GameObject currentPayment;
    [SerializeField] private GameObject currentCashChange;

    [Header("Tool")]
    [SerializeField] private ToolShop currentTool;
    [SerializeField] private LayerMask caseLayer;

    [Header("Platform")]
    bool isMobile;
    [SerializeField] private float mouseSensitivity;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.freezeRotation = true;

        if (playerCamera != null)
            playerCamera.gameObject.tag = "MainCamera";

        playerState = PlayerState.Idle;

        SetupPlatform();
    }

    private void OnEnable()
    {
        playerEvent.OnMove += UpdateMoveInput;
        playerEvent.OnRotate += UpdateRotateInput;
        playerEvent.OnUseEDC += UseEDC;
        storeEvent.OnExitEDC += ExitEDC;
    }

    private void OnDisable()
    {
        playerEvent.OnMove -= UpdateMoveInput;
        playerEvent.OnRotate -= UpdateRotateInput;
        playerEvent.OnInteract -= ScanItem;
        playerEvent.OnUseEDC -= UseEDC;
        storeEvent.OnExitEDC -= ExitEDC;
    }

    private void SetupPlatform()
    {
#if UNITY_ANDROID || UNITY_IOS
        isMobile = true;
#else
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isMobile = false;
#endif
    }

    void UseEDC(EDCMachine edcMachine)
    {
        edcMachine.gameObject.transform.parent = edcPoint.transform;
        edcMachine.gameObject.transform.localPosition = Vector3.zero;
        edcMachine.gameObject.transform.rotation = edcPoint.rotation;

        SetCursorLock(false);
    }

    void ExitEDC()
    {
        SetCursorLock(true);
    }

    private void Update()
    {
        if (isMobile)
        {
            HandleRotation();
        }
        else if (!isMobile && Cursor.lockState == CursorLockMode.Locked)
        {
            HandleInputPerPlatform();
            HandleMouseInteraction();
        }
        
        HandleRaycast();
    }

    private void FixedUpdate()
    {
        if (playerState == PlayerState.Idle)
        {
            HandleMovement();
        }
    }

    void HandleMouseInteraction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            playerEvent.OnInteract?.Invoke();
        }
    }

    void HandleInputPerPlatform()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        currentMoveInput = new Vector3(h, 0, v);

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        currentLookInput = new Vector2(mouseX, mouseY);

        float rotateY = currentLookInput.x;

        transform.Rotate(Vector3.up * rotateY);
        float rotateX = currentLookInput.y;

        xRotation -= rotateX;
        xRotation = Mathf.Clamp(xRotation, verticalLookLimitBottom, verticalLookLimitTop);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void SetCursorLock(bool isLock)
    {
        if (!isMobile)
        {
            if (isLock)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    private void HandleMovement()
    {
        Vector3 moveDir = (transform.right * currentMoveInput.x) + (transform.forward * currentMoveInput.z);
        rb.velocity = new Vector3(moveDir.x * moveSpeed, rb.velocity.y, moveDir.z * moveSpeed);
    }

    private void UpdateMoveInput(Vector3 input)
    {
        currentMoveInput = input;
    }

    private void HandleRotation()
    {
        float rotateY = currentLookInput.x * lookSensitivity;
        transform.Rotate(Vector3.up * rotateY);

        float rotateX = currentLookInput.y * lookSensitivity;
        xRotation -= rotateX;
        xRotation = Mathf.Clamp(xRotation, verticalLookLimitBottom, verticalLookLimitTop);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void UpdateRotateInput(Vector2 input)
    {
        currentLookInput = input;
    }

    private void HandleRaycast()
    {
        RaycastHit hit;
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        LayerMask maskToUse = (currentTool == null)
            ? allLayer
            : (currentTool.TargetLayer | caseLayer);

        if (Physics.Raycast(ray, out hit, raycastDistance, allLayer))
        {
            if (playerState == PlayerState.Idle || playerState == PlayerState.Cashier)
            {
                GameObject hitObject = hit.collider.gameObject;

                if (hitObject != currentTarget)
                {
                    ClearPreviousTarget();
                    currentTarget = hitObject;
                    SetupNewTarget(hitObject, hit.collider.gameObject.layer);
                }

                UpdateUIPosition(hit.point);
            }
        }
        else
        {
            if (currentTarget != null)
            {
                ClearPreviousTarget();
            }
            ResetRaycastVisuals();
        }
    }

    private void SetupNewTarget(GameObject obj, int layer)
    {
        if (currentTool == null)
        {
            Debug.Log($"Not Have tool {layer}");
            if (layer == LayerMask.NameToLayer("Item"))
            {
                currentItem = obj;
                playerEvent.OnInteract += ScanItem;
                SetUIState(true, false);
            }
            else if (layer == LayerMask.NameToLayer("Tool"))
            {
                playerEvent.OnInteract += GrabTool;
                SetUIState(true, false);
            }
            else if (layer == LayerMask.NameToLayer("Case"))
            {
                playerEvent.OnInteract += InteractCase;
                SetUIState(true, false);
            }
            else if (layer == LayerMask.NameToLayer("Payment"))
            {
                currentPayment = obj;

                playerEvent.OnInteract += TakePayment;
                SetUIState(true, false);
            }
            else if (layer == LayerMask.NameToLayer("CashChange"))
            {
                currentCashChange = obj;

                playerEvent.OnInteract += TakeCashChange;
                SetUIState(true, false);
            }
            else
            {
                SetUIState(false, true);
            }
        }
        else
        {
            Debug.Log($"Have tool {layer}");
            if (layer == LayerMask.NameToLayer("Case"))
            {
                playerEvent.OnInteract += InteractCase;
                SetUIState(true, false);
            }
            else if (layer == LayerMask.NameToLayer("Stain"))
            {
                if (currentTool.CompareTag("CardBox"))
                {
                    playerEvent.OnInteract += DropTool;
                }
                playerEvent.OnInteract += useTool;
                SetUIState(true, false);
            }
            else if (layer == LayerMask.NameToLayer("Container"))
            {
                playerEvent.OnInteract += useTool;
                SetUIState(true, false);
            }
            else
            {
                if (currentTool.CompareTag("CardBox"))
                {
                    playerEvent.OnInteract += DropTool;
                }

                SetUIState(false, true);
            }
        }
    }

    private void ClearPreviousTarget()
    {
        if (currentTarget == null) return;

        playerEvent.OnInteract -= ScanItem;
        playerEvent.OnInteract -= GrabTool;
        playerEvent.OnInteract -= DropTool;
        playerEvent.OnInteract -= useTool;
        playerEvent.OnInteract -= TakePayment;
        playerEvent.OnInteract -= TakeCashChange;
        playerEvent.OnInteract -= InteractCase;

        currentTarget = null;
        ResetGrabTarget();
        ResetPaymentTarget();
        ResetCashChangeTargetTarget();
    }

    private void SetUIState(bool redActive, bool whiteActive)
    {
        if (redCirclePrefab != null) redCirclePrefab.SetActive(redActive);
        if (whiteCirclePrefab != null) whiteCirclePrefab.SetActive(whiteActive);
    }

    private void UpdateUIPosition(Vector3 position)
    {
        if (redCirclePrefab != null && redCirclePrefab.activeSelf)
        {
            redCirclePrefab.transform.position = position;
        }

        if (whiteCirclePrefab != null && whiteCirclePrefab.activeSelf)
        {
            whiteCirclePrefab.transform.position = position;
        } 
    }

    private void ResetRaycastVisuals()
    {
        SetUIState(false, false);
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * raycastDistance, Color.black);
    }

    private void ResetGrabTarget()
    {
        currentItem = null;
    }

    private void ResetPaymentTarget()
    {
        currentPayment = null;
    }
    
    private void ResetCashChangeTargetTarget()
    {
        currentCashChange = null;
    }

    void GrabTool()
    {
        currentTool = currentTarget.GetComponent<ToolShop>();
        currentTool.Grab(handPoint);
    }

    public void RemoveTool()
    {
        currentTool = null;
    }

    void InteractCase()
    {
        Case newCase = currentTarget.GetComponent<Case>();
        newCase.InteractCase(this, currentTool);
    }

    public void GrabToolFromCase(ToolShop tool)
    {
        if (currentTool != null)
        {
            AudioManager.Instance.PlaySFX("Click");
            DropTool();
            RemoveTool();
        }

        currentTool = tool;
        currentTool.Grab(handPoint);
    }

    void DropTool()
    {
        if (currentTool != null)
        {
            AudioManager.Instance.PlaySFX("Click");
            currentTool.Drop();
            RemoveTool();
        }
    }

    void useTool()
    {
        if (currentTool != null)
        {
            AudioManager.Instance.PlaySFX("Click");
            currentTool.Use(currentTarget);
        }
    }

    void ScanItem()
    {
        if (!currentItem) return;

        itemOnHand = currentItem.GetComponent<Item>();

        if (!itemOnHand) return;

        AudioManager.Instance.PlaySFX("Scan");
        itemOnHand.ChangeState(ItemState.Scan);
    }

    void TakePayment()
    {
        if (currentPayment != null)
        {
            AudioManager.Instance.PlaySFX("Money");
            Payment tmpPayment = currentPayment.GetComponent<Payment>();
            storeEvent.TakePayment(tmpPayment.Price, tmpPayment.PaymentType);
        }
    }

    void TakeCashChange()
    {
        if (currentCashChange != null)
        {
            AudioManager.Instance.PlaySFX("Money");
            CashChange tmpCashChange = currentCashChange.GetComponent<CashChange>();
            tmpCashChange.Take();
        }
    }
}

[System.Serializable]
public enum PlayerState
{ 
    Idle,
    Cashier,
    Cleaning,
    UsingTool,
}