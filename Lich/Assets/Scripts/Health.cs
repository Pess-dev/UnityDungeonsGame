using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Health : MonoBehaviour
{
    private float HP;

    [SerializeField]
    private float maxHP;

    [SerializeField]
    private Object deathPrefab;

    public bool mortal = true;


    public UnityEvent hit;

    private void Start()
    {
        HP = maxHP;
    }

    public float GetHP() { return HP; }

    public void Damage(float value)
    {
        HP -= value;
        hit.Invoke();
        CheckDeath();
    }

    public void ChangeHP(float delta) 
    { 
        HP += delta;
        CheckDeath();
    }

    public void SetHP(float newHP)
    {
        HP = newHP;
        CheckDeath();
    }

    private void CheckDeath()
    {
        if (HP > 0) return;
        if (!mortal) return;

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (deathPrefab != null)
            Instantiate(deathPrefab, transform.position, transform.rotation);
    }

}
