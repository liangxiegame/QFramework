using QFramework;

public class MsgWithInt : QMsg
{
	public int Value;

	public MsgWithInt(ushort msgId, int value) : base(msgId)
	{
		this.Value = value;
	}
}

public class MsgWithFloat : QMsg
{
	public float Value;

	public MsgWithFloat(ushort msgId, float value) : base(msgId)
	{
		this.Value = value;
	}
}