using UnityEngine;
public enum RoomType
{
    Empty,      // No room at this grid position
    Start,      // The first room the player begins in
    Normal,     // A regular room with enemies or minor obstacles
    Boss,       // The boss room
    Secret,     // Hidden room (optional)
    Corridor    // A simple connection room (optional)
}