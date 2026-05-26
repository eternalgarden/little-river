using System;
using System.Reactive.Linq;
using Godot;
using Rzeka;

namespace LittleRiver;

public partial class AudioPlayer : Node3D
{
    [Export]
    AudioStreamPlayer _ambienceSoundPlayer;

    [Export]
    AudioStreamPlayer _effectSoundPlayer;

    [Export]
    AudioStream _buttonSound;

    [Export]
    AudioStream _mainMenuAmbience;

    [Export]
    AudioStream _gameAmbience;

    IRzeka rzeka => LittleSource.Rzeka;
    CollectibleDisposable Q { get; set; }

    IDisposable _audioLoopToken;

    public override void _EnterTree()
    {
        Q = new();
        RegisterSpells();
    }

    public override void _Ready() { }

    public override void _ExitTree()
    {
        Q.Dispose();
    }

    void RegisterSpells()
    {
        Q += rzeka.Weave<UIButtonPressed>(
            this,
            pressed =>
                pressed.Subscribe(_ =>
                {
                    _effectSoundPlayer.Stream = _buttonSound;
                    _effectSoundPlayer.Play();
                })
        );

        Q += rzeka.Weave<SceneEnteredTree>(
            this,
            sceneEntered =>
                sceneEntered.Subscribe(scene =>
                {
                    AudioStream stream = scene.Scene switch
                    {
                        SceneEnteredTree.SceneEnum.MainMenu => _mainMenuAmbience,
                        SceneEnteredTree.SceneEnum.Game => _gameAmbience,
                        _ => _mainMenuAmbience,
                    };
                    _audioLoopToken?.Dispose();
                    _ambienceSoundPlayer.Stream = stream;
                    _ambienceSoundPlayer.Play();
                    _audioLoopToken = _ambienceSoundPlayer.OnFinished().Subscribe(_ => _ambienceSoundPlayer.Play());
                })
        );
    }
}
