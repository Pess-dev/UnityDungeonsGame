using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    Unit unit;

    public float seekRadius = 50f;

    public float retreatRadius = 4f;

    private NavMeshPath path;

    private float ignoreDistance = 0f;

    private Transform targetTransform = null;

    public float xRotatingModifier = 0.1f;
    public float yRotatingModifier = 0.1f;

    public bool active = true;

    public float attackDelay = 0.5f;

    public float obstacleJumpDistance = 2f;

    public Tactic tactic = Tactic.Simple;

    private enum State
    {
        Idle,
        Attack,
        Chase,
        Retreat,
        SeekWeapon
    }

    public enum Tactic
    {
        Simple,
        Dash,
        SmartDash,
    }

    public static int enemyKilled = 0;

    [SerializeField]
    private State currentState;

    private IEnumerator coroutine;

    private void Awake()
    {
        currentState = State.Idle;

        path = new NavMeshPath();
    }

    private void Start()
    {
        unit = GetComponent<Unit>();

        unit.gameObject.GetComponent<Health>().hit.AddListener(() => { active = true; });
        
        unit.health.death.AddListener(() => { enemyKilled++; });
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Idle:
                {
                    Idle();
                    break;
                }
            case State.Chase:
                {
                    Chase();
                    break;
                }
            case State.Retreat:
                {
                    Retreat();
                    break;
                }
            case State.Attack:
                {
                    coroutine = Attack();
                    StartCoroutine(coroutine);
                    break;
                }
            case State.SeekWeapon:
                {
                    SeekWeapon();
                    break;
                }
        }

        if (targetTransform != null && active)
        {
            if (path.corners.Length > 1)
            {
                MoveUnitTo(path.corners[1]);
                RaycastHit hit;
                if (Physics.Raycast(transform.position, (path.corners[1] - transform.position).normalized, out hit, obstacleJumpDistance))
                    if (hit.transform.tag == "Obstacle")
                        unit.Jump();
            }
            else
                MoveUnitTo(targetTransform.position);

            RotateUnitTo(targetTransform.position);
        }
    }
    private void MoveUnitTo(Vector3 point)
    {
        Vector3 direction = point - transform.position;

        if (direction.magnitude < ignoreDistance)
            return;
        unit.SetMoveDirection(direction.normalized);
    }

    private void RotateUnitTo(Vector3 point)
    {
        Vector3 direction = (point - unit.Head.position).normalized;

        float dx = unit.xRotation - 90 + Vector3.Angle(direction, Vector3.up);
        float dy = Vector3.SignedAngle(transform.forward, direction - Vector3.up * direction.y,Vector3.up);

        Vector3 deltaEuler = new Vector3(-dx*xRotatingModifier,dy*yRotatingModifier,0);

        unit.RotateLocal(deltaEuler * Time.deltaTime);
    }
    private void UpdateTargets()
    {
        if (unit.firstItem == null)
        {
            currentState = State.SeekWeapon;
            return;
        }

        SeekTarget();
        if (targetTransform == null)
            currentState = State.Idle;
    }

    private void Idle()
    {
        UpdateTargets();
        if (targetTransform != null)
        {
            currentState = State.Chase;
            return;
        }
    }

    private IEnumerator Attack()
    {
        if (!active)
            yield break;

        yield return new WaitForSeconds(attackDelay);
        unit.UseItem();
        currentState = State.Chase;
    }

    private void Chase()
    {
        UpdateTargets();
        if (currentState != State.Chase)
            return;

        float attackDistance = seekRadius;
        if (unit.firstItem.itemType == Item.ItemType.Melee)
            attackDistance = unit.firstItem.GetComponent<Melee>().attackRadius;

        if (!unit.firstItem.GetUsable())
        {
            currentState = State.Retreat;
            return;
        }

        if (tactic == Tactic.SmartDash && (targetTransform.position - unit.attackPoint.position).magnitude < attackDistance * 2)
        {
            currentState = State.Attack;
            Vector3 dashDirection = (targetTransform.position - unit.transform.position).normalized;
            dashDirection -= Vector3.up * dashDirection.y;
            unit.Dash(dashDirection);
        }

        if ((targetTransform.position - unit.attackPoint.position).magnitude < attackDistance)
            currentState = State.Attack;
    }

    private void Retreat()
    {
        if (targetTransform == null)
        {
            currentState = State.Idle;
            return;
        }
        if (unit.firstItem.GetUsable())
        {
            currentState = State.Chase;
            return;
        }

        if ((transform.position - targetTransform.position).magnitude > retreatRadius)
        {
            currentState = State.Chase;
            return;
        }

        Vector3 retreatDirection = (transform.position - targetTransform.position - Vector3.up * (transform.position.y - targetTransform.position.y)).normalized;

        if (tactic == Tactic.Dash || tactic == Tactic.SmartDash)
            unit.Dash(retreatDirection);

        NavMesh.CalculatePath(transform.position, transform.position + retreatDirection, NavMesh.AllAreas, path);
    }

    private void SeekWeapon()
    {
        GrabNearestItem();
        if (unit.firstItem != null)
        {
            currentState = State.Idle;
            return;
        }

        path.ClearCorners();
        targetTransform = null;

        Collider[] hits = Physics.OverlapSphere(transform.position, seekRadius);
        foreach (Collider collider in hits)
        {
            Item item = collider.GetComponentInParent<Item>();
            if (item == null)
                continue;
            if (!item.GetGrabbable() || item.GetGrabbed())
                continue;
            
            NavMeshPath newPath = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, item.transform.position, NavMesh.AllAreas, newPath);

            if (path.corners.Length == 0 || PathLength(newPath) < PathLength(path))
            {
                path = newPath;
                targetTransform = item.transform;
            }
        }

        if (targetTransform == null)
        {
            SeekTarget();
            currentState = State.Retreat;
        }
    }

    private void SeekTarget()
    {
        path.ClearCorners();
        
        if (targetTransform != null)
        {
            NavMesh.CalculatePath(transform.position, targetTransform.position, NavMesh.AllAreas, path);
            return;
        }
        
        Collider[] hits = Physics.OverlapSphere(transform.position, seekRadius);
        foreach (Collider collider in hits)
        {
            Unit seekUnit = collider.GetComponent<Unit>();
            if (seekUnit == null)
                continue;

            if (seekUnit.side == unit.side)
                continue;

            NavMeshPath newPath = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, collider.transform.position, NavMesh.AllAreas, newPath);

            if (path.corners.Length == 0 || PathLength(newPath) < PathLength(path))
            {
                path = newPath;
                targetTransform = seekUnit.Head;
            }
        }
    }

    private void GrabNearestItem()
    {
        Item item = null;

        Collider[] collisions = Physics.OverlapSphere(unit.Head.position, unit.interactDistance);

        foreach (Collider collision in collisions)
        {
            if (collision.transform.GetComponentInParent<Item>() == null)
                continue;

            Item collisionItem = collision.transform.GetComponentInParent<Item>();

            if (collisionItem.GetGrabbed())
                continue;
               
            if (item == null)
            {
                item = collisionItem;
                continue;
            }

            if ((collisionItem.transform.position - transform.position).magnitude < (item.transform.position - transform.position).magnitude)
                item = collisionItem; 
        }

        if (item != null)
            unit.GrabItem(item);
    }

   

    private float PathLength(NavMeshPath path)
    {
        if (path == null)
            return float.PositiveInfinity;
        float sum = 0;
        for (int i = 0; i < path.corners.Length - 1; i++)
            sum += (path.corners[i] - path.corners[i + 1]).magnitude;
        return sum;
    }

    private void OnDrawGizmos()
    {
        if (path == null)
            return;
        Gizmos.color = Color.red;
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            Gizmos.DrawSphere(path.corners[i + 1], 0.5f) ;
            Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);
        }
        Gizmos.color = Color.blue;
        if (targetTransform!=null)
        Gizmos.DrawSphere(targetTransform.position, 0.5f);

    }
}
