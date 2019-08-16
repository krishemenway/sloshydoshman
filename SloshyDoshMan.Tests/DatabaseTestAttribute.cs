using NUnit.Framework;
using NUnit.Framework.Interfaces;
using SloshyDoshMan.Service;
using System;
using System.Transactions;

namespace SloshyDoshMan.Tests
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class DatabaseTestsAttribute : CategoryAttribute, ITestAction
	{
		static DatabaseTestsAttribute()
		{
			Program.SetupConfiguration(new string[0]);
		}

		public ActionTargets Targets => ActionTargets.Default;

		public void AfterTest(ITest test)
		{
			_transaction.Dispose();
		}

		public void BeforeTest(ITest test)
		{
			_transaction = new TransactionScope();
		}

		private TransactionScope _transaction;
	}
}
