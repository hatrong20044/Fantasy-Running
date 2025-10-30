using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : Singleton<PauseManager>
{
    private readonly List<IPausable> pausables = new List<IPausable>();
    public bool IsPaused { get; private set; }
    protected override void Awake()
    {
        base.Awake();
    }
    public void Register(IPausable obj)
    {
        if (!pausables.Contains(obj))
            pausables.Add(obj);
    }

    public void Unregister(IPausable obj)
    {
        pausables.Remove(obj);
    }

    public void PauseAll()
    {
        if (IsPaused) return;

        IsPaused = true;
        Time.timeScale = 0f;

        foreach (var p in pausables)
            p.OnPause();
    }

    public void ResumeAll()
    {
        if (!IsPaused) return;

        IsPaused = false;
        Time.timeScale = 1f;

        foreach (var p in pausables)
            p.OnResume();
    }
}
