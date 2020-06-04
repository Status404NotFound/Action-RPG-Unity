using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace FR
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

        float l_thumb;
        float r_thumb_x;

        float b_timer;

        float delta;
        bool lockOnInput;

        public GamePhase curPhase;
        public StatesManager states;
        public CameraManager camManager;
        public Transform camTrans;
        public TransformVariable lockOnTransform;

        public bool isLockedOn;
        public int enemyIndex;
        public List<Transform> enemies = new List<Transform>();

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

            if(states.charState != StatesManager.CharState.roll)
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

            r_thumb_x = Input.GetAxis(StaticStrings.rightAxisX);

            if (b_input)
                b_timer += delta;

            lockOnInput = Input.GetButtonDown(StaticStrings.L);

            if (lockOnInput)
            {
                isLockedOn = !isLockedOn;
                if (isLockedOn)
                {
                    if(enemies.Count == 0)
                    {
                        isLockedOn = false;
                    }
                    else
                    {
                        enemyIndex++;
                        if (enemyIndex > enemies.Count - 1)
                        {
                            enemyIndex = 0;
                        }
                        lockOnTransform.value = enemies[enemyIndex];
                    }
                }
                else
                {
                    lockOnTransform.value = null;
                }
            }

            if (isLockedOn)
            {
                if (enemies.Count == 0)
                {
                    isLockedOn = false;
                }
                else
                {
                    if (r_thumb_x < -.8f || Input.GetKeyDown(KeyCode.Alpha1))
                    {
                        enemyIndex--;
                        if (enemyIndex < 0)
                        {
                            enemyIndex = enemies.Count - 1;
                        }
                        lockOnTransform.value = enemies[enemyIndex];
                    }
                    if (r_thumb_x > .8f || Input.GetKeyDown(KeyCode.Alpha2))
                    {
                        enemyIndex++;
                        if (enemyIndex > enemies.Count - 1)
                        {
                            enemyIndex = 0;
                        }
                        lockOnTransform.value = enemies[enemyIndex];
                    }
                }

                if (lockOnTransform.value == null)
                {
                    isLockedOn = false;
                }
                else
                {
                    float v = Vector3.Distance(states.mTransform.position, lockOnTransform.value.position);
                    if (v > 15)
                    {
                        lockOnTransform.value = null;
                        isLockedOn = false;
                    }
                }
            }

            if (states.inp.lockOnTransform != lockOnTransform.value)
            {
                states.inp.lockOnTransform = lockOnTransform.value;
            }
        }

        void InGame_UpdateStates_Update()
        {
            states.inp.rb = rb_input;
            states.inp.lb = lb_input;
            states.inp.rt = rt_input;
            states.inp.lt = lt_input;

            if (b_input)
            {
                b_timer += delta;

                if (b_timer > 0.5f)
                {
                    states.states.isRunning = true;
                }
            }
            else
            {
                if (b_timer > 0.05f && b_timer < 0.5f)
                {
                    states.HandleRoll();
                }
                b_timer = 0;

                states.states.isRunning = false;
            }

            states.states.isLockedOn = isLockedOn;
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