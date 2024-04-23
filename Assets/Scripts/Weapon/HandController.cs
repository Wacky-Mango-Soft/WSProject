using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : CloseWeaponController
{
    // 활성화 여부
    public static bool isActivate = true;
    public static Item currentKit; // 설치하려는 킷 (연금 테이블)

    private bool isPreview = false;

    private GameObject go_preview; // 설치할 키트 프리뷰
    private Vector3 previewPos; // 설치할 키트 위치
    [SerializeField] private float rangeAdd; // 건축시 추가 사정거리

    [SerializeField]
    private QuickSlotController theQuickSlot;
    [SerializeField]
    private ItemEffectDatabase theItemEffectDatabase;
    private float directAnimWeight = 1; // 애니메이션 블렌딩을 위한 멤버 변수

    private void Start()
    {
        WeaponManager.currentWeapon = currentCloseWeapon.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentCloseWeapon.anim;
        //thePlayerController = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        if (isActivate && GameManager.instance.canPlayerMove)
        {
            if (currentKit == null)
            {
                if (QuickSlotController.go_HandItem == null) {
                    TryAttack();
                }
                else {
                    TryEating();
                }
            }
            else
            {
                if(!isPreview)
                {
                    InstallPreviewKit();
                }
                PreviewPositionUpdate();
                Build();
            }
        }
    }

    private void InstallPreviewKit()
    {
        isPreview = true;
        go_preview = Instantiate(currentKit.kitPreviewPrefab, transform.position, Quaternion.identity);
    }

    private void PreviewPositionUpdate()
    {
        Debug.Log("PreviewPositionUpdate");
        if(Physics.Raycast(transform.position, transform.forward, out hitInfo, currentCloseWeapon.range + rangeAdd, layerMask))
        {
            previewPos = hitInfo.point;
            go_preview.transform.position = previewPos;
        }
    }

    private void Build()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (go_preview.GetComponent<PreviewObject>().IsBuildable())
            {
                theQuickSlot.DecreaseSelectedItem(); // 슬롯 아이템 개수 -1;
                GameObject temp = Instantiate(currentKit.kitPrefab, previewPos, Quaternion.identity);
                temp.name = currentKit.itemName;
                Destroy(go_preview);
                currentKit = null;
                isPreview = false;
            }
        }
    }

    public void Cancel()
    {
        Destroy(go_preview);
        currentKit = null;
        isPreview = false;
    }


    private void TryEating()
    {
        if (Input.GetButtonDown("Fire2") && !theQuickSlot.GetIsCoolTime())
        {
            WeaponManager.thePlayerAnimator.Animator.SetLayerWeight(1, 1);
            WeaponManager.thePlayerAnimator.Animator.SetTrigger("Eat");
            StartCoroutine(AnimationLayerBlendingCoroutine());
            SoundManager.instance.PlaySE("Eat");
            //#0 아이템 효과 사용
            theItemEffectDatabase.UseItem(theQuickSlot.GetSelectedSlot().item);
            theQuickSlot.DecreaseSelectedItem();
        }
    }

    // 자연스러운 아바타 마스크 블랜딩 기능
    private IEnumerator AnimationLayerBlendingCoroutine() {
        while (directAnimWeight > 0) {
            if (WeaponManager.thePlayerAnimator.Animator.GetCurrentAnimatorStateInfo(1).normalizedTime > 0.5f) {
                directAnimWeight -= Time.deltaTime;
                WeaponManager.thePlayerAnimator.Animator.SetLayerWeight(1, directAnimWeight);
            }
            yield return null;
        }
        WeaponManager.thePlayerAnimator.Animator.SetLayerWeight(1, 0);
        directAnimWeight = 1f;
    }

    protected override IEnumerator HitCoroutine()
    {
        while (isSwing)
        {
            if (CheckObject())
            {
                isSwing = false;
                //Debug.Log(hitInfo.transform.name);
            }
            yield return null;
        }
    }

    public override void CloseWeaponChange(CloseWeapon _CloseWeapon)
    { 
        base.CloseWeaponChange(_CloseWeapon);
        isActivate = true;
    }
}
