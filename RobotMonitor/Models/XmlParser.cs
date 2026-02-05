using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace RobotMonitor_3.Models
{

    public class SavedData
    {
        public bool SentReceivedSymbol { get; set; }
        public bool ShowTimeStamp { get; set; }
        public int ServerPort { get; set; }
        public string ClientIP { get; set; }
        public int ClientPort { get; set; }
        public bool IsReadHex { get; set; }
        public bool IsWriteHex { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public int dev1StackedShotCount { get; set; }
        public int dev1StackedDeviceCount { get; set; }
        public int dev1UnstackedShotCount { get; set; }
        public int dev1UnstackedDeviceCount { get; set; }
        public int dev2StackedShotCount { get; set; }
        public int dev2StackedDeviceCount { get; set; }
        public int dev2UnstackedShotCount { get; set; }
        public int dev2UnstackedDeviceCount { get; set; }
        public int dev3StackedShotCount { get; set; }
        public int dev3StackedDeviceCount { get; set; }
        public int dev3UnstackedShotCount { get; set; }
        public int dev3UnstackedDeviceCount { get; set; }
        public string keyboardData { get; set; }
        public int startBtnDelay { get; set; }
        public int homeBtnDelay { get; set; }
        public int countResetBtnDelay { get; set; }
        public string PassWord { get; set; }
        public string SavedDeviceName1 { get; set; }
        public string SavedDeviceName2 { get; set; }
        public string SavedDeviceName3 { get; set; }
        public string SavedDeviceName4 { get; set; }
        public string SavedDeviceName5 { get; set; }
        public string SavedDeviceName6 { get; set; }
        public string SavedDeviceName7 { get; set; }
        public string SavedDeviceName8 { get; set; }
        public string SavedDeviceName9 { get; set; }
        public string SavedDeviceName10 { get; set; }
        public int SavedDeviceSize1 { get; set; }
        public int SavedDeviceSize2 { get; set; }
        public int SavedDeviceSize3 { get; set; }
        public int SavedDeviceSize4 { get; set; }
        public int SavedDeviceSize5 { get; set; }
        public int SavedDeviceSize6 { get; set; }
        public int SavedDeviceSize7 { get; set; }
        public int SavedDeviceSize8 { get; set; }
        public int SavedDeviceSize9 { get; set; }
        public int SavedDeviceSize10 { get; set; }

        public SavedData()
        {
            SentReceivedSymbol = true;
            ShowTimeStamp = true;
        }
    }

    public class XmlParser
    {
        public SavedData SavedData { get; set; }
        private string path = (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\RobotMonitor_3");
        private XmlSerializer SavedDataXmlSerializer = new XmlSerializer(typeof(SavedData));

        public XmlParser()
        {
            SavedDataLoad();
        }

        public void SavedDataSave()
        {
            try
            {
                Directory.CreateDirectory(path);

                using (TextWriter tw = new StreamWriter(path + @"\SavedData.xml"))
                {
                    SavedDataXmlSerializer.Serialize(tw, SavedData);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"데이터 저장 실패.\r 에러 : {ex.Message}");
            }
        }

        public void SavedDataLoad()
        {
            try
            {
                if (File.Exists(path + @"\SavedData.xml"))
                {
                    using (TextReader tr = new StreamReader(path + @"\SavedData.xml"))
                    {
                        SavedData = SavedDataXmlSerializer.Deserialize(tr) as SavedData;
                    }
                }
                else
                {
                    SavedData = new SavedData()
                    {
                        SentReceivedSymbol = true,
                        ShowTimeStamp = true,
                        ServerPort = 1541,
                        ClientIP = "192.168.125.20",
                        ClientPort = 1541,
                        IsReadHex = false,
                        IsWriteHex = false,
                        Prefix = "",
                        Suffix = ",",
                        dev1StackedDeviceCount = 0,
                        dev1StackedShotCount = 0,
                        dev1UnstackedDeviceCount = 0,
                        dev1UnstackedShotCount = 0,
                        dev2StackedDeviceCount = 0,
                        dev2StackedShotCount = 0,
                        dev2UnstackedDeviceCount = 0,
                        dev2UnstackedShotCount = 0,
                        dev3StackedDeviceCount = 0,
                        dev3StackedShotCount = 0,
                        dev3UnstackedDeviceCount = 0,
                        dev3UnstackedShotCount = 0,
                        keyboardData = "",
                        startBtnDelay = 0,
                        homeBtnDelay = 0,
                        countResetBtnDelay = 0,
                        PassWord = "1111",
                        SavedDeviceName1  = "",
                        SavedDeviceName2  = "",
                        SavedDeviceName3  = "",
                        SavedDeviceName4  = "",
                        SavedDeviceName5  = "",
                        SavedDeviceName6  = "",
                        SavedDeviceName7  = "",
                        SavedDeviceName8  = "",
                        SavedDeviceName9  = "",
                        SavedDeviceName10 = "",
                        SavedDeviceSize1  = 0,
                        SavedDeviceSize2  = 0,
                        SavedDeviceSize3  = 0,
                        SavedDeviceSize4  = 0,
                        SavedDeviceSize5  = 0,
                        SavedDeviceSize6  = 0,
                        SavedDeviceSize7  = 0,
                        SavedDeviceSize8  = 0,
                        SavedDeviceSize9  = 0,
                        SavedDeviceSize10 = 0
                    };
                }
            }
            catch
            {

            }
        }

        public void SetDefault()
        {
            SavedData.SentReceivedSymbol = true;
            SavedData.ShowTimeStamp = true;
            SavedData.ServerPort = 1541;
            SavedData.ClientIP = "192.168.125.20";
            SavedData.ClientPort = 1541;
            SavedData.IsReadHex = false;
            SavedData.IsWriteHex = false;
            SavedData.Prefix = "";
            SavedData.Suffix = ",";
            SavedData.dev1StackedDeviceCount = 0;
            SavedData.dev1StackedShotCount = 0;
            SavedData.dev1UnstackedDeviceCount = 0;
            SavedData.dev1UnstackedShotCount = 0;
            SavedData.dev2StackedDeviceCount = 0;
            SavedData.dev2StackedShotCount = 0;
            SavedData.dev2UnstackedDeviceCount = 0;
            SavedData.dev2UnstackedShotCount = 0;
            SavedData.dev3StackedDeviceCount = 0;
            SavedData.dev3StackedShotCount = 0;
            SavedData.dev3UnstackedDeviceCount = 0;
            SavedData.dev3UnstackedShotCount = 0;
            SavedData.keyboardData = "";
            SavedData.startBtnDelay = 0;
            SavedData.homeBtnDelay = 0;
            SavedData.countResetBtnDelay = 0;
            SavedData.PassWord = "1111";
            SavedData.SavedDeviceName1 = "";
            SavedData.SavedDeviceName2  = "";
            SavedData.SavedDeviceName3  = "";
            SavedData.SavedDeviceName4  = "";
            SavedData.SavedDeviceName5  = "";
            SavedData.SavedDeviceName6  = "";
            SavedData.SavedDeviceName7  = "";
            SavedData.SavedDeviceName8  = "";
            SavedData.SavedDeviceName9  = "";
            SavedData.SavedDeviceName10 = "";
            SavedData.SavedDeviceSize1  = 0;
            SavedData.SavedDeviceSize2  = 0;
            SavedData.SavedDeviceSize3  = 0;
            SavedData.SavedDeviceSize4  = 0;
            SavedData.SavedDeviceSize5  = 0;
            SavedData.SavedDeviceSize6  = 0;
            SavedData.SavedDeviceSize7  = 0;
            SavedData.SavedDeviceSize8  = 0;
            SavedData.SavedDeviceSize9  = 0;
            SavedData.SavedDeviceSize10 = 0;
        }
    }


}
