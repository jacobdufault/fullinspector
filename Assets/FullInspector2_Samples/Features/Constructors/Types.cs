namespace FullInspector.Samples.Other.Constructors {
    public interface IInterface { }

    public class Implementation1 : IInterface {
        public int A;

        public Implementation1(int a) {
            A = a;
        }
    }

    public class Implementation2 : IInterface {
        public string B;

        public Implementation2(string b) {
            B = b;
        }
    }
}