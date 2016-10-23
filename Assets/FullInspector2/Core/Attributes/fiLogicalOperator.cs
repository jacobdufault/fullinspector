using System;
using FullInspector.Internal;

namespace FullInspector {
    public enum fiLogicalOperator {
        AND,
        OR
    }

    public static class fiLogicalOperatorSupport {
        public static bool ComputeValue(fiLogicalOperator op, string[] memberNames, object element) {
            bool finalValue = GetInitialValue(op);

            for (int i = 0; i < memberNames.Length; ++i) {
                string memberName = memberNames[i];
                bool result = fiRuntimeReflectionUtility.GetBooleanReflectedMember(
                    element.GetType(), element, memberName, /*defaultValue:*/true);
                finalValue = Combine(op, finalValue, result);
            }

            return finalValue;
        }

        private static bool GetInitialValue(fiLogicalOperator op) {
            switch (op) {
                case fiLogicalOperator.AND:
                    return true;
                case fiLogicalOperator.OR:
                    return false;
            }
            throw new NotImplementedException();
        }

        private static bool Combine(fiLogicalOperator op, bool a, bool b) {
            switch (op) {
                case fiLogicalOperator.AND:
                    return a && b;
                case fiLogicalOperator.OR:
                    return a || b;
            }
            throw new NotImplementedException();
        }
    }
}