using System;

namespace RythhmMagic
{
	public static class Tools
	{
		public static float RoundUp(this float toRound, float multiple)
		{
			toRound = (float)Math.Round(toRound, 2);
			multiple = (float)Math.Round(multiple, 2);

			if (toRound % multiple == 0) return toRound;
			return (multiple - toRound % multiple) + toRound;
		}

		public static float RoundDown(this float toRound, float multiple)
		{
			toRound = (float)Math.Round(toRound, 2);
			multiple = (float)Math.Round(multiple, 2);

			return toRound - toRound % multiple;
		}

		public static float AutoRound(this float toRound, float multiple)
		{
			toRound = (float)Math.Round(toRound, 2);
			multiple = (float)Math.Round(multiple, 2);

			var roundDownValue = toRound % multiple;
			var roundUpValue = multiple - roundDownValue;

			if (roundUpValue < roundDownValue)
				return RoundUp(toRound, multiple);
			else
				return RoundDown(toRound, multiple);
		}

        public static float Round3Decimal(this float num)
        {
            return (float)Math.Round(num, 3);
        }
	}
}