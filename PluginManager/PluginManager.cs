using System;
using System.IO;
using System.Reflection;
using Interfaces;

namespace PluginManager
{
    public class PluginManager : IPluginManager
    {
        private IAvailablePlugins _colAvailablePlugins = new AvailablePlugins();
        private readonly IAvailablePlugins _colUninitializedPlugins = new AvailablePlugins();

        /// <summary>
        /// A Collection of all Plugins Found and Loaded by the FindPlugins() Method
        /// </summary>
        public IAvailablePlugins AvailablePlugins
        {
            get { return _colAvailablePlugins; }
            set { _colAvailablePlugins = value; }
        }

        public IAvailablePlugin GetPluginByName(String name)
        {
            //Find the plugin on which was clicked
            foreach (AvailablePlugin plugin in AvailablePlugins)
            {
                if (name == plugin.Instance.Name)
                {
                    return plugin;
                }
            }

            return null;
        }

        /// <summary>
        /// Searches the Application's Startup Directory for Plugins
        /// </summary>
        public void FindPlugins(IPluginHost pluginHost)
        {
            FindPlugins(AppDomain.CurrentDomain.BaseDirectory, pluginHost);
        }

        /// <summary>
        /// Searches the passed path for Plugins
        /// </summary>
        /// <param name="path">Directory to search for Plugins in</param>
        /// <param name="pluginHost"></param>
        public void FindPlugins(string path, IPluginHost pluginHost)
        {
            //First empty the collection, we're reloading them all
            _colAvailablePlugins.Clear();

            //Go through all the folders in the plugin directory
            foreach (string folder in Directory.GetDirectories(path))
            {
                //Go through all the files in the plugin directory
                foreach (string fileOn in Directory.GetFiles(folder))
                {
                    FileInfo file = new FileInfo(fileOn);

                    //Preliminary check, must be .dll
                    if (file.Extension.Equals(".dll"))
                    {
                        //Add the 'plugin'
                        AddPlugin(fileOn, pluginHost);
                    }
                }
            }
        }

        /// <summary>
        /// Unloads and Closes all AvailablePlugins
        /// </summary>
        public void ClosePlugins()
        {
            foreach (IAvailablePlugin pluginOn in _colAvailablePlugins)
            {
                //Close all plugin instances
                //We call the plugins Dispose sub first incase it has to do 
                //Its own cleanup stuff
                if (pluginOn.Instance != null) 
                    pluginOn.Instance.Dispose();

                //After we give the plugin a chance to tidy up, get rid of it
                pluginOn.Instance = null;
            }

            //Finally, clear our collection of available plugins
            _colAvailablePlugins.Clear();
        }

        public IAvailablePlugins GetUnloadedPlugins()
        {
            return _colUninitializedPlugins;
        }

        /// <summary>
        /// Check if all dependencies of a plugins are loaded
        /// </summary>
        /// <param name="plugin">Plugin to check</param>
        /// <returns>Returns if all plugins were loaded</returns>
        private bool AllDependenciesAvailable(IAvailablePlugin plugin)
        {
            bool allDependenciesLoaded = true;

            foreach (String dependency in plugin.Instance.getDependencies())
            {
                bool foundPlugin = false;
                foreach (AvailablePlugin loadedPlugin in _colAvailablePlugins)
                {
                    if (loadedPlugin.Instance.Name != dependency) 
                        continue;
                    
                    foundPlugin = true;
                    break;
                }

                if (foundPlugin) 
                    continue;
                
                allDependenciesLoaded = false;
                break;
            }

            return allDependenciesLoaded;
        }

        /// <summary>
        /// Try to initialize unintialized plugins
        /// </summary>
        private void ReCheckUninitializedPlugins()
        {
            foreach (AvailablePlugin plugin in _colUninitializedPlugins)
            {
                if (!AllDependenciesAvailable(plugin)) 
                    continue;
                
                _colUninitializedPlugins.Remove(plugin);
                AddPlugin(plugin);

                //Return because AddPlugin also calls this function
                return;
            }
        }

        private void AddPlugin(IAvailablePlugin newPlugin)
        {
            //Call the initialization sub of the plugin
            newPlugin.Instance.Initialize();

            //Add the new plugin to our collection here
            _colAvailablePlugins.Add(newPlugin);

            //Try to initialize unintialized plugins
            ReCheckUninitializedPlugins();
        }

        private void AddPlugin(string fileName, IPluginHost pluginHost)
        {
            //Create a new assembly from the plugin file we're adding..
            Assembly pluginAssembly = Assembly.LoadFrom(fileName);

            //Next we'll loop through all the Types found in the assembly
            foreach (Type pluginType in pluginAssembly.GetTypes())
            {
                if (!pluginType.IsPublic) continue;
                if (pluginType.IsAbstract) continue;

                //Gets a type object of the interface we need the plugins to match
                Type typeInterface = pluginType.GetInterface("Interfaces.IPlugin", true);

                //Make sure the interface we want to use actually exists
                if (typeInterface != null)
                {
                    //Create a new available plugin since the type implements the IPlugin interface
                    AvailablePlugin newPlugin = new AvailablePlugin
                                                          {
                                                              AssemblyPath = fileName,
                                                              Instance = (IPlugin)Activator.CreateInstance(
                                                                      pluginAssembly.GetType(pluginType.ToString()))
                                                          };

                    //Set the filename where we found it

                    //Create a new instance and store the instance in the collection for later use
                    //We could change this later on to not load an instance.. we have 2 options
                    //1- Make one instance, and use it whenever we need it.. it's always there
                    //2- Don't make an instance, and instead make an instance whenever we use it, then close it
                    //For now we'll just make an instance of all the plugins

                    //Set the Plugin's host to this class which inherited IPluginHost
                    newPlugin.Instance.Host = pluginHost;

                    //Check if the Plugin has dependencies to other plugins which
                    //were not loaded yet
                    if (AllDependenciesAvailable(newPlugin))
                    {
                        AddPlugin(newPlugin);
                    }
                    else
                    {
                        //Add plugin with unloaded dependencies in a list
                        _colUninitializedPlugins.Add(newPlugin);
                    }

                    //cleanup a bit
                    newPlugin = null;
                }

                typeInterface = null; //Mr. Clean			
            }

            pluginAssembly = null; //more cleanup
        }

    }
}
