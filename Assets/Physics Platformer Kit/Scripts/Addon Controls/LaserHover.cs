using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody))]
public class LaserHover : MonoBehaviour
{
    [SerializeField] private PlayerMove playerMove;
    [SerializeField] private GameObject lasers;
    [SerializeField][Min(1e-5f)] private float hoverTime = .25f, maxHoverForce = 40f, minHoverForce = 20f;

    private new Rigidbody rigidbody;
    private Coroutine hoverRoutine;
    private float remainingHoverTime;

    private void Awake()
    {
        Assert.IsNotNull(playerMove);
        Assert.IsNotNull(lasers);

        rigidbody = GetComponent<Rigidbody>();
        remainingHoverTime = hoverTime;

        playerMove.Landed.AddListener(Land);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump")) _ = TryStartHover();
        if (Input.GetButtonUp("Jump")) EndHover();
    }

    private bool TryStartHover()
    {
        if (playerMove.IsGrounded) return false;

        if (remainingHoverTime > 0f) rigidbody.linearVelocity = new Vector3(rigidbody.linearVelocity.x, 0f, rigidbody.linearVelocity.z);
        hoverRoutine ??= StartCoroutine(HoverRoutine());
        lasers.SetActive(true);

        return true;
    }

    private IEnumerator HoverRoutine()
    {
        while (true)
        {
            float currentHoverForce = Mathf.Lerp(minHoverForce, maxHoverForce, remainingHoverTime / hoverTime);
            rigidbody.AddForce(currentHoverForce * Vector3.up, ForceMode.Acceleration);

            remainingHoverTime -= Time.fixedDeltaTime;
            
            yield return new WaitForFixedUpdate();
        }
    }

    private void Land()
    {
        EndHover();
        remainingHoverTime = hoverTime;
    }

    private void EndHover()
    {
        if (hoverRoutine != null)
        {
            StopCoroutine(hoverRoutine);
            hoverRoutine = null;
            remainingHoverTime = 0f;
        }
        lasers.SetActive(false);
    }
}