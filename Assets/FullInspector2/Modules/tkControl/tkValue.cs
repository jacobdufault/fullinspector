using System;

namespace FullInspector {
    public partial class tk<T, TContext> {
        public static Value<TValue> Val<TValue>(Value<TValue>.GeneratorNoContext generator) {
            return generator;
        }

        public static Value<TValue> Val<TValue>(Value<TValue>.Generator generator) {
            return generator;
        }

        public static Value<TValue> Val<TValue>(TValue value) {
            return value;
        }

        public struct Value<TValue> {
            private Generator _generator;
            private TValue _direct;

            public Value(Generator generator) {
                _generator = generator;
                _direct = default(TValue);
            }

            public Value(GeneratorNoContext generator) {
                _generator = (o, context) => generator(o);
                _direct = default(TValue);
            }


            public TValue GetCurrentValue(T instance, TContext context) {
                if (_generator == null) return _direct;
                return _generator(instance, context);
            }

            public static implicit operator Value<TValue>(TValue direct) {
                return new Value<TValue> {
                    _generator = null,
                    _direct = direct
                };
            }

            public delegate TValue Generator(T input, TContext context);
            public delegate TValue GeneratorNoContext(T input);

            public static implicit operator Value<TValue>(Generator generator) {
                return new Value<TValue> {
                    _generator = generator,
                    _direct = default(TValue)
                };
            }

            public static implicit operator Value<TValue>(GeneratorNoContext generator) {
                return new Value<TValue> {
                    _generator = (obj, context) => generator(obj),
                    _direct = default(TValue)
                };
            }


            public static implicit operator Value<TValue>(Func<T, int, TValue> generator) {
                return new Value<TValue>();
            }

            public static implicit operator Value<TValue>(Func<T, TValue> generator) {
                return new Value<TValue> {
                    _generator = (obj, context) => generator(obj),
                    _direct = default(TValue)
                };
            }
        }
    }
}