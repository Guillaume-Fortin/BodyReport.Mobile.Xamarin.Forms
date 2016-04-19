using System;
using BodyReportMobile.Core.Framework;
using Android.Content;
using Java.Security;
using Android.Runtime;
using Javax.Crypto;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BodyReport.Droid
{
    /// <summary>
    /// Take on https://github.com/xamarin/Xamarin.Auth/blob/master/src/Xamarin.Auth.Android/AndroidAccountStore.cs
    /// </summary>
	public class SecurityDroid : ISecurity
	{
		public void SaveUserInfo (string userId, string userName, string password)
		{
            CreateStore();

            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add("userId", userId);
            values.Add("userName", userName);
            values.Add("password", password);
            Save(values, "BodyReport");
        }

		public bool GetUserInfo (out string userId, out string userName, out string password)
		{
			bool result = false;
			userId = password = userName = string.Empty;

			try
			{
                CreateStore();
                var listValues = FindAccountsForService("BodyReport");
                if (listValues.ContainsKey("userId") && listValues.ContainsKey("userName") && listValues.ContainsKey("password"))
                {
                    userId = listValues["userId"];
                    userName = listValues["userName"];
                    password = listValues["password"];
                    result = true;
                }
            }
            catch
			{
			}

            return result;
		}

        Context context;
        KeyStore ks;
        KeyStore.PasswordProtection prot;
        static readonly object fileLock = new object();
        const string FileName = "BodyReport.Accounts";
        static readonly char[] Password = null;

        private void CreateStore()
        {
            this.context = Android.App.Application.Context;
            ks = KeyStore.GetInstance(KeyStore.DefaultType);
            prot = new KeyStore.PasswordProtection(Password);
            try
            {
                lock (fileLock)
                {
                    using (var s = context.OpenFileInput(FileName))
                    {
                        ks.Load(s, Password);
                    }
                }
            }
            catch (Java.IO.FileNotFoundException)
            {
                //ks.Load (null, Password);
                LoadEmptyKeyStore(Password);
            }
        }

        public Dictionary<string, string> FindAccountsForService(string serviceId)
        {
            Dictionary<string, string> values = null;

            var postfix = "-" + serviceId;

            var aliases = ks.Aliases();
            while (aliases.HasMoreElements)
            {
                var alias = aliases.NextElement().ToString();
                if (alias.EndsWith(postfix))
                {
                    var e = ks.GetEntry(alias, prot) as KeyStore.SecretKeyEntry;
                    if (e != null)
                    {
                        var bytes = e.SecretKey.GetEncoded();
                        var serializedValues = System.Text.Encoding.UTF8.GetString(bytes);
                        values = JsonConvert.DeserializeObject<Dictionary<string, string>>(serializedValues);
                    }
                }
            }
            return values;
        }

        public void Save(Dictionary<string, string> values, string serviceId)
        {
            var alias = MakeAlias(serviceId);

            var secretKey = new SecretAccount(values);
            var entry = new KeyStore.SecretKeyEntry(secretKey);
            ks.SetEntry(alias, entry, prot);

            Save();
        }

        static string MakeAlias(string serviceId)
        {
            return "-" + serviceId;
        }

        void Save()
        {
            lock (fileLock)
            {
                using (var s = context.OpenFileOutput(FileName, FileCreationMode.Private))
                {
                    ks.Store(s, Password);
                }
            }
        }

        class SecretAccount : Java.Lang.Object, ISecretKey
        {
            byte[] bytes;
            public SecretAccount(Dictionary<string, string> values)
            {
                string serializedValues = JsonConvert.SerializeObject(values, new JsonSerializerSettings { Formatting = Formatting.None });
                bytes = System.Text.Encoding.UTF8.GetBytes(serializedValues);
            }
            public byte[] GetEncoded()
            {
                return bytes;
            }
            public string Algorithm
            {
                get
                {
                    return "RAW";
                }
            }
            public string Format
            {
                get
                {
                    return "RAW";
                }
            }
        }

        static IntPtr id_load_Ljava_io_InputStream_arrayC;
        /// <summary>
		/// Work around Bug https://bugzilla.xamarin.com/show_bug.cgi?id=6766
		/// </summary>
        void LoadEmptyKeyStore(char[] password)
        {
            if (id_load_Ljava_io_InputStream_arrayC == IntPtr.Zero)
            {
                id_load_Ljava_io_InputStream_arrayC = JNIEnv.GetMethodID(ks.Class.Handle, "load", "(Ljava/io/InputStream;[C)V");
            }
            IntPtr intPtr = IntPtr.Zero;
            IntPtr intPtr2 = JNIEnv.NewArray(password);
            JNIEnv.CallVoidMethod(ks.Handle, id_load_Ljava_io_InputStream_arrayC, new JValue[]
                {
                new JValue (intPtr),
                new JValue (intPtr2)
                });
            JNIEnv.DeleteLocalRef(intPtr);
            if (password != null)
            {
                JNIEnv.CopyArray(intPtr2, password);
                JNIEnv.DeleteLocalRef(intPtr2);
            }
        }
    }
}

