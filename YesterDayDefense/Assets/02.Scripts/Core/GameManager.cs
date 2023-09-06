using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField]
    private Transform _gameUIPanel;

    [SerializeField]
    private List<PoolableMono> _poolingList = new List<PoolableMono>();
    [SerializeField]
    private List<PoolableMono> _monsterpoolingList = new List<PoolableMono>();

    [field:SerializeField]
    public int Money { get; private set; } = 0;

    //나중에 여기다가 몬스터 몇마리 남았는지 필요함

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Debug.LogError($"{transform} : GameManager is Multiple running!");

        if(_gameUIPanel != null)
            UIManager.Instance = new UIManager(_gameUIPanel);

        PoolManager.Instance = new PoolManager(transform);
        foreach(var pool in _poolingList)
            PoolManager.Instance.CreatePool(pool);
        foreach (var pool in _monsterpoolingList)
            PoolManager.Instance.CreatePool(pool,0);

    }

    public void PlusMoney(int plus)     => Money += plus;
    public void SpentMoney(int spent)   => Money -= spent;
}