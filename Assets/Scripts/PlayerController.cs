using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    // ���ǵ� ���� ����
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float crouchSpeed;
    private float applySpeed;   
    [SerializeField] private float jumpForce;

    // ���� ����
    private bool isWalk = false;
    private bool isRun = false;
    private bool isCrouch = false;
    private bool isGround = true;

    // ������ üũ ����
    private Vector3 lastPos;

    // �ɾ��� �� �󸶳� ������ �����ϴ� ����
    [SerializeField]
    private float crouchPosY;
    private float originPosY;
    private float applyCrouchPosY;

    // ī�޶� �ΰ���
    [SerializeField, Range(1, 10)] private float lookSensitivity;

    // ī�޶� �Ѱ�
    [SerializeField] private float cameraRotationLimit;
    private float currentCameraRotationX;

    // ������Ʈ
    [SerializeField] Camera theCamera;
    private Rigidbody myRigid;
    private CapsuleCollider capsuleCollider;
    private GunController theGunController;
    private Crosshair theCrosshair;

    // Start is called before the first frame update
    void Start()
    {
        myRigid = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        theGunController = FindObjectOfType<GunController>();
        theCrosshair = FindObjectOfType<Crosshair>();

        // �ʱ�ȭ
        applySpeed = walkSpeed;
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;
    }

    // Update is called once per frame
    void Update()
    {
        IsGround();
        TryJump();
        TryRun();
        TryCrounch();
        Move();
        MoveCheck();
        CameraRotation();
        CharacterRotation();
    }

    // �ɱ� �õ�
    private void TryCrounch() {
        if (Input.GetKeyDown(KeyCode.LeftControl)) {
            Crouch();
        }
    }

    // �ɱ� ����
    private void Crouch() {
        isCrouch = !isCrouch;
        theCrosshair.CrouchingAnimation(isCrouch);

        if (isCrouch) {
            applySpeed = crouchSpeed;
            applyCrouchPosY = crouchPosY;
        }
        else {
            applySpeed = walkSpeed;
            applyCrouchPosY = originPosY;
        }

        StartCoroutine(CrouchCoroutine());
    }

    // �ε巯�� �ɱ� ����
    IEnumerator CrouchCoroutine() {
        float _posY = theCamera.transform.localPosition.y;
        int count = 0;

        while(_posY != applyCrouchPosY) {
            count++;
            _posY = Mathf.Lerp(_posY, applyCrouchPosY, 0.1f);
            theCamera.transform.localPosition = new Vector3(0f, _posY, 0f);
            if (count > 30) { break; }
            yield return null;
        }

        theCamera.transform.localPosition = new Vector3(0f, applyCrouchPosY, 0f);
    }

    // ���� üũ
    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
        theCrosshair.RunningAnimation(!isGround);
    }

    // ���� �õ�
    private void TryJump() {
        if (Input.GetKeyDown(KeyCode.Space) && isGround) {
            jump();
        }
    }
    
    // ����
    private void jump() {
        if (isCrouch) {
            Crouch();
        }
        myRigid.velocity = transform.up * jumpForce;
    }

    // �޸��� �õ�
    private void TryRun() {
        if (Input.GetKey(KeyCode.LeftShift)) {
            Running();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift)) {
            RunningCancle();
        }
    }

    // �޸��� ����
    private void Running() {
        if (isCrouch) {
            Crouch();
        }
        theGunController.CancelFineSight();
        isRun = true;
        theCrosshair.RunningAnimation(isRun);
        applySpeed = runSpeed;
    }

    // �޸��� ���
    private void RunningCancle() {
        isRun = false;
        applySpeed = walkSpeed;
        theCrosshair.RunningAnimation(isRun);
    }

    // ������ ����
    private void Move() {
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;

        // #1 unity 2019 ver code. transform & rigidbody transform unmatched. fix needed.
        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
        // #1 fix
        transform.position = myRigid.position;
    }

    // ������ üũ
    private void MoveCheck()
    {
        if (!isRun && !isCrouch && isGround)
        {
            if(Vector3.Distance(lastPos, transform.position) >= 0.01f)
            {
                isWalk = true;
            }
            else
            {
                isWalk = false;
            }

            theCrosshair.WalkingAnimation(isWalk);
            lastPos = transform.position;
        }
    }

    // �¿� ĳ���� ȸ��
    private void CharacterRotation() {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
    }

    // ���� ĳ���� ȸ��
    private void CameraRotation() {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

}
