namespace FutureNHS.WOPIHost.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Ensure that any new feature flag is initialised in a disabled state to assure adherence to 
    /// the expectations of the automated deployment process
    /// </remarks>
    public sealed class Features
    {
        private bool fileserver_allowFileEdit { get; set; }

        public bool AllowFileEdit => fileserver_allowFileEdit;

    }
}
