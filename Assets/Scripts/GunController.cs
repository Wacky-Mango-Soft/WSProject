using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    // ���� ������ ��
    [SerializeField] private Gun currentGun;

    // ���� �ӵ� ���
    private float currentFireRate;

    // ���� ����
    private bool isReload = false;
    private bool isFineSightMode = false;

    // ���� ������ ��
    private Vector3 originPos;

    // ȿ���� ���
    private AudioSource audioSource;

    // �ݵ� ���� ����
    Vector3 recoilBack;
    Vector3 retroActionRecoilBack;

    // ������ �浹 ���� �޾ƿ� ����
    private RaycastHit hitInfo;

    // �ʿ��� ������Ʈ
    [SerializeField] private Camera theCam;
    private Crosshair theCrosshair;

    // �ǰ� ����Ʈ
    [SerializeField] private GameObject hitEffectPrefab;

    private void Start()
    {
        originPos = Vector3.zero;
        recoilBack = new Vector3(currentGun.retroActionForce, originPos.y, originPos.z);
        retroActionRecoilBack = new Vector3(currentGun.retroActionFineSightForce, currentGun.fineSightOriginPos.y, currentGun.fineSightOriginPos.z);
        audioSource = GetComponent<AudioSource>();
        theCrosshair = FindObjectOfType<Crosshair>();
    }

    void Update()
    {
        GunFireRateCalc();
        TryFire();
        TryReload();
        TryFineSight();
    }

    // ����ӵ� ����
    private void GunFireRateCalc() {
        if (currentFireRate > 0) {
            currentFireRate -= Time.deltaTime;
        }
    }

    // �߻� �õ�
    private void TryFire() {
        if (Input.GetButton("Fire1") && currentFireRate <= 0 && !isReload) {
            Fire();
        }
    }

    // �߻� �� ���
    private void Fire() {
        if (!isReload)
        {
            if (currentGun.currentBulletCount > 0) {
                Shoot();
            } else {
                CancelFineSight();
                StartCoroutine(ReloadCoroutine());
            }
        }
    }

    // �߻� �� ���
    private void Shoot()
    {
        theCrosshair.FireAnimation();
        currentGun.currentBulletCount--;
        currentFireRate = currentGun.fireRate; // ���� �ӵ� ����
        PlaySE(currentGun.fire_Sound);
        currentGun.muzzleFlash.Play();
        Hit();
        StopAllCoroutines();
        StartCoroutine(RetroActionCoroutine());
    }

    // �߻� �� ó��
    private void Hit()
    {
        if(Physics.Raycast(theCam.transform.position, theCam.transform.forward + 
            new Vector3(UnityEngine.Random.Range(-theCrosshair.GetAccuracy() - currentGun.accuracy, theCrosshair.GetAccuracy() + currentGun.accuracy),
                        UnityEngine.Random.Range(-theCrosshair.GetAccuracy() - currentGun.accuracy, theCrosshair.GetAccuracy() + currentGun.accuracy),
                        0)
            , out hitInfo, currentGun.range)) {
            GameObject clone = Instantiate(hitEffectPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            Destroy(clone, 2f);
        }
    }

    // ������ �õ�
    private void TryReload()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReload && currentGun.currentBulletCount < currentGun.reloadBulletCount)
        {
            CancelFineSight();
            StartCoroutine(ReloadCoroutine());
        }
    }

    // ������ ó��
    IEnumerator ReloadCoroutine() {
        if (currentGun.carryBulletCount > 0) {
            isReload = true;
            currentGun.anim.SetTrigger("Reload");

            currentGun.carryBulletCount += currentGun.currentBulletCount;
            currentGun.currentBulletCount = 0;

            yield return new WaitForSeconds(currentGun.reloadTime);

            if (currentGun.carryBulletCount >= currentGun.reloadBulletCount) {
                currentGun.currentBulletCount = currentGun.reloadBulletCount;
                currentGun.carryBulletCount -= currentGun.reloadBulletCount;
            }
            else {
                currentGun.currentBulletCount = currentGun.carryBulletCount;
                currentGun.carryBulletCount = 0;
            }
            isReload = false;
        } 
        else
        {
            Debug.Log("carry bullet count = 0");
        }
    }

    // ������ �õ�
    private void TryFineSight()
    {
        if (Input.GetButtonDown("Fire2") && !isReload)
        {
            FineSight();
        }
        if (Input.GetButtonUp("Fire2") && !isReload)
        {
            FineSightExit();
        }
    }

    // ������ ���
    public void CancelFineSight()
    {
        if (isFineSightMode)
        {
            FineSightExit();
        }
    }

    // ������ ����
    private void FineSight()
    {
        isFineSightMode = true;
        currentGun.anim.SetBool("FineSightMode", isFineSightMode);
        theCrosshair.FineSightAnimation(isFineSightMode);
        StopAllCoroutines();
        StartCoroutine(FineSightActiveCoroutine());
    }

    // ������ ����
    private void FineSightExit()
    {
        isFineSightMode = false;
        currentGun.anim.SetBool("FineSightMode", isFineSightMode);
        theCrosshair.FineSightAnimation(isFineSightMode);
        StopAllCoroutines();
        StartCoroutine(FineSightDeActiveCoroutine());
    }

    // ������ Ȱ��ȭ �ڷ�ƾ
    IEnumerator FineSightActiveCoroutine()
    {
        while(currentGun.transform.localPosition != currentGun.fineSightOriginPos)
        {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.2f);
            yield return null;
        }
    }

    // ������ ��Ȱ��ȭ �ڷ�ƾ
    IEnumerator FineSightDeActiveCoroutine()
    {
        while(currentGun.transform.localPosition != originPos)
        {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.2f);
            yield return null;
        }
    }

    // �ݵ� �ڷ�ƾ
    IEnumerator RetroActionCoroutine()
    {
        if(!isFineSightMode)
        {
            currentGun.transform.localPosition = originPos;
            // recoil start
            while(currentGun.transform.localPosition.x <= currentGun.retroActionForce - 0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, recoilBack, 0.4f);
                yield return null;
            }
            // gun recovery position
            while(currentGun.transform.localPosition != originPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.1f);
                yield return null;
            }
        } else
        {
            currentGun.transform.localPosition = currentGun.fineSightOriginPos;
            // fineSightMode recoil start
            while (currentGun.transform.localPosition.x <= currentGun.retroActionFineSightForce - 0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, retroActionRecoilBack, 0.4f);
                yield return null;
            }
            // fineSightMode gun recovery position
            while (currentGun.transform.localPosition != currentGun.fineSightOriginPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.1f);
                yield return null;
            }
        }
    }

    // ���� ���
    private void PlaySE(AudioClip _clip) {
        audioSource.clip = _clip;
        audioSource.Play();
    }

    public Gun GetGun() {
        return currentGun;
    }

    public bool GetFineSightMode()
    {
        return isFineSightMode;
    }

}
