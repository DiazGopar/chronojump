/*
 * This file is part of ChronoJump
 *
 * ChronoJump is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or   
 *    (at your option) any later version.
 *    
 * ChronoJump is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
 *    GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *   Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 *
 * Xavier de Blas: 
 * http://www.xdeblas.com, http://www.deporteyciencia.com (parleblas)
 */

using System;
using System.Data;
using System.IO;
using System.Collections; //ArrayList
using Mono.Data.SqliteClient;
using System.Data.SqlClient;


class Sqlite
{
	protected static SqliteConnection dbcon;
	protected static SqliteCommand dbcmd;
	static string sqlFile = "chronojump.db";
	static string connectionString = "URI=file:" + sqlFile ;

	public static void Connect()
	{
		dbcon = new SqliteConnection();
		dbcon.ConnectionString = connectionString;
		dbcmd = new SqliteCommand();
		dbcmd.Connection = dbcon;
	}

	public static void CreateFile()
	{
		Console.WriteLine("creating file...");
		dbcon.Open();
		dbcon.Close();
	}

	public static bool CheckTables()
	{
		return (File.Exists(sqlFile));
	}
	
	public static void CreateTables()
	{
		dbcon.Open();

		SqlitePerson.createTable();
		SqliteJump.createTable();
		SqliteJump.rjCreateTable();
		SqliteSession.createTable();
		SqlitePersonSession.createTable();
		SqlitePreferences.createTable();

		SqlitePreferences.insert ("databaseVersion", "0.1");
		SqlitePreferences.insert ("digitsNumber", "7");
		SqlitePreferences.insert ("showHeight", "True");
		SqlitePreferences.insert ("simulated", "True");
		SqlitePreferences.insert ("weightStatsPercent", "True");

		dbcon.Close();
	}
}
