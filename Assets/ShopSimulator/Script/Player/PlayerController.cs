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

    [Header("Payment")]
    [SerializeField] private Transform edcPoint;
    [SerializeField] private GameObject currentPayment;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.freezeRotation = true;

        if (playerCamera != null)
            playerCamera.gameObject.tag = "MainCamera";

        playerState = PlayerState.Idle;
    }

    private void OnEnable()
    {
        playerEvent.OnMove += UpdateMoveInput;
        playerEvent.OnRotate += UpdateRotateInput;
        playerEvent.OnUseEDC += UseEDC;
    }

    private void OnDisable()
    {
        playerEvent.OnMove -= UpdateMoveInput;
        playerEvent.OnRotate -= UpdateRotateInput;
        playerEvent.OnInteract -= ScanItem;
        playerEvent.OnUseEDC -= UseEDC;
    }

    void UseEDC(EDCMachine edcMachine)
    {
        edcMachine.gameObject.transform.parent = edcPoint.transform;
        edcMachine.gameObject.transform.localPosition = Vector3.zero;
        edcMachine.gameObject.transform.rotation = edcPoint.rotation;
    }

    private void Update()
    {
        HandleRotation();
        HandleRaycast();
    }

    private void FixedUpdate()
    {
        if (playerState == PlayerState.Idle)
        {
            HandleMovement();
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

        if (Physics.Raycast(ray, out hit, raycastDistance, allLayer))
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
        if (layer == LayerMask.NameToLayer("Item"))
        {
            currentItem = obj;
            playerEvent.OnInteract += ScanItem;
            SetUIState(true, false);
        }
        else if (layer == LayerMask.NameToLayer("Tool"))
        {
            playerEvent.OnInteract += GrabItem;
            SetUIState(true, false);
        }
        else if (layer == LayerMask.NameToLayer("Stain"))
        {
            playerEvent.OnInteract += CleanStain;
            SetUIState(true, false);
        }
        else if (layer == LayerMask.NameToLayer("Payment"))
        {
            currentPayment = obj;

            playerEvent.OnInteract += TakePayment;
            SetUIState(true, false);
        }
        else
        {
            SetUIState(false, true);
        }
    }

    private void ClearPreviousTarget()
    {
        if (currentTarget == null) return;

        playerEvent.OnInteract -= ScanItem;
        playerEvent.OnInteract -= GrabItem;
        playerEvent.OnInteract -= CleanStain;
        playerEvent.OnInteract -= TakePayment;

        currentTarget = null;
        ResetGrabTarget();
        ResetPaymentTarget();
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

    void GrabItem()
    {

    }

    void CleanStain()
    {

    }

    void ScanItem()
    {
        if (!currentItem) return;

        itemOnHand = currentItem.GetComponent<Item>();

        if (!itemOnHand) return;

        itemOnHand.ChangeState(ItemState.Scan);
    }

    void TakePayment()
    {
        if (currentPayment != null)
        {
            Payment tmpPayment = currentPayment.GetComponent<Payment>();
            storeEvent.TakePayment(tmpPayment.Price, tmpPayment.PaymentType);
        }
    }
}

[System.Serializable]
public enum PlayerState
{ 
    Idle,
    Cashier,
    Cleaning,
}