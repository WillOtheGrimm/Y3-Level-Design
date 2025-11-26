using UnityEngine;

public interface IResettable
{
    public Transform Transform { get; }
    public bool ResetTransform { get; }

    public virtual void ResetSelf() { }
}