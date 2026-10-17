using System.Reflection;
using System.Xml.Serialization;
using MistInput;

namespace MBMPlay.Common;

public class PlaySceneKeyConfig
{
	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection BackBar { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection BackBar4 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection BackBarTime { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection ForwardBar { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection ForwardBar4 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection ForwardBarTime { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection IncreaseMusicSpeed { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection DecreaseMusicSpeed { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection ResetMusicSpeed { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection ToggleAutoplay { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection BackToStart { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection BackToStartInTestPlay { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection Reload { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection ChangeLNMode { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection Random { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection SRandom { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection Mirror { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection Flip { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection ClearRandomMirror { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection Rerandomize { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection MasterVolumeUp { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection MasterVolumeUp1 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection MasterVolumeUp10 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection MasterVolumeMax { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection MasterVolumeDown { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection MasterVolumeDown1 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection MasterVolumeDown10 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection MasterVolumeMute { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection KeyVolumeUp { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection KeyVolumeUp1 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection KeyVolumeUp10 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection KeyVolumeMax { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection KeyVolumeDown { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection KeyVolumeDown1 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection KeyVolumeDown10 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection KeyVolumeMute { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection BgmVolumeUp { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection BgmVolumeUp1 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection BgmVolumeUp10 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection BgmVolumeMax { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection BgmVolumeDown { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection BgmVolumeDown1 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection BgmVolumeDown10 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection BgmVolumeMute { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection HiSpeedUp { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection HiSpeedUp1 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection HiSpeedUp005 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection HiSpeedDown { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection HiSpeedDown1 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection HiSpeedDown005 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection HiSpeedReset { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection ChangeHSFix { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection ChangeHSFixBack { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection Player1LaneCoverToggle { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection Player2LaneCoverToggle { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection LaneCoverUp { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection LaneCoverUp10 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection LaneCoverUp01 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection LaneCoverUp005 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection LaneCoverDown { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection LaneCoverDown10 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection LaneCoverDown01 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection LaneCoverDown005 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection LiftUp { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection LiftUp10 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection LiftUp01 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection LiftUp005 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection LiftDown { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection LiftDown10 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection LiftDown01 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection LiftDown005 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection ChangePlaySide { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection DisplayNoteCount { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection JudgeTimingUp { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection JudgeTimingUp10 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection JudgeTimingDown { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection JudgeTimingDown10 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection JudgeTimingReset { get; set; }

	public static PlaySceneKeyConfig Default
	{
		get
		{
			PlaySceneKeyConfig playSceneKeyConfig = new PlaySceneKeyConfig();
			playSceneKeyConfig.BackBar = new InputCollection
			{
				new KeyboardInput(Key.Down),
				new MouseButtonInput(MouseInput.WheelDown)
			};
			playSceneKeyConfig.BackBar4 = new InputCollection
			{
				new KeyboardInput(Key.Shift, Key.Down),
				new MultiDevicesInput(new KeyboardInput(Key.Shift), new MouseButtonInput(MouseInput.WheelDown))
			};
			playSceneKeyConfig.BackBarTime = new InputCollection
			{
				new KeyboardInput(Key.Control, Key.Down),
				new MultiDevicesInput(new KeyboardInput(Key.Control), new MouseButtonInput(MouseInput.WheelDown))
			};
			playSceneKeyConfig.ForwardBar = new InputCollection
			{
				new KeyboardInput(Key.Up),
				new MouseButtonInput(MouseInput.WheelUp)
			};
			playSceneKeyConfig.ForwardBar4 = new InputCollection
			{
				new KeyboardInput(Key.Shift, Key.Up),
				new MultiDevicesInput(new KeyboardInput(Key.Shift), new MouseButtonInput(MouseInput.WheelUp))
			};
			playSceneKeyConfig.ForwardBarTime = new InputCollection
			{
				new KeyboardInput(Key.Control, Key.Up),
				new MultiDevicesInput(new KeyboardInput(Key.Control), new MouseButtonInput(MouseInput.WheelUp))
			};
			playSceneKeyConfig.IncreaseMusicSpeed = new InputCollection
			{
				new KeyboardInput(Key.D8)
			};
			playSceneKeyConfig.DecreaseMusicSpeed = new InputCollection
			{
				new KeyboardInput(Key.D7)
			};
			playSceneKeyConfig.ResetMusicSpeed = new InputCollection
			{
				new KeyboardInput(Key.D7, Key.D8)
			};
			playSceneKeyConfig.ToggleAutoplay = new InputCollection
			{
				new KeyboardInput(Key.F10)
			};
			playSceneKeyConfig.BackToStart = new InputCollection
			{
				new KeyboardInput(Key.Space),
				new MouseButtonInput(MouseInput.CenterClick)
			};
			playSceneKeyConfig.BackToStartInTestPlay = new InputCollection
			{
				new KeyboardInput(Key.Alt, Key.Space),
				new KeyboardInput(Key.Control, Key.Space),
				new MouseButtonInput(MouseInput.CenterClick)
			};
			playSceneKeyConfig.Reload = new InputCollection
			{
				new KeyboardInput(Key.Shift, Key.Space)
			};
			playSceneKeyConfig.ChangeLNMode = new InputCollection
			{
				new KeyboardInput(Key.Control, Key.Shift, Key.Space)
			};
			playSceneKeyConfig.Random = new InputCollection
			{
				new KeyboardInput(Key.F9)
			};
			playSceneKeyConfig.SRandom = new InputCollection
			{
				new KeyboardInput(Key.Shift, Key.F9)
			};
			playSceneKeyConfig.Mirror = new InputCollection
			{
				new KeyboardInput(Key.F8)
			};
			playSceneKeyConfig.Flip = new InputCollection
			{
				new KeyboardInput(Key.Shift, Key.F8)
			};
			playSceneKeyConfig.ClearRandomMirror = new InputCollection
			{
				new KeyboardInput(Key.BackSpace)
			};
			playSceneKeyConfig.Rerandomize = new InputCollection
			{
				new KeyboardInput(Key.F5)
			};
			playSceneKeyConfig.MasterVolumeUp = new InputCollection
			{
				new KeyboardInput(Key.D4)
			};
			playSceneKeyConfig.MasterVolumeUp1 = new InputCollection
			{
				new KeyboardInput(Key.Control, Key.D4)
			};
			playSceneKeyConfig.MasterVolumeUp10 = new InputCollection
			{
				new KeyboardInput(Key.Shift, Key.D4)
			};
			playSceneKeyConfig.MasterVolumeMax = new InputCollection
			{
				new KeyboardInput(Key.Control, Key.Shift, Key.D4)
			};
			playSceneKeyConfig.MasterVolumeDown = new InputCollection
			{
				new KeyboardInput(Key.D3)
			};
			playSceneKeyConfig.MasterVolumeDown1 = new InputCollection
			{
				new KeyboardInput(Key.Control, Key.D3)
			};
			playSceneKeyConfig.MasterVolumeDown10 = new InputCollection
			{
				new KeyboardInput(Key.Shift, Key.D3)
			};
			playSceneKeyConfig.MasterVolumeMute = new InputCollection
			{
				new KeyboardInput(Key.Control, Key.Shift, Key.D3)
			};
			playSceneKeyConfig.KeyVolumeUp = new InputCollection
			{
				new KeyboardInput(Key.W)
			};
			playSceneKeyConfig.KeyVolumeUp1 = new InputCollection
			{
				new KeyboardInput(Key.Control, Key.W)
			};
			playSceneKeyConfig.KeyVolumeUp10 = new InputCollection
			{
				new KeyboardInput(Key.Shift, Key.W)
			};
			playSceneKeyConfig.KeyVolumeMax = new InputCollection
			{
				new KeyboardInput(Key.Control, Key.Shift, Key.W)
			};
			playSceneKeyConfig.KeyVolumeDown = new InputCollection
			{
				new KeyboardInput(Key.Q)
			};
			playSceneKeyConfig.KeyVolumeDown1 = new InputCollection
			{
				new KeyboardInput(Key.Control, Key.Q)
			};
			playSceneKeyConfig.KeyVolumeDown10 = new InputCollection
			{
				new KeyboardInput(Key.Shift, Key.Q)
			};
			playSceneKeyConfig.KeyVolumeMute = new InputCollection
			{
				new KeyboardInput(Key.Control, Key.Shift, Key.Q)
			};
			playSceneKeyConfig.BgmVolumeUp = new InputCollection
			{
				new KeyboardInput(Key.R)
			};
			playSceneKeyConfig.BgmVolumeUp1 = new InputCollection
			{
				new KeyboardInput(Key.Control, Key.R)
			};
			playSceneKeyConfig.BgmVolumeUp10 = new InputCollection
			{
				new KeyboardInput(Key.Shift, Key.R)
			};
			playSceneKeyConfig.BgmVolumeMax = new InputCollection
			{
				new KeyboardInput(Key.Control, Key.Shift, Key.R)
			};
			playSceneKeyConfig.BgmVolumeDown = new InputCollection
			{
				new KeyboardInput(Key.E)
			};
			playSceneKeyConfig.BgmVolumeDown1 = new InputCollection
			{
				new KeyboardInput(Key.Control, Key.E)
			};
			playSceneKeyConfig.BgmVolumeDown10 = new InputCollection
			{
				new KeyboardInput(Key.Shift, Key.E)
			};
			playSceneKeyConfig.BgmVolumeMute = new InputCollection
			{
				new KeyboardInput(Key.Control, Key.Shift, Key.E)
			};
			playSceneKeyConfig.HiSpeedUp = new InputCollection
			{
				new KeyboardInput(Key.D2)
			};
			playSceneKeyConfig.HiSpeedUp1 = new InputCollection
			{
				new KeyboardInput(Key.Shift, Key.D2)
			};
			playSceneKeyConfig.HiSpeedUp005 = new InputCollection
			{
				new KeyboardInput(Key.Control, Key.D2)
			};
			playSceneKeyConfig.HiSpeedDown = new InputCollection
			{
				new KeyboardInput(Key.D1)
			};
			playSceneKeyConfig.HiSpeedDown1 = new InputCollection
			{
				new KeyboardInput(Key.Shift, Key.D1)
			};
			playSceneKeyConfig.HiSpeedDown005 = new InputCollection
			{
				new KeyboardInput(Key.Control, Key.D1)
			};
			playSceneKeyConfig.HiSpeedReset = new InputCollection
			{
				new KeyboardInput(Key.D1, Key.D2)
			};
			playSceneKeyConfig.ChangeHSFix = new InputCollection
			{
				new KeyboardInput(Key.D6)
			};
			playSceneKeyConfig.ChangeHSFixBack = new InputCollection
			{
				new KeyboardInput(Key.D5)
			};
			playSceneKeyConfig.Player1LaneCoverToggle = new InputCollection
			{
				new KeyboardInput(Key.F3)
			};
			playSceneKeyConfig.Player2LaneCoverToggle = new InputCollection
			{
				new KeyboardInput(Key.F4)
			};
			playSceneKeyConfig.LaneCoverUp = new InputCollection
			{
				new KeyboardInput(Key.F1)
			};
			playSceneKeyConfig.LaneCoverUp10 = new InputCollection
			{
				new KeyboardInput(Key.Shift, Key.F1)
			};
			playSceneKeyConfig.LaneCoverUp01 = new InputCollection
			{
				new KeyboardInput(Key.Control, Key.F1)
			};
			playSceneKeyConfig.LaneCoverUp005 = new InputCollection
			{
				new KeyboardInput(Key.Control, Key.Shift, Key.F1)
			};
			playSceneKeyConfig.LaneCoverDown = new InputCollection
			{
				new KeyboardInput(Key.F2)
			};
			playSceneKeyConfig.LaneCoverDown10 = new InputCollection
			{
				new KeyboardInput(Key.Shift, Key.F2)
			};
			playSceneKeyConfig.LaneCoverDown01 = new InputCollection
			{
				new KeyboardInput(Key.Control, Key.F2)
			};
			playSceneKeyConfig.LaneCoverDown005 = new InputCollection
			{
				new KeyboardInput(Key.Control, Key.Shift, Key.F2)
			};
			playSceneKeyConfig.LiftUp = new InputCollection
			{
				new KeyboardInput(Key.PageUp)
			};
			playSceneKeyConfig.LiftUp10 = new InputCollection
			{
				new KeyboardInput(Key.Shift, Key.PageUp)
			};
			playSceneKeyConfig.LiftUp01 = new InputCollection
			{
				new KeyboardInput(Key.Control, Key.PageUp)
			};
			playSceneKeyConfig.LiftUp005 = new InputCollection
			{
				new KeyboardInput(Key.Control, Key.Shift, Key.PageUp)
			};
			playSceneKeyConfig.LiftDown = new InputCollection
			{
				new KeyboardInput(Key.PageDown)
			};
			playSceneKeyConfig.LiftDown10 = new InputCollection
			{
				new KeyboardInput(Key.Shift, Key.PageDown)
			};
			playSceneKeyConfig.LiftDown01 = new InputCollection
			{
				new KeyboardInput(Key.Control, Key.PageDown)
			};
			playSceneKeyConfig.LiftDown005 = new InputCollection
			{
				new KeyboardInput(Key.Control, Key.Shift, Key.PageDown)
			};
			playSceneKeyConfig.ChangePlaySide = new InputCollection
			{
				new KeyboardInput(Key.D0)
			};
			playSceneKeyConfig.DisplayNoteCount = new InputCollection
			{
				new KeyboardInput(Key.D9)
			};
			playSceneKeyConfig.JudgeTimingUp = new InputCollection
			{
				new KeyboardInput(Key.F12)
			};
			playSceneKeyConfig.JudgeTimingUp10 = new InputCollection
			{
				new KeyboardInput(Key.Shift, Key.F12)
			};
			playSceneKeyConfig.JudgeTimingDown = new InputCollection
			{
				new KeyboardInput(Key.F11)
			};
			playSceneKeyConfig.JudgeTimingDown10 = new InputCollection
			{
				new KeyboardInput(Key.Shift, Key.F11)
			};
			playSceneKeyConfig.JudgeTimingReset = new InputCollection
			{
				new KeyboardInput(Key.F11, Key.F12)
			};
			return playSceneKeyConfig;
		}
	}

	public PlaySceneKeyConfig()
	{
	}

	public PlaySceneKeyConfig(PlaySceneKeyConfig keyConfig)
	{
		if (keyConfig == null)
		{
			return;
		}
		PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
		foreach (PropertyInfo propertyInfo in properties)
		{
			if (propertyInfo.CanWrite && propertyInfo.PropertyType == typeof(InputCollection) && propertyInfo.GetIndexParameters().Length == 0)
			{
				object value = propertyInfo.GetValue(keyConfig);
				if (value != null)
				{
					propertyInfo.SetValue(this, new InputCollection((InputCollection)value));
				}
			}
		}
	}
}
