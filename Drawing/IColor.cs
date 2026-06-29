using System.Runtime.InteropServices;

namespace SystemEx.Drawing {
    public interface IColor<T> {
        // Helligkeit / Sättigung (Farbraumabhängig)
        T Saturation(float delta);
        T Brightness(float delta);

        // Farb-zu-Farb Operationen
        T Addition(T value);
        T Subtraction(T value);
        T Multiplication(T value);
        T Division(T value);

        // Komponentenweise Operationen
        T Addition(float a, float b, float c);
        T Subtraction(float a, float b, float c);
        T Multiplication(float a, float b, float c);
        T Division(float a, float b, float c);

        // Interpolation
        T Lerp(T value, float amount);

    }
}