﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : PoolableMono
{
    private int _attackDamage;

    private float _exploveRadius;
    private bool _isIceBullet;

    [Header("기본 속성값")]
    public TurretAI.TurretType type = TurretAI.TurretType.Single;
    public Transform target;
    public LayerMask targetLayer;
    public bool lockOn;
    //public bool track;
    public float speed = 1;
    public float turnSpeed = 1;
    public bool catapult;

    public float knockBack = 0.1f;
    public float boomTimer = 1;
    private readonly float _defaultBoomTimeLimit = 1f;
    //public Vector3 _startPosition;
    //public float dist;

    public PoolingParticle explosion;
    private PoolManager _poolManager;

    private void Awake()
    {
        _poolManager = PoolManager.Instance;
    }

    private void Update()
    {
        if (target == null)
        {
            Explosion();
            return;
        }

        if (transform.position.y < -0.2F)
        {
            Explosion();
        }
        

        if (type == TurretAI.TurretType.Catapult)
        {
            boomTimer -= Time.deltaTime;
            if (boomTimer < 0)
            {
                Explosion();
            }
            if (lockOn)
            {
                Vector3 Vo = CalculateCatapult(target.transform.position, transform.position, 1);

                transform.GetComponent<Rigidbody>().velocity = Vo;
                lockOn = false;
            }
        }else if(type == TurretAI.TurretType.Dual)
        {
            Vector3 dir = target.position - transform.position;
            //float distThisFrame = speed * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, dir, Time.deltaTime * turnSpeed, 0.0f);
            Debug.DrawRay(transform.position, newDirection, Color.red);

            //transform.Translate(dir.normalized * distThisFrame, Space.World);
            //transform.LookAt(target);

            transform.Translate(Vector3.forward * Time.deltaTime * speed);
            transform.rotation = Quaternion.LookRotation(newDirection);

        }else if (type == TurretAI.TurretType.Single)
        {
            float singleSpeed = speed * Time.deltaTime;
            transform.Translate(transform.forward * singleSpeed * 2, Space.World);
        }
    }

    Vector3 CalculateCatapult(Vector3 target, Vector3 origen, float time)
    {
        Vector3 distance = target - origen;
        Vector3 distanceXZ = distance;
        distanceXZ.y = 0;

        float Sy = distance.y;
        float Sxz = distanceXZ.magnitude;

        float Vxz = Sxz / time;
        float Vy = Sy / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;

        Vector3 result = distanceXZ.normalized;
        result *= Vxz;
        result.y = Vy;

        return result;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag != "Enemy")
            return;

        // 명중효과 작성
        if (type == TurretAI.TurretType.Catapult)
        {
            Collider[] cols =
            Physics.OverlapSphere(transform.position,
                _exploveRadius, targetLayer);

            for(int i = 0; i < cols.Length; ++i)
            {
                if (cols[i].TryGetComponent<Monster>(out Monster mob))
                    EnemyDamage(mob);
            }
        }
        else
        {
            if (other.TryGetComponent<Monster>(out Monster mob))
            {
                EnemyDamage(mob);
            }
        }

        Explosion();
    }

    private void EnemyDamage(Monster mob)
    {
        mob.OnDamage(_attackDamage, _isIceBullet);
    }

    public void SettingProjectileProperty(int damage, float exploveRadius = 0, bool isIceBullet = false)
    {
        _attackDamage = damage;
        _exploveRadius = exploveRadius;
        _isIceBullet = isIceBullet;
    }
    public void Explosion()
    {
        Debug.Log("사라짐");

        if (explosion != null)
        {
            PoolingParticle explosionParticle = 
                (_poolManager.Pop(explosion.name) as PoolingParticle);

            explosionParticle.Play();
            explosionParticle.SetPosition(transform.position);
        }
            
        _poolManager.Push(this);
    }

    public override void Reset()
    {
        if (catapult)
        {
            lockOn = true;
        }

        if (type == TurretAI.TurretType.Single)
        {
            Vector3 dir = target.position - transform.position;
            transform.rotation = Quaternion.LookRotation(dir);
            
        }

        if (type == TurretAI.TurretType.Catapult)
            boomTimer = _defaultBoomTimeLimit;
    }
}
