#if EXANITE_UNIDI && ODIN_INSPECTOR
using System.Collections.Generic;
using UniDi;

namespace Exanite.Core.DependencyInjection
{
    public class DiBindingInstaller : InstallerBase
    {
        private readonly List<ObjectBinding> bindings;

        public DiBindingInstaller(List<ObjectBinding> bindings)
        {
            this.bindings = bindings;
        }

        public override void InstallBindings()
        {
            foreach (var binding in bindings)
            {
                binding.Install(Container);
            }
        }
    }
}
#endif
