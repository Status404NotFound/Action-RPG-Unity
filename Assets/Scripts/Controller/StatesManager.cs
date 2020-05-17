using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA.Scriptable;
using SA.Managers;
using SA.Inventory;

namespace SA
{
    public class StatesManager : MonoBehaviour
    {
        public ControllerStats stats;
        public States states;
        public InputVariables inp;
        public GameObject activeModel;

        public InventoryManager inv_manager;
        public WeaponManager w_manager;
        [HideInInspector]
        public ResourcesManager r_manager;

        #region References
        [HideInInspector]
        public Animator anim;
        [HideInInspector]
        public AnimatorHook a_hook;
        [HideInInspector]
        public Rigidbody rigid;
        [HideInInspector]
        public Collider controllerCollider;
        #endregion

        [HideInInspector]
        public LayerMask ignoreLayers;
        [HideInInspector]
        public LayerMask ignoreForGroundCheck;

        public float delta;
        public Transform mTransform;

        public CharState charState;
        public enum CharState
        {
            moving, onAir, armsInteracting, overrideLayerInteracting
        }

        #region Init
        public void Init()
        {
            if (r_manager == null)
            {
                r_manager = Resources.Load("ResourcesManager") as ResourcesManager;
            }

            mTransform = this.transform;
            SetupAnimator();
            rigid = GetComponent<Rigidbody>();
            rigid.angularDrag = 999;
            rigid.drag = 4;
            rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            gameObject.layer = 8;
            ignoreLayers = ~(1 << 9);
            ignoreForGroundCheck = ~(1 << 9 | 1 << 10);

            a_hook = activeModel.AddComponent<AnimatorHook>();
            a_hook.Init(this);

            InitInventory();
            InitWeaponManager();
        }

        void InitInventory()
        {
            if (inv_manager.rh_item)
            {
                WeaponToRuntime(inv_manager.rh_item, ref inv_manager.rh);
                EquipWeapon(inv_manager.rh, false);
            }
            if (inv_manager.lh_item)
            {
                WeaponToRuntime(inv_manager.lh_item, ref inv_manager.lh);
                EquipWeapon(inv_manager.lh, true);
            }
        }

        void WeaponToRuntime(Item obj, ref RuntimeWeapon slot)
        {
            Weapon w = (Weapon)obj;
            GameObject go = Instantiate(w.modelPrefab) as GameObject;
            go.SetActive(false);
            RuntimeWeapon rw = new RuntimeWeapon
            {
                instance = go,
                w_actual = w
            };

            slot = rw;
            r_manager.runtime.RegisterRW(rw);
        }

        void EquipWeapon(RuntimeWeapon rw, bool isLeft)
        {
            Vector3 p = Vector3.zero;
            Vector3 e = Vector3.zero;
            Vector3 s = new Vector3(35,35,35);
            Transform par = null;

            if (isLeft)
            {
                p = rw.w_actual.lh_position.pos;
                e = rw.w_actual.lh_position.eulers;
                par = anim.GetBoneTransform(HumanBodyBones.LeftHand);
            }
            else
            {
                p = rw.w_actual.rh_position.pos;
                e = rw.w_actual.rh_position.eulers;
                par = anim.GetBoneTransform(HumanBodyBones.RightHand);
            }
            rw.instance.transform.parent = par;
            rw.instance.transform.localPosition = p;
            rw.instance.transform.localEulerAngles = e;
            rw.instance.transform.localScale = s;

            rw.instance.SetActive(true);
        }

        void InitWeaponManager()
        {
            if(inv_manager.lh == null && inv_manager.rh == null)
                return;

            if(inv_manager.rh != null)
            {
                WeaponManager.ActionContainer rb = w_manager.GetAction(InputType.rb);
                rb.action = inv_manager.rh.w_actual.GetAction(InputType.rb);
                rb.isMirrored = false;

                WeaponManager.ActionContainer rt = w_manager.GetAction(InputType.rt);
                rt.action = inv_manager.rh.w_actual.GetAction(InputType.rt);
                rt.isMirrored = false;

                if(inv_manager.lh == null)
                {
                    WeaponManager.ActionContainer lb = w_manager.GetAction(InputType.lb);
                    lb.action = inv_manager.rh.w_actual.GetAction(InputType.lb);
                    lb.isMirrored = false;

                    WeaponManager.ActionContainer lt = w_manager.GetAction(InputType.lt);
                    lt.action = inv_manager.rh.w_actual.GetAction(InputType.lt);
                    lt.isMirrored = false;
                }
                else
                {
                    WeaponManager.ActionContainer lb = w_manager.GetAction(InputType.lb);
                    lb.action = inv_manager.lh.w_actual.GetAction(InputType.rb);
                    lb.isMirrored = true;

                    WeaponManager.ActionContainer lt = w_manager.GetAction(InputType.lt);
                    lt.action = inv_manager.lh.w_actual.GetAction(InputType.rt);
                    lt.isMirrored = true;
                }

                return;
            }
            if (inv_manager.lh != null)
            {
                WeaponManager.ActionContainer rb = w_manager.GetAction(InputType.rb);
                rb.action = inv_manager.lh.w_actual.GetAction(InputType.rb);
                rb.isMirrored = true;

                WeaponManager.ActionContainer rt = w_manager.GetAction(InputType.rt);
                rt.action = inv_manager.lh.w_actual.GetAction(InputType.rt);
                rt.isMirrored = true;

                WeaponManager.ActionContainer lb = w_manager.GetAction(InputType.lb);
                lb.action = inv_manager.lh.w_actual.GetAction(InputType.lb);
                lb.isMirrored = false;

                WeaponManager.ActionContainer lt = w_manager.GetAction(InputType.lt);
                lt.action = inv_manager.lh.w_actual.GetAction(InputType.lt);
                lt.isMirrored = false;
            }
        }

        void SetupAnimator()
        {
            if (activeModel == null)
            {
                anim = GetComponentInChildren<Animator>();
                activeModel = anim.gameObject;
            }
            if(anim == null)
            {
                anim = GetComponentInChildren<Animator>();
            }
            anim.applyRootMotion = false;
            anim.GetBoneTransform(HumanBodyBones.LeftHand).localScale = Vector3.one;
            anim.GetBoneTransform(HumanBodyBones.RightHand).localScale = Vector3.one;
        }
        #endregion

        #region Fixed Update
        public void Fixed_Tick(float d)
        {
            delta = d;
            states.onGround = OnGround();

            switch (charState)
            {
                case CharState.moving:
                    HandleRotation();
                    HandleMovement();
                    break;
                case CharState.onAir:
                    break;
                case CharState.armsInteracting:
                    break;
                case CharState.overrideLayerInteracting:
                    rigid.drag = 0;
                    Vector3 v = rigid.velocity;
                    Vector3 tv = inp.animDelta;
                    tv *= 170;                          //Stap whith attack
                    tv.y = v.y;
                    rigid.velocity = tv;
                    break;
                default:
                    break;
            }
        }

        void HandleRotation()
        {
            Vector3 targetDir = (states.isLockedOn == false) ?
                inp.moveDir :
                (inp.lockOnTransform == null) ?
                inp.lockOnTransform.position - mTransform.position :
                inp.moveDir;

            targetDir.y = 0;
            if (targetDir == Vector3.zero)
            {
                targetDir = mTransform.forward;
            }

            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(mTransform.rotation, tr, delta * inp.moveAmount * stats.rotateSpeed);
            mTransform.rotation = targetRotation;

        }

        void HandleMovement()
        {
            Vector3 v = mTransform.forward;
            if (states.isLockedOn)
                v = inp.moveDir;

            
            if (inp.moveAmount > 0)
                rigid.drag = 0;
            else
                rigid.drag = 4;
            
            if (states.isRunning)
                v *= inp.moveAmount * stats.sprintSpeed;
            else
                v *= inp.moveAmount * stats.moveSpeed;

            v.y = rigid.velocity.y;
            rigid.velocity = v;

        }
        #endregion

        #region Update
        public void Tick(float d)
        {
            delta = d;
            states.onGround = OnGround();

            switch (charState)
            {
                case CharState.moving:
                    bool interact = CheckForInteractionInput();
                    if (!interact)
                    {
                        HandleMovementAnim();
                    }
                    break;
                case CharState.onAir:
                    break;
                case CharState.armsInteracting:

                    break;
                case CharState.overrideLayerInteracting:
                    states.animIsInteracting = anim.GetBool("isInteracting");
                    if (states.animIsInteracting == false)
                    {
                        if (states.isInteracting)
                        {
                            states.isInteracting = false;
                            ChangeState(CharState.moving);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        bool CheckForInteractionInput()
        {
            WeaponManager.ActionContainer a = null;

            if (inp.rb)
            {
                a = GetActionContainer(InputType.rb);
                if(a.action != null)
                    if (a.action.action_obj != null)
                    {
                        HandleAction(a);
                        return true;
                    }
            }
            if (inp.rt)
            {
                a = GetActionContainer(InputType.rt);
                if (a.action != null)
                    if (a.action.action_obj != null)
                    {
                        HandleAction(a);
                        return true;
                    }
            }
            if (inp.lb)
            {
                a = GetActionContainer(InputType.lb);
                if (a.action != null)
                    if (a.action.action_obj != null)
                {
                    HandleAction(a);
                    return true;
                }
            }
            if (inp.lt)
            {
                a = GetActionContainer(InputType.lt);
                if (a.action != null)
                    if (a.action.action_obj != null)
                {
                    HandleAction(a);
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Manager Functions
        void HandleAction(WeaponManager.ActionContainer a)
        {
            switch (a.action.actionType)
            {
                case ActionType.attack:
                    AttackAction aa = (AttackAction)a.action.action_obj;
                    PlayAttackAction(a,aa);
                    break;
                case ActionType.block:
                    break;
                case ActionType.spell:
                    break;
                case ActionType.parry:
                    break;
                default:
                    break;
            }
        }

        WeaponManager.ActionContainer GetActionContainer(InputType inp)
        {
            WeaponManager.ActionContainer ac = w_manager.GetAction(inp);
            if (ac == null)
                return null;

            return ac;
        }

        void PlayInteractAnimation(string a)
        {
            anim.CrossFade(a, 0.2f);
            //anim.PlayInFixedTime(a, 5, 0.2f);
        }

        void PlayAttackAction(WeaponManager.ActionContainer a, AttackAction aa)
        {
            anim.SetBool(StaticStrings.mirror, a.isMirrored);
            PlayInteractAnimation(aa.attack_anim.value);
            if (aa.changeSpeed)
            {
                anim.SetFloat("speed", aa.animSpeed);
            }
            ChangeState(CharState.overrideLayerInteracting);
        }

        void HandleMovementAnim()
        {
            if (states.isLockedOn)
            {

            }
            else
            {
                anim.SetBool(StaticStrings.run, states.isRunning);
                anim.SetFloat(StaticStrings.vertical, inp.moveAmount, 0.15f, delta);
            }
        }

        void ChangeState(CharState t)
        {
            charState = t;
            switch (t)
            {
                case CharState.moving:
                    anim.applyRootMotion = false;
                    break;
                case CharState.onAir:
                    anim.applyRootMotion = false;
                    break;
                case CharState.armsInteracting:
                    anim.applyRootMotion = false;
                    break;
                case CharState.overrideLayerInteracting:
                    anim.applyRootMotion = true;
                    anim.SetBool("isInteracting", true);
                    states.isInteracting = true;
                    break;
                default:
                    break;
            }
        }
        #endregion

        bool OnGround()
        {
            bool retVel = false;

            Vector3 origin = mTransform.position;
            origin.y += 0.4f;
            Vector3 dir = -Vector3.up;
            float dis = 0.7f;
            RaycastHit hit;
            if (Physics.Raycast(origin, dir, out hit, dis, ignoreForGroundCheck))
            {
                retVel = true;
                Vector3 targetPosition = hit.point;
                mTransform.position = targetPosition;
            }

            return retVel;
        }
    }

    [System.Serializable]
    public class WeaponManager
    {
        public ActionContainer[] action_containers;

        public ActionContainer GetAction(InputType t)
        {
            for (int i = 0; i < action_containers.Length; i++)
            {
                if (action_containers[i].inp == t)
                    return action_containers[i];
            }
            return null;
        }

        /*
        public void Init()
        {
            action_containers = new ActionContainer[4];
            for (int i = 0; i < action_containers.Length; i++)
            {
                ActionContainer a = new ActionContainer();
                a.inp = (InputType)i;
                action_containers[i] = a;
            }
        }
        */

        [System.Serializable]
        public class ActionContainer
        {
            public InputType inp;
            public bool isMirrored;
            public Action action;
        }
    }

    [System.Serializable]
    public class InventoryManager
    {
        public RuntimeWeapon rh;
        public RuntimeWeapon lh;

        public Item rh_item;
        public Item lh_item;
        public Item conumable;
        public Item spell;
    }

    [System.Serializable]
    public class InputVariables
    {
        public float moveAmount;
        public float horizontal;
        public float vertical;
        public Vector3 moveDir;
        public Vector3 animDelta;
        public Transform lockOnTransform;

        public bool rt;
        public bool lt;
        public bool rb;
        public bool lb;
    }

    [System.Serializable]
    public class States
    {
        public bool onGround;
        public bool isRunning;
        public bool isLockedOn;
        public bool isInAction;
        public bool isMoveEnabled;
        public bool isDamageOn;
        public bool isRotateEnabled;
        public bool isAttackEnabled;
        public bool isSpellcasting;
        public bool isIKEnabled;
        public bool isUsingItem;
        public bool isAbledToBeParried;
        public bool isParryOn;
        public bool isLaftHand;
        public bool animIsInteracting;
        public bool isInteracting;
        public bool closeWeapons;
        public bool isInvisible;
    }

    [System.Serializable]
    public class NetworkStates
    {
        public bool isLocal;
        public bool isInRoom;
    }
}