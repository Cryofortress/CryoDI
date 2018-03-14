using System;

namespace CryoDI.Tests
{
    public class SingletonCounter : IDisposable
    {
        public static int CreatedCount { get; private set; }
        public static int DisposedCount { get; private set; }

        public int InstanceId { get; private set; }

        public SingletonCounter()
        {
            InstanceId = ++CreatedCount;
        }

        public void Dispose()
        {
            DisposedCount++;
        }
    }

    public class TypeCounter : IDisposable
    {
        public static int CreatedCount { get; private set; }
        public static int DisposedCount { get; private set; }

        public int InstanceId { get; private set; }

        public TypeCounter()
        {
            InstanceId = ++CreatedCount;
        }

        public void Dispose()
        {
            DisposedCount++;
        }
    }

    public class InstanceCounter : IDisposable
    {
        public static int CreatedCount { get; private set; }
        public static int DisposedCount { get; private set; }

        public int InstanceId { get; private set; }

        public InstanceCounter()
        {
            InstanceId = ++CreatedCount;
        }

        public void Dispose()
        {
            DisposedCount++;
        }
    }
}