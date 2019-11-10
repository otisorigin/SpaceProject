using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthCounter : MonoBehaviour
{
    public Text Counter;
    private void Awake()
    {
        GetComponentInParent<Unit>().OnHealthPctChanged += HandleHealthChanged;
    }

    public void InitHealthCounter(int maxHealth)
    {
        
    }
    
    private void HandleHealthChanged(float pct)
    {
        //StartCoroutine(ChangeToPct(pct));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
