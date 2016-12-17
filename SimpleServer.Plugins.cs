﻿using DMP9Labs.IO.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServer.Plugins
{
    public interface IPlugin
    {
        void OnEnable();
        void OnDisable();
    }
    public abstract class Plugin : PluginBase, IPlugin
    {
        public abstract void OnEnable();
        public abstract void OnDisable();
        public Logger GetLogger()
        {
            return SimpleServer.PluginLoggerRegistry[this];
        }
    }
    public class Logger : TextWriter
    {
        public override Encoding Encoding
        {
            get
            {
                return Encoding.ASCII;
            }
        }
    }
    public class Loader : PluginProvider
    {
        public static Plugin GetPlugin()
        {
            return null;
        }
    }
}
