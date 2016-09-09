#region File Header
// -------------------------------------------------------------------------------
// 
//  Copyright ABB Lorentzen & Wettre AB 2016
// 
//  The copyright to the computer programs herein is the property of
//  ABB Lorentzen & Wettre AB. The programs may be used and/or copied only with
//  the written permission from ABB Lorentzen & Wettre AB or in accordance with
//  the terms or conditions stipulated in the agreement/contract under which
//  the programs have been supplied.
//  The copyright and the foregoing restriction on reproduction, disclosure and use,
//  extend to all media in which the information may be embodied.
// 
// -------------------------------------------------------------------------------
//  File       : ApplicationSettings.cs
//  Author     : Mikael Brunnegård
//  Description: //TODO - Description of file ApplicationSettings.cs
//  Notes      :
// -------------------------------------------------------------------------------
#endregion

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PomodoroTimer.Configuration
{
    [Serializable]
    public class ApplicationSettings
    {
        private const string CompanyName        = "ABB";
        private const string ProductName        = "PomodoroTimer";
        private const string SettingsFileName   = "PomodoroTimer.Config.xml";

        private int DefaultLazerFocusedTime     = 20;
        private int DefaultProgressCheckingTime = 3;
        private int DefaultBreakTime            = 5;


        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(ApplicationSettings));

        private static string SettingsFullFileName
        {
            get
            {
                string fullName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                fullName = Path.Combine(fullName, CompanyName);
                fullName = Path.Combine(fullName, ProductName);
                fullName = Path.Combine(fullName, SettingsFileName);

                return fullName;
            }
        }

        public int LazerFocusedTime     { get; set; }
        public int ProgressCheckingTime { get; set; }
        public int BreakTime            { get; set; }

        public static ApplicationSettings Instance { get; private set; }


        static ApplicationSettings()
        {
            Load();
        }

        public ApplicationSettings()
        {
        }

        public void UseDefault()
        {
            LazerFocusedTime        = DefaultLazerFocusedTime;
            ProgressCheckingTime    = DefaultProgressCheckingTime;
            BreakTime               = DefaultBreakTime;
        }

        private static void Load()
        {
            Instance = null;

            try
            {
                using (Stream fileStream = new FileStream(SettingsFullFileName, FileMode.Open, FileAccess.Read))
                {
                    using (XmlReader reader = new XmlTextReader(fileStream))
                    {
                        if (Serializer.CanDeserialize(reader))
                        {
                            Instance = Serializer.Deserialize(reader) as ApplicationSettings;
                        }
                    }
                }
            }
            catch (FileNotFoundException exception)
            {
                Instance = null;
                System.Diagnostics.Debug.Print(exception.Message);
            }
            catch (DirectoryNotFoundException exception)
            {
                Instance = null;
                System.Diagnostics.Debug.Print(exception.Message);
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.Assert(
                    false,
                    $"Loading ApplicationSettings failed:{Environment.NewLine}{exception.GetType().Name}: {exception.Message}");
            }

            if (Instance == null)
            {
                Instance = new ApplicationSettings();
                Instance.UseDefault();
                Instance.Save();
            }
        }

        public void Save()
        {
            FileInfo fileInfo = new FileInfo(SettingsFullFileName);

            if (!fileInfo.Exists)
            {
                string directory = fileInfo.DirectoryName;
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }

            using (StreamWriter writer = new StreamWriter(SettingsFullFileName, false, Encoding.Unicode))
            {
                Serializer.Serialize(writer, Instance);
            }
        }

    }
}