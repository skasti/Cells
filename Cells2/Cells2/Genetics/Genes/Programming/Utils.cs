using System;

namespace Cells.Genetics.Genes.Programming;

public static class Utils
{
    public static bool IsNumeric(object obj)
    {
        if (obj == null) return false;

        TypeCode typeCode = Type.GetTypeCode(obj.GetType());

        return typeCode switch
        {
            TypeCode.Byte or TypeCode.SByte or
            TypeCode.Int16 or TypeCode.UInt16 or
            TypeCode.Int32 or TypeCode.UInt32 or
            TypeCode.Int64 or TypeCode.UInt64 or
            TypeCode.Single or TypeCode.Double or
            TypeCode.Decimal => true,
            _ => false
        };
    }

    // Check if 'a' is greater than 'b'
    public static bool IsGreaterThan(object a, object b) => CompareObjects(a, b) > 0;

    // Check if 'a' is less than 'b'
    public static bool IsLessThan(object a, object b) => CompareObjects(a, b) < 0;

    // Check if 'a' is equal to 'b'
    public static bool IsEqual(object a, object b) => CompareObjects(a, b) == 0;

    // Check if 'a' is greater than or equal to 'b'
    public static bool IsGreaterThanOrEqual(object a, object b) => CompareObjects(a, b) >= 0;

    // Check if 'a' is less than or equal to 'b'
    public static bool IsLessThanOrEqual(object a, object b) => CompareObjects(a, b) <= 0;

    // Core comparison method: Returns -1, 0, or 1
    private static int CompareObjects(object a, object b)
    {
        if (a == null || b == null)
            throw new ArgumentNullException("Cannot compare null values.");

        if (a is IComparable comparableA && b is IComparable comparableB)
        {
            // Allow cross-type numeric comparisons (e.g., int vs. double)
            if (a.GetType() == b.GetType() || (IsNumeric(a) && IsNumeric(b)))
            {
                return comparableA.CompareTo(Convert.ChangeType(b, a.GetType()));
            }
            throw new ArgumentException($"{a.GetType().Name} & {b.GetType().Name}");
        }

        throw new ArgumentException("Objects must implement IComparable.");
    }

    public static object Add(object a, object b) => PerformOperation(a, b, (x, y) => x + y);
    public static object Subtract(object a, object b) => PerformOperation(a, b, (x, y) => x - y);
    public static object Multiply(object a, object b) => PerformOperation(a, b, (x, y) => x * y);

    // Core method for numeric operations
    private static object PerformOperation(object a, object b, Func<dynamic, dynamic, dynamic> operation)
    {
        if (a == null || b == null)
            throw new ArgumentNullException("Cannot perform operations on null values.");

        if (!IsNumeric(a) || !IsNumeric(b))
            throw new ArgumentException("Both arguments must be numeric types.");

        // Ensure the result is of the same type as 'a'
        Type targetType = a.GetType();
        dynamic left = Convert.ChangeType(a, targetType);
        dynamic right = Convert.ChangeType(b, targetType);
        var result = operation(left, right);
        return ConvertToType(result, targetType);
    }

    // Explicitly handle conversion with wrap-around support
    private static object ConvertToType(dynamic value, Type targetType)
    {
        if (targetType == typeof(byte)) return (byte)value;
        if (targetType == typeof(sbyte)) return (sbyte)value;
        if (targetType == typeof(short)) return (short)value;
        if (targetType == typeof(ushort)) return (ushort)value;
        if (targetType == typeof(int)) return (int)value;
        if (targetType == typeof(uint)) return (uint)value;
        if (targetType == typeof(long)) return (long)value;
        if (targetType == typeof(ulong)) return (ulong)value;
        if (targetType == typeof(float)) return (float)value;
        if (targetType == typeof(double)) return (double)value;
        if (targetType == typeof(decimal)) return (decimal)value;

        throw new InvalidOperationException($"Unsupported numeric type: {targetType}");
    }
}
