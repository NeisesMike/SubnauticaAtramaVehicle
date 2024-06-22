using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VehicleFramework;

namespace R5Submersible
{
    public class JetController : MonoBehaviour, IPlayerListener
    {
        private float resting;
        private float extended;
        public float smoothTime = 1.2f; // Adjust the smooth time as needed
        private Vector3 targetPosition;
        private Vector3 currentVelocity = Vector3.zero;

        public void Initialize (float rest, float extend)
        {
            resting = rest;
            extended = extend;
            targetPosition = transform.localPosition;
        }
        public void Update()
        {
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPosition, ref currentVelocity, smoothTime);
        }
        public void Retract()
        {
            targetPosition = new Vector3(resting, transform.localPosition.y, transform.localPosition.z);
        }
        public void Extend()
        {
            targetPosition = new Vector3(extended, transform.localPosition.y, transform.localPosition.z);
        }
        public void DelayedRetraction(float seconds)
        {
            UWE.CoroutineHost.StartCoroutine(DelayedRetractionHelper(seconds));
        }
        public IEnumerator DelayedRetractionHelper(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            Retract();
        }

        void IPlayerListener.OnPlayerEntry()
        {
            Extend();
        }

        void IPlayerListener.OnPlayerExit()
        {
            DelayedRetraction(3f);
        }

        void IPlayerListener.OnPilotBegin()
        {
        }

        void IPlayerListener.OnPilotEnd()
        {
        }
    }
}
