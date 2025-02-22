﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FastReport.Web
{
    class Resources
    {
        #region Singletone

        public static readonly Resources Instance;
        static readonly string AssemblyName;
        static readonly Assembly _assembly;

        static Resources()
        {
            Instance = new Resources();

            _assembly = Assembly.GetExecutingAssembly();
            AssemblyName = _assembly.GetName().Name;
        }

        private Resources()
        {
        }

        #endregion

        readonly ConcurrentDictionary<string, string> cache1 = new ConcurrentDictionary<string, string>();
        readonly ConcurrentDictionary<string, byte[]> cache2 = new ConcurrentDictionary<string, byte[]>();

        public async Task<string> GetContent(string name)
        {
            if (cache1.TryGetValue(name, out string value))
                return value;

            var fullname = $"{AssemblyName}.Resources.{name}";
            var resourceStream = _assembly.GetManifestResourceStream(fullname);
            if (resourceStream == null)
                return null;

            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                var res = await reader.ReadToEndAsync();
                cache1[name] = res;
                return res;
            }
        }

        public string GetContentSync(string name)
        {
            if (cache1.TryGetValue(name, out string value))
                return value;

            var fullname = $"{AssemblyName}.Resources.{name}";
            var resourceStream = _assembly.GetManifestResourceStream(fullname);
            if (resourceStream == null)
                return null;

            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                var res = reader.ReadToEnd();
                cache1[name] = res;
                return res;
            }
        }

        public async Task<byte[]> GetBytes(string name)
        {
            if (cache2.TryGetValue(name, out byte[] value))
                return value;

            var fullname = $"{AssemblyName}.Resources.{name}";
            var resourceStream = _assembly.GetManifestResourceStream(fullname);
            if (resourceStream == null)
                return null;

            var buffer = new byte[resourceStream.Length];
            await resourceStream.ReadAsync(buffer, 0, buffer.Length);
            cache2[name] = buffer;
            return buffer;
        }
    }
}
