using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlAnalyser
{
	public class SqlParser
	{
		public static IEnumerable<TSqlBatch> Parse(string sql, SqlVersion version, out IList<ParseError> errors)
		{
			var parser = GetParser(version);
			
			using (var reader = new StringReader(sql.Trim()))
			{
				var fragment = parser.Parse(reader, out errors);
					
				if (fragment is TSqlScript script)
				{
					return script.Batches;
				}
				
				return new TSqlBatch[]{ };
			}
		}

		public string ReScript(TSqlBatch batch)
		{
			var tokens = batch.ScriptTokenStream
				.Skip(batch.FirstTokenIndex)
				.Take(batch.FragmentLength)
				.Select(x => x.Text);
			
			return string.Join(string.Empty, tokens);
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