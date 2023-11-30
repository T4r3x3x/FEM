namespace ReaserchPaper.Assemblier
{
	public interface IAssemblier
	{
		/// <summary>
		/// Результатом работы метода является изменение передаваемой СЛАУ, плохое решение с точки зрения читаемости и предсказуемости кода, зато сильно повышает производительность.
		/// </summary>
		/// <param name="slae"></param>
		/// <param name="timeLayer">
		/// В общем случае задача может быть элиптическая (тогда параметр просто не указываеться), тогда никакого timeLayer'а не будет, 
		/// но как правильно декомпозировать это я не знаю :(</param>
		/// <returns></returns>
		public void Assembly(Slae slae, int timeLayer = 0);
	}
}
