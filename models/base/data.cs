using System;
using System.Data.SQLite;

namespace Model
{
	public interface IData
	{
		object[] SqlParameters ();
	}
}

