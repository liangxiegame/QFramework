namespace Zenject
{
    // Zero parameters
    public class MemoryPool<TValue> : MemoryPoolBase<TValue>, IMemoryPool<TValue>, IFactory<TValue>
    {
        public TValue Spawn()
        {
            var item = GetInternal();

            if (!Container.IsValidating)
            {
#if ZEN_INTERNAL_PROFILING
                using (ProfileTimers.CreateTimedBlock("User Code"))
#endif
#if UNITY_EDITOR
                using (ProfileBlock.Start("{0}.Reinitialize", GetType()))
#endif
                {
                    Reinitialize(item);
                }
            }
            return item;
        }

        protected virtual void Reinitialize(TValue item)
        {
            // Optional
        }

        TValue IFactory<TValue>.Create()
        {
            return Spawn();
        }
    }

    // One parameter
    public class MemoryPool<TParam1, TValue>
        : MemoryPoolBase<TValue>, IMemoryPool<TParam1, TValue>, IFactory<TParam1, TValue>
    {
        public TValue Spawn(TParam1 param)
        {
            var item = GetInternal();

            if (!Container.IsValidating)
            {
#if ZEN_INTERNAL_PROFILING
                using (ProfileTimers.CreateTimedBlock("User Code"))
#endif
#if UNITY_EDITOR
                using (ProfileBlock.Start("{0}.Reinitialize", GetType()))
#endif
                {
                    Reinitialize(param, item);
                }
            }

            return item;
        }

        protected virtual void Reinitialize(TParam1 p1, TValue item)
        {
            // Optional
        }

        TValue IFactory<TParam1, TValue>.Create(TParam1 p1)
        {
            return Spawn(p1);
        }
    }

    // Two parameters
    public class MemoryPool<TParam1, TParam2, TValue>
        : MemoryPoolBase<TValue>, IMemoryPool<TParam1, TParam2, TValue>, IFactory<TParam1, TParam2, TValue>
    {
        public TValue Spawn(TParam1 param1, TParam2 param2)
        {
            var item = GetInternal();

            if (!Container.IsValidating)
            {
#if ZEN_INTERNAL_PROFILING
                using (ProfileTimers.CreateTimedBlock("User Code"))
#endif
#if UNITY_EDITOR
                using (ProfileBlock.Start("{0}.Reinitialize", GetType()))
#endif
                {
                    Reinitialize(param1, param2, item);
                }
            }

            return item;
        }

        protected virtual void Reinitialize(TParam1 p1, TParam2 p2, TValue item)
        {
            // Optional
        }

        TValue IFactory<TParam1, TParam2, TValue>.Create(TParam1 p1, TParam2 p2)
        {
            return Spawn(p1, p2);
        }
    }

    // Three parameters
    public class MemoryPool<TParam1, TParam2, TParam3, TValue>
        : MemoryPoolBase<TValue>, IMemoryPool<TParam1, TParam2, TParam3, TValue>, IFactory<TParam1, TParam2, TParam3, TValue>
    {
        public TValue Spawn(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            var item = GetInternal();

            if (!Container.IsValidating)
            {
#if ZEN_INTERNAL_PROFILING
                using (ProfileTimers.CreateTimedBlock("User Code"))
#endif
#if UNITY_EDITOR
                using (ProfileBlock.Start("{0}.Reinitialize", GetType()))
#endif
                {
                    Reinitialize(param1, param2, param3, item);
                }
            }
            return item;
        }

        protected virtual void Reinitialize(TParam1 p1, TParam2 p2, TParam3 p3, TValue item)
        {
            // Optional
        }

        TValue IFactory<TParam1, TParam2, TParam3, TValue>.Create(TParam1 p1, TParam2 p2, TParam3 p3)
        {
            return Spawn(p1, p2, p3);
        }
    }

    // Four parameters
    public class MemoryPool<TParam1, TParam2, TParam3, TParam4, TValue>
        : MemoryPoolBase<TValue>, IMemoryPool<TParam1, TParam2, TParam3, TParam4, TValue>, IFactory<TParam1, TParam2, TParam3, TParam4, TValue>
    {
        public TValue Spawn(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            var item = GetInternal();

            if (!Container.IsValidating)
            {
#if ZEN_INTERNAL_PROFILING
                using (ProfileTimers.CreateTimedBlock("User Code"))
#endif
#if UNITY_EDITOR
                using (ProfileBlock.Start("{0}.Reinitialize", GetType()))
#endif
                {
                    Reinitialize(param1, param2, param3, param4, item);
                }
            }
            return item;
        }

        protected virtual void Reinitialize(TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4, TValue item)
        {
            // Optional
        }

        TValue IFactory<TParam1, TParam2, TParam3, TParam4, TValue>.Create(TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4)
        {
            return Spawn(p1, p2, p3, p4);
        }
    }

    // Five parameters
    public class MemoryPool<TParam1, TParam2, TParam3, TParam4, TParam5, TValue>
        : MemoryPoolBase<TValue>, IMemoryPool<TParam1, TParam2, TParam3, TParam4, TParam5, TValue>, IFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TValue>
    {
        public TValue Spawn(
            TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
        {
            var item = GetInternal();
            if (!Container.IsValidating)
            {
#if ZEN_INTERNAL_PROFILING
                using (ProfileTimers.CreateTimedBlock("User Code"))
#endif
#if UNITY_EDITOR
                using (ProfileBlock.Start("{0}.Reinitialize", GetType()))
#endif
                {
                    Reinitialize(param1, param2, param3, param4, param5, item);
                }
            }
            return item;
        }

        protected virtual void Reinitialize(TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4, TParam5 p5, TValue item)
        {
            // Optional
        }

        TValue IFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TValue>.Create(TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4, TParam5 p5)
        {
            return Spawn(p1, p2, p3, p4, p5);
        }
    }

    // Six parameters
    public class MemoryPool<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TValue>
        : MemoryPoolBase<TValue>, IMemoryPool<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TValue>,
        IFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TValue>
    {
        public TValue Spawn(
            TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6)
        {
            var item = GetInternal();

            if (!Container.IsValidating)
            {
#if ZEN_INTERNAL_PROFILING
                using (ProfileTimers.CreateTimedBlock("User Code"))
#endif
#if UNITY_EDITOR
                using (ProfileBlock.Start("{0}.Reinitialize", GetType()))
#endif
                {
                    Reinitialize(param1, param2, param3, param4, param5, param6, item);
                }
            }
            return item;
        }

        protected virtual void Reinitialize(TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4, TParam5 p5, TParam6 p6, TValue item)
        {
            // Optional
        }

        TValue IFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TValue>.Create(TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4, TParam5 p5, TParam6 p6)
        {
            return Spawn(p1, p2, p3, p4, p5, p6);
        }
    }

    // Seven parameters
    public class MemoryPool<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TValue>
        : MemoryPoolBase<TValue>, IMemoryPool<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TValue>,
        IFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TValue>
    {
        public TValue Spawn(
            TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7)
        {
            var item = GetInternal();

            if (!Container.IsValidating)
            {
#if ZEN_INTERNAL_PROFILING
                using (ProfileTimers.CreateTimedBlock("User Code"))
#endif
#if UNITY_EDITOR
                using (ProfileBlock.Start("{0}.Reinitialize", GetType()))
#endif
                {
                    Reinitialize(param1, param2, param3, param4, param5, param6, param7, item);
                }
            }
            return item;
        }

        protected virtual void Reinitialize(TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4, TParam5 p5, TParam6 p6, TParam7 p7, TValue item)
        {
            // Optional
        }

        TValue IFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TValue>.Create(TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4, TParam5 p5, TParam6 p6, TParam7 p7)
        {
            return Spawn(p1, p2, p3, p4, p5, p6, p7);
        }
    }

    // Eight parameters
    public class MemoryPool<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TValue>
        : MemoryPoolBase<TValue>, IMemoryPool<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TValue>,
        IFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TValue>
    {
        public TValue Spawn(
            TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7, TParam8 param8)
        {
            var item = GetInternal();

            if (!Container.IsValidating)
            {
#if ZEN_INTERNAL_PROFILING
                using (ProfileTimers.CreateTimedBlock("User Code"))
#endif
#if UNITY_EDITOR
                using (ProfileBlock.Start("{0}.Reinitialize", GetType()))
#endif
                {
                    Reinitialize(param1, param2, param3, param4, param5, param6, param7, param8, item);
                }
            }
            return item;
        }

        protected virtual void Reinitialize(TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4, TParam5 p5, TParam6 p6, TParam7 p7, TParam8 p8, TValue item)
        {
            // Optional
        }

        TValue IFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TValue>.Create(
            TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4, TParam5 p5, TParam6 p6, TParam7 p7, TParam8 p8)
        {
            return Spawn(p1, p2, p3, p4, p5, p6, p7, p8);
        }
    }
}
