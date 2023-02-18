using NUnit.Framework;

namespace CryoDI.Tests
{
    public class TestResolver
    {
        private class MyClassA
        {
            
        }
        
        private class MyClassB
        {
            [Dependency]
            public IResolver<MyClassA> Resolver { get; private set; }
        }
        
        private class MyClassC
        {
            [Param]
            public int MyParameter { get; private set; } 
        }
        
        private class MyClassD
        {
            [Dependency]
            public IResolver<MyClassC> Resolver { get; private set; }
        }

        [Test]
        public void SimpleResolveTest()
        {
            var container = new CryoContainer();
            container.RegisterType<MyClassA>();
            container.RegisterType<MyClassB>();

            var b = container.Resolve<MyClassB>();
            Assert.That(b.Resolver, Is.Not.Null);
            var resolved = b.Resolver.Resolve();
            Assert.That(resolved, Is.Not.Null);
            var resolved2 = b.Resolver.Resolve();
            Assert.That(resolved2, Is.Not.Null);
            Assert.That(resolved2, Is.Not.SameAs(resolved));
        }
        
        [Test]
        public void ResolveWithParametersTest()
        {
            var container = new CryoContainer();
            container.RegisterType<MyClassC>();
            container.RegisterType<MyClassD>();

            var b = container.Resolve<MyClassD>();
            Assert.That(b.Resolver, Is.Not.Null);
            var resolved = b.Resolver.Resolve(2);
            Assert.That(resolved, Is.Not.Null);
            Assert.That(resolved.MyParameter, Is.EqualTo(2));
        }
        
        [Test]
        public void ResolveByNameTest()
        {
            var container = new CryoContainer();

            var instanceOne = new MyClassA();
            var instanceTwo = new MyClassA();
            container.RegisterInstance<MyClassA>(instanceOne, "One");
            container.RegisterInstance<MyClassA>(instanceTwo, "Two");
            container.RegisterType<MyClassB>();

            var b = container.Resolve<MyClassB>();
            Assert.That(b.Resolver, Is.Not.Null);
            var resolvedOne = b.Resolver.ResolveByName("One");
            Assert.That(resolvedOne, Is.SameAs(instanceOne));
            var resolvedTwo = b.Resolver.ResolveByName("Two");
            Assert.That(resolvedTwo, Is.SameAs(instanceTwo));
        }
        
        [Test]
        public void CanResolveTest()
        {
            var container = new CryoContainer();
            
            container.RegisterType<MyClassB>();

            var b = container.Resolve<MyClassB>();
            Assert.That(b.Resolver, Is.Not.Null);
            Assert.That(b.Resolver.CanResolve(), Is.False);
            
            container.RegisterType<MyClassA>();
            Assert.That(b.Resolver.CanResolve(), Is.True);
        }
        
        [Test]
        public void CanResolveByNameTest()
        {
            var container = new CryoContainer();
            
            container.RegisterType<MyClassB>();

            var b = container.Resolve<MyClassB>();
            Assert.That(b.Resolver, Is.Not.Null);
            Assert.That(b.Resolver.CanResolveByName("One"), Is.False);
            
            container.RegisterType<MyClassA>();
            Assert.That(b.Resolver.CanResolveByName("One"), Is.False);
            
            container.RegisterType<MyClassA>("One");
            Assert.That(b.Resolver.CanResolve(), Is.True);
        }
    }
}