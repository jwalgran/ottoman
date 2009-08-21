using MbUnit.Framework;

namespace SineSignal.Ottoman.Tests
{
	public class OttomanSpecBase<T> where T : class
	{
		protected T Sut { get; private set; }

		[SetUp]
		public void SetUp()
		{
			Sut = EstablishContext();
			Because();
		}

		[TearDown]
		public void TearDown()
		{
			AfterEachSpecification();
		}

		protected virtual T EstablishContext()
		{
			return default(T);
		}

		protected virtual void Because() { }
		protected virtual void AfterEachSpecification() { }
	}
}