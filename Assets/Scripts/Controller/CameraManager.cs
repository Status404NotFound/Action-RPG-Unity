using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class CameraManager : MonoBehaviour
    {
        public bool lockon;
        public float followSpeed = 9;
        public float mouseSpeed = 2;
        public float controllerSpeed = 7;

        public Transform target;
        public Transform lockonTransform;

        public Transform pivot;
        public Transform camTrans;
        public Transform mTransform;
        StatesManager states;

        float turnSmoothing = 0.1f;
        public float minAngle = -35;
        public float maxAngle = 35;

        public float defZ;
        public float curZ;
        public float zSpeed = 19;

        float smoothX;
        float smoothY;
        float smoothXvelocity;
        float smoothYvelocity;
        public float lookAngle;
        public float tiltAngle;

        bool usedRightAxis;

        bool changeTargetLeft;
        bool changeTargetRight;

        public void Init(StatesManager st)
        {
            states = st;
            target = st.transform;
            curZ = defZ;
            mTransform = this.transform;
        }

        public void Tick(float d)
        {
            float h = Input.GetAxis(StaticStrings.mouseX);
            float v = Input.GetAxis(StaticStrings.mouseY);

            float c_h = Input.GetAxis(StaticStrings.rightAxisX);
            float c_v = Input.GetAxis(StaticStrings.rightAxisY);

            float targetSpeed = mouseSpeed;

            if (usedRightAxis)
            {
                if (Mathf.Abs(c_h) < 0.6f)
                {
                    usedRightAxis = false;
                }
            }

            if (c_h != 0 || c_v != 0)
            {
                h = c_h;
                v = -c_v;
                targetSpeed = controllerSpeed;
            }

            FollowTarget(d);
            HandleRotations(d, v, h, targetSpeed);
            HandlePivotPosition();
        }

        void FollowTarget(float d)
        {
            float speed = d * followSpeed;
            Vector3 targetPosition = Vector3.Lerp(transform.position, target.position, speed);
            transform.position = targetPosition;
        }

        void HandleRotations(float d, float v, float h, float targetSpeed)
        {
            if (turnSmoothing > 0)
            {
                smoothX = Mathf.SmoothDamp(smoothX, h, ref smoothXvelocity, turnSmoothing);
                smoothY = Mathf.SmoothDamp(smoothY, v, ref smoothYvelocity, turnSmoothing);
            }
            else
            {
                smoothX = h;
                smoothY = v;
            }

            lookAngle += smoothX * targetSpeed;
            transform.rotation = Quaternion.Euler(0, lookAngle, 0);

            tiltAngle -= smoothY * targetSpeed;
            tiltAngle = Mathf.Clamp(tiltAngle, minAngle, maxAngle);
            pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);
        }
        void HandlePivotPosition()
        {
            float targetZ = defZ;
            CameraCollision(defZ, ref targetZ);

            curZ = Mathf.Lerp(curZ, targetZ, states.delta * zSpeed);
            Vector3 tp = Vector3.zero;
            tp.z = curZ;
            camTrans.localPosition = tp;
        }

        void CameraCollision(float targetZ, ref float actualZ)
        {
            float step = Mathf.Abs(targetZ);
            int stepCount = 2;
            float stepIncrement = step / stepCount;

            RaycastHit hit;
            Vector3 origin = pivot.position;
            Vector3 direction = -pivot.forward;

            if(Physics.Raycast(origin,direction,out hit, step, states.ignoreForGroundCheck))
            {
                float distance = Vector3.Distance(hit.point, origin);
                actualZ = -(distance / 2);
            }
            else
            {
                for(int s = 0; s<stepCount +1; s++)
                {
                    for(int i = 0; i < 4; i++)
                    {
                        Vector3 dir = Vector3.zero;
                        Vector3 secondOrigin = origin + (direction * s) * stepIncrement;
                        switch (i)
                        {
                            case 0:
                                dir = camTrans.right;
                                break;
                            case 1:
                                dir = -camTrans.right;
                                break;
                            case 2:
                                dir = camTrans.up;
                                break;
                            case 3:
                                dir = -camTrans.up;
                                break;
                        }

                        if(Physics.Raycast(secondOrigin,dir,out hit, 0.2f, states.ignoreForGroundCheck))
                        {
                            float distance = Vector3.Distance(secondOrigin, origin);
                            actualZ = -(distance / 2);
                            if (actualZ < 0.2f)
                                actualZ = 0;

                            return;
                        }
                    }
                }
            }
        }

        public static CameraManager singleton;
        private void Awake()
        {
            if (singleton != null)
            {
                Object.Destroy(singleton);
            }
            singleton = this;
        }

    }
}