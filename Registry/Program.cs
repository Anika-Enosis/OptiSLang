using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace GetRegistry
{
    class Program
    {
        static RegistryKey GetLatestVersion(List<RegistryKey> Keys)
        {
            List<string> Versions = new List<string>();
            List<string> CopyOfversions = new List<string>();
            //List<int> PositionOfKey = new List<int>();
            int i = 0;
            foreach (RegistryKey Key in Keys)
            {
                string KeyString = Key.ToString();
                string[] PathValues = KeyString.Split("\\");
                string Version = (PathValues[PathValues.Length - 1]);
                Versions.Add(Version);
                //PositionOfKey.Add(i);
                i++;

            }

            CopyOfversions = Versions;
            CopyOfversions.Sort();
            CopyOfversions.Reverse();

            int Index = Versions.IndexOf(CopyOfversions[0]);

            /*int LatestVersion = versions[0];
            int PositionOfLatestVersionInList = PositionOfKey[0];

            for(int j = 1; j < versions.Count; j++)
            {
                if(versions[j] > LatestVersion)
                {
                    LatestVersion = versions[j];
                    PositionOfLatestVersionInList = PositionOfKey[j];
                }
            }*/

            return Keys[Index];
        }

        static string GetRegistryValue(RegistryKey Key)
        {
            string Path = "";
            foreach (string ValueName in Key.GetValueNames())
            {
                //Console.WriteLine("{0,-8}: {1}", valueName,  key.GetValue(valueName).ToString());

                if (ValueName == "InstallPath")
                {
                    Path = Key.GetValue(ValueName).ToString();
                }
            }
            return Path;
        }
        static string GetOptiSLangExecutable()
        {
            string OsOptslAnsysRegPath = "SOFTWARE\\Dynardo\\ANSYS optiSLang";
            string OsOptslRegPath = "SOFTWARE\\Dynardo\\optiSLang";
            string[] RegPaths = { OsOptslAnsysRegPath, OsOptslRegPath };

            List<RegistryKey> Keys = CollectSubKeys(RegPaths);

            RegistryKey LatestVersion = GetLatestVersion(Keys);

            string InstallationPath = GetRegistryValue(LatestVersion);

            // Done only for Windows 
            string ExecutableName = "optislang.exe";

            string OptislangInstallation = InstallationPath + ExecutableName;

            foreach (RegistryKey Key in Keys)
            {
                Key.Close();
            }

            return OptislangInstallation;
            //Console.WriteLine(LatestVersion);


        }

        static List<RegistryKey> CollectSubKeys(string[] RegPaths)
        {
            
            List<RegistryKey> Keys = new List<RegistryKey>(); 
            foreach (string Path in RegPaths)
            {
                
                try
                {
                    RegistryKey Key = Registry.LocalMachine.OpenSubKey(Path);

                    if(Key != null)
                    {
                        foreach (string SubKeyName in Key.GetSubKeyNames())
                        {
                            RegistryKey TempKey = Key.OpenSubKey(SubKeyName);
                            Keys.Add(TempKey);
                            
                        }

                        Key.Close();
                    }
                    

                    RegistryKey Key1 = Registry.CurrentUser.OpenSubKey(Path);

                    if (Key1 != null)
                    {
                        foreach (string SubKeyName in Key1.GetSubKeyNames())
                        {
                            RegistryKey TempKey = Key1.OpenSubKey(SubKeyName);
                            Keys.Add(TempKey);
                            
                        }

                        Key1.Close();
                    }
                    
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }

                if(Keys.Count == 0)
                {
                    continue;
                }

                
            }
            return Keys;

        }
        static void Main(string[] args)
        {
            string OptislangInstallation = GetOptiSLangExecutable();
            Console.WriteLine(OptislangInstallation);
        }
    }
}
