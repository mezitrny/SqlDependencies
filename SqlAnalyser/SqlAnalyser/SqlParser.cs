using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Domain.Model.Entities.ScriptEntity;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlAnalyser
{
	public class SqlParser : ISqlparser
	{
		public IEnumerable<string> Parse(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
			{
				return new List<string>();
			}

			var scriptFragment = InnerParse(text, true);

			if (!(scriptFragment is TSqlScript tsqlScriptFragment))
			{
				return new List<string>();
			}

			return GenerateScripts(tsqlScriptFragment);
		}

		private IEnumerable<string> GenerateScripts(TSqlFragment scriptFragment)
		{
			return SplitByGo(scriptFragment)
				.Select(batch => string.Join("", batch).Trim())
				.Where(result => result.Any());
		}

		private IEnumerable<IEnumerable<string>> SplitByGo(TSqlFragment scriptFragment)
		{
			var batch = new List<string>();

			foreach (var token in scriptFragment.ScriptTokenStream)
			{
				if (token.TokenType != TSqlTokenType.Go)
				{
					batch.Add(token.Text);
					continue;
				}

				yield return batch;
				batch = new List<string>();
			}

			if (batch.Any())
			{
				yield return batch;
			}
		}

		private TSqlFragment InnerParse(
			string sql,
			bool quotedIdentifiers)
		{
			using (var sr = new StringReader(sql.Trim()))
			{
				var parser = new TSql140Parser(quotedIdentifiers);
				var scriptFragment = parser.Parse(sr, out var errorlist);

				if (errorlist == null || errorlist.Count <= 0)
				{
					return scriptFragment;
				}

				var message = string.Join(
					"\n", 
					errorlist.Select(x => $"{x.Number}({x.Line}/{x.Column}): {x.Message}"));

				throw new ArgumentException($"Unparsable sql: {message}");
			}
		}
	}
}