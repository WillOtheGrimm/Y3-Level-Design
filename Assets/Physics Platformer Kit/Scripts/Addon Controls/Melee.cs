using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Melee : MonoBehaviour
{
    [SerializeField] private PlayerMove playerMove;
    [SerializeField][Min(1e-5f)] private float attackDuration = .5f, attackInterval = .5f;
    [SerializeField][Min(0f)] private int attackDamage = 1;
    [SerializeField][Min(1e-5f)] private float attackPushHeight = .25f, attackPushForce = 5f;
    [SerializeField] private Bounds groundBounds = new(Vector3.forward, Vector3.one), airBounds = new(Vector3.zero, 2f * Vector3.one);
    [SerializeField] private LayerMask attackMask = 1;
    [SerializeField] private UnityEvent groundAttackStarted, airAttackStarted, attackEnded;

    private new Rigidbody rigidbody;
    private Coroutine attackRoutine;
    private float lastAttackTime;

    #region Unity Messages
    private void Awake()
    {
        Assert.IsNotNull(playerMove);

        rigidbody = GetComponent<Rigidbody>();
        lastAttackTime = -attackInterval;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Melee")) _ = TryAttack();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = attackRoutine != null && playerMove.IsGrounded ? Color.green : Color.white;
        Gizmos.DrawWireCube(transform.position + transform.TransformDirection(groundBounds.center), groundBounds.size);
        Gizmos.color = attackRoutine != null && !playerMove.IsGrounded ? Color.green : Color.white;
        Gizmos.DrawWireCube(transform.position + transform.TransformDirection(airBounds.center), airBounds.size);
    } 
    #endregion

    private bool TryAttack()
    {
        if (Time.time < lastAttackTime + attackInterval || attackRoutine != null) return false;

        bool isGrounded = playerMove.IsGrounded;
        attackRoutine = StartCoroutine(AttackRoutine(isGrounded ? groundBounds : airBounds));
        if (!isGrounded) rigidbody.linearVelocity = new Vector3(rigidbody.linearVelocity.x, 0f, rigidbody.linearVelocity.z);

        (isGrounded ? groundAttackStarted : airAttackStarted).Invoke();

        return true;
    }

    private IEnumerator AttackRoutine(Bounds bounds)
    {
        HashSet<Collider> hitColliders = new();
        float timeElapsed = 0f;
        while (timeElapsed < attackDuration)
        {
            Collider[] targets = Physics.OverlapBox(transform.position + transform.TransformDirection(bounds.center), bounds.extents, transform.rotation, attackMask);
            foreach (Collider target in targets)
            {
                if (!hitColliders.Add(target) || target.gameObject == gameObject) continue;

                if (target.TryGetComponent(out DealDamage dealDamage))
                {
                    GetComponent<DealDamage>().Attack(target.gameObject, attackDamage, attackPushHeight, attackPushForce);
                }
            }

            timeElapsed += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        lastAttackTime = Time.time;
        attackEnded.Invoke();

        attackRoutine = null;
    }
}