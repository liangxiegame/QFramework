using QFramework;

public enum UIEventHomeLayer
{
	Began = QMgrID.UI,
	BtnStartClick,
	BtnShopClick,
	BtnSettingClick,
	End,
}

public enum UIEventShopLayer
{
	Began = UIEventHomeLayer.End,
	BtnBackClick,
	End,
}

public enum UIEventSettingLayer
{
	Began = UIEventShopLayer.End,
	BtnBackClick,
	BtnHelpClick,
	End,
}

public enum UIEventHelpLayer
{
	Began = UIEventSettingLayer.End,
	BtnBackClick,
	End,
}

public class GameOverEvent
{
	
}


public enum GameEvent
{
	Began = QMgrID.Game,
	Pause,
	Resume,

	CoinAdd,
	EnergyAdd,
	MeterAdd,
	ResetPropModel,
	ResetGameModel,
	SetMeter,
	SetTimeScale,
	End
}

public enum PlayerEvent
{
	Began = GameEvent.End,
	Land,
	End
}

public enum PropEvent {
	Began = PlayerEvent.End,
	MagnetiteBegan,
	MagnetiteEnded,
	End
}

public enum InfoEvent
{
	Began = QMgrID.PCConnectMobile,
	Reset,
	SetTheme,
	CoinAdd,
	EnergyAdd,
	End,
}