namespace Sevriukoff.MetaRun.Domain.Base;

public readonly struct Vector3
{
    public float X { get; }
    public float Y { get; }
    public float Z { get; }

    public Vector3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }
    
    public static Vector3 operator +(Vector3 left, Vector3 right)
    {
        float newX = left.X + right.X;
        float newY = left.Y + right.Y;
        float newZ = left.Z + right.Z;
        return new Vector3(newX, newY, newZ);
    }

    public static Vector3 operator -(Vector3 left, Vector3 right)
    {
        float newX = left.X - right.X;
        float newY = left.Y - right.Y;
        float newZ = left.Z - right.Z;
        return new Vector3(newX, newY, newZ);
    }

    public static bool operator >(Vector3 left, Vector3 right)
    {
        return (left.X > right.X) && (left.Y > right.Y) && (left.Z > right.Z);
    }
    
    public static bool operator <(Vector3 left, Vector3 right)
    {
        return (left.X < right.X) && (left.Y < right.Y) && (left.Z < right.Z);
    }
    
    public override string ToString()
    {
        return $"{X:F0}{Y:F0}{Z:F0}";
    }
}
