using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provodnik
{
    public static class NinjectContext
    {
        private static readonly Lazy<IKernel> lazy =
            new Lazy<IKernel>(() => new StandardKernel(new NinjectConfig()));
        
        public static T Get<T>()
        {
            return lazy.Value.Get<T>(); 
        }
        
        public static IKernel Kernel { get { return lazy.Value; } }

        /*public static void SetUp(params INinjectModule[] modules)
        {
            Kernel = new StandardKernel(modules);
        }*/
    }

    public class NinjectConfig : NinjectModule
    {
        public override void Load()
        {
            Bind<ProvodnikContext>().To<ProvodnikContext>().WithConstructorArgument("nameOrConnectionString", "DefaultConnection");
            /*Bind<SomeRealization>().ToSelf()
            Bind<Interface1>().To<Realization1>();
            Bind<AbstractClass2>().To<Relization2>();
            Bind<Realization3>().To<Realization3>();
            */
        }
    }
}
