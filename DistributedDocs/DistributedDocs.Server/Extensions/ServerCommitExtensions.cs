using System;
using DistributedDocs.DocumentChanges;
using DistributedDocs.Server.ClientModels;
using DistributedDocs.Server.Models.ServerModels;
using DistributedDocs.VersionHistory;

namespace DistributedDocs.Server.Extensions
{
	internal static class ServerCommitExtensions
	{
		public static ICommit<ITextDiff> ToHistoryCommit(this ServerCommit serverCommit)
		{
			var commit = serverCommit.Commit;
			if (commit is null)
			{
				throw new ArgumentNullException(nameof(serverCommit.Commit),
					"Has no client commit inside");
			}


			var diff = new TextDiff(commit.BeginIndex,
				commit.EndIndex, commit.String);

			return new Commit<ITextDiff>(serverCommit.CommitId,
				new AuthorInfo(serverCommit.UserGuid, serverCommit.UserName ?? string.Empty), diff);

		}

		public static ServerCommit FromHistoryCommit(this ICommit<ITextDiff> commit, Guid documentId)
		{
			return new ServerCommit
			{
				UserGuid = commit.Author.Guid,
				CommitId = commit.Id,
				Commit = new ClientCommit
				{
					DocumentId = documentId,
					BeginIndex = commit.Change.StartIndex,
					EndIndex = commit.Change.EndIndex,
					String = commit.Change.Text,
				},
			};

		}
	}
}