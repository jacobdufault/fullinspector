using FullInspector.Modules;

namespace FullInspector.Modules {
    public class BaseSerializedFunc : BaseSerializationDelegate { }
}

namespace FullInspector {
    public class SerializedFunc<TResult> : BaseSerializedFunc {
        public TResult Invoke() {
            return (TResult)DoInvoke(null);
        }
    }
    public class SerializedFunc<TParam1, TResult> : BaseSerializedFunc {
        public TResult Invoke(TParam1 param1) {
            return (TResult)DoInvoke(param1);
        }
    }
    public class SerializedFunc<TParam1, TParam2, TResult> : BaseSerializedFunc {
        public TResult Invoke(TParam1 param1, TParam2 param2) {
            return (TResult)DoInvoke(param1, param2);
        }
    }
    public class SerializedFunc<TParam1, TParam2, TParam3, TResult> : BaseSerializedFunc {
        public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3) {
            return (TResult)DoInvoke(param1, param2, param3);
        }
    }
    public class SerializedFunc<TParam1, TParam2, TParam3, TParam4, TResult> : BaseSerializedFunc {
        public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4) {
            return (TResult)DoInvoke(param1, param2, param3, param4);
        }
    }
    public class SerializedFunc<TParam1, TParam2, TParam3, TParam4, TParam5, TResult> : BaseSerializedFunc {
        public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5) {
            return (TResult)DoInvoke(param1, param2, param3, param4, param5);
        }
    }
    public class SerializedFunc<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TResult> : BaseSerializedFunc {
        public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6) {
            return (TResult)DoInvoke(param1, param2, param3, param4, param5, param6);
        }
    }
    public class SerializedFunc<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TResult> : BaseSerializedFunc {
        public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7) {
            return (TResult)DoInvoke(param1, param2, param3, param4, param5, param6, param7);
        }
    }
    public class SerializedFunc<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TResult> : BaseSerializedFunc {
        public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7, TParam8 param8) {
            return (TResult)DoInvoke(param1, param2, param3, param4, param5, param6, param7, param8);
        }
    }
    public class SerializedFunc<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TResult> : BaseSerializedFunc {
        public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7, TParam8 param8, TParam9 param9) {
            return (TResult)DoInvoke(param1, param2, param3, param4, param5, param6, param7, param8, param9);
        }
    }
}
