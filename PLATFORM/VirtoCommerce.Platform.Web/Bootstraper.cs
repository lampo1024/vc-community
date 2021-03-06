﻿using Microsoft.Practices.Unity;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Unity.WebApi;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Modularity;
using Common.Logging;

namespace VirtoCommerce.Platform.Web
{
    public class VirtoCommercePlatformWebBootstraper : UnityBootstrapper
    {
        private readonly string _modulesVirtualPath;
        private readonly string _modulesPhysicalPath;
        private readonly string _assembliesPath;
		private static ILog _logger = LogManager.GetLogger("platform");
        public VirtoCommercePlatformWebBootstraper(string modulesVirtualPath, string modulesPhysicalPath, string assembliesPath)
        {
            _modulesVirtualPath = modulesVirtualPath;
            _modulesPhysicalPath = modulesPhysicalPath;
            _assembliesPath = assembliesPath;
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            var manifestProvider = new ModuleManifestProvider(_modulesPhysicalPath);
            return new ManifestModuleCatalog(manifestProvider, _modulesVirtualPath, _assembliesPath);
        }

        /// <summary>
        /// Configures the <see cref="IUnityContainer"/>. May be overwritten in a derived class to add specific
        /// type mappings required by the application.
        /// </summary>
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            var options = new ModuleInitializerOptions();
            Container.RegisterInstance<IModuleInitializerOptions>(options);
        }

        public override void Run(bool runWithDefaultConfiguration)
        {
            base.Run(runWithDefaultConfiguration);

            //registering Unity for MVC
            //DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            //registering Unity for web API
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(Container);

            //It necessary because WEB API does not get assemblies from AppDomain.
            GlobalConfiguration.Configuration.Services.Replace(typeof(IAssembliesResolver), new CustomAssemblyResolver(ModuleCatalog.Modules));

            var moduleCatalog = ModuleCatalog as ManifestModuleCatalog;
            if (moduleCatalog != null)
            {
                Container.RegisterInstance(moduleCatalog.ManifestProvider);
            }

            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            AuthConfig.RegisterAuth();
        }

        public class CustomAssemblyResolver : DefaultAssembliesResolver
        {
            private readonly IEnumerable<ModuleInfo> _modules;

            public CustomAssemblyResolver(IEnumerable<ModuleInfo> modules)
            {
                _modules = modules;
            }

            public override ICollection<Assembly> GetAssemblies()
            {
                var baseAssemblies = base.GetAssemblies();
                var assemblies = new List<Assembly>(baseAssemblies);

                var moduleAssemblies = _modules
                    .Where(m => !string.IsNullOrEmpty(m.Ref))
                    .Select(m => Assembly.LoadFrom(m.Ref))
                    .ToList();

                foreach (var moduleAssembly in moduleAssemblies)
                {
                    AddAssemblyWithReferencesRecursive(moduleAssembly, assemblies);
                }

                return assemblies;
            }


            static void AddAssemblyWithReferencesRecursive(Assembly assembly, List<Assembly> assemblies)
            {
                if (!assemblies.Contains(assembly))
                {
                    assemblies.Add(assembly);

                    var referencedAssemblies = assembly
                        .GetReferencedAssemblies()
                        .Select(LoadAssembly)
                        .Where(a => a != null)
                        .ToList();

                    foreach (var referencedAssembly in referencedAssemblies)
                    {
                        AddAssemblyWithReferencesRecursive(referencedAssembly, assemblies);
                    }
                }
            }

            static Assembly LoadAssembly(AssemblyName name)
            {
                Assembly result = null;

                try
                {
					_logger.DebugFormat("Loading assembly '{0}'.", name);
                    result = Assembly.Load(name);
                }
                catch (FileLoadException)
                {
					_logger.DebugFormat("Cannot load assembly '{0}'.", name);
                }

                if (result == null && name.Version != null)
                {
                    var nameWithoutVersion = (AssemblyName)name.Clone();
                    nameWithoutVersion.Version = null;

                    result = LoadAssembly(nameWithoutVersion);
                }

                return result;
            }
        }
    }
}
