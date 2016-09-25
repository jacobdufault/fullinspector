using System;
using UnityEngine;

namespace FullInspector.Internal {

    public abstract class fiBaseAnimValue<T> {
        private double m_LerpPosition = 1.0;
        public float speed = 2f;
        private T m_Start;
        [SerializeField]
        private T m_Target;
        private double m_LastTime;
        private bool m_Animating;

        public bool isAnimating {
            get {
                return this.m_Animating;
            }
        }

        protected float lerpPosition {
            get {
                double num = 1.0 - this.m_LerpPosition;
                return (float)(1.0 - num * num * num * num);
            }
        }

        protected T start {
            get {
                return this.m_Start;
            }
        }

        public T target {
            get {
                return this.m_Target;
            }
            set {
                if (this.m_Target.Equals((object)value))
                    return;
                this.BeginAnimating(value, this.value);
            }
        }

        public T value {
            get {
                return this.GetValue();
            }
            set {
                this.StopAnim(value);
            }
        }

        protected fiBaseAnimValue(T value) {
            this.m_Start = value;
            this.m_Target = value;
        }

        private static T2 Clamp<T2>(T2 val, T2 min, T2 max) where T2 : IComparable<T2> {
            if (val.CompareTo(min) < 0)
                return min;
            if (val.CompareTo(max) > 0)
                return max;
            return val;
        }

        protected void BeginAnimating(T newTarget, T newStart) {
            this.m_Start = newStart;
            this.m_Target = newTarget;
            fiLateBindings.EditorApplication.AddUpdateFunc(Update);
            this.m_Animating = true;
            this.m_LastTime = fiLateBindings.EditorApplication.timeSinceStartup;
            this.m_LerpPosition = 0.0;
        }

        private void Update() {
            if (!this.m_Animating)
                return;
            this.UpdateLerpPosition();
            if ((double)this.lerpPosition < 1.0)
                return;
            this.m_Animating = false;
            fiLateBindings.EditorApplication.RemUpdateFunc(Update);
        }

        private void UpdateLerpPosition() {
            double timeSinceStartup = fiLateBindings.EditorApplication.timeSinceStartup;
            this.m_LerpPosition = fiBaseAnimValue<T>.Clamp<double>(this.m_LerpPosition + (timeSinceStartup - this.m_LastTime) * (double)this.speed, 0.0, 1.0);
            this.m_LastTime = timeSinceStartup;
        }

        protected void StopAnim(T newValue) {
            bool flag = false;
            if ((!newValue.Equals((object)this.GetValue()) || this.m_LerpPosition < 1.0))
                flag = true;
            this.m_Target = newValue;
            this.m_Start = newValue;
            this.m_LerpPosition = 1.0;
            this.m_Animating = false;
            if (!flag)
                return;
        }

        protected abstract T GetValue();
    }

    [Serializable]
    public class fiAnimFloat : fiBaseAnimValue<float> {
        [SerializeField]
        private float m_Value;

        public fiAnimFloat(float value)
            : base(value) {
        }

        protected override float GetValue() {
            this.m_Value = Mathf.Lerp(this.start, this.target, this.lerpPosition);
            return this.m_Value;
        }
    }

    [Serializable]
    public class fiAnimBool : fiBaseAnimValue<bool> {
        [SerializeField]
        private float m_Value;

        public float faded {
            get {
                this.GetValue();
                return this.m_Value;
            }
        }

        public fiAnimBool()
            : base(false) {
        }

        public fiAnimBool(bool value)
            : base(value) {
        }

        protected override bool GetValue() {
            float from = !this.target ? 1f : 0.0f;
            float to = 1f - from;
            this.m_Value = Mathf.Lerp(from, to, this.lerpPosition);
            return (double)this.m_Value > 0.5;
        }

        public float Fade(float from, float to) {
            return Mathf.Lerp(from, to, this.faded);
        }
    }
}
