using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonController : MonoBehaviour
{
    private DungeonProvider dungeonProvider;

    private void Start() {
        dungeonProvider = FindObjectOfType<DungeonProvider>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (dungeonProvider != null) {
                dungeonProvider.CurrentDungeon = gameObject.name;
                dungeonProvider.OnTriggerEnter(other);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (dungeonProvider != null) {
                dungeonProvider.CurrentDungeon = gameObject.name;
                dungeonProvider.OnTriggerExit(other);
        }
    }
}
