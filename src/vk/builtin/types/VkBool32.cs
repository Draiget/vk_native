namespace vk.builtin.types;

/// <summary>
/// <p>Vulkan boolean type stored in a 4-byte unsigned integer.</p>
/// <p>VkBool32 represents boolean True and False values, since C does not have a sufficiently portable built-in boolean type.</p>
/// </summary>
public readonly struct VkBool32 : IEquatable<VkBool32>
{
    /// <summary>
    /// The raw value of the <see cref="VkBool32"/>. A value of 0 represents <b>false</b>, all other values represent <b>true</b>.
    /// </summary>
    public uint Value { get; }

    /// <summary>
    /// Represents the boolean <b>true</b> value. Has a raw value of 1.
    /// </summary>
    public static readonly VkBool32 True = new (1);

    /// <summary>
    /// Represents the boolean <b>false</b> value. Has a raw value of 0.
    /// </summary>
    public static readonly VkBool32 False = new (0);

    /// <summary>
    /// Constructs a new <see cref="VkBool32"/> with the given raw value. 
    /// </summary>
    /// <param name="value">4-byte unsigned representation of <b>bool</b></param>
    public VkBool32(uint value) {
        Value = value;
    }

    /// <summary>
    /// Returns whether another <see cref="VkBool32"/> value is considered equal to this one.
    /// Two <see cref="VkBool32"/>s are considered equal when their raw values are equal.
    /// </summary>
    /// <param name="other">The value to compare to.</param>
    /// <returns>True if the other value's underlying raw value is equal to this instance's. False otherwise.</returns>
    public bool Equals(VkBool32 other) {
        return Value.Equals(other.Value);
    }

    public override bool Equals(object? obj) {
        return obj is VkBool32 other && Equals(other);
    }

    public override int GetHashCode() {
        return Value.GetHashCode();
    }

    public static implicit operator bool(VkBool32 b) => b.Value != 0;
    public static implicit operator uint(VkBool32 b) => b.Value;
    public static implicit operator VkBool32(bool b) => b ? True : False;
    public static implicit operator VkBool32(uint value) => new (value);

    public static bool operator ==(VkBool32 left, VkBool32 right) => left.Value == right.Value;
    public static bool operator !=(VkBool32 left, VkBool32 right) => left.Value != right.Value;

    public override string ToString() {
        return $"{(this ? "True" : "False")} ({Value})";
    }
}
