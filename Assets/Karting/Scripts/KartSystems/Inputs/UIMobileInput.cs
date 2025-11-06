using KartGame.KartSystems;
using UnityEngine;

namespace KartGame.KartSystems
{
    public class UIMobileInput : BaseInput
    {
        private bool accelerate = false;
        private bool brake = false;
        private float turnInput = 0f;
        private int turnDirection = 0;


        public void OnAccelerateDown() => accelerate = true;
        public void OnAccelerateUp() => accelerate = false;

        public void OnBrakeDown() => brake = true;
        public void OnBrakeUp() => brake = false;

        public void OnTurnLeftDown() => turnDirection = -1;
        public void OnTurnRightDown() => turnDirection = 1;
        public void OnTurnUp() => turnDirection = 0;

        public override InputData GenerateInput()
        {
            if (turnDirection != 0)
                turnInput = Mathf.MoveTowards(turnInput, turnDirection, Time.deltaTime * 2f);
            else
                turnInput = Mathf.MoveTowards(turnInput, 0, Time.deltaTime * 2f);

            return new InputData
            {
                Accelerate = accelerate,
                Brake = brake,
                TurnInput = turnInput
            };
        }
    }
}
