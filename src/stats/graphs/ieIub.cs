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
using Gtk;
using System.Collections; //ArrayList

using NPlot.Gtk;
using NPlot;
using System.Drawing;
using System.Drawing.Imaging;


public class GraphIeIub : StatIeIub
{
	protected string operation;
	private Random myRand = new Random();

	//for simplesession
	GraphSerie serieIndex;
	GraphSerie serieJump1;
	GraphSerie serieJump2;

	public GraphIeIub (ArrayList sessions, string indexType, int newPrefsDigitsNumber, bool showSex, int statsJumpsType, int limit) 
	{
		this.dataColumns = 3; //for Simplesession (index, jump1, jump2)
		this.jumpType = jumpType;
		this.limit = limit;
		
		this.indexType = indexType; //"IE" or "IUB"
		if(indexType == "IE") {
			jump1="CMJ";
			jump2="SJ";
		} else { //IUB
			jump1="ABK";
			jump2="CMJ";
		}
		
		columnsString[0] = "Jumper";
		columnsString[1] = indexType;
		columnsString[2] = jump1;
		columnsString[3] = jump2;
		
		completeConstruction (treeview, sessions, newPrefsDigitsNumber, showSex, statsJumpsType);
	
		if (statsJumpsType == 2) {
			this.operation = "MAX";
		} else {
			this.operation = "AVG";
		}

		CurrentGraphData.WindowTitle = Catalog.GetString("ChronoJump graph");
		string mySessions = "single session";
		if(sessions.Count > 1) {
			mySessions = "multiple sessions";
		}
		CurrentGraphData.GraphTitle = indexType + " " + operation + 
			Catalog.GetString(" values chart of ") + mySessions;
		
		
		if(sessions.Count == 1) {
			//four series, the four columns
			serieIndex = new GraphSerie();
			serieJump1 = new GraphSerie();
			serieJump2 = new GraphSerie();
				
			serieIndex.Title = Catalog.GetString("Index");
			serieJump1.Title = jump1;
			serieJump2.Title = jump2;
			
			serieIndex.IsLeftAxis = false;
			serieJump1.IsLeftAxis = true;
			serieJump2.IsLeftAxis = true;

			serieIndex.SerieMarker = new Marker (Marker.MarkerType.FilledCircle, 
					6, new Pen (Color.FromName("Red"), 2.0F));
			serieJump1.SerieMarker = new Marker (Marker.MarkerType.TriangleUp, 
					6, new Pen (Color.FromName("LightGreen"), 2.0F));
			serieJump2.SerieMarker = new Marker (Marker.MarkerType.TriangleUp, 
					6, new Pen (Color.FromName("LightBlue"), 2.0F));
		
			//for the line between markers
			serieIndex.SerieColor = Color.FromName("Red");
			serieJump1.SerieColor = Color.FromName("LightGreen");
			serieJump2.SerieColor = Color.FromName("LightBlue");
		
			CurrentGraphData.LabelLeft = Catalog.GetString("seconds");
			CurrentGraphData.LabelRight = Catalog.GetString("%, cm");
		} else {
			for(int i=0; i < sessions.Count ; i++) {
				string [] stringFullResults = sessions[i].ToString().Split(new char[] {':'});
				CurrentGraphData.XAxisNames.Add(stringFullResults[1].ToString());
			}
			CurrentGraphData.LabelLeft = Catalog.GetString("%");
			CurrentGraphData.LabelRight = "";
		}
	}

	protected override void printData (string [] statValues) 
	{
		if(sessions.Count == 1) {
			int i = 0;
			//we need to save this transposed
			foreach (string myValue in statValues) 
			{
				if(i == 0) {
					//don't plot AVG and SD rows
					if( myValue == Catalog.GetString("AVG") || myValue == Catalog.GetString("SD") ) {
						//good moment for adding created series to GraphSeries ArrayList
						//check don't do it two times
						if(GraphSeries.Count == 0) {
							GraphSeries.Add(serieIndex);
							GraphSeries.Add(serieJump1);
							GraphSeries.Add(serieJump2);
						}
						
						return;
					}
					CurrentGraphData.XAxisNames.Add(myValue);
				} else if(i == 1) {
					serieIndex.SerieData.Add(myValue);
				} else if(i == 2) {
					serieJump1.SerieData.Add(myValue);
				} else if(i == 3) {
					serieJump2.SerieData.Add(myValue);
				}
				i++;
			}
		} else {
			GraphSerie mySerie = new GraphSerie();
			mySerie.IsLeftAxis = true;
		
			int myR = myRand.Next(255);
			int myG = myRand.Next(255);
			int myB = myRand.Next(255);
			
			mySerie.SerieMarker = new Marker (Marker.MarkerType.Cross1, 
					6, new Pen (Color.FromArgb(myR, myG, myB), 2.0F));
			
			mySerie.SerieColor = Color.FromArgb(myR, myG, myB);
			
			int i=0;
			foreach (string myValue in statValues) {
				if( myValue == Catalog.GetString("AVG") || myValue == Catalog.GetString("SD") ) {
					return;
				}
				if(i == 0) {
					mySerie.Title = myValue;
				} else {
					mySerie.SerieData.Add(myValue);
				}
				i++;
			}
			GraphSeries.Add(mySerie);
		}
	}
}
