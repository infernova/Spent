using System.Collections;
using System.Collections.Generic;

namespace StarstruckFramework
{
	public static class Math
	{
		private static long currentN;

		public static long Factorial (int n)
		{
			if (n < 0) return 0;
			if (n < 2) return 1;

			long p = 1;
			long r = 1;
			currentN = 1;

			int h = 0, shift = 0, high = 1;
			int log2n = (int)System.Math.Log (n, 2);

			while (h != n)
			{
				shift += h;
				h = n >> log2n--;
				long len = high;
				high = (h - 1) | 1;
				len = (high - len) / 2;

				if (len > 0)
				{
					p *= Product (len);
					r *= p;
				}
			}

			return r << shift;
		}

		private static long Product (long n)
		{
			long m = n / 2;
			if (m == 0) return currentN += 2;
			if (n == 2) return (currentN += 2) * (currentN += 2);
			return Product (n - m) * Product (m);
		}

		public static int CombinationWithRepeat (int n, int k)
		{
			return Combination (n + k - 1, k);
		}

		public static int Combination (int n, int k)
		{
			return (int)(Factorial (n) / (Factorial (k) * Factorial (n - k)));
		}
	}
}