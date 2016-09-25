using FullInspector.Modules;

namespace FullInspector.Modules {
    public class BaseSerializedAction : BaseSerializationDelegate { }
}

namespace FullInspector {
    public class SerializedAction : BaseSerializedAction {
        public void Invoke() {
            DoInvoke(null);
        }
    }
    public class SerializedAction<TParam1> : BaseSerializedAction {
        public void Invoke(TParam1 param1) {
            DoInvoke(param1);
        }
    }
    public class SerializedAction<TParam1, TParam2> : BaseSerializedAction {
        public void Invoke(TParam1 param1, TParam2 param2) {
            DoInvoke(param1, param2);
        }
    }
    public class SerializedAction<TParam1, TParam2, TParam3> : BaseSerializedAction {
        public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3) {
            DoInvoke(param1, param2, param3);
        }
    }
    public class SerializedAction<TParam1, TParam2, TParam3, TParam4> : BaseSerializedAction {
        public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4) {
            DoInvoke(param1, param2, param3, param4);
        }
    }
    public class SerializedAction<TParam1, TParam2, TParam3, TParam4, TParam5> : BaseSerializedAction {
        public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5) {
            DoInvoke(param1, param2, param3, param4, param5);
        }
    }
    public class SerializedAction<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> : BaseSerializedAction {
        public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6) {
            DoInvoke(param1, param2, param3, param4, param5, param6);
        }
    }
    public class SerializedAction<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7> : BaseSerializedAction {
        public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7) {
            DoInvoke(param1, param2, param3, param4, param5, param6, param7);
        }
    }
    public class SerializedAction<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8> : BaseSerializedAction {
        public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7, TParam8 param8) {
            DoInvoke(param1, param2, param3, param4, param5, param6, param7, param8);
        }
    }
    public class SerializedAction<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9> : BaseSerializedAction {
        public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7, TParam8 param8, TParam9 param9) {
            DoInvoke(param1, param2, param3, param4, param5, param6, param7, param8, param9);
        }
    }
}
