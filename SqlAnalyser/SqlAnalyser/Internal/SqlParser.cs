using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlAnalyser.Internal
{
	public static class SqlParser
	{
		public static IEnumerable<TSqlBatch> Parse(string sql, SqlVersion version, out IList<ParseError> errors)
		{
			var parser = GetParser(version);
			
			using (var reader = new StringReader(sql.Trim()))
			{
				var fragment = parser.Parse(reader, out errors);

				if (errors.Any() || !(fragment is TSqlScript script))
				{
					return new TSqlBatch[]{ };
				}
					
				return script.Batches;
			}
		}

		private static TSqlParser GetParser(SqlVersion level)
		{
			switch (level)
			{
				case SqlVersion.Sql80:
					return new TSql80Parser(true);
				case SqlVersion.Sql90:
					return new TSql90Parser(true);
				case SqlVersion.Sql100:
					return new TSql100Parser(true);
				case SqlVersion.Sql110:
					return new TSql110Parser(true);
				case SqlVersion.Sql120:
					return new TSql120Parser(true);
				case SqlVersion.Sql130:
					return new TSql130Parser(true);
				case SqlVersion.Sql140:
					return new TSql140Parser(true);
				default:
					throw new ArgumentOutOfRangeException(nameof(level));
			}
		}
	}
}