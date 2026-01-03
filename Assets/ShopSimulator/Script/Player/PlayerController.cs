using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private PlayerEvents playerEvent;

    private Rigidbody rb;

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
    [SerializeField] private LayerMask itemLayer; // Layer untuk objek Item
    [SerializeField] private GameObject whiteCirclePrefab; // Prefab lingkaran merah
    [SerializeField] private GameObject redCirclePrefab; // Prefab lingkaran merah

    [Header("Item")]
    private GameObject currentItem;
    private Item itemOnHand;
    [SerializeField] private Transform handPoint;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.freezeRotation = true;

        if (playerCamera != null)
            playerCamera.gameObject.tag = "MainCamera";
    }

    private void OnEnable()
    {
        playerEvent.OnMove += UpdateMoveInput;
        playerEvent.OnRotate += UpdateRotateInput;
        playerEvent.OnGrab += GrabItem;
        playerEvent.OnScan += ScanItem;
    }

    private void OnDisable()
    {
        playerEvent.OnMove -= UpdateMoveInput;
        playerEvent.OnRotate -= UpdateRotateInput;
        playerEvent.OnGrab -= GrabItem;
        playerEvent.OnScan -= ScanItem;
    }

    private void Update()
    {
        HandleRotation();
        HandleRaycast();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        //float moveX = playerUI.MoveJoystick.Horizontal;
        //float moveZ = playerUI.MoveJoystick.Vertical;

        Vector3 moveDir = (transform.right * currentMoveInput.x) + (transform.forward * currentMoveInput.z);
        rb.velocity = new Vector3(moveDir.x * moveSpeed, rb.velocity.y, moveDir.z * moveSpeed);

        //Vector3 moveDir = (transform.right * moveX) + (transform.forward * moveZ);

        //rb.velocity = new Vector3(moveDir.x * moveSpeed, rb.velocity.y, moveDir.z * moveSpeed);
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
        if (Physics.Raycast(ray, out hit, raycastDistance, itemLayer))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.red);

            currentItem = hit.collider.gameObject;

            if (whiteCirclePrefab != null)
            {
                whiteCirclePrefab.SetActive(false);
            }

            if (redCirclePrefab != null)
            {
                redCirclePrefab.SetActive(true);
                redCirclePrefab.transform.position = hit.point;
            }
        }
        else if (Physics.Raycast(ray, out hit, raycastDistance, allLayer))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.white);

            ResetGrabTarget();

            if (redCirclePrefab != null)
            {
                redCirclePrefab.SetActive(false);
            }

            if (whiteCirclePrefab != null)
            {
                whiteCirclePrefab.SetActive(true);
                whiteCirclePrefab.transform.position = hit.point;
            }
        }
        else
        {
            ResetGrabTarget();
            if (redCirclePrefab != null) redCirclePrefab.SetActive(false);
            if (whiteCirclePrefab != null) whiteCirclePrefab.SetActive(false);

            Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.black);
        }
    }

    private void ResetGrabTarget()
    {
        currentItem = null;
    }

    public void GrabItem()
    {
        if (currentItem != null)
        {
            //Destroy(currentItem);

            if (itemOnHand == null)
            {
                itemOnHand = currentItem.GetComponent<Item>();
                itemOnHand.ChangeState(ItemState.Take);
                itemOnHand.transform.parent = handPoint;
                itemOnHand.transform.localPosition = new Vector3(0, 0, 0);
            }
            else
            {
                DropItem();
            }
            ResetGrabTarget();
        }
    }

    void DropItem()
    {

    }

    void ScanItem()
    {
        if (!itemOnHand) return;

        itemOnHand.ChangeState(ItemState.Scan);
    }
}