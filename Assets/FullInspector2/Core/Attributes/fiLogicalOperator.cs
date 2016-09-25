using System;

namespace FullInspector {
    public enum fiLogicalOperator {
        AND,
        OR
    }

    public static class fiLogicalOperatorSupport {
        public static bool GetInitialValue(fiLogicalOperator op) {
            switch (op) {
                case fiLogicalOperator.AND:
                    return true;
                case fiLogicalOperator.OR:
                    return false;
            }
            throw new NotImplementedException();
        }

        public static bool Combine(fiLogicalOperator op, bool a, bool b) {
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