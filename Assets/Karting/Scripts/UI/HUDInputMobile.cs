using KartGame.KartSystems;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HUDInputMobile : MonoBehaviour
{
    private Button buttonLeft;
    private Button buttonRight;
    private Button buttonUp;
    private Button buttonDown;

    public ArcadeKart player;
    private void Awake()
    {
        var buttons = GetComponentsInChildren<Button>();
        foreach (var button in buttons)
        {
            if (button.gameObject.name == "ButtonLeft")
                buttonLeft = button;
            else if (button.gameObject.name == "ButtonRight")
                buttonRight = button;
            else if (button.gameObject.name == "ButtonUp")
                buttonUp = button;
            else if (button.gameObject.name == "ButtonDown")
                buttonDown = button;
        }
    }

    public void BindPlayer(ArcadeKart target)
    {
        player = target;

        var input = player.gameObject.GetComponent<UIMobileInput>();
        if (input == null) return;

        AddTrigger(buttonLeft, e => input.OnTurnLeftDown(), e => input.OnTurnUp());
        AddTrigger(buttonRight, e => input.OnTurnRightDown(), e => input.OnTurnUp());
        AddTrigger(buttonUp, e => input.OnAccelerateDown(), e => input.OnAccelerateUp());
        AddTrigger(buttonDown, e => input.OnBrakeDown(), e => input.OnBrakeUp());

    }

    private void AddTrigger(Button btn, UnityAction<BaseEventData> downCallback, UnityAction<BaseEventData> upCallback)
    {
        if (!btn) return;

        var trigger = btn.GetComponent<EventTrigger>() ?? btn.gameObject.AddComponent<EventTrigger>();

        if (downCallback != null)
        {
            var entryDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            entryDown.callback.AddListener(downCallback.Invoke);
            trigger.triggers.Add(entryDown);
        }

        if (upCallback != null)
        {
            var entryUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            entryUp.callback.AddListener(upCallback.Invoke);
            trigger.triggers.Add(entryUp);
        }
    }
}
