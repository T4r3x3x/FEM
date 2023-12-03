namespace FemProducer
{
	public static class Tools
	{
		public static int BinarySearch(IList<int> list, int value, int l, int r)
		{
			while (l != r)
			{
				int mid = (l + r) / 2 + 1;

				if (list[mid] > value)
					r = mid - 1;
				else
					l = mid;
			}

			return list[l] == value ? l : -1;
		}
	}
}
