namespace ILRuntime.Runtime.Generated
{
    partial class CLRBindings
    {
        static partial void OnInitialize(ILRuntime.Runtime.Enviorment.AppDomain app);
        static partial void OnShutdown(ILRuntime.Runtime.Enviorment.AppDomain app);

        /// <summary>
        /// Initialize the CLR binding, please invoke this AFTER CLR Redirection registration
        /// </summary>
        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            OnInitialize(app);
        }

        /// <summary>
        /// Release the CLR binding, please invoke this BEFORE ILRuntime Appdomain destroy
        /// </summary>
        public static void Shutdown(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            OnShutdown(app);
        }
    }
}
