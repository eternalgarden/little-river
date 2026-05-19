using Godot;

namespace LittleRiver;

[Tool]
public partial class FloatEffect : Node3D
{
    [Export] float _floatSpeed = 1f;
    [Export] float _maxHeightOffset = 0.5f;
    [Export] float _rotationSpeed = 1f;

    Vector3 _initialPosition;
    float _time = 0f;

    public override void _EnterTree()
    {
        SetProcess(true);
        _initialPosition = Position;
    }

    public override void _Process(double delta)
    {
        _time += _floatSpeed * (float)delta;
        Position = new Vector3(
            _initialPosition.X,
            _initialPosition.Y + Mathf.Sin(_time) * _maxHeightOffset,
            _initialPosition.Z);
        RotateY(_rotationSpeed * (float)delta);
    }
}
