using System.Collections.Generic;

namespace Domain.Model.Entities.ScriptEntity
{
	public interface ISqlparser
	{
		IEnumerable<string> Parse(string text);
	}
}