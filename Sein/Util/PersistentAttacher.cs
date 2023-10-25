using UnityEngine;

namespace Sein.Util;

internal abstract class PersistentAttacher<A, B> : MonoBehaviour where A : PersistentAttacher<A, B> where B : PersistentMonoBehaviour
{
    public static void Hook()
    {
        GameObject obj = new($"{typeof(A).Name}");
        DontDestroyOnLoad(obj);
        obj.AddComponent<A>();
    }

    protected abstract GameObject? FindParent();

    private B? instance;

    protected void Update()
    {
        if (instance != null) return;
        var obj = FindParent();
        if (obj == null) return;

        instance = obj.AddComponent<B>();
        instance.OnDestroyEvent += () => instance = null;
    }
}

internal abstract class PersistentMonoBehaviour : MonoBehaviour
{
    public delegate void DestroyEvent();
    public event DestroyEvent? OnDestroyEvent;

    protected virtual void OnDestroy() => OnDestroyEvent?.Invoke();
}
