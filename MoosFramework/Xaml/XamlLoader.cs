using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Moos.Framework.Xaml
{
    static partial class XamlLoader
    {
        public static void Load(object view, string xaml) => Load(view, xaml, false);
        public static void Load(object view, string xaml, bool useDesignProperties) => Load(view, xaml, null, useDesignProperties);
        public static void Load(object view, string xaml, Assembly rootAssembly) => Load(view, xaml, rootAssembly, false);

        public static void Load(object view, Type callingType)
        {

        }

        public static void Load(object view, string xaml, Assembly rootAssembly, bool useDesignProperties)
        {

        }

        public static object Create(string xaml, bool doNotThrow = false) => Create(xaml, doNotThrow, false);

        public static object Create(string xaml, bool doNotThrow, bool useDesignProperties)
        {
            return null;
        }

        /*
        public static IResourceDictionary LoadResources(string xaml, IResourcesProvider rootView)
        {

        }
        */

        static string GetXamlForType(Type type, object instance, out bool useDesignProperties)
        {
            useDesignProperties = true;

            return null;
        }

        //if the assembly was generated using a version of XamlG that doesn't outputs XamlResourceIdAttributes, we still need to find the resource, and load it
        static readonly Dictionary<Type, string> XamlResources = new Dictionary<Type, string>();

        static string LegacyGetXamlForType(Type type)
        {
            return null;
        }

        //legacy...
        static bool ResourceMatchesFilename(Assembly assembly, string resource, string filename)
        {
            return false;
        }

        //part of the legacy as well...
        static string ReadResourceAsXaml(Type type, Assembly assembly, string likelyTargetName, bool validate = false)
        {
            return null;
        }
     }
}
