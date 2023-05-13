using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Health : MonoBehaviour
{
    [SerializeField]
    private float HP;

    [SerializeField]
    private float maxHP;

    [SerializeField]
    private Object deathPrefab;

    [SerializeField]
    private Transform deathPrefabPlace;

    public bool mortal = true;

    public UnityEvent hit;

    public Object dropPrefab;
     

    private void Start()
    {
        HP = maxHP;
    }

    public float GetHP() { return HP; }
    public float GetMaxHP() { return maxHP; }

    public void Damage(float value)
    {
        if (mortal)
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
        if (HP > maxHP) HP = maxHP;
        if (HP > 0) return;
        if (!mortal) return;

        Kill();
    }

    public void Kill()
    {
        Vector3 pos = transform.position;
        if (deathPrefabPlace != null)
            pos = deathPrefabPlace.position;
        if (deathPrefab != null)
            Instantiate(deathPrefab, pos, transform.rotation);

        if (dropPrefab != null)
            Instantiate(dropPrefab, pos, transform.rotation);

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        
    }

}
