using System;

public static partial class UnityExtensions
{
	[Flags]
	public enum AxisConstraints
	{
		none = 0,
		x = 1,
		y = 2,
		z = 4,
	}

	public enum LoopType
	{
		None,
		Loop,
		Pingpong
	}
	
}
