﻿using FemProducer;

using Newtonsoft.Json;

namespace ReaserchPaper
{
	internal class JsonTaskBuilder : ITaskBuilder
	{
		void ITaskBuilder.Load(string path)
		{
			using (StreamReader r = new StreamReader("file.json"))
			{
				string json = r.ReadToEnd();
				List<Item> items = JsonConvert.DeserializeObject<List<Item>>(json);
			}
		}

		public class Item
		{
			public int millis;
			public string stamp;
			public DateTime datetime;
			public string light;
			public float temp;
			public float vcc;
		}
	}
}
