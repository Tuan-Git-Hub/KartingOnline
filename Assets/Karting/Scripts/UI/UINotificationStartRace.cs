using TMPro;
using UnityEngine;

public class UINotificationStartRace : MonoBehaviour
{
    TextMeshProUGUI textCountTime;

    private void Awake()
    {
        textCountTime = GetComponentInChildren<TextMeshProUGUI>();
    }
  
    void Update()
    {
        if (GameManager.Instance.haveCountdownTimeBeforeRace)
        {
            float ctime = GameManager.Instance.countdownTime;
            if (ctime > 0)
            {
                textCountTime.text = $"{Mathf.CeilToInt(ctime)}";
            }
            else
            {
                textCountTime.text = "GO";
            }
        }
        else
        {
            gameObject.SetActive(false);
        }    
    }
}
