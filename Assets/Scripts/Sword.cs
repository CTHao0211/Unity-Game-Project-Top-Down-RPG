using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;   // nhớ thêm dòng này

public class Sword : MonoBehaviour
{
    [SerializeField] private GameObject slashAnimPrefab;
    [SerializeField] private Transform slashAnimSpawnPoint;

    private PlayerControls playerControls;
    private Animator myAnimator;
    private PlayerController playerController;
    private ActiveWeapon activeWeapon;

    private GameObject slashAnim;

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        activeWeapon = GetComponentInParent<ActiveWeapon>();
        myAnimator = GetComponent<Animator>();
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        Debug.Log("Sword OnEnable");
        // đăng kí input ở đây luôn cho chắc
        if (playerControls == null)
            playerControls = new PlayerControls();

        playerControls.Combat.Attack.started += OnAttackInput;
        playerControls.Enable();
    }

    private void OnDisable()
    {
        if (playerControls != null)
        {
            playerControls.Combat.Attack.started -= OnAttackInput;
            playerControls.Disable();
        }
    }

    private void OnAttackInput(InputAction.CallbackContext ctx)
    {
        Attack();
    }

    void Start()
    {
        Debug.Log("Sword Start");
        // (có thể bỏ đăng kí ở Start đi cho đỡ rối)
        // playerControls.Combat.Attack.started += _ => Attack();
    }

    private void Update()
    {
        MouseFollowWithOffset();
    }

    private void Attack()
    {
        Debug.Log("Attack() called");

        if (slashAnimPrefab == null)
        {
            Debug.LogError("slashAnimPrefab is NULL!", this);
            return;
        }
        if (slashAnimSpawnPoint == null)
        {
            Debug.LogError("slashAnimSpawnPoint is NULL!", this);
            return;
        }

        myAnimator.SetTrigger("Attack");

        slashAnim = Instantiate(
            slashAnimPrefab,
            slashAnimSpawnPoint.position,
            Quaternion.identity
        );

        Debug.Log("Spawned slash: " + slashAnim.name, slashAnim);
    }

    public void SwingUpFlipAnim()
    {
        if (slashAnim == null)
        {
            Debug.LogWarning("SwingUpFlipAnim called but slashAnim is null");
            return;
        }

        slashAnim.transform.rotation = Quaternion.Euler(-180, 0, 0);

        if (playerController.FacingLeft)
        {
            slashAnim.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    public void SwingDownFlipAnim()
    {
        if (slashAnim == null)
        {
            Debug.LogWarning("SwingDownFlipAnim called but slashAnim is null");
            return;
        }

        slashAnim.transform.rotation = Quaternion.Euler(0, 0, 0);

        if (playerController.FacingLeft)
        {
            slashAnim.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    private void MouseFollowWithOffset()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(playerController.transform.position);

        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;

        if (mousePos.x < playerScreenPoint.x)
        {
            activeWeapon.transform.rotation = Quaternion.Euler(0, -180, angle);
        }
        else
        {
            activeWeapon.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
