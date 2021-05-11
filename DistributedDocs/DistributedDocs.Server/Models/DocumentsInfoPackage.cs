using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DistributedDocs.Server.ClientModels;
using DistributedDocs.Server.Users;
using Newtonsoft.Json;

namespace DistributedDocs.Server.Models
{
	internal sealed class DocumentsInfoPackage
	{
		public Dictionary<DocumentInfo, List<User>>? DocumentInfos { get; set; }

		public byte[] Serialize()
		{
			//var ms = new MemoryStream();
			//using (BsonDataWriter writer = new BsonDataWriter(ms))
			//{
			//	JsonSerializer serializer = new JsonSerializer();
			//	serializer.Serialize(writer, this);
			//}

			//var bsonByteArray = ms.ToArray();
			//return bsonByteArray;
			var dict = DocumentInfos.Keys
				.ToDictionary(d => d.DocumentId,
					d => d.DocumentName);


			var json = JsonConvert.SerializeObject(dict);
			return Encoding.Unicode.GetBytes(json);
		}

		public static DocumentsInfoPackage Deserialize(byte[] bsonBytes)
		{
			//var ms = new MemoryStream(bsonBytes);

			//using (BsonDataReader reader = new BsonDataReader(ms))
			//{
			//	JsonSerializer serializer = new JsonSerializer(); 
			//	return serializer.Deserialize<DocumentsInfoPackage>(reader) ?? 
			//	       throw new ArgumentException($"Cannot deserialize bson bytes to {nameof(DocumentsInfoPackage)}");
			//}

			var json = Encoding.Unicode.GetString(bsonBytes);
			var dict = JsonConvert.DeserializeObject<Dictionary<Guid, string>>(json) ??
					   throw new ArgumentException($"Cannot deserialize bson bytes to {nameof(DocumentsInfoPackage)}");

			return new DocumentsInfoPackage
			{
				DocumentInfos = dict
					.ToDictionary(
						d => new DocumentInfo
						{
							DocumentId = d.Key,
							DocumentName = d.Value,
						},
						d => new List<User>()),
			};
		}
	}
}