using Godot;

namespace LittleRiver;

[Tool]
public partial class OrbitCamera : Camera3D
{
    [Export]
    Node3D _target;

    [Export]
    float _distance = 5f;

    [Export]
    float _yOffset = 1f;

    [Export]
    float _xRotationDegrees = 20f;

    [Export]
    float _orbitSpeed = 1f;

    float _orbitAngle = 0f;

    public override void _Process(double delta)
    {
        if (_target == null)
            return;

        _orbitAngle += _orbitSpeed * (float)delta;

        float xRad = Mathf.DegToRad(_xRotationDegrees);
        Vector3 orbitCenter = _target.GlobalPosition + Vector3.Up * _yOffset;
        Vector3 offset =
            new Vector3(
                Mathf.Sin(_orbitAngle) * Mathf.Cos(xRad),
                Mathf.Sin(xRad),
                Mathf.Cos(_orbitAngle) * Mathf.Cos(xRad)
            ) * _distance;

        GlobalPosition = orbitCenter + offset;
        LookAt(orbitCenter);
    }
}
