using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum State {
    None,
    Minimap,
    Map
}

[Serializable]
public class MapResolution {
    // 다용도 멤버 변수
    public string mapName;
    public State state;

    // 미니맵 이미지의 크기 정보
    public float mapWidth;
    public float mapHeight;

    // 미니맵의 부모인 맵의 앵커 정보
    public Vector2 anchorMin;
    public Vector2 anchorMax;

    // 미니맵의 부모인 맵의 앵커포지션 정보
    public Vector2 anchorPosition;
}

public class MapController : MonoBehaviour
{
    [SerializeField] private GameObject mapObj; // 미니맵 이미지 오브젝트
    [SerializeField] private Transform playerTransform; // 플레이어의 Transform 컴포넌트
    [SerializeField] private Image playerIndicator; // 맵에 표시할 플레이어의 이미지 표시기
    [SerializeField] private List<MapResolution> mapResolutionList;
    private int mapIndexCounter;

    public static State currentState;

    // 맵 이미지의 크기와 게임 월드의 크기 설정
    private float mapWidth;
    private float mapHeight;
    private float worldWidth = 5000f;
    private float worldHeight = 5000f;

    // 맵 이미지의 중심 위치 설정
    private float mapCenterX;
    private float mapCenterY;

    // 월드 테레인의 위치 설정
    private float worldCenterX = -2500f;
    private float worldCenterZ = -2500f;

    void Start() {
        mapIndexCounter = 1;
        SetMappingProtocol(mapResolutionList[mapIndexCounter]);
        GameManager.instance.isMapOpen = true;
    }

    private void SetMappingProtocol(MapResolution mapResolution)
    {
        currentState = mapResolution.state;

        mapWidth = mapResolution.mapWidth;
        mapHeight = mapResolution.mapHeight;
        mapCenterX = mapWidth / 2f;
        mapCenterY = mapHeight / 2f;

        RectTransform mapObjRectTransform = mapObj.GetComponent<RectTransform>();
        mapObjRectTransform.sizeDelta = new Vector2(mapWidth, mapHeight);

        RectTransform parentRectTF = GetComponent<RectTransform>();
        parentRectTF.anchorMin = mapResolution.anchorMin;
        parentRectTF.anchorMax = mapResolution.anchorMax;
        parentRectTF.anchoredPosition = mapResolution.anchorPosition;
    }

    void Update()
    {
        if (GameManager.instance.isMapOpen)
            DynamicPlayerIndicator();

        if (Input.GetKeyDown(KeyCode.M)) {
            mapIndexCounter++;
            int currentIndex = mapIndexCounter % mapResolutionList.Count;
            if (currentIndex == 0) {
                GameManager.instance.isMapOpen = false;
            } else {
                GameManager.instance.isMapOpen = true;
            }
            MapCheck(currentIndex);
        }
    }

    private void MapCheck(int mapIndexCounter)
    {
        SetMappingProtocol(mapResolutionList[mapIndexCounter]);
        mapObj.SetActive(GameManager.instance.isMapOpen);
    }

    private void DynamicPlayerIndicator() {
        // 플레이어의 현재 위치를 가져옴
        Vector3 playerPosition = playerTransform.position;

        // 플레이어의 위치를 게임 월드의 중심을 기준으로 맵 이미지에 매핑
        float mapX = Mathf.Clamp((playerPosition.x - worldCenterX) / worldWidth * mapWidth, 0f, mapWidth);
        float mapY = Mathf.Clamp((playerPosition.z - worldCenterZ) / worldHeight * mapHeight, 0f, mapHeight);

        // 플레이어가 바라보는 방향에 따라 회전값을 계산
        Vector3 forward = playerTransform.forward;
        float angle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;

        // 맵 이미지에 플레이어 위치를 표시
        playerIndicator.rectTransform.anchoredPosition = new Vector2(mapX - mapCenterX, mapY - mapCenterY);
        playerIndicator.rectTransform.rotation = Quaternion.Euler(0f, 0f, -angle);
    }
}
