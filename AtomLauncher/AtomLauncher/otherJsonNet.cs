﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace AtomLauncher
{
    class otherJsonNet
    {
        /// <summary>
        /// Parses the minecraft json related to a specific version.
        /// </summary>
        /// <param name="jsonFile">The json file for a sepcific version. Example: "C:\LOCATION\1.6.2.json"</param>
        /// <param name="useNightly">Whether to use the nightly builds or not. Could be unstable.</param>
        /// <returns>Returns a dictonary with all of the elements.</returns>
        internal static Dictionary<string, string[]> getVersionData(string jsonFile, bool useNightly = false)
        {
            var json = System.IO.File.ReadAllText(jsonFile);
            dynamic version = JsonConvert.DeserializeObject(json);
            string[] libraries = { "" };
            int l = 0;
            string[] natives = { "" };
            int n = 0;
            foreach (var param in version.libraries)
            {
                bool isNative = false;
                bool addFile = true;
                if (param.rules != null)
                {
                    if (param.rules[0].action == "allow")
                    {
                        if (useNightly)
                        {
                            if (param.rules[0].os == null)
                            {
                                addFile = false;
                            }
                        }
                        else
                        {
                            if (param.rules[0].os != null)
                            {
                                addFile = false;
                            }
                        }
                    }
                }

                if (addFile)
                {
                    string fileName = param.name;
                    string[] colonSplit = fileName.Split(new char[] { ':' }, 3);
                    string[] folderSplit = colonSplit[0].Split(new char[] { '.' });
                    string compileFolder = "";
                    for (int a = 0; a < folderSplit.Length; a++)
                    {
                        if (a == 0)
                        {
                            compileFolder = folderSplit[a];
                        }
                        else
                        {
                            compileFolder = compileFolder + @"\" + folderSplit[a];
                        }
                    }
                    if (param.natives != null)
                    {
                        isNative = true;
                        compileFolder = compileFolder + @"\" + colonSplit[1] + @"\" + colonSplit[2] + @"\" + colonSplit[1] + "-" + colonSplit[2] + "-" + param.natives.windows + ".jar";
                    }
                    else
                    {
                        compileFolder = compileFolder + @"\" + colonSplit[1] + @"\" + colonSplit[2] + @"\" + colonSplit[1] + "-" + colonSplit[2] + ".jar";
                    }
                    if (l > libraries.Length - 1)
                    {
                        Array.Resize(ref libraries, libraries.Length + 1);
                    }
                    libraries[l] = compileFolder;
                    l++;
                    if (isNative)
                    {
                        if (n > natives.Length - 1)
                        {
                            Array.Resize(ref natives, natives.Length + 1);
                        }
                        natives[n] = compileFolder;
                        n++;
                    }
                }
            }
            Dictionary<string, string[]> dict = new Dictionary<string, string[]>{
                {"id"                  , new string[] { version.id }},
                {"time"                , new string[] { version.time }},
                {"releaseTime"         , new string[] { version.releaseTime }},
                {"Type"                , new string[] { version.type }},
                {"minecraftArguments"  , new string[] { version.minecraftArguments }},
                {"mainClass"           , new string[] { version.mainClass }},
              //{"libraries"           , new string[] { "net/sf/jopt-simple/jopt-simple/4.5/jopt-simple-4.5.jar" "etc" "etc" }},
                {"libraries"           , libraries },
              //{"natives"             , new string[] { "net/sf/jopt-simple/jopt-simple/4.5/jopt-simple-4.5.jar" "etc" "etc" }},
                {"natives"             , natives   }
            };
            return dict;
        }

        /// <summary>
        /// Parses the minecraft verion list.
        /// </summary>
        /// <param name="jsonFile">The json file for the list of versions. Example: "C:\LOCATION\verions.json"</param>
        /// <returns>Returns a dictonary with all of the elements.</returns>
        internal static Dictionary<string, string[]> getVersionList(string jsonFile)
        {
            var json = System.IO.File.ReadAllText(jsonFile);
            dynamic version = JsonConvert.DeserializeObject(json);
            Dictionary<string, string[]> dict = new Dictionary<string, string[]>{
                {"AL_LatestID", new string[] { version.latest.release, version.latest.snapshot }},
              //{"1.6.4",       new string[] { "time", "releaseTime", "Type" }}
            };
            foreach (var entry in version.versions)
            {
                string keyString = entry.id;
                string[] arrString = { entry.time, entry.releaseTime, entry.type };
                dict.Add(keyString, arrString); //Odd errors if put in directly.
            }
            return dict;
        }
    }
}