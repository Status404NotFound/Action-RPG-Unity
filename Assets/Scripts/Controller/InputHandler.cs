using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace SA
{
    public class InputHandler : MonoBehaviour
    {
        float vertical;
        float horizontal;

        bool b_input;
        bool x_input;
        bool a_input;
        bool y_input;

        bool rb_input;
        bool rt_input;
        bool lb_input;
        bool lt_input;

        float rt_axis;
        float lt_axis;

        float b_timer;

        float delta;

        public GamePhase curPhase;
        public StatesManager states;
        public CameraManager camManager;
        public Transform camTrans;

        #region Init
        private void Start()
        {
            InitInGame();
        }

        public void InitInGame()
        {
            states.r_manager = Resources.Load("ResourcesManager") as Managers.ResourcesManager;
            states.r_manager.Init();

            states.Init();
            camManager.Init(states);
            camTrans = camManager.mTransform;
        }
        #endregion

        #region Fixed Update
        private void FixedUpdate()
        {
            delta = Time.deltaTime;
            GetInput_FixedUpdate();

            switch (curPhase)
            {
                case GamePhase.inGame:
                    InGame_UpdateStates_FixedUpdate();
                    states.Fixed_Tick(delta);
                    camManager.Tick(delta);
                    break;
                case GamePhase.inMenu:
                    break;
                case GamePhase.inInventory:
                    break;
                default:
                    break;
            }
        }

        void GetInput_FixedUpdate()
        {
            vertical = Input.GetAxis(StaticStrings.Vertical);
            horizontal = Input.GetAxis(StaticStrings.Horizontal);
        }

        void InGame_UpdateStates_FixedUpdate()
        {
            states.inp.vertical = vertical;
            states.inp.horizontal = horizontal;
            states.inp.moveAmount = Mathf.Clamp01((Mathf.Abs(vertical)) + (Mathf.Abs(horizontal)));

            Vector3 moveDir = camTrans.forward * vertical;
            moveDir += camTrans.right * horizontal;
            moveDir.Normalize();
            states.inp.moveDir = moveDir;
        }
        #endregion

        #region Update
        private void Update()
        {
            delta = Time.deltaTime;
            GetInput_Update();

            switch (curPhase)
            {
                case GamePhase.inGame:
                    InGame_UpdateStates_Update();
                    states.Tick(delta);
                    break;
                case GamePhase.inMenu:
                    break;
                case GamePhase.inInventory:
                    break;
                default:
                    break;
            }
        }

        void GetInput_Update()
        {
            b_input = Input.GetButton(StaticStrings.B);
            a_input = Input.GetButtonUp(StaticStrings.A);
            y_input = Input.GetButtonUp(StaticStrings.Y);
            x_input = Input.GetButton(StaticStrings.X);
            rt_input = Input.GetButton(StaticStrings.RT);
            rt_axis = Input.GetAxis(StaticStrings.RT);

            if (rt_axis != 0)
                rt_input = true;


            lt_input = Input.GetButton(StaticStrings.LT);
            lt_axis = Input.GetAxis(StaticStrings.LT);

            if (lt_axis != 0)
                lt_input = true;

            rb_input = Input.GetButton(StaticStrings.RB);
            lb_input = Input.GetButton(StaticStrings.LB);

            if (b_input)
                b_timer += delta;

            states.inp.rb = rb_input;
            states.inp.lb = lb_input;
            states.inp.rt = rt_input;
            states.inp.lt = lt_input;

        }

        void InGame_UpdateStates_Update()
        {

        }
        #endregion
    }

    public enum InputType
    {
        rt,lt,rb,lb
    }

    public enum GamePhase
    {
        inGame,inMenu,inInventory
    }
}