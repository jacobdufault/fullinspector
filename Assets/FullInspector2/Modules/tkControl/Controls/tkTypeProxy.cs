using UnityEngine;

namespace FullInspector {
    public class tkTypeProxy<TFrom, TContextFrom, TTo, TContextTo> : tkControl<TTo, TContextTo> {
        private tkControl<TFrom, TContextFrom> _control;

        public tkTypeProxy(tkControl<TFrom, TContextFrom> control) {
            _control = control;
        }

        private static T Cast<T>(object val) {
            return (T)val;
        }

        public override bool ShouldShow(TTo obj, TContextTo context, fiGraphMetadata metadata) {
            return _control.ShouldShow(Cast<TFrom>(obj), Cast<TContextFrom>(context), metadata);
        }

        protected override TTo DoEdit(Rect rect, TTo obj, TContextTo context, fiGraphMetadata metadata) {
            return Cast<TTo>(_control.Edit(rect, Cast<TFrom>(obj), Cast<TContextFrom>(context), metadata));
        }

        protected override float DoGetHeight(TTo obj, TContextTo context, fiGraphMetadata metadata) {
            return _control.GetHeight(Cast<TFrom>(obj), Cast<TContextFrom>(context), metadata);
        }
    }
}