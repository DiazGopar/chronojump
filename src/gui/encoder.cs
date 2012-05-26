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
 * Copyright (C) 2004-2012   Xavier de Blas <xaviblas@gmail.com> 
 */

using System;
using System.IO; 
using Gtk;
using Gdk;
using Glade;
using System.Collections;
using System.Threading;
using Mono.Unix;


public partial class ChronoJumpWindow 
{
	[Widget] Gtk.SpinButton spin_encoder_bar_limit;
	[Widget] Gtk.SpinButton spin_encoder_jump_limit;
	[Widget] Gtk.SpinButton spin_encoder_smooth;

	[Widget] Gtk.Button button_encoder_capture;
	[Widget] Gtk.Label label_encoder_person_weight;
	[Widget] Gtk.RadioButton radiobutton_encoder_concentric;
	[Widget] Gtk.RadioButton radiobutton_encoder_capture_bar;
	[Widget] Gtk.Viewport viewport_image_encoder_capture;
	[Widget] Gtk.Image image_encoder_bell;
	[Widget] Gtk.SpinButton spin_encoder_capture_time;
	[Widget] Gtk.SpinButton spin_encoder_capture_min_height;
	[Widget] Gtk.Image image_encoder_capture;
	[Widget] Gtk.TreeView treeview_encoder_curves;
	[Widget] Gtk.ProgressBar encoder_pulsebar_capture;
	[Widget] Gtk.Button button_encoder_delete_selected;
	[Widget] Gtk.Button button_encoder_save_selected;
	[Widget] Gtk.Button button_encoder_save_stream;
	
	[Widget] Gtk.Button button_encoder_analyze;
	[Widget] Gtk.RadioButton radiobutton_encoder_analyze_powerbars;
	[Widget] Gtk.RadioButton radiobutton_encoder_analyze_single;
	[Widget] Gtk.RadioButton radiobutton_encoder_analyze_side;
	[Widget] Gtk.RadioButton radiobutton_encoder_analyze_superpose;
	[Widget] Gtk.Label label_encoder_analyze_eccon;
	[Widget] Gtk.Box hbox_encoder_analyze_eccon;
	[Widget] Gtk.RadioButton radiobutton_encoder_eccon_both;
	[Widget] Gtk.RadioButton radiobutton_encoder_eccon_together;
	[Widget] Gtk.Label label_encoder_analyze_curve_num;
	[Widget] Gtk.SpinButton spin_encoder_analyze_curve_num;
	[Widget] Gtk.Viewport viewport_image_encoder_analyze;
	[Widget] Gtk.Image image_encoder_analyze;
	[Widget] Gtk.ProgressBar encoder_pulsebar_analyze;

	TreeStore encoderStore;

	ArrayList encoderCurves;
        Gtk.ListStore encoderListStore;

	Thread encoderThread;

	int image_encoder_width;
	int image_encoder_height;

	private string encoderAnalysis="powerBars";
	private string ecconLast;
	enum encoderModes { CAPTURE, ANALYZE }
	
	GenericWindow genericWinForEncoder;
	
	//TODO: store encoder data: auto save, and show on a treeview. Put button to delete current (or should be called "last")

	//TODO: put chronopic detection in a generic place. Done But:
	//TODO: solve the problem of connecting two different chronopics
	
	private void encoderInitializeVariables() {
		encoder_pulsebar_capture.Fraction = 1;
		encoder_pulsebar_capture.Text = "";
		encoder_pulsebar_analyze.Fraction = 1;
		encoder_pulsebar_analyze.Text = "";
		
		//the glade cursor_changed does not work on mono 1.2.5 windows
		treeview_encoder_curves.CursorChanged += on_treeview_encoder_curves_cursor_changed; 
		sensitiveEncoderRowButtons(false);
	}

	private void on_radiobutton_encoder_capture_bar_toggled (object obj, EventArgs args) {
		spin_encoder_bar_limit.Sensitive = true;
		spin_encoder_jump_limit.Sensitive = false;
	}
	private void on_radiobutton_encoder_capture_jump_toggled (object obj, EventArgs args) {
		spin_encoder_bar_limit.Sensitive = false;
		spin_encoder_jump_limit.Sensitive = true;
	}

	private void on_radiobutton_encoder_eccon_toggled (object obj, EventArgs args) {
		if(radiobutton_encoder_concentric.Active) {
			label_encoder_analyze_eccon.Sensitive=false;
			hbox_encoder_analyze_eccon.Sensitive=false;
		} else if(radiobutton_encoder_analyze_powerbars.Active) {
			label_encoder_analyze_eccon.Sensitive=true;
			hbox_encoder_analyze_eccon.Sensitive=true;
		}
	}

	//TODO: garantir path windows	
	void on_button_encoder_capture_clicked (object o, EventArgs args) 
	{
		double heightHigherCondition = -1;
		if(repetitiveConditionsWin.EncoderHeightHigher)		
			heightHigherCondition = repetitiveConditionsWin.EncoderHeightHigherValue;
		double heightLowerCondition = -1;
		if(repetitiveConditionsWin.EncoderHeightLower)		
			heightLowerCondition = repetitiveConditionsWin.EncoderHeightLowerValue;
	
		double meanSpeedHigherCondition = -1;
		if(repetitiveConditionsWin.EncoderMeanSpeedHigher)		
			meanSpeedHigherCondition = repetitiveConditionsWin.EncoderMeanSpeedHigherValue;
		double meanSpeedLowerCondition = -1;
		if(repetitiveConditionsWin.EncoderMeanSpeedLower)		
			meanSpeedLowerCondition = repetitiveConditionsWin.EncoderMeanSpeedLowerValue;
	
		double maxSpeedHigherCondition = -1;
		if(repetitiveConditionsWin.EncoderMaxSpeedHigher)		
			maxSpeedHigherCondition = repetitiveConditionsWin.EncoderMaxSpeedHigherValue;
		double maxSpeedLowerCondition = -1;
		if(repetitiveConditionsWin.EncoderMaxSpeedLower)		
			maxSpeedLowerCondition = repetitiveConditionsWin.EncoderMaxSpeedLowerValue;
	
		int powerHigherCondition = -1;
		if(repetitiveConditionsWin.EncoderPowerHigher)		
			powerHigherCondition = repetitiveConditionsWin.EncoderPowerHigherValue;
		int powerLowerCondition = -1;
		if(repetitiveConditionsWin.EncoderPowerLower)		
			powerLowerCondition = repetitiveConditionsWin.EncoderPowerLowerValue;
		
		int peakPowerHigherCondition = -1;
		if(repetitiveConditionsWin.EncoderPeakPowerHigher)		
			peakPowerHigherCondition = repetitiveConditionsWin.EncoderPeakPowerHigherValue;
		int peakPowerLowerCondition = -1;
		if(repetitiveConditionsWin.EncoderPeakPowerLower)		
			peakPowerLowerCondition = repetitiveConditionsWin.EncoderPeakPowerLowerValue;

		//capture data
		EncoderParams ep = new EncoderParams(
				(int) spin_encoder_capture_time.Value, 
				(int) spin_encoder_capture_min_height.Value, 
				!radiobutton_encoder_capture_bar.Active,
				findMass(true),
				Util.ConvertToPoint((double) spin_encoder_smooth.Value), //R decimal: '.'
				findEccon(true),					//force ecS (ecc-conc separated)
				heightHigherCondition, heightLowerCondition,
				meanSpeedHigherCondition, meanSpeedLowerCondition,
				maxSpeedHigherCondition, maxSpeedLowerCondition,
				powerHigherCondition, powerLowerCondition,
				peakPowerHigherCondition, peakPowerLowerCondition
				); 

		EncoderStruct es = new EncoderStruct(
				"",					//no data input
				"",					//no graph ouptut
				Util.GetEncoderDataTempFileName(), "", ep);				

		Util.RunPythonEncoder(Constants.EncoderScriptCapture, es, true);

		encoderThreadStart(encoderModes.CAPTURE);
	}
		
	void on_button_encoder_recalculate_clicked (object o, EventArgs args) 
	{
		if (File.Exists(Util.GetEncoderDataTempFileName()))
			encoderThreadStart(encoderModes.CAPTURE);
		else
			encoder_pulsebar_capture.Text = Catalog.GetString("Missing data.");
	}
	
	private void encoderUpdateTreeView()
	{
		string contents = Util.ReadFile(Util.GetEncoderCurvesTempFileName());
		if (contents == null) {
			//TODO: no data: make some of the gui unsensitive ??
			button_encoder_analyze.Sensitive = false;
		} else {
			removeColumns();
			int curvesNum = createTreeViewEncoder(contents);
			if(! radiobutton_encoder_concentric.Active)
				curvesNum = curvesNum / 2;
			spin_encoder_analyze_curve_num.SetRange(1,curvesNum);
			button_encoder_analyze.Sensitive = true;
		}
	}
	
	private void removeColumns() {
		Gtk.TreeViewColumn [] myColumns = treeview_encoder_curves.Columns;
		foreach (Gtk.TreeViewColumn column in myColumns) 
			treeview_encoder_curves.RemoveColumn (column);
	}


	//this is called by non gtk thread. Don't do gtk stuff here
	private void encoderCreateCurvesGraphR() 
	{
		EncoderParams ep = new EncoderParams(
				(int) spin_encoder_capture_min_height.Value, 
				!radiobutton_encoder_capture_bar.Active,
				findMass(true),
				findEccon(true),					//force ecS (ecc-conc separated)
				"curves",
				Util.ConvertToPoint((double) spin_encoder_smooth.Value), //R decimal: '.'
			       	0, 			//curve is not used here
				image_encoder_width, image_encoder_height); 

		EncoderStruct es = new EncoderStruct(
				Util.GetEncoderDataTempFileName(), 
				Util.GetEncoderGraphTempFileName(),
				Util.GetEncoderCurvesTempFileName(), 
				"NULL", ep);
		
		Util.RunPythonEncoder(Constants.EncoderScriptGraphCall, es,false);

		//store this to show 1,2,3,4,... or 1e,1c,2e,2c,... in RenderN
		//if is not stored, it can change when changed eccon radiobutton on cursor is in treeview
		ecconLast = findEccon(false);
	}
		
	void on_button_encoder_load_stream_clicked (object o, EventArgs args) 
	{
		Log.WriteLine("TODO: Load stream");
	}

	void on_button_encoder_delete_selected_clicked (object o, EventArgs args) 
	{
		Log.WriteLine("TODO: Delete selected");
	}

	void on_button_encoder_save_selected_clicked (object o, EventArgs args) 
	{
		Log.WriteLine("TODO: Save selected");
	}

	void on_button_encoder_save_stream_clicked (object o, EventArgs args) 
	{
		genericWinForEncoder = GenericWindow.Show(Catalog.GetString("Add an optional description"), Constants.GenericWindowShow.TEXTVIEW);
		genericWinForEncoder.SetTextview("");
		genericWinForEncoder.SetButtonAcceptLabel(Catalog.GetString("Save"));

		genericWinForEncoder.Button_accept.Clicked += new EventHandler(on_save_stream_description_add_accepted);
	}
	
	private void on_save_stream_description_add_accepted (object o, EventArgs args) {
		genericWinForEncoder.Button_accept.Clicked -= new EventHandler(on_save_stream_description_add_accepted);
		string desc = genericWinForEncoder.TextviewSelected;
		
		Log.WriteLine(desc);
	
		//Saving file
		//Util.MoveTempEncoderData (currentSession.UniqueID, currentPerson.UniqueID);
		string fileName = Util.CopyTempEncoderData (currentSession.UniqueID, currentPerson.UniqueID, currentPerson.Name);

		//Adding on SQL
		SqliteEncoder.Insert(false, "-1", 
				currentPerson.UniqueID, currentSession.UniqueID, 
				fileName,
				Util.GetEncoderSessionDataDir(currentSession.UniqueID),	//url
				(! radiobutton_encoder_capture_bar.Active).ToString(),
				findMass(false), //when save on sql, do not include person weight
				findEccon(true),					//force ecS (ecc-conc separated)
				(int) spin_encoder_capture_time.Value, 
				(int) spin_encoder_capture_min_height.Value, 
				(double) spin_encoder_smooth.Value,
				desc);
		
		encoder_pulsebar_capture.Text = Catalog.GetString("Saved.");
	}


	//TODO: garantir path windows	
	private void on_button_encoder_analyze_clicked (object o, EventArgs args) 
	{
		encoderThreadStart(encoderModes.ANALYZE);
	}
	
	//this is called by non gtk thread. Don't do gtk stuff here
	private void analyze () 
	{
		EncoderParams ep = new EncoderParams(
				(int) spin_encoder_capture_min_height.Value, 
				!radiobutton_encoder_capture_bar.Active,
				findMass(true),
				findEccon(false),		//do not force ecS (ecc-conc separated)
				encoderAnalysis,
				Util.ConvertToPoint((double) spin_encoder_smooth.Value), //R decimal: '.'
				(int) spin_encoder_analyze_curve_num.Value, 
				image_encoder_width, image_encoder_height); 

		EncoderStruct es = new EncoderStruct(
				Util.GetEncoderDataTempFileName(), 
				Util.GetEncoderGraphTempFileName(),
				"NULL", "NULL", ep);		//no data ouptut

		Util.RunPythonEncoder(Constants.EncoderScriptGraphCall, es, false);
	}

//TODO: auto close capturing window

	//show curve_num only on simple and superpose
	private void on_radiobutton_encoder_analyze_single_toggled (object obj, EventArgs args) {
		label_encoder_analyze_curve_num.Sensitive=true;
		spin_encoder_analyze_curve_num.Sensitive=true;
		encoderAnalysis="single";
		//together, mandatory
		label_encoder_analyze_eccon.Sensitive=false;
		hbox_encoder_analyze_eccon.Sensitive=false;
		radiobutton_encoder_eccon_together.Active = true;
	}

	private void on_radiobutton_encoder_analyze_superpose_toggled (object obj, EventArgs args) {
		label_encoder_analyze_curve_num.Sensitive=true;
		spin_encoder_analyze_curve_num.Sensitive=true;
		encoderAnalysis="superpose";
		//together, mandatory
		label_encoder_analyze_eccon.Sensitive=false;
		hbox_encoder_analyze_eccon.Sensitive=false;
		radiobutton_encoder_eccon_together.Active = true;
	}
	private void on_radiobutton_encoder_analyze_side_toggled (object obj, EventArgs args) {
		label_encoder_analyze_curve_num.Sensitive=false;
		spin_encoder_analyze_curve_num.Sensitive=false;
		encoderAnalysis="side";
		//together, mandatory
		label_encoder_analyze_eccon.Sensitive=false;
		hbox_encoder_analyze_eccon.Sensitive=false;
		radiobutton_encoder_eccon_together.Active = true;
	}
	private void on_radiobutton_encoder_analyze_powerbars_toggled (object obj, EventArgs args) {
		label_encoder_analyze_curve_num.Sensitive=false;
		spin_encoder_analyze_curve_num.Sensitive=false;
		encoderAnalysis="powerBars";
		//can select together or separated
		if(! radiobutton_encoder_concentric.Active) {
			label_encoder_analyze_eccon.Sensitive=true;
			hbox_encoder_analyze_eccon.Sensitive=true;
		}
	}

	private string findMass(bool includePerson) {
		double mass = 0;
		if(radiobutton_encoder_capture_bar.Active)
			mass = spin_encoder_bar_limit.Value;
		else {
			mass = spin_encoder_jump_limit.Value;
			if(includePerson)
				mass += Convert.ToDouble(label_encoder_person_weight.Text);
		}

		return Util.ConvertToPoint(mass); //R decimal: '.'
	}
	
	private string findEccon(bool ecconSeparated) {	
		if(radiobutton_encoder_concentric.Active)
			return "c";
		else {
			if(ecconSeparated || ! radiobutton_encoder_eccon_together.Active)
				return "ecS";
			else 
				return "ec";
		}
	}


	/* TreeView stuff */	

	//returns curves num
	private int createTreeViewEncoder(string contents) {
		string [] columnsString = {
			"n",
			Catalog.GetString("Start") + "\n (s)",
			Catalog.GetString("Duration") + "\n (s)",
			Catalog.GetString("Height") + "\n (cm)",
			Catalog.GetString("MeanSpeed") + "\n (m/s)",
			Catalog.GetString("MaxSpeed") + "\n (m/s)", //duration (s): width
			Catalog.GetString("MeanPower") + "\n (W)",
			Catalog.GetString("PeakPower") + "\n (W)",
			Catalog.GetString("PeakPowerTime") + "\n (s)",
			Catalog.GetString("PeakPower/PPT") + "\n (W/s)"
		};


		encoderCurves = new ArrayList ();

		string line;
		int curvesCount = 0;
		using (StringReader reader = new StringReader (contents)) {
			line = reader.ReadLine ();	//headers
			Log.WriteLine(line);
			do {
				line = reader.ReadLine ();
				Log.WriteLine(line);
				if (line == null)
					break;

				curvesCount ++;

				string [] cells = line.Split(new char[] {','});
				cells = fixDecimals(cells);
				//iter = encoderStore.AppendValues(cells);

				encoderCurves.Add (new EncoderCurve (cells[0], cells[1], cells[2], 
							cells[3], cells[4], cells[5], cells[6], 
							cells[7], cells[8], cells[9]));

			} while(true);
		}

		encoderListStore = new Gtk.ListStore (typeof (EncoderCurve));
		foreach (EncoderCurve curve in encoderCurves) {
			encoderListStore.AppendValues (curve);
		}

		treeview_encoder_curves.Model = encoderListStore;

		treeview_encoder_curves.HeadersVisible=true;

		int i=0;
		foreach(string myCol in columnsString) {
			Gtk.TreeViewColumn aColumn = new Gtk.TreeViewColumn ();
			CellRendererText aCell = new CellRendererText();
			aColumn.Title=myCol;
			aColumn.PackStart (aCell, true);

			//crt1.Foreground = "red";
			//crt1.Background = "blue";
		
			switch(i){	
				case 0:
					aColumn.SetCellDataFunc (aCell, new Gtk.TreeCellDataFunc (RenderN));
					break;
				case 1:
					aColumn.SetCellDataFunc (aCell, new Gtk.TreeCellDataFunc (RenderStart));
					break;
				case 2:
					aColumn.SetCellDataFunc (aCell, new Gtk.TreeCellDataFunc (RenderWidth));
					break;
				case 3:
					aColumn.SetCellDataFunc (aCell, new Gtk.TreeCellDataFunc (RenderHeight));
					break;
				case 4:
					aColumn.SetCellDataFunc (aCell, new Gtk.TreeCellDataFunc (RenderMeanSpeed));
					break;
				case 5:
					aColumn.SetCellDataFunc (aCell, new Gtk.TreeCellDataFunc (RenderMaxSpeed));
					break;
				case 6:
					aColumn.SetCellDataFunc (aCell, new Gtk.TreeCellDataFunc (RenderMeanPower));
					break;
				case 7:
					aColumn.SetCellDataFunc (aCell, new Gtk.TreeCellDataFunc (RenderPeakPower));
					break;
				case 8:
					aColumn.SetCellDataFunc (aCell, new Gtk.TreeCellDataFunc (RenderPeakPowerT));
					break;
				case 9:
					aColumn.SetCellDataFunc (aCell, new Gtk.TreeCellDataFunc (RenderPP_PPT));
					break;
			}
			

			treeview_encoder_curves.AppendColumn (aColumn);
			i++;
		}
		return curvesCount;
	}


	/* rendering columns */
	private string assignColor(double found, bool higherActive, bool lowerActive, double higherValue, double lowerValue) 
	{
		//more at System.Drawing.Color (Monodoc)
		string colorGood= "ForestGreen"; 
		string colorBad= "red";
		string colorNothing= "";	
		//colorNothing will use default color on system, previous I used black,
		//but if the color of the users theme is not 000000, then it looked too different

		if(higherActive && found >= higherValue)
			return colorGood;
		else if(lowerActive && found <= lowerValue)
			return colorBad;
		else
			return colorNothing;
	}

	private string assignColor(double found, bool higherActive, bool lowerActive, int higherValue, int lowerValue) 
	{
		return assignColor(found, higherActive, lowerActive, 
				Convert.ToDouble(higherValue), Convert.ToDouble(lowerValue));
	}

	private void RenderN (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
	{
		EncoderCurve curve = (EncoderCurve) model.GetValue (iter, 0);
		if(ecconLast == "c")
			(cell as Gtk.CellRendererText).Text = 
				String.Format(UtilGtk.TVNumPrint(curve.N,1,0),Convert.ToInt32(curve.N));
		else {
			string phase = "e";
			bool isEven = (Convert.ToInt32(curve.N) % 2 == 0); //check if it's even (in spanish "par")
			if(isEven)
				phase = "c";
				
			(cell as Gtk.CellRendererText).Text = 
				decimal.Truncate((Convert.ToInt32(curve.N) +1) /2).ToString() + phase;
		}
	}
	private void RenderStart (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
	{
		EncoderCurve curve = (EncoderCurve) model.GetValue (iter, 0);
		double myStart = Convert.ToDouble(curve.Start)/1000; //ms->s
		(cell as Gtk.CellRendererText).Text = 
			String.Format(UtilGtk.TVNumPrint(myStart.ToString(),6,3),myStart); 
	}
	private void RenderWidth (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
	{
		EncoderCurve curve = (EncoderCurve) model.GetValue (iter, 0);
		double myWidth = Convert.ToDouble(curve.Width)/1000; //ms->s
		(cell as Gtk.CellRendererText).Text = 
			String.Format(UtilGtk.TVNumPrint(myWidth.ToString(),5,3),myWidth); 
	}
	private void RenderHeight (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
	{
		EncoderCurve curve = (EncoderCurve) model.GetValue (iter, 0);
		string heightToCm = (Convert.ToDouble(curve.Height)/10).ToString();
		string myColor = assignColor(
				Convert.ToDouble(heightToCm),
				repetitiveConditionsWin.EncoderHeightHigher, 
				repetitiveConditionsWin.EncoderHeightLower, 
				repetitiveConditionsWin.EncoderHeightHigherValue,
				repetitiveConditionsWin.EncoderHeightLowerValue);
		if(myColor != "")
			(cell as Gtk.CellRendererText).Foreground = myColor;
		else
			(cell as Gtk.CellRendererText).Foreground = null;	//will show default color

		(cell as Gtk.CellRendererText).Text = 
			String.Format(UtilGtk.TVNumPrint(heightToCm,5,1),Convert.ToDouble(heightToCm));
	}
	
	private void RenderMeanSpeed (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
	{
		EncoderCurve curve = (EncoderCurve) model.GetValue (iter, 0);
		string myColor = assignColor(
				Convert.ToDouble(curve.MeanSpeed),
				repetitiveConditionsWin.EncoderMeanSpeedHigher, 
				repetitiveConditionsWin.EncoderMeanSpeedLower, 
				repetitiveConditionsWin.EncoderMeanSpeedHigherValue,
				repetitiveConditionsWin.EncoderMeanSpeedLowerValue);
		if(myColor != "")
			(cell as Gtk.CellRendererText).Foreground = myColor;
		else
			(cell as Gtk.CellRendererText).Foreground = null;	//will show default color

		//no need of UtilGtk.TVNumPrint, always has 1 digit on left of decimal
		(cell as Gtk.CellRendererText).Text = 
			String.Format("{0,10:0.000}",Convert.ToDouble(curve.MeanSpeed));
	}

	private void RenderMaxSpeed (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
	{
		EncoderCurve curve = (EncoderCurve) model.GetValue (iter, 0);
		string myColor = assignColor(
				Convert.ToDouble(curve.MaxSpeed),
				repetitiveConditionsWin.EncoderMaxSpeedHigher, 
				repetitiveConditionsWin.EncoderMaxSpeedLower, 
				repetitiveConditionsWin.EncoderMaxSpeedHigherValue,
				repetitiveConditionsWin.EncoderMaxSpeedLowerValue);
		if(myColor != "")
			(cell as Gtk.CellRendererText).Foreground = myColor;
		else
			(cell as Gtk.CellRendererText).Foreground = null;	//will show default color

		//no need of UtilGtk.TVNumPrint, always has 1 digit on left of decimal
		(cell as Gtk.CellRendererText).Text = 
			String.Format("{0,10:0.000}",Convert.ToDouble(curve.MaxSpeed));
	}
	
	private void RenderMeanPower (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
	{
		EncoderCurve curve = (EncoderCurve) model.GetValue (iter, 0);
		string myColor = assignColor(
				Convert.ToDouble(curve.MeanPower),
				repetitiveConditionsWin.EncoderPowerHigher, 
				repetitiveConditionsWin.EncoderPowerLower, 
				repetitiveConditionsWin.EncoderPowerHigherValue,
				repetitiveConditionsWin.EncoderPowerLowerValue);
		if(myColor != "")
			(cell as Gtk.CellRendererText).Foreground = myColor;
		else
			(cell as Gtk.CellRendererText).Foreground = null;	//will show default color

		(cell as Gtk.CellRendererText).Text = 
			String.Format(UtilGtk.TVNumPrint(curve.MeanPower,9,3),Convert.ToDouble(curve.MeanPower));
	}

	private void RenderPeakPower (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
	{
		EncoderCurve curve = (EncoderCurve) model.GetValue (iter, 0);
		string myColor = assignColor(
				Convert.ToDouble(curve.PeakPower),
				repetitiveConditionsWin.EncoderPeakPowerHigher, 
				repetitiveConditionsWin.EncoderPeakPowerLower, 
				repetitiveConditionsWin.EncoderPeakPowerHigherValue,
				repetitiveConditionsWin.EncoderPeakPowerLowerValue);
		if(myColor != "")
			(cell as Gtk.CellRendererText).Foreground = myColor;
		else
			(cell as Gtk.CellRendererText).Foreground = null;	//will show default color

		(cell as Gtk.CellRendererText).Text = 
			String.Format(UtilGtk.TVNumPrint(curve.PeakPower,9,3),Convert.ToDouble(curve.PeakPower));
	}

	private void RenderPeakPowerT (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
	{
		EncoderCurve curve = (EncoderCurve) model.GetValue (iter, 0);
		double myPPT = Convert.ToDouble(curve.PeakPowerT)/1000; //ms->s
		(cell as Gtk.CellRendererText).Text = 
			String.Format(UtilGtk.TVNumPrint(myPPT.ToString(),7,3),myPPT);
	}

	private void RenderPP_PPT (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
	{
		EncoderCurve curve = (EncoderCurve) model.GetValue (iter, 0);
		(cell as Gtk.CellRendererText).Text = 
			String.Format(UtilGtk.TVNumPrint(curve.PP_PPT,6,1),curve.PP_PPT);
	}

	/* end of rendering cols */
	
	
	private string [] fixDecimals(string [] cells) {
		for(int i=1; i <= 3; i++)
			cells[i] = Util.TrimDecimals(Convert.ToDouble(Util.ChangeDecimalSeparator(cells[i])),1);
		for(int i=4; i <= 8; i++)
			cells[i] = Util.TrimDecimals(Convert.ToDouble(Util.ChangeDecimalSeparator(cells[i])),3);
		cells[9] = Util.TrimDecimals(Convert.ToDouble(Util.ChangeDecimalSeparator(cells[8])),1); //pp/ppt
		return cells;
	}
	
	private int treeviewEncoderCurvesEventSelectedID() {
		TreeIter iter = new TreeIter();
		TreeModel myModel = treeview_encoder_curves.Model;
		if (treeview_encoder_curves.Selection.GetSelected (out myModel, out iter)) 
			return Convert.ToInt32(((EncoderCurve) (treeview_encoder_curves.Model.GetValue (iter, 0))).N);
			//this return an int, also in ec
		else 
			return 0;
	}
	
	private void on_treeview_encoder_curves_cursor_changed (object o, EventArgs args) {
		if (treeviewEncoderCurvesEventSelectedID() == 0)
			sensitiveEncoderRowButtons(false);
		else {
			sensitiveEncoderRowButtons(true);
			Log.WriteLine(treeviewEncoderCurvesEventSelectedID().ToString());
		}
	}

	private void sensitiveEncoderRowButtons(bool sensitive) {
		button_encoder_delete_selected.Sensitive = sensitive;
		button_encoder_save_selected.Sensitive = sensitive;
	}
	
	/* end of TreeView stuff */	

	/* thread stuff */

	private void encoderThreadStart(encoderModes mode) {
		if(mode == encoderModes.CAPTURE) {
			//image is inside (is smaller than) viewport
			image_encoder_width = UtilGtk.WidgetWidth(viewport_image_encoder_capture)-3; 
			image_encoder_height = UtilGtk.WidgetHeight(viewport_image_encoder_capture)-3;

			encoder_pulsebar_capture.Text = Catalog.GetString("Please, wait.");
			treeview_encoder_curves.Sensitive = false;
			encoderThread = new Thread(new ThreadStart(encoderCreateCurvesGraphR));
			GLib.Idle.Add (new GLib.IdleHandler (pulseGTKEncoderCapture));
		} else {
			//the -3 is because image is inside (is smaller than) viewport
			image_encoder_width = UtilGtk.WidgetWidth(viewport_image_encoder_analyze)-3; 
			image_encoder_height = UtilGtk.WidgetHeight(viewport_image_encoder_analyze)-3;

			encoder_pulsebar_analyze.Text = Catalog.GetString("Please, wait.");
			encoderThread = new Thread(new ThreadStart(analyze));
			GLib.Idle.Add (new GLib.IdleHandler (pulseGTKEncoderAnalyze));
		}
		encoderThread.Start(); 
	}
	
	private bool pulseGTKEncoderCapture ()
	{
		if(! encoderThread.IsAlive) {
			finishPulsebar(encoderModes.CAPTURE);
			sensitiveEncoderRowButtons(false);
			Log.Write("dying");
			return false;
		}
		updatePulsebar(encoderModes.CAPTURE); //activity on pulsebar
		Thread.Sleep (50);
		Log.Write(encoderThread.ThreadState.ToString());
		return true;
	}
	
	private bool pulseGTKEncoderAnalyze ()
	{
		if(! encoderThread.IsAlive) {
			finishPulsebar(encoderModes.ANALYZE);
			sensitiveEncoderRowButtons(false);
			Log.Write("dying");
			return false;
		}
		updatePulsebar(encoderModes.ANALYZE); //activity on pulsebar
		Thread.Sleep (50);
		Log.Write(encoderThread.ThreadState.ToString());
		return true;
	}
	
	private void updatePulsebar (encoderModes mode) {
		if(mode == encoderModes.CAPTURE) 
			encoder_pulsebar_capture.Pulse();
		else
			encoder_pulsebar_analyze.Pulse();
	}
	
	private void finishPulsebar(encoderModes mode) {
		if(mode == encoderModes.CAPTURE) {
			encoder_pulsebar_capture.Fraction = 1;
			encoder_pulsebar_capture.Text = "";
			treeview_encoder_curves.Sensitive = true;
			
			Pixbuf pixbuf = new Pixbuf (Util.GetEncoderGraphTempFileName()); //from a file
			image_encoder_capture.Pixbuf = pixbuf;

			encoderUpdateTreeView();
		} else {
			encoder_pulsebar_analyze.Fraction = 1;
			encoder_pulsebar_analyze.Text = "";
			
			//TODO pensar en si s'ha de fer 1er amb mida petita i despres amb gran (en el zoom), o si es una sola i fa alguna edicio
			Pixbuf pixbuf = new Pixbuf (Util.GetEncoderGraphTempFileName()); //from a file
			image_encoder_analyze.Pixbuf = pixbuf;
		}
	}
	
	/* end of thread stuff */
	
}	

