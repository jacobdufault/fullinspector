using System;
using System.Collections.Generic;
using System.Reflection;
using FullSerializer.Internal;
using UnityEngine;

namespace FullInspector {
    public interface tkIControl {
        object Edit(Rect rect, object obj, object context, fiGraphMetadata metadata);

        float GetHeight(object obj, object context, fiGraphMetadata metadata);

        void InitializeId(ref int nextId);

        Type ContextType { get; }
    }

    /// <summary>
    /// The default tk context object, used if one is not explicitly specified.
    /// </summary>
    public class tkDefaultContext : tkContextLabelRequest {
        public GUIContent Label { get; set; }
    }

    /// <summary>
    /// Extend this interface on the context object to receive the current label used
    /// in the inspector for the object.
    /// </summary>
    public interface tkContextLabelRequest {
        GUIContent Label { get; set; }
    }

    public class tk<T> : tk<T, tkDefaultContext> {
    }

    /// <summary>
    /// A style can be applied to any tkControl{T}. Typically styles are generic
    /// styling components that are universally applicable, such as changing the
    /// color of the object.
    /// </summary>
    public abstract class tkStyle<T, TContext> {
        public abstract void Activate(T obj, TContext context);
        public abstract void Deactivate(T obj, TContext context);
    }

    /// <summary>
    /// A control defines some abstract operation. Some rules define layout operations for a
    /// set of sub-controls while other controls do the actual editing.
    /// </summary>
    public abstract class tkControl<T, TContext> : tkIControl {
        private int _uniqueId;

        /// <summary>
        /// This will return metadata that will be unique for this control. The metadata
        /// object that is passed down to each control is shared among every control -- if
        /// a control needs to store metadata, then it should fetch the instance metadata to
        /// do so, otherwise the metadata will be shared among every instance of that control
        /// for each object.
        /// </summary>
        protected fiGraphMetadata GetInstanceMetadata(fiGraphMetadata metadata) {
            return metadata.Enter(_uniqueId).Metadata;
        }

        /// <summary>
        /// Returns the context type that this object uses. This could be alternatively fetched using reflection,
        /// but for now this is easier.
        /// </summary>
        public Type ContextType {
            get { return typeof(TContext); }
        }

        protected abstract T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata);

        protected abstract float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata);

        /// <summary>
        /// Should this control be displayed? When this is false then the parent control should
        /// act as if this control does not exist.
        /// </summary>
        public virtual bool ShouldShow(T obj, TContext context, fiGraphMetadata metadata) {
            return true;
        }

        /// <summary>
        /// This is a helper to set Styles for just a single style.
        /// </summary>
        public tkStyle<T, TContext> Style {
            set {
                Styles = new List<tkStyle<T, TContext>> { value };
            }
        }

        /// <summary>
        /// The list of styles that will be applied to the control before/after editing.
        /// </summary>
        /// <remarks>
        /// Something very interesting in .NET: You can invoke the collection initializer via an object
        /// initializer. ie,
        /// 
        /// class Obj { public List<int> coll; }
        /// void NullRefException() {
        ///     var o = new Obj { coll = { 1, 2, 3 } }
        /// }
        /// 
        /// This code is equivalent to
        /// 
        /// var o = new Obj();
        /// o.coll.Add(1);
        /// ...
        /// 
        /// Now the exception is obvious (o.coll is null in o.coll.Add).
        /// 
        /// This means that we *must* allocate an object to Styles to prevent accidental null-ptr bugs
        /// </remarks>
        public List<tkStyle<T, TContext>> Styles {
            get {
                if (_styles == null) _styles = new List<tkStyle<T, TContext>>();
                return _styles;
            }
            set {
                _styles = value;
            }
        }
        private List<tkStyle<T, TContext>> _styles;

        public T Edit(Rect rect, T obj, TContext context, fiGraphMetadata metadata) {
            if (Styles == null) {
                return DoEdit(rect, obj, context, metadata);
            }

            for (int i = 0; i < Styles.Count; ++i) {
                Styles[i].Activate(obj, context);
            }

            var result = DoEdit(rect, obj, context, metadata);

            for (int i = 0; i < Styles.Count; ++i) {
                Styles[i].Deactivate(obj, context);
            }

            return result;
        }
        public object Edit(Rect rect, object obj, object context, fiGraphMetadata metadata) {
            return Edit(rect, (T)obj, (TContext)context, metadata);
        }

        public float GetHeight(T obj, TContext context, fiGraphMetadata metadata) {
            if (Styles == null) {
                return DoGetHeight(obj, context, metadata);
            }


            for (int i = 0; i < Styles.Count; ++i) {
                Styles[i].Activate(obj, context);
            }

            var result = DoGetHeight(obj, context, metadata);

            for (int i = 0; i < Styles.Count; ++i) {
                Styles[i].Deactivate(obj, context);
            }

            return result;
        }
        public float GetHeight(object obj, object context, fiGraphMetadata metadata) {
            return GetHeight((T)obj, (TContext)context, metadata);
        }

        void tkIControl.InitializeId(ref int nextId) {
            _uniqueId = nextId++;

            foreach (var control in NonMemberChildControls) {
                control.InitializeId(ref nextId);
            }

            var type = GetType();
            while (type != null) {
                foreach (var member in type.GetDeclaredMembers()) {
                    Type memberType;
                    if (TryGetMemberType(member, out memberType) == false) continue;

                    if (typeof(tkIControl).IsAssignableFrom(memberType)) {
                        tkIControl control;
                        if (TryReadValue(member, this, out control) == false) continue;
                        if (control == null) continue;

                        control.InitializeId(ref nextId);
                    }

                    else if (typeof(IEnumerable<tkIControl>).IsAssignableFrom(memberType)) {
                        IEnumerable<tkIControl> controls;
                        if (TryReadValue(member, this, out controls) == false) continue;
                        if (controls == null) continue;

                        foreach (var control in controls) control.InitializeId(ref nextId);
                    }
                }

                type = type.Resolve().BaseType;
            }
        }

        /// <summary>
        /// If this control stores tkIControl instances outside of tkIControl members or IEnumerable{tkIControl} members,
        /// then this function should return those controls.
        /// </summary>
        protected virtual IEnumerable<tkIControl> NonMemberChildControls {
            get {
                yield break;
            }
        }

        private static bool TryReadValue<TValue>(MemberInfo member, object context, out TValue value) {
            if (member is FieldInfo) {
                value = (TValue)((FieldInfo)member).GetValue(context);
                return true;
            }
            if (member is PropertyInfo) {
                value = (TValue)((PropertyInfo)member).GetValue(context, null);
                return true;
            }

            value = default(TValue);
            return false;
        }

        private static bool TryGetMemberType(MemberInfo member, out Type memberType) {
            if (member is FieldInfo) {
                memberType = ((FieldInfo)member).FieldType;
                return true;
            }
            if (member is PropertyInfo) {
                memberType = ((PropertyInfo)member).PropertyType;
                return true;
            }

            memberType = null;
            return false;
        }
    }
}