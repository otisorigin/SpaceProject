using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private float updateSpeedSeconds = 0.5f;

    private void Awake()
    {
        GetComponentInParent<Unit>().OnHealthPctChanged += HandleHealthChanged;
    }

    private void Update()
    {
        
    }

    private void HandleHealthChanged(float pct)
    {
        StartCoroutine(ChangeToPct(pct));
    }

    private IEnumerator ChangeToPct(float pct)
    {
        var image = transform.GetChild(1).GetComponent<Image>();
        float preChangePct = image.fillAmount;
        float elapsed = 0f;

        while (elapsed < updateSpeedSeconds)
        {
            elapsed += Time.deltaTime;
            image.fillAmount = Mathf.Lerp(preChangePct, pct, elapsed / updateSpeedSeconds);
            yield return null;
        }

        image.fillAmount = pct;
    }

//    void Start()
//    {
//        //transform.GetComponent<Canvas>().enabled = true;
//    }
//
//    // Update is called once per frame
//    void Update()
//    {
////        var unitRotation = transform.parent.transform.rotation;
////        if (unitRotation.z > 90.0f || unitRotation.z < -90.0f)
////        {
////            Debug.Log("!!!");
////        }
//    }
}
