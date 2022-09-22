// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Utils.DeterministicRNG
using System;

namespace Common.Utils
{
	public class DeterministicRNG
	{
		protected const int N = 624;

		protected const int M = 397;

		protected const uint MATRIX_A = 2567483615u;

		protected const uint UPPER_MASK = 2147483648u;

		protected const uint LOWER_MASK = 2147483647u;

		protected const uint TEMPER1 = 2636928640u;

		protected const uint TEMPER2 = 4022730752u;

		protected const int TEMPER3 = 11;

		protected const int TEMPER4 = 7;

		protected const int TEMPER5 = 15;

		protected const int TEMPER6 = 18;

		protected uint[] mt;

		protected int mti;

		private uint[] mag01;

		public DeterministicRNG()
			: this(Environment.TickCount)
		{
		}

		public DeterministicRNG(int seed)
		{
			mt = new uint[624];
			mti = 625;
			mag01 = new uint[2]
			{
				0u,
				2567483615u
			};
			mt[0] = (uint)seed;
			for (int i = 1; i < 624; i++)
			{
				mt[i] = (uint)(1812433253 * (mt[i - 1] ^ (mt[i - 1] >> 30)) + i);
			}
		}

		public uint NextUInt32()
		{
			if (mti >= 624)
			{
				gen_rand_all();
				mti = 0;
			}
			uint num = mt[mti++];
			num ^= num >> 11;
			num = (uint)((int)num ^ ((int)(num << 7) & -1658038656));
			num = (uint)((int)num ^ ((int)(num << 15) & -272236544));
			return num ^ (num >> 18);
		}

		public int Next(int lowerBound, int upperBound)
		{
			int num = upperBound - lowerBound;
			return lowerBound + (int)(NextFloat() * (float)num);
		}

		public float NextFloat()
		{
			return (float)(double)NextUInt32() / 4.2949673E+09f;
		}

		protected void gen_rand_all()
		{
			int num = 1;
			uint num2 = (uint)((int)mt[0] & int.MinValue);
			uint num3;
			do
			{
				num3 = mt[num];
				mt[num - 1] = (mt[num + 396] ^ ((num2 | (num3 & int.MaxValue)) >> 1) ^ mag01[num3 & 1]);
				num2 = (uint)((int)num3 & int.MinValue);
			}
			while (++num < 228);
			do
			{
				num3 = mt[num];
				mt[num - 1] = (mt[num + -228] ^ ((num2 | (num3 & int.MaxValue)) >> 1) ^ mag01[num3 & 1]);
				num2 = (uint)((int)num3 & int.MinValue);
			}
			while (++num < 624);
			num3 = mt[0];
			mt[623] = (mt[396] ^ ((num2 | (num3 & int.MaxValue)) >> 1) ^ mag01[num3 & 1]);
		}
	}
}
