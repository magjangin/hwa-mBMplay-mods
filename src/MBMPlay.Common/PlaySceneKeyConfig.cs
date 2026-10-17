using System.Reflection;
using System.Xml.Serialization;
using MistInput;

namespace MBMPlay.Common;

public partial class PlaySceneKeyConfig
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
