using UnityEngine;

namespace Components.Units
{
    public class MovementComponent
    {
        public Vector2 NextPosition { get; set; }
        public float Speed { get; set; }
        public float RotationSpeed { get; set; }
    }
}