using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ModUtils
{
	public abstract class ModOptionsSave
	{
		public static ModOptionsSave gModOptions;

		public abstract IOptionsSave OptionsSave
		{
			get;
			set;
		}

		protected abstract string FileName
		{
			get;
		}

		public string OptionsFilePath
		{
			get { return GameFileIO.UserCloudDataPath + FileName; }
		}

		public void ApplyOptions(bool isInGame)
		{
			OptionsSave.ApplyOptions(isInGame);
		}

		public void ReloadModOptions(bool isInGame)
		{
			Load(isInGame);
		}

		public void Save()
		{
			try {
				Debug.Log("[ModOptionsSave] Saving: " + OptionsFilePath);
				using (FileStream fileStream = new FileStream(OptionsFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
					new XmlSerializer(OptionsSave.GetType()).Serialize(XmlWriter.Create((Stream)fileStream, new XmlWriterSettings()
					{
						Indent = true
					}), OptionsSave);
			} catch (Exception ex) {
				Debug.Log(ex);
			}
		}

		public void Load(bool isInGame)
		{
			try {
				Debug.Log((object)("[ModOptionsSave] Loading: " + OptionsFilePath));
				if (File.Exists(OptionsFilePath)) {
					using (FileStream fileStream = new FileStream(OptionsFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
						OptionsSave = (IOptionsSave)new XmlSerializer(OptionsSave.GetType()).Deserialize((Stream)fileStream);
				}
			} catch (Exception ex) {
				Debug.Log(ex);
			}
			this.ApplyOptions(isInGame);
			this.Save();
		}
	}
}
