using System; // Required for [Flags] attribute

[Flags] // This attribute allows you to combine enum values (e.g., North | East)
public enum RoomDoors
{
    None = 0,
    North = 1 << 0, // 0001 in binary, decimal 1
    East = 1 << 1,  // 0010 in binary, decimal 2
    South = 1 << 2, // 0100 in binary, decimal 4
    West = 1 << 3   // 1000 in binary, decimal 8
}