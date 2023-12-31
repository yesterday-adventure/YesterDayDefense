using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    public static MonsterManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Debug.LogError($"{transform} : MonsterManager is multiply running");
    }

    private void FixedUpdate()
    {
        if(transform.childCount == 0 && WaveTimeline.Instance.isOver)
            GameManager.Instance.isGameClear = true;
    }

    public void GameOver()
    {
        Monster[] monsters= GetComponentsInChildren<Monster>();
        foreach (Monster monster in monsters)
        {
            monster.Animator.SetBool(monster.OverHash, true);
            monster.StopAllCoroutines();
        }
    }
}
