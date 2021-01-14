using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Raccoon.Devkits.UnitTestHelper
{
    public class ControllerCreateService
    {
        public ControllerCreateService(
            string typeActivatorCacheFieldName = "_typeActivatorCache",
            string createInstanceMethodName = "CreateInstance")
        {
            TypeActivatorCacheFieldName = typeActivatorCacheFieldName;
            CreateInstanceMethodName = createInstanceMethodName;
        }
        public string TypeActivatorCacheFieldName { get; }
        public string CreateInstanceMethodName { get; }

        public TController Create<TController>(IServiceProvider serviceProvider)
            where TController : ControllerBase
        {
            IControllerActivator controllerActivator = serviceProvider
                .GetRequiredService<IControllerActivator>();
            object typeActivatorCache = controllerActivator.GetType()
                .GetField(TypeActivatorCacheFieldName, BindingFlags.Instance | BindingFlags.NonPublic)!
                .GetValue(controllerActivator)!;
            return (typeActivatorCache.GetType().GetMethod(CreateInstanceMethodName)!
                .MakeGenericMethod(typeof(TController)).Invoke(typeActivatorCache, new object[]
                {
                    serviceProvider,
                    typeof(TController)
                }) as TController)!;
        }
    }
}
