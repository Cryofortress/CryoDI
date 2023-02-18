using NUnit.Framework;

namespace CryoDI.Tests
{
    public class TestBuilder
    {
        private class MyClassA
        {
            [Param]
            public int MyParameter { get; private set; }
        }

        private class MyClassB
        {
            [Dependency]
            public IBuilder<MyClassA> Buidler{ get; private set; }
        }

        [Test]
        public void BuilderTest()
        {
            var container = new CryoContainer();
            container.RegisterType<MyClassB>();

            var b = container.Resolve<MyClassB>();
            Assert.That(b.Buidler, Is.Not.Null);

            var a = new MyClassA();
            b.Buidler.BuildUp(a, 2);
            Assert.That(a.MyParameter, Is.EqualTo(2));
        }
    }
}