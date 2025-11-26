using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ResetHandler : MonoBehaviour
{
    public UnityEvent Reset => reset;

    [SerializeField] private UnityEvent reset;

    private readonly Dictionary<IResettable, TransformState> resettables = new();

    private void Awake()
    {
        foreach (IResettable resettable in FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IResettable>())
        {
            resettables.Add(resettable, new TransformState(resettable.Transform));
        }
    }

    public void ResetAll()
    {
        foreach ((IResettable resettable, TransformState state) in resettables)
        {
            if (resettable == null) continue;

            if (resettable.ResetTransform) resettable.Transform.SetPositionAndRotation(state.position, state.rotation);

            resettable.ResetSelf();
        }

        reset.Invoke();
    }

    private class TransformState
    {
        public readonly Vector3 position;
        public readonly Quaternion rotation;

        public TransformState(Transform transform)
        {
            transform.GetPositionAndRotation(out position, out rotation);
        }
    }
}