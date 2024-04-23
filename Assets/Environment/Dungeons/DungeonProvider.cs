using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Dungeon {
    [SerializeField] public string dungeonName;
    [SerializeField] public GameObject dungeonPrefab;
    [SerializeField] public GameObject dungeoninitializer;
}

public class DungeonProvider : MonoBehaviour
{
    [SerializeField] private List<Dungeon> dungeonList;
    private string currentDungeon;
    public string CurrentDungeon { get => currentDungeon; set => currentDungeon = value; }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            foreach (var dungeon in dungeonList)
            {
                if (dungeon.dungeoninitializer.name == currentDungeon) {
                    Debug.Log($"Player가 {dungeon.dungeonName}에 입장합니다.");
                    dungeon.dungeoninitializer.GetComponent<MeshRenderer>().enabled = false;
                    dungeon.dungeonPrefab.SetActive(true);
                } else {
                    Debug.Log($"해당하는 던전을 리스트에서 찾을수 없습니다. 던전 리스트를 확인해주세요.");
                }
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) {
            foreach (var dungeon in dungeonList)
            {
                if (dungeon.dungeoninitializer.name == currentDungeon) {
                    Debug.Log($"Player가 {dungeon.dungeonName}에서 퇴장합니다.");
                    dungeon.dungeoninitializer.GetComponent<MeshRenderer>().enabled = true;
                    dungeon.dungeonPrefab.SetActive(false);
                } else {
                    Debug.Log($"해당하는 던전을 리스트에서 찾을수 없습니다. 던전 리스트를 확인해주세요.");
                }
            }
        }
    }
}
