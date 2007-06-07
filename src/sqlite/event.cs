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
//using System.Data;
//using System.IO;
//using System.Collections; //ArrayList
using Mono.Data.SqliteClient;
using System.Data.SqlClient;


/* this class has some initializations used for all events */
 
class SqliteEvent : Sqlite
{
	/*
	 * create and initialize tables
	 */
	
	protected internal static void createGraphLinkTable()
	{
		dbcmd.CommandText = 
			"CREATE TABLE graphLinkTable ( " +
			"uniqueID INTEGER PRIMARY KEY, " +
			"tableName TEXT, " +
			"eventName TEXT, " +	
			"graphFileName TEXT, " +	//all images arew in the same dir
			"other1 TEXT, " +		//reserved for future
			"other2 TEXT )";		//reserved for future
		dbcmd.ExecuteNonQuery();
	}
	
	public static int GraphLinkInsert(string tableName, string eventName, string graphFileName, bool dbconOpened)
	{
		if(! dbconOpened) {
			dbcon.Open();
		}
		dbcmd.CommandText = "INSERT INTO graphLinkTable" + 
				"(uniqueID, tableName, eventName, graphFileName, other1, other2)" +
				" VALUES (NULL, '" + tableName + "', '" + eventName + "', '" + graphFileName + "', '', '')" ;
		Console.WriteLine(dbcmd.CommandText.ToString());
		dbcmd.ExecuteNonQuery();
		int myLast = dbcon.LastInsertRowId;
		if(! dbconOpened) {
			dbcon.Close();
		}

		return myLast;
	}
	
	public static string GraphLinkSelectFileName(string tableName, string eventName)
	{
		dbcon.Open();

		dbcmd.CommandText = "SELECT graphFileName FROM graphLinkTable WHERE tableName == '" + tableName + "' AND eventName =='" + eventName + "'";
		
		Console.WriteLine(dbcmd.CommandText.ToString());
		dbcmd.ExecuteNonQuery();

		SqliteDataReader reader;
		reader = dbcmd.ExecuteReader();

		string returnString = "";	
		while(reader.Read()) {
			returnString = reader[0].ToString();
		}
	
		return returnString;
	}
		
}
