using System;
using System.Collections.Generic;
using System.IO;
using DistributedDocs.Server.ClientModels;
using DistributedDocs.Server.Users;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace DistributedDocs.Server.Models
{
	internal sealed class DocumentsInfoPackage
	{
		public IReadOnlyDictionary<DocumentInfo, IReadOnlyCollection<IUser>>? DocumentInfos { get; set; }

		public byte[] Serialize()
		{
			var ms = new MemoryStream();
			using (BsonDataWriter writer = new BsonDataWriter(ms))
			{
				JsonSerializer serializer = new JsonSerializer();
				serializer.Serialize(writer, this);
			}

			var bsonByteArray = ms.ToArray();
			return bsonByteArray;
		}

		public static DocumentsInfoPackage Deserialize(byte[] bsonBytes)
		{
			var ms = new MemoryStream(bsonBytes);

			using (BsonDataReader reader = new BsonDataReader(ms))
			{
				JsonSerializer serializer = new JsonSerializer(); 
				return serializer.Deserialize<DocumentsInfoPackage>(reader) ?? 
				       throw new ArgumentException($"Cannot deserialize bson bytes to {nameof(DocumentsInfoPackage)}");
			}
		}
	}
}