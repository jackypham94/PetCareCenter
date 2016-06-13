using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PhotoSharingApp.Universal.Models;

namespace PhotoSharingApp.Universal.Services
{
    class Authentication
    {
        public ReturnUser CurrentUser { get; set; }

        public void GetCurrentUser()
        {
            DeserelizeDataFromJson("user");
        }

        public void LogOut()
        {
            //ReturnUser user = new ReturnUser();
            SerelizeDataToJson("user");
        }

        public void SerelizeDataToJson(string fileName)
        {
            var folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var filePath = folder.Path + @"\" + fileName + ".json";
            using (StreamWriter file = File.CreateText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, "");
            }
        }

        private void DeserelizeDataFromJson(string fileName)
        {
            CurrentUser = new ReturnUser();
            var folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var filePath = folder.Path + @"\" + fileName + ".json";
            using (StreamReader file = File.OpenText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                CurrentUser = (ReturnUser)serializer.Deserialize(file, typeof(ReturnUser));
                if (CurrentUser != null)
                {
                    CurrentUser.Password = Base64Decode(CurrentUser.Password);
                }
            }
        }
        private static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
