using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealthUI : MonoBehaviour
{
    public OnionBossPhase1 boss;
    public Progress_bar progress;
    
    // Start is called before the first frame update
    void Start()
    {
        progress.max = boss.MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        progress.current = boss.Health;
    }
}
