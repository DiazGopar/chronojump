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
 *  Copyright (C) 2004-2017   Xavier de Blas <xaviblas@gmail.com> 
 */

using System;
using System.Data;
using System.Text; //StringBuilder
using System.IO;   //for Path
using System.Collections; //ArrayList
using System.Collections.Generic; //List<T>
using Mono.Unix;

public class EncoderParams
{
	//graph.R need both to know displacedMass depending on encoderConfiguration
	//and plot both as entry data in the table of result data
	private string massBody; //to pass always as "." to R.
	private string massExtra; //to pass always as "." to R
	
	private int minHeight;
	private int exercisePercentBodyWeight; //was private bool isJump; (if it's 0 is like "jump")
	private string eccon;
	private string analysis;
	private string analysisVariables;
	private string analysisOptions;		//p: propulsive
	private bool captureCheckFullyExtended;
	private int captureCheckFullyExtendedValue;
					
	//encoderConfiguration conversions
	//in signals and curves, need to do conversions (invert, inertiaMomentum, diameter)
	private EncoderConfiguration encoderConfiguration;	
	
	private string smoothCon; //to pass always as "." to R
	private int curve;
	private int width;
	private int height;
	private string decimalSeparator;	//used in export data from R to csv
	//private bool inverted; //used only in runEncoderCapturePython. In graph.R will be used encoderConfigurationName

	public EncoderParams()
	{
	}

	
	//to graph.R	
	public EncoderParams(int minHeight, int exercisePercentBodyWeight, string massBody, string massExtra, 
			string eccon, string analysis, string analysisVariables, string analysisOptions, 
			bool captureCheckFullyExtended, int captureCheckFullyExtendedValue,
			EncoderConfiguration encoderConfiguration,
			string smoothCon, int curve, int width, int height, string decimalSeparator)
	{
		this.minHeight = minHeight;
		this.exercisePercentBodyWeight = exercisePercentBodyWeight;
		this.massBody = massBody;
		this.massExtra = massExtra;
		this.eccon = eccon;
		this.analysis = analysis;
		this.analysisVariables = analysisVariables;
		this.analysisOptions = analysisOptions;
		this.captureCheckFullyExtended = captureCheckFullyExtended;
		this.captureCheckFullyExtendedValue = captureCheckFullyExtendedValue;
		this.encoderConfiguration = encoderConfiguration;
		this.smoothCon = smoothCon;
		this.curve = curve;
		this.width = width;
		this.height = height;
		this.decimalSeparator = decimalSeparator;
	}
	
	public string ToStringROptions () 
	{
		string capFullyExtendedStr = "-1";
		if(captureCheckFullyExtended)
			capFullyExtendedStr = captureCheckFullyExtendedValue.ToString(); 
		
		return 
			"#minHeight\n" + 	minHeight + "\n" + 
			"#exercisePercentBodyWeight\n" + exercisePercentBodyWeight + "\n" + 
			"#massBody\n" + 	massBody + "\n" + 
			"#massExtra\n" + 	massExtra + "\n" + 
			"#eccon\n" + 		eccon + "\n" + 
			"#analysis\n" + 	analysis + "\n" + 
			"#analysisVariables\n" + analysisVariables + "\n" + 
			"#analysisOptions\n" + analysisOptions + "\n" + 
			"#captureCheckFullyExtended\n" + capFullyExtendedStr + "\n" + 
			encoderConfiguration.ToStringOutput(EncoderConfiguration.Outputs.ROPTIONS) + "\n" +
			"#smoothCon\n" + 	smoothCon + "\n" + 
			"#curve\n" + 		curve + "\n" + 
			"#width\n" + 		width + "\n" + 
			"#height\n" + 		height + "\n" + 
			"#decimalSeparator\n" + decimalSeparator
			;
	}
	
	public string Analysis {
		get { return analysis; }
	}
	

	~EncoderParams() {}
}

public class EncoderStruct
{
	public EncoderStruct() {
	}

	public string InputData;
	public string OutputGraph;
	public string OutputData1;
	public string EncoderRPath; //to load other R scripts
	public string EncoderTempPath; //use for Status, Special, GraphParams....
	public EncoderParams Ep;

	//pass this to R
	public EncoderStruct(string InputData, string OutputGraph, 
			string OutputData1, 
			string EncoderRPath, string EncoderTempPath,
			EncoderParams Ep)
	{
		this.InputData = InputData;
		this.OutputGraph = OutputGraph;
		this.OutputData1 = OutputData1;
		this.EncoderRPath = EncoderRPath;
		this.EncoderTempPath = EncoderTempPath;
		this.Ep = Ep;
	}

	~EncoderStruct() {}
}

public class EncoderGraphROptions
{
	public string inputData;
	public string outputGraph;
	public string outputData1;
	public string encoderRPath;
	public string encoderTempPath;
	public EncoderParams ep;
	public string title;
	public string operatingSystem;
	public string englishWords;
	public string translatedWords;
	public bool debug;
	public bool crossValidate;
	
	public EncoderGraphROptions(
			string inputData, string outputGraph, string outputData1, 
			string encoderRPath, string encoderTempPath,
			EncoderParams ep,
			string title, string operatingSystem,
			string englishWords, string translatedWords,
			bool debug, bool crossValidate)
	{
		this.inputData = inputData;
		this.outputGraph = outputGraph;
		this.outputData1 = outputData1;
		this.encoderRPath = encoderRPath;
		this.encoderTempPath = encoderTempPath;
		this.ep = ep;
		this.title = title;
		this.operatingSystem = operatingSystem;
		this.englishWords = englishWords;
		this.translatedWords = translatedWords;
		this.debug = debug;
		this.crossValidate = crossValidate;
	}

	public override string ToString() {
		return 
			"#inputdata\n" + 	inputData + "\n" + 
			"#outputgraph\n" + 	outputGraph + "\n" + 
			"#outputdata1\n" + 	outputData1 + "\n" + 
			"#encoderRPath\n" + 	encoderRPath + "\n" + 
			"#encoderTempPath\n" + 	encoderTempPath + "\n" + 
			ep.ToStringROptions() + "\n" + 
			"#title\n" + 		title + "\n" + 
			"#operatingsystem\n" + 	operatingSystem + "\n" +
			"#englishWords\n" + 	englishWords + "\n" +
			"#translatedWords\n" + 	translatedWords + "\n" +
			"#debug\n" +		Util.BoolToRBool(debug) + "\n" +
			"#crossValidate\n" +	Util.BoolToRBool(crossValidate) + "\n";
	}
	

	~EncoderGraphROptions() {}
}


//used on TreeViews capture and analyze
//in ec and ecS there are two separated curves, unfortunately, here is not known if it's ecc or con
public class EncoderCurve
{
	public bool Record;	//only on capture
	public string N;
	public string Series;
	public string Exercise;
	public string Laterality;	//only on analyze
	public double ExtraWeight;
	public double DisplacedWeight;
	public int Inertia;
	public string Start;
	public string Duration;
	public string Height;
	public string MeanSpeed;
	public string MaxSpeed;
	public string MaxSpeedT;
	public string MeanPower;
	public string PeakPower;
	public string PeakPowerT;
	public string PP_PPT;
	public string MeanForce;
	public string MaxForce;
	public string MaxForceT;
	
	public EncoderCurve () {
	}

	//used on TreeView capture
	public EncoderCurve (bool record, string n, 
			string start, string duration, string height, 
			string meanSpeed, string maxSpeed, string maxSpeedT,
			string meanPower, string peakPower, string peakPowerT, 
			string PP_PPT,
			string meanForce, string maxForce, string maxForceT
			)
	{
		this.Record = record;
		this.N = n;
		this.Start = start;
		this.Duration = duration;
		this.Height = height;
		this.MeanSpeed = meanSpeed;
		this.MaxSpeed = maxSpeed;
		this.MaxSpeedT = maxSpeedT;
		this.MeanPower = meanPower;
		this.PeakPower = peakPower;
		this.PeakPowerT = peakPowerT;
		this.PP_PPT = PP_PPT;	//PeakPower / PeakPowerTime
		this.MeanForce = meanForce;
		this.MaxForce = maxForce;
		this.MaxForceT = maxForceT;
	}

	//used on TreeView analyze
	public EncoderCurve (string n, string series, string exercise, 
			string laterality,
			double extraWeight, double displacedWeight,
			int inertia,
			string start, string duration, string height,
			string meanSpeed, string maxSpeed, string maxSpeedT,
			string meanPower, string peakPower, string peakPowerT, 
			string PP_PPT,
			string meanForce, string maxForce, string maxForceT)
	{
		this.N = n;
		this.Series = series;
		this.Exercise = exercise;
		this.Laterality = laterality;
		this.ExtraWeight = extraWeight;
		this.DisplacedWeight = displacedWeight;
		this.Inertia = inertia;
		this.Start = start;
		this.Duration = duration;
		this.Height = height;
		this.MeanSpeed = meanSpeed;
		this.MaxSpeed = maxSpeed;
		this.MaxSpeedT = maxSpeedT;
		this.MeanPower = meanPower;
		this.PeakPower = peakPower;
		this.PeakPowerT = peakPowerT;
		this.PP_PPT = PP_PPT;	//PeakPower / PeakPowerTime
		this.MeanForce = meanForce;
		this.MaxForce = maxForce;
		this.MaxForceT = maxForceT;
	}

	//http://stackoverflow.com/questions/894263/how-to-identify-if-a-string-is-a-number
	//this does not check if decimal point is a different character (eg '.' or ',')
	//note new method IsNumber on util.cs is better than this
	public bool IsNumberN() {
		int num;
		return int.TryParse(N, out num);
	}

	//check if last char is 'e' or 'c'
	private bool isValidLastCharN() {
		if(N.Length <= 1)
			return false;
		
		char lastChar = N[N.Length-1];
		if(lastChar == 'e' || lastChar == 'c')
			return true;
		
		return false;
	}
	//check if it's "21c" or "15e"
	public bool IsNumberNandEorC() {
		if(N.Length <= 1)
			return false;

		int num;
		if(int.TryParse(N.Substring(0, N.Length-1), out num) && isValidLastCharN())
			return true;

		return false;
	}
	//at least for RenderNAnalyze
	public bool IsValidN() {
		if (N == "MAX" || N == "AVG" || N == "SD" || IsNumberN() || IsNumberNandEorC())
			return true;
		return false;
	}

	public double GetParameter(string parameter) {
		switch(parameter) {
			case Constants.MeanSpeed:
				return Convert.ToDouble(MeanSpeed);
			case Constants.MaxSpeed:
				return Convert.ToDouble(MaxSpeed);
			case Constants.MeanForce:
				return Convert.ToDouble(MeanForce);
			case Constants.MaxForce:
				return Convert.ToDouble(MaxForce);
			case Constants.MeanPower:
				return Convert.ToDouble(MeanPower);
			case Constants.PeakPower:
				return Convert.ToDouble(PeakPower);
			default:
				return Convert.ToDouble(MeanPower);
		}
	}

	public string ToCSV(bool captureOrAnalyze, string decimalSeparator) {

		//latin:	2,3 ; 2,5
		//non-latin:	2.3 , 2.5

		string sep = ":::";
		
		string str = "";
		//TODO: if capture not shown because some variables like Inertia are not defined
		if(! captureOrAnalyze)
			str = 
				N + sep + Series + sep + Exercise + sep + Laterality + sep +
				ExtraWeight + sep + DisplacedWeight + sep + Inertia + sep + 
				Start + sep + Duration + sep + Height + sep + 
				MeanSpeed + sep + MaxSpeed + sep + MaxSpeedT + sep + 
				MeanPower + sep + PeakPower + sep + PeakPowerT + sep + 
				PP_PPT + sep +
				MeanForce + sep + MaxForce + sep + MaxForceT;
		
		if(decimalSeparator == "COMMA")
			str = Util.ConvertToComma(str);
		else
			str = Util.ConvertToPoint(str);
			
		if(decimalSeparator == "COMMA")
			return Util.ChangeChars(str, ":::", ";");
		else
			return Util.ChangeChars(str, ":::", ",");
	}
	
	public double MeanSpeedD { get { return Convert.ToDouble(MeanSpeed); } }
	public double MaxSpeedD  { get { return Convert.ToDouble(MaxSpeed);  } }
	public double MeanPowerD { get { return Convert.ToDouble(MeanPower); } }
	public double PeakPowerD { get { return Convert.ToDouble(PeakPower); } }
	public double MeanForceD { get { return Convert.ToDouble(MeanForce); } }
	public double MaxForceD  { get { return Convert.ToDouble(MaxForce);  } }

	
	~EncoderCurve() {}
}


//to know which is the best curve in a signal...
public class EncoderSignal
{
	private ArrayList curves;

	public EncoderSignal (ArrayList curves) {
		this.curves = curves;
	}

	public int CurvesNum() {
		return curves.Count;
	}

	//this can be an eccentric or concentric curve
	public int FindPosOfBest(string variable) {
		double bestValue = 0;
		int bestValuePos = 0;
		int i = 0;
		
		foreach(EncoderCurve curve in curves) 
		{
			if(curve.GetParameter(variable) > bestValue) {
				bestValue = curve.GetParameter(variable);
				bestValuePos = i;
			}

			i++;
		}
		return bestValuePos;
	}
	
	//this is an ecc-con curve
	public int FindPosOfBestEccCon(string variable) 
	{
		double eccValue = 0;
		double conValue = 0;

		double bestValue = 0; //will be ecc-con average
		int bestValuePos = 0; //will be the position of the ecc
		int i = 0;
		
		bool ecc = true;
		foreach(EncoderCurve curve in curves) 
		{
			if(ecc) {
				eccValue = curve.GetParameter(variable);
			} else {
				conValue = curve.GetParameter(variable);
				if( ( (eccValue + conValue) / 2 ) > bestValue) {
					bestValue = (eccValue + conValue) / 2;
					bestValuePos = i -1;
				}
			}

			ecc = ! ecc;
			i ++;
		}
		return bestValuePos;
	}
	
	~EncoderSignal() {}
}


//related to encoderSignalCurve table
public class EncoderSignalCurve {
	public int uniqueID;
	public int signalID;
	public int curveID;
	public int msCentral;
	
	public EncoderSignalCurve(int uniqueID, int signalID, int curveID, int msCentral) {
		this.uniqueID = uniqueID;
		this.signalID = signalID;
		this.curveID = curveID;
		this.msCentral = msCentral;
	}
	
	public override string ToString() {
		return uniqueID.ToString() + ":" + signalID.ToString() + ":" + 
			curveID.ToString() + ":" + msCentral.ToString();
	}
	
	~EncoderSignalCurve() {}
}


//used on TreeView
public class EncoderNeuromuscularData
{
	public string n; 
	public int e1_range;
	public int e1_t;
	public double e1_fmax;
	public double e1_rfd_avg;
	public double e1_i;

	public int ca_range;
	public int cl_t;
	public double cl_rfd_avg;
	public double cl_i;

	public double cl_f_avg;
	public double cl_vf;
	public double cl_f_max;

	public double cl_s_avg;
	public double cl_s_max;
	public double cl_p_avg;
	public double cl_p_max;

	public EncoderNeuromuscularData () {
	}

	//used on TreeView analyze
	public EncoderNeuromuscularData (
			string n, 
			int e1_range, int e1_t, double e1_fmax, double e1_rfd_avg, double e1_i,
			int ca_range, int cl_t, double cl_rfd_avg, double cl_i, 
			double cl_f_avg, double cl_vf, double cl_f_max, 
			double cl_s_avg, double cl_s_max, double cl_p_avg, double cl_p_max
			)
	{
		this.n = n;
		this.e1_range = e1_range; 
		this.e1_t = e1_t;
		this.e1_fmax = e1_fmax;
		this.e1_rfd_avg = e1_rfd_avg;
		this.e1_i = e1_i;
		this.ca_range = ca_range;
		this.cl_t = cl_t;
		this.cl_rfd_avg = cl_rfd_avg;
		this.cl_i = cl_i;
		this.cl_f_avg = cl_f_avg;
		this.cl_vf = cl_vf;
		this.cl_f_max = cl_f_max;
		this.cl_s_avg = cl_s_avg;
		this.cl_s_max = cl_s_max;
		this.cl_p_avg = cl_p_avg;
		this.cl_p_max = cl_p_max;
	}

	//reading contents file from graph.R
	public EncoderNeuromuscularData (string [] cells)
	{
		//cell[0] is not converted because is string
		for(int i = 1 ; i < cells.Length ;  i ++)
			cells[i] = Util.TrimDecimals(Convert.ToDouble(Util.ChangeDecimalSeparator(cells[i])),3);
	
		this.n 		= cells[0];
		this.e1_range 	= Convert.ToInt32(cells[1]); 
		this.e1_t 	= Convert.ToInt32(cells[2]);
		this.e1_fmax 	= Convert.ToDouble(cells[3]);
		this.e1_rfd_avg	= Convert.ToDouble(cells[4]);
		this.e1_i	= Convert.ToDouble(cells[5]);
		this.ca_range	= Convert.ToInt32(cells[6]);
		this.cl_t 	= Convert.ToInt32(cells[7]);
		this.cl_rfd_avg = Convert.ToDouble(cells[8]);
		this.cl_i 	= Convert.ToDouble(cells[9]);
		this.cl_f_avg 	= Convert.ToDouble(cells[10]);
		this.cl_vf 	= Convert.ToDouble(cells[11]);
		this.cl_f_max 	= Convert.ToDouble(cells[12]);
		this.cl_s_avg 	= Convert.ToDouble(cells[13]);
		this.cl_s_max 	= Convert.ToDouble(cells[14]);
		this.cl_p_avg 	= Convert.ToDouble(cells[15]);
		this.cl_p_max 	= Convert.ToDouble(cells[16]);
	}

	public string ToCSV(string decimalSeparator) {
		//latin:	2,3 ; 2,5
		//non-latin:	2.3 , 2.5

		string sep = ":::";
		string str = 
			n + sep + e1_range.ToString() + sep + 
			e1_t.ToString() + sep + e1_fmax.ToString() + sep + 
			e1_rfd_avg.ToString() + sep + e1_i.ToString() + sep + 
			ca_range.ToString() + sep + cl_t.ToString() + sep + 
			cl_rfd_avg.ToString() + sep + cl_i.ToString() + sep + 
			cl_f_avg.ToString() + sep + cl_vf.ToString() + sep + cl_f_max.ToString() + sep + 
			cl_s_avg.ToString() + sep + cl_s_max.ToString() + sep + 
			cl_p_avg.ToString() + sep + cl_p_max.ToString();

		if(decimalSeparator == "COMMA")
			str = Util.ConvertToComma(str);
		else
			str = Util.ConvertToPoint(str);
			
		if(decimalSeparator == "COMMA")
			return Util.ChangeChars(str, ":::", ";");
		else
			return Util.ChangeChars(str, ":::", ",");
	}
}

public class EncoderSQL
{
	public string uniqueID;
	public int personID;
	public int sessionID;
	public int exerciseID;
	public string eccon;
	public string laterality;
	public string extraWeight;
	public string signalOrCurve;
	public string filename;
	public string url;	//URL of data of signals and curves. Stored in DB as relative. Used in software as absolute. See SqliteEncoder
	public int time;
	public int minHeight;
	public string description;
	public string status;	//active or inactive curves
	public string videoURL;	//URL of video of signals. Stored in DB as relative. Used in software as absolute. See SqliteEncoder
	
	//encoderConfiguration conversions
	//in signals and curves, need to do conversions (invert, inertiaMomentum, diameter)
	public EncoderConfiguration encoderConfiguration;
//	public int inertiaMomentum; //kg*cm^2
//	public double diameter;
	
	public string future1;
	public string future2;
	public string future3;

	public string exerciseName;
	
	public string ecconLong;
	
	public EncoderSQL ()
	{
	}

	public EncoderSQL (string uniqueID, int personID, int sessionID, int exerciseID, 
			string eccon, string laterality, string extraWeight, string signalOrCurve, 
			string filename, string url, int time, int minHeight, 
			string description, string status, string videoURL, 
			EncoderConfiguration encoderConfiguration,
			string future1, string future2, string future3, 
			string exerciseName
			)
	{
		this.uniqueID = uniqueID;
		this.personID = personID;
		this.sessionID = sessionID;
		this.exerciseID = exerciseID;
		this.eccon = eccon;
		this.laterality = laterality;
		this.extraWeight = extraWeight;
		this.signalOrCurve = signalOrCurve;
		this.filename = filename;
		this.url = url;
		this.time = time;
		this.minHeight = minHeight;
		this.description = description;
		this.status = status;
		this.videoURL = videoURL;
		this.encoderConfiguration = encoderConfiguration;
		this.future1 = future1;	//on curves: meanPower
		this.future2 = future2;
		this.future3 = future3;
		this.exerciseName = exerciseName;

		if(eccon == "c")
			ecconLong = Catalog.GetString("Concentric");
		else if(eccon == "ec" || eccon == "ecS")
			ecconLong = Catalog.GetString("Eccentric-concentric");
		else
			ecconLong = Catalog.GetString("Concentric-eccentric");
	}

	//used on encoder table
	public enum Eccons { ALL, ecS, ceS, c } 

	public string GetDate(bool pretty) {
		int pointPos = filename.LastIndexOf('.');
		int dateLength = 19; //YYYY-MM-DD_hh-mm-ss
		string date = filename.Substring(pointPos - dateLength, dateLength);
		if(pretty) {
			string [] dateParts = date.Split(new char[] {'_'});
			date = dateParts[0] + " " + dateParts[1].Replace('-',':');
		}
		return date;
	}

	public string GetFullURL(bool convertPathToR) {
		string str = url + Path.DirectorySeparatorChar + filename;
		/*	
			in Linux is separated by '/'
			in windows is separated by '\'
			but R needs always '/', then do the conversion
		 */
		if(convertPathToR && UtilAll.IsWindows())
			str = str.Replace("\\","/");

		return str;
	}

	//showMeanPower is used in curves, but not in signal
	public string [] ToStringArray (int count, bool checkboxes, bool video, bool encoderConfigPretty, bool showMeanPower) {
		int all = 9;
		if(checkboxes)
			all ++;
		if(video)
			all++;
		if(showMeanPower)
			all++;


		string [] str = new String [all];
		int i=0;
		str[i++] = uniqueID;
	
		if(checkboxes)
			str[i++] = "";	//checkboxes
	
		str[i++] = count.ToString();
		str[i++] = Catalog.GetString(exerciseName);
		str[i++] = Catalog.GetString(laterality);
		str[i++] = extraWeight;
		
		if(showMeanPower)
			str[i++] = future1;

		if(encoderConfigPretty)
			str[i++] = encoderConfiguration.ToStringPretty();
		else
			str[i++] = encoderConfiguration.code.ToString();
		
		str[i++] = ecconLong;
		str[i++] = GetDate(true);
		
		if(video) {
			if(videoURL != "")
				str[i++] = Catalog.GetString("Yes");
			else
				str[i++] = Catalog.GetString("No");
		}

		str[i++] = description;
		return str;
	}

	//uniqueID:name
	public EncoderSQL ChangePerson(string newIDAndName) {
		int newPersonID = Util.FetchID(newIDAndName);
		string newPersonName = Util.FetchName(newIDAndName);
		string newFilename = filename;

		personID = newPersonID;

		/*
		 * this can fail because person name can have an "-"
		string [] filenameParts = filename.Split(new char[] {'-'});
		filenameParts[0] = newPersonID.ToString();
		filenameParts[1] = newPersonName;
		//the rest will be the same: curveID, timestamp, extension 
		filename = Util.StringArrayToString(filenameParts, "-");
		*/


		/*
		 * filename curve has personID-name-uniqueID-fulldate.txt
		 * filename signal as personID-name-fulldate.txt
		 * in both cases name can have '-' (fuck)
		 * eg: filename curve:
		 * 163-personname-840-2013-04-05_14-11-11.txt
		 * filename signal
		 * 163-personname-2013-04-05_14-03-45.txt
		 *
		 * then,
		 * on curve:
		 * last 23 letters are date and ".txt",
		 * write newPersonID-newPersonName-uniqueID-last23letters
		 * 
		 * on signal:
		 * last 23 letters are date and ".txt",
		 * write newPersonID-newPersonName-last23letters
		 */

		if(signalOrCurve == "curve") 
			newFilename = newPersonID + "-" + newPersonName + "-" + uniqueID + "-" + GetDate(false) + ".txt";
		else 
			newFilename = newPersonID + "-" + newPersonName + "-" + GetDate(false) + ".txt";

		bool success = false;
		success = Util.FileMove(url, filename, newFilename);
		if(success)
			filename = newFilename;

		//will update SqliteEncoder
		return (this);
	}


	/* 
	 * translations stuff
	 * used to store in english and show translated in GUI
	 */
		
	private string [] lateralityOptionsEnglish = { "RL", "R", "L" }; //attention: if this changes, change it also in gui/encoder.cs createEncoderCombos()
	public string LateralityToEnglish() 
	{
		int count = 0;
		foreach(string option in lateralityOptionsEnglish) {
			if(Catalog.GetString(option) == laterality)
				return lateralityOptionsEnglish[count];
			count ++;
		}
		//default return first value
		return lateralityOptionsEnglish[0];
	}



	//used in NUnit
	public string Filename
	{
		set { filename = value; }
	}

}

//related to all reps, not only active
public class EncoderPersonCurvesInDBDeep
{
	public double extraWeight;
	public int count; //this count is all reps (not only active)

	public EncoderPersonCurvesInDBDeep(double w, int c) {
		this.extraWeight = w;
		this.count = c;
	}

	public override string ToString() {
		return count.ToString() + "*" + extraWeight.ToString();// + "Kg";
	}
}
public class EncoderPersonCurvesInDB
{
	public int personID;
	public int sessionID;
	public string sessionName;
	public string sessionDate;
	public int countActive;
	public int countAll;
	public List<EncoderPersonCurvesInDBDeep> lDeep;
	
	public EncoderPersonCurvesInDB() {
	}
	public EncoderPersonCurvesInDB(int personID, int sessionID, string sessionName, string sessionDate) 
	{
		this.personID =		personID;
		this.sessionID = 	sessionID;
		this.sessionName = 	sessionName;
		this.sessionDate = 	sessionDate;
	}

	public string [] ToStringArray(bool deep) {
		string [] s;

		//the "" will be for the checkbox on genericWin
		if(deep) {
			s = new string[]{ sessionID.ToString(), "", sessionName, sessionDate,
				//countActive.ToString(), countAll.ToString()
				countAll.ToString(), DeepPrint()
			};
		} else {
			s = new string[]{ sessionID.ToString(), "", sessionName, sessionDate,
				//countActive.ToString(), countAll.ToString()
				countAll.ToString()
			};
		}

		return s;
	}

	private string DeepPrint() {
		string s = "";
		string sep = "";
		foreach(EncoderPersonCurvesInDBDeep e in lDeep) {
			s += sep + e.ToString();
			sep = " ";
		}
		return s;
	}
}

public class EncoderExercise
{
	public int uniqueID;
	public string name;
	public int percentBodyWeight;
	public string ressistance;
	public string description;
	public double speed1RM;

	public EncoderExercise() {
	}

	public EncoderExercise(string name) {
		this.name = name;
	}

	public EncoderExercise(int uniqueID, string name, int percentBodyWeight, 
			string ressistance, string description, double speed1RM)
	{
		this.uniqueID = uniqueID;
		this.name = name;
		this.percentBodyWeight = percentBodyWeight;
		this.ressistance = ressistance;
		this.description = description;
		this.speed1RM = speed1RM;
	}

	public bool IsPredefined() {
		if(
				name == "Bench press" ||
				name == "Squat" ||
				name == "Free" ||
				name == "Jump" ||
				name == "Inclined plane" ||
				name == "Inclined plane BW" )
			return true;
		else 
			return false;
	}

	~EncoderExercise() {}
}

public class Encoder1RM
{
	public int uniqueID;
	public int personID;
	public int sessionID;
	public DateTime date;
	public int exerciseID;
	public double load1RM;
	
	public string personName;
	public string exerciseName;
	
	public Encoder1RM() {
	}

	public Encoder1RM(int uniqueID, int personID, int sessionID, DateTime date, int exerciseID, double load1RM)
	{
		this.uniqueID = uniqueID;
		this.personID = personID;
		this.sessionID = sessionID;
		this.date = date;
		this.exerciseID = exerciseID;
		this.load1RM = load1RM;
	}

	public Encoder1RM(int uniqueID, int personID, int sessionID, DateTime date, int exerciseID, double load1RM, string personName, string exerciseName)
	{
		this.uniqueID = uniqueID;
		this.personID = personID;
		this.sessionID = sessionID;
		this.date = date;
		this.exerciseID = exerciseID;
		this.load1RM = load1RM;
		this.personName = personName;
		this.exerciseName = exerciseName;
	}

	public string [] ToStringArray() {
		string [] s = { uniqueID.ToString(), load1RM.ToString() };
		return s;
	}
	
	public string [] ToStringArray2() {
		string [] s = { uniqueID.ToString(), personName, exerciseName, load1RM.ToString(), date.ToShortDateString() };
		return s;
	}


	~Encoder1RM() {}
}

public class EncoderCaptureCurve {
	public bool up;
	public int startFrame;
        public int endFrame;

	public EncoderCaptureCurve(int startFrame, int endFrame)
	{
		this.startFrame = startFrame;
		this.endFrame = endFrame;
	}
	
	public string DirectionAsString() {
		if(up)
			return "UP";
		else
			return "DOWN";
	}

	~EncoderCaptureCurve() {}
}

public class EncoderCaptureCurveArray {
	public ArrayList ecc;	//each of the EncoderCaptureCurve
	public int curvesAccepted; //starts at int 0. How many ecc have been accepted (will be rows in treeview_encoder_capture_curves)
	
	public EncoderCaptureCurveArray() {
		ecc = new ArrayList();
		curvesAccepted = 0;
	}
	
	~EncoderCaptureCurveArray() {}
}

public class EncoderBarsData {
	public double MeanSpeed;
	public double MaxSpeed;
	public double MeanForce;
	public double MaxForce;
	public double MeanPower;
	public double PeakPower;
	
	public EncoderBarsData(double meanSpeed, double maxSpeed, double meanForce, double maxForce, double meanPower, double peakPower) {
		this.MeanSpeed = meanSpeed;
		this.MaxSpeed  = maxSpeed;
		this.MeanForce = meanForce;
		this.MaxForce  = maxForce;
		this.MeanPower = meanPower;
		this.PeakPower = peakPower;
	}

	public double GetValue (string option) {
		if(option == Constants.MeanSpeed)
			return MeanSpeed;
		else if(option == Constants.MaxSpeed)
			return MaxSpeed;
		else if(option == Constants.MeanForce)
			return MeanForce;
		else if(option == Constants.MaxForce)
			return MaxForce;
		else if(option == Constants.MeanPower)
			return MeanPower;
		else // option == Constants.PeakPower
			return PeakPower;
	}
	
	~EncoderBarsData() {}
}

public class EncoderConfigurationSQLObject
{
	public int uniqueID;
	public Constants.EncoderGI encoderGI;
	public bool active; //true or false. One true for each encoderGI (GRAVITATORY, INERTIAL)
	public string name;
	public EncoderConfiguration encoderConfiguration;
	public string description;

	public EncoderConfigurationSQLObject()
	{
		uniqueID = -1;
	}

	public EncoderConfigurationSQLObject(int uniqueID,
			Constants.EncoderGI encoderGI, bool active, string name,
			EncoderConfiguration encoderConfiguration,
			string description)
	{
		this.uniqueID = uniqueID;
		this.encoderGI = encoderGI;
		this.active = active;
		this.name = name;
		this.encoderConfiguration = encoderConfiguration;
		this.description = description;
	}

	//converts encoderConfiguration string from SQL
	public EncoderConfigurationSQLObject(int uniqueID,
			Constants.EncoderGI encoderGI, bool active, string name,
			string encoderConfigurationString,
			string description)
	{
		string [] strFull = encoderConfigurationString.Split(new char[] {':'});
		EncoderConfiguration econf = new EncoderConfiguration(
				(Constants.EncoderConfigurationNames)
				Enum.Parse(typeof(Constants.EncoderConfigurationNames), strFull[0]) );
		econf.ReadParamsFromSQL(strFull);

		this.uniqueID = uniqueID;
		this.encoderGI = encoderGI;
		this.active = active;
		this.name = name;
		this.encoderConfiguration = econf;
		this.description = description;
	}

	//imports from file
	public EncoderConfigurationSQLObject(string contents)
	{
		string line;
		using (StringReader reader = new StringReader (contents)) {
			do {
				line = reader.ReadLine ();

				if (line == null)
					break;
				if (line == "" || line[0] == '#')
					continue;

				string [] parts = line.Split(new char[] {'='});
				if(parts.Length != 2)
					continue;

				uniqueID = -1;
				if(parts[0] == "encoderGI")
				{
					if(Enum.IsDefined(typeof(Constants.EncoderGI), parts[1]))
						encoderGI = (Constants.EncoderGI) Enum.Parse(typeof(Constants.EncoderGI), parts[1]);
				}

				//active is not needed on import, because on import it's always marked as active
				else if(parts[0] == "active" && parts[1] != "")
					active = (parts[1] == "True");
				else if(parts[0] == "name" && parts[1] != "")
					name = parts[1];
				else if(parts[0] == "EncoderConfiguration")
				{
					string [] ecFull = parts[1].Split(new char[] {':'});
					if(Enum.IsDefined(typeof(Constants.EncoderConfigurationNames), ecFull[0]))
					{
						//create object
						encoderConfiguration = new EncoderConfiguration(
								(Constants.EncoderConfigurationNames)
								Enum.Parse(typeof(Constants.EncoderConfigurationNames), ecFull[0]) );
						//assign the rest of params
						encoderConfiguration.ReadParamsFromSQL(ecFull);
					}
				}
				else if(parts[0] == "description" && parts[1] != "")
					description = parts[1];
			} while(true);
		}
	}

	public string ToSQLInsert()
	{
		 string idStr = uniqueID.ToString();
		 if(idStr == "-1")
			 idStr = "NULL";

		 return idStr +
			 ", \"" + encoderGI.ToString() + "\"" +
			 ", \"" + active.ToString() + "\"" +
			 ", \"" + name + "\"" +
			 ", \"" + encoderConfiguration.ToStringOutput(EncoderConfiguration.Outputs.SQL) + "\"" +
			 ", \"" + description + "\"" +
			 ", \"\", \"\", \"\""; //future1, future2, future3
	}

	public string ToFile()
	{
		return
			"#Case sensitive!\n" +
			"#Comments start with sharp sign\n" +
			"#Options are key/values with an = separating them\n" +
			"#DO NOT write comments in the same line than key/value pairs\n" +
			"#\n" +
			"#DO NOT WRITE SPACES JUST BEFORE OR AFTER THE '=' SIGN\n" +
			"#This work:\n" +
			"#name=My encoder config\n" +
			"#This doesn't work:\n" +
			"#name= My encoder config\n" +
			"#\n" +
			"#Whitelines are allowed\n" +
			"\nname=" + name + "\n" +
			"description=" + description + "\n" +
			"\n#encoderGI must be GRAVITATORY or INERTIAL\n" +
			"encoderGI=" + encoderGI.ToString() + "\n" +
			"\n#EncoderConfiguration if exists, this will be used and cannot be changed\n" +
"#name:d:D:anglePush:angleWeight:inertiaMachine:gearedDown:inertiaTotal:extraWeightN:extraWeightGrams:extraWeightLenght:list_d\n" +
"#list_d is list of anchorages in centimeters. each value separated by '_' . Decimal separator is '.'\n" +
			"EncoderConfiguration=" + encoderConfiguration.ToStringOutput(EncoderConfiguration.Outputs.SQL);
	}
}

public class EncoderConfiguration
{
	public Constants.EncoderConfigurationNames name;
	public Constants.EncoderType type;
	public int position; //used to find values on the EncoderConfigurationList. Numeration changes on every encoder and on not inertial/inertial
	public string image;
	public string code;	//this code will be stored untranslated but will be translated just to be shown
	public string text;
	public bool has_d;	//axis
	public bool has_D;	//external disc or pulley
	public bool has_angle_push;
	public bool has_angle_weight;
	public bool has_inertia;
	public bool has_gearedDown;
	public bool rotaryFrictionOnAxis;
	public double d;	//axis 		//ATTENTION: this inertial param can be changed on main GUI
	public double D;	//external disc or pulley
	public int anglePush;
	public int angleWeight;
	
	public int inertiaMachine; //this is the inertia without the disc
	
	// see methods: GearedUpDisplay() SetGearedDownFromDisplay(string gearedUpStr) 
	public int gearedDown;	//demultiplication
	
	public int inertiaTotal; //this is the inertia used by R
	public int extraWeightN; //how much extra weights (inertia) //ATTENTION: this param can be changed on main GUI
	public int extraWeightGrams; //weight of each extra weight (inertia)
	public double extraWeightLength; //length from center to center (cm) (inertia)
	
	public List<double> list_d;	//list of diameters depending on the anchorage position 


	public string textDefault = Catalog.GetString("Linear encoder attached to a barbell.") + "\n" + 
		Catalog.GetString("Also common gym tests like jumps or chin-ups.");

	//this is the default values
	public EncoderConfiguration() {
		name = Constants.EncoderConfigurationNames.LINEAR;
		type = Constants.EncoderType.LINEAR;
		position = 0;
		image = Constants.FileNameEncoderLinearFreeWeight;
		code = Constants.DefaultEncoderConfigurationCode;
		text = textDefault;
		has_d = false;
		has_D = false;
		has_angle_push = false;
		has_angle_weight = false;
		has_inertia = false;
		has_gearedDown = false; //gearedDown can be changed by user
		rotaryFrictionOnAxis = false;
		d = -1;
		D = -1;
		anglePush = -1;
		angleWeight = -1;
		inertiaMachine = -1;
		gearedDown = 1;
		inertiaTotal = -1;
		extraWeightN = 0;
		extraWeightGrams = 0;
		extraWeightLength = 1;
		list_d = new List<double>(); 
	}

	// note: if this changes, change also in:
	// UtilEncoder.EncoderConfigurationList(enum encoderType)
	
	public EncoderConfiguration(Constants.EncoderConfigurationNames name) {
		this.name = name;
		has_d = false;
		has_D = false;
		has_angle_push = false;
		has_angle_weight = false;
		has_inertia = false;
		has_gearedDown = false; //gearedDown can be changed by user
		rotaryFrictionOnAxis = false;
		gearedDown = 1;
		list_d = new List<double>(); 

		// ---- LINEAR ----
		// ---- not inertial
		if(name == Constants.EncoderConfigurationNames.LINEAR) {
			type = Constants.EncoderType.LINEAR;
			position = 0;
			image = Constants.FileNameEncoderLinearFreeWeight;
			code = Constants.DefaultEncoderConfigurationCode;
			text = textDefault;
		}
		else if(name == Constants.EncoderConfigurationNames.LINEARINVERTED) {
			type = Constants.EncoderType.LINEAR;
			position = 1;
			image =Constants.FileNameEncoderLinearFreeWeightInv;
			code = "Linear inv - barbell";
			text = Catalog.GetString("Linear encoder inverted attached to a barbell.");
		}
		else if(name == Constants.EncoderConfigurationNames.WEIGHTEDMOVPULLEYLINEARONPERSON1) {
			type = Constants.EncoderType.LINEAR;
			position = 2;
			image = Constants.FileNameEncoderWeightedMovPulleyOnPerson1;
			code = "Linear - barbell - moving pulley";
			text = Catalog.GetString("Linear encoder attached to a barbell.") + " " + 
				Catalog.GetString("Barbell is connected to a weighted moving pulley.") 
				+ " " + Catalog.GetString("Mass is geared down by 2."); 
		
			gearedDown = 2;
		}
		else if(name == Constants.EncoderConfigurationNames.WEIGHTEDMOVPULLEYLINEARONPERSON1INV) {
			type = Constants.EncoderType.LINEAR;
			position = 3;
			image = Constants.FileNameEncoderWeightedMovPulleyOnPerson1Inv;
			code = "Linear inv - barbell - moving pulley";
			text = Catalog.GetString("Linear encoder inverted attached to a barbell.") + " " + 
				Catalog.GetString("Barbell is connected to a weighted moving pulley.")
				+ " " + Catalog.GetString("Mass is geared down by 2."); 
		
			gearedDown = 2;
		}
		else if(name == Constants.EncoderConfigurationNames.WEIGHTEDMOVPULLEYLINEARONPERSON2) {
			type = Constants.EncoderType.LINEAR;
			position = 4;
			image = Constants.FileNameEncoderWeightedMovPulleyOnPerson2;
			code = "Linear - barbell - pulley - moving pulley";
			text = Catalog.GetString("Linear encoder attached to a barbell.") + " " + 
				Catalog.GetString("Barbell is connected to a fixed pulley that is connected to a weighted moving pulley.")
				+ " " + Catalog.GetString("Mass is geared down by 2."); 
		
			gearedDown = 2;
		}
		else if(name == Constants.EncoderConfigurationNames.WEIGHTEDMOVPULLEYLINEARONPERSON2INV) {
			type = Constants.EncoderType.LINEAR;
			position = 5;
			image = Constants.FileNameEncoderWeightedMovPulleyOnPerson2Inv;
			code = "Linear inv - barbell - pulley - moving pulley";
			text = Catalog.GetString("Linear encoder inverted attached to a barbell.") + " " + 
				Catalog.GetString("Barbell is connected to a fixed pulley that is connected to a weighted moving pulley.")
				+ " " + Catalog.GetString("Mass is geared down by 2."); 
		
			gearedDown = 2;
		}
		else if(name == Constants.EncoderConfigurationNames.WEIGHTEDMOVPULLEYONLINEARENCODER) {
			type = Constants.EncoderType.LINEAR;
			position = 6;
			image = Constants.FileNameEncoderWeightedMovPulleyOnLinearEncoder;
			code = "Linear - moving pulley";
			text = Catalog.GetString("Linear encoder attached to a weighted moving pulley.")
				+ " " + Catalog.GetString("Mass is geared down by 2."); 
		
			gearedDown = 2;
		}
		else if(name == Constants.EncoderConfigurationNames.LINEARONPLANE) {
			type = Constants.EncoderType.LINEAR;
			position = 7;
			image = Constants.FileNameEncoderLinearOnPlane;
			code = "Linear - inclined plane";
			text = Catalog.GetString("Linear encoder on a inclined plane.") + "\n" + 
				Catalog.GetString("Suitable also for horizontal movement. Just set a 0 push angle.");
			
			has_angle_push = true;
			has_angle_weight = false;
		}
		else if(name == Constants.EncoderConfigurationNames.LINEARONPLANEWEIGHTDIFFANGLE) {
			type = Constants.EncoderType.LINEAR;
			position = 8;
			image = Constants.FileNameEncoderLinearOnPlaneWeightDiffAngle;
			code = "Linear - inclined plane different angle";
			text = Catalog.GetString("Linear encoder on a inclined plane moving a weight in different angle.") + "\n" +
				Catalog.GetString("Suitable also for horizontal movement. Just set a 0 push angle.");
			
			has_angle_push = true;
			has_angle_weight = true;
		}
		// ---- inertial
		else if(name == Constants.EncoderConfigurationNames.LINEARINERTIAL) {
			type = Constants.EncoderType.LINEAR;
			position = 0;
			image = Constants.FileNameEncoderLinearInertial;
			code = "Linear - inertial machine";
			text = Catalog.GetString("Linear encoder on inertia machine.") + "\n" + 
				Catalog.GetString("Configuration NOT Recommended! Please use a rotary encoder.") + "\n" +
				Catalog.GetString("On inertial machines, 'd' means the average diameter where the pull-push string is rolled");
			
			has_d = true;
			has_inertia = true;
		}
		// ---- ROTARY FRICTION ----
		// ---- not inertial
		else if(name == Constants.EncoderConfigurationNames.ROTARYFRICTIONSIDE) {
			type = Constants.EncoderType.ROTARYFRICTION;
			position = 0;
			image = Constants.FileNameEncoderFrictionSide;
			code = "Rotary friction - pulley";
			text = Catalog.GetString("Rotary friction encoder on pulley.");
		}
		else if(name == Constants.EncoderConfigurationNames.ROTARYFRICTIONAXIS) {
			type = Constants.EncoderType.ROTARYFRICTION;
			position = 1;
			image = Constants.FileNameEncoderFrictionAxis;
			code = "Rotary friction - pulley axis";
			text = Catalog.GetString("Rotary friction encoder on pulley axis.");

			has_d = true;
			has_D = true;
		}
		else if(name == Constants.EncoderConfigurationNames.WEIGHTEDMOVPULLEYROTARYFRICTION) {
			type = Constants.EncoderType.ROTARYFRICTION;
			position = 2;
			image = Constants.FileNameEncoderFrictionWithMovPulley;
			code = "Rotary friction - moving pulley";
			text = Catalog.GetString("Rotary friction encoder on weighted moving pulley.");
		}
		// ---- inertial
		// ---- rotary friction not on axis
		else if(name == Constants.EncoderConfigurationNames.ROTARYFRICTIONSIDEINERTIAL) {
			type = Constants.EncoderType.ROTARYFRICTION;
			position = 0;
			image = Constants.FileNameEncoderFrictionSideInertial;
			code = "Rotary friction - inertial machine side";
			text = Catalog.GetString("Rotary friction encoder on inertial machine side.") + "\n" +
				Catalog.GetString("On inertial machines, 'd' means the average diameter where the pull-push string is rolled");

			has_d = true;
			has_D = true;
			has_inertia = true;
		}
		else if(name == Constants.EncoderConfigurationNames.ROTARYFRICTIONSIDEINERTIALLATERAL) {
			type = Constants.EncoderType.ROTARYFRICTION;
			position = 1;
			image = Constants.FileNameEncoderFrictionSideInertialLateral;
			code = "Rotary friction - inertial machine side - horizontal movement";
			text = Catalog.GetString("Rotary friction encoder on inertial machine when person is moving horizontally.") + "\n" +
				Catalog.GetString("On inertial machines, 'd' means the average diameter where the pull-push string is rolled");

			has_d = true;
			has_D = true;
			has_inertia = true;
		}
		else if(name == Constants.EncoderConfigurationNames.ROTARYFRICTIONSIDEINERTIALMOVPULLEY) {
			type = Constants.EncoderType.ROTARYFRICTION;
			position = 2;
			image = Constants.FileNameEncoderFrictionSideInertialMovPulley;
			code = "Rotary friction - inertial machine side geared up";
			text = Catalog.GetString("Rotary friction encoder on inertial machine geared up.") + "\n" +
				Catalog.GetString("On inertial machines, 'd' means the average diameter where the pull-push string is rolled") + "\n" + 
				Catalog.GetString("Inertial machine rolls twice faster than body."); 

			has_d = true;
			has_D = true;
			has_inertia = true;
			has_gearedDown = true;
		}

		// ---- rotary friction on axis
		else if(name == Constants.EncoderConfigurationNames.ROTARYFRICTIONAXISINERTIAL) {
			type = Constants.EncoderType.ROTARYFRICTION;
			position = 0;
			image = Constants.FileNameEncoderFrictionAxisInertial;
			code = "Rotary friction axis - inertial machine axis";
			text = Catalog.GetString("Rotary friction encoder on inertial machine axis.") + "\n" +
				Catalog.GetString("On inertial machines, 'd' means the average diameter where the pull-push string is rolled");

			has_d = true;
			has_inertia = true;
			rotaryFrictionOnAxis = true;
		}
		else if(name == Constants.EncoderConfigurationNames.ROTARYFRICTIONAXISINERTIALLATERAL) {
			type = Constants.EncoderType.ROTARYFRICTION;
			position = 1;
			image = Constants.FileNameEncoderFrictionAxisInertialLateral;
			code = "Rotary friction - inertial machine axis - horizontal movement";
			text = Catalog.GetString("Rotary friction encoder on inertial machine when person is moving horizontally.") + "\n" +
				Catalog.GetString("On inertial machines, 'd' means the average diameter where the pull-push string is rolled");

			has_d = true;
			has_inertia = true;
			rotaryFrictionOnAxis = true;
		}
		else if(name == Constants.EncoderConfigurationNames.ROTARYFRICTIONAXISINERTIALMOVPULLEY) {
			type = Constants.EncoderType.ROTARYFRICTION;
			position = 2;
			image = Constants.FileNameEncoderFrictionAxisInertialMovPulley;
			code = "Rotary friction - inertial machine axis geared up";
			text = Catalog.GetString("Rotary friction encoder on inertial machine geared up.") + "\n" +
				Catalog.GetString("On inertial machines, 'd' means the average diameter where the pull-push string is rolled") + "\n" + 
				Catalog.GetString("Inertial machine rolls twice faster than body."); 

			has_d = true;
			has_inertia = true;
			rotaryFrictionOnAxis = true;
			has_gearedDown = true;
		}

		// ---- ROTARY AXIS ----
		// ---- not inertial
		else if(name == Constants.EncoderConfigurationNames.ROTARYAXIS) {
			type = Constants.EncoderType.ROTARYAXIS;
			position = 0;
			image = Constants.FileNameEncoderRotaryAxisOnAxis;
			code = "Rotary axis - pulley axis";
			text = Catalog.GetString("Rotary axis encoder on pulley axis.");

			has_D = true;
		}
		else if(name == Constants.EncoderConfigurationNames.WEIGHTEDMOVPULLEYROTARYAXIS) {
			type = Constants.EncoderType.ROTARYAXIS;
			position = 1;
			image = Constants.FileNameEncoderAxisWithMovPulley;
			code = "Rotary axis - moving pulley";
			text = Catalog.GetString("Rotary axis encoder on weighted moving pulley.")
				+ " " + Catalog.GetString("Mass is geared down by 2."); 
			
			gearedDown = 2;
		}
		// ---- inertial
		else if(name == Constants.EncoderConfigurationNames.ROTARYAXISINERTIAL) {
			type = Constants.EncoderType.ROTARYAXIS;
			position = 0;
			image = Constants.FileNameEncoderAxisInertial;
			code = "Rotary axis - inertial machine";
			text = Catalog.GetString("Rotary axis encoder on inertial machine.") + "\n" +
				Catalog.GetString("On inertial machines, 'd' means the average diameter where the pull-push string is rolled");

			has_d = true;
			has_inertia = true;
		}
		else if(name == Constants.EncoderConfigurationNames.ROTARYAXISINERTIALLATERAL) {
			type = Constants.EncoderType.ROTARYAXIS;
			position = 1;
			image = Constants.FileNameEncoderAxisInertialLateral;
			code = "Rotary axis - inertial machine - horizontal movement";
			text = Catalog.GetString("Rotary axis encoder on inertial machine when person is moving horizontally.") + "\n" +
				Catalog.GetString("On inertial machines, 'd' means the average diameter where the pull-push string is rolled");

			has_d = true;
			has_inertia = true;
		}
		else if(name == Constants.EncoderConfigurationNames.ROTARYAXISINERTIALMOVPULLEY) {
			type = Constants.EncoderType.ROTARYAXIS;
			position = 2;
			image = Constants.FileNameEncoderAxisInertialMovPulley;
			code = "Rotary axis - inertial machine geared up";
			text = Catalog.GetString("Rotary axis encoder on inertial machine geared up.") + "\n" +
				Catalog.GetString("On inertial machines, 'd' means the average diameter where the pull-push string is rolled") + "\n" + 
				Catalog.GetString("Inertial machine rolls twice faster than body."); 

			has_d = true;
			has_inertia = true;
			has_gearedDown = true;
		}
	}

	public void SetInertialDefaultOptions() {
		//after creating Constants.EncoderConfigurationNames.ROTARYAXISINERTIAL
		inertiaMachine = 900;
		d = 5;
		list_d = new List<double>(); 
		list_d.Add(d);
	}

	public void ReadParamsFromSQL (string [] strFull) 
	{
		//adds other params
		this.d = 	   Convert.ToDouble(Util.ChangeDecimalSeparator(strFull[1]));
		this.D = 	   Convert.ToDouble(Util.ChangeDecimalSeparator(strFull[2]));
		this.anglePush =   Convert.ToInt32(strFull[3]);
		this.angleWeight = Convert.ToInt32(strFull[4]);
		this.inertiaMachine = 	Convert.ToInt32(strFull[5]);
		this.gearedDown =  Convert.ToInt32(strFull[6]);
	
		//this params started at 1.5.1
		if(strFull.Length > 7) {
			this.inertiaTotal = 	Convert.ToInt32(strFull[7]);
			this.extraWeightN = 	Convert.ToInt32(strFull[8]);
			this.extraWeightGrams = Convert.ToInt32(strFull[9]);
			this.extraWeightLength = Convert.ToDouble(Util.ChangeDecimalSeparator(strFull[10]));
			if(strFull.Length > 11) //this param starts at 1.5.3
				this.list_d = readList_d(strFull[11]);
		} else {
			this.inertiaTotal = 	inertiaMachine;
			this.extraWeightN = 	0;
			this.extraWeightGrams = 0;
			this.extraWeightLength = 1;
		}

		//if we load a signal previous to 1.5.3, put d in list_d to have something to be sent to R
		if(this.list_d.Count == 0)
			this.list_d.Add(d);
		else if (this.list_d.Count == 1 && this.list_d[0] == 0) {
			//check if diameter is zero is safest because some tests have been done while list_d has been completely implemented
			this.list_d[0] = this.d;
		}
	}
	//list_d contains the different diameters (byt eh anchorages). They are stored as '_'
	private List<double> readList_d(string listFromSQL) 
	{
		List<double> l = new List<double>(); 
		string [] strFull = listFromSQL.Split(new char[] {'_'});
		foreach (string s in strFull) {
			double d = Convert.ToDouble(Util.ChangeDecimalSeparator(s));
			l.Add(d);
		}
		return l;
	}

	public enum Outputs { ROPTIONS, RCSV, SQL, SQLECWINCOMPARE}
	/*
	 * SQLECWINCOMPARE is to know it two encoderConfigurations on encoderConfigurationWindow are the same
	 * because two inertial params change outside that window
	 */
	
	public string ToStringOutput(Outputs o) 
	{
		//for R and SQL		
		string str_d = Util.ConvertToPoint(d);
		string str_D = Util.ConvertToPoint(D);
		
		string sep = "";

		if(o == Outputs.ROPTIONS) {
			sep = "\n";
			return 
				"#name" + sep + 	name + sep + 
				"#str_d" + sep + 	str_d + sep + 
				"#str_D" + sep + 	str_D + sep + 
				"#anglePush" + sep + 	anglePush.ToString() + sep + 
				"#angleWeight" + sep + 	angleWeight.ToString() + sep +
				"#inertiaTotal" + sep + inertiaTotal.ToString() + sep + 
				"#gearedDown" + sep + 	gearedDown.ToString()
				;
		}
		else if (o == Outputs.RCSV) { //not single curve
			sep = ",";
			//do not need inertiaMachine, extraWeightN, extraWeightGrams, extraWeightLength (unneded for the R calculations)
			return 
				name + sep + 
				str_d + sep + 
				str_D + sep + 
				anglePush.ToString() + sep + 
				angleWeight.ToString() + sep +
				inertiaTotal.ToString() + sep + 
				gearedDown.ToString()
				;
		}
		else { //(o == Outputs.SQL || o == OUTPUTS.SQLECWINCOMPARE)
			sep = ":";

			string my_str_d = str_d;
			string my_str_extraWeightN = extraWeightN.ToString();

			if(o == Outputs.SQLECWINCOMPARE)
			{
				//this inertial params can be changed on main GUI
				my_str_d = "%";
				my_str_extraWeightN  = "%";
			}
			return 
				name + sep + 
				my_str_d + sep +
				str_D + sep + 
				anglePush.ToString() + sep + 
				angleWeight.ToString() + sep +
				inertiaMachine.ToString() + sep + 
				gearedDown.ToString() + sep + 
				inertiaTotal.ToString() + sep + 
				my_str_extraWeightN + sep +
				extraWeightGrams.ToString() + sep +
				extraWeightLength.ToString() + sep +
				writeList_d(list_d)
				;
		}
	}
	private string writeList_d(List<double> l) {
		string str = "";
		string sep = "";
		foreach(double d in l) {
			str += sep + Util.ConvertToPoint(d);
			sep = "_";
		}
		return str;
	}
	
	//just to show on a treeview	
	public string ToStringPretty() {
		string sep = "; ";

		string str_d = "";
		if(d != -1)
			str_d = sep + "d=" + d.ToString();

		string str_D = "";
		if(D != -1)
			str_D = sep + "D=" + D.ToString();

		string str_anglePush = "";
		if(anglePush != -1)
			str_anglePush = sep + "push angle=" + anglePush.ToString();

		string str_angleWeight = "";
		if(angleWeight != -1)
			str_angleWeight = sep + "weight angle=" + angleWeight.ToString();

		string str_inertia = "";
		if(has_inertia && inertiaTotal != -1)
			str_inertia = sep + "inertia total=" + inertiaTotal.ToString();

		string str_gearedDown = "";
		if(gearedDown != 1)	//1 is the default
			str_gearedDown = sep + "geared down=" + gearedDown.ToString();

		return code + str_d + str_D + str_anglePush + str_angleWeight + str_inertia + str_gearedDown;
	}

	/*
	 * IMPORTANT: on GUI is gearedDown is shown as UP (for clarity: 4, 3, 2, 1, 1/2, 1/3, 1/4)
	 * on C#, R, SQL we use "gearedDown" for historical reasons. So a conversion is done on displaying data to user
	 * gearedDown is stored as integer on database and is converted to this gearedUp for GUI
	 * R will do another conversion and will use the double
	 *   4   ,    3    ,  2  , 1/2, 1/3, 1/4		#gearedUp string (GUI)
	 *  -4   ,   -3    , -2  ,   2,   3,   4		#gearedDown
	 *   0.25,    0.333,  0.5,   2,   3,   4		#gearedDown on R (see readFromFile.gearedDown() on util.cs)
	 */
	public string GearedUpDisplay() 
	{
		switch(gearedDown) {
			case -4:
				return "4";
			case -3:
				return "3";
			case -2:
				return "2";
			case 2:
				return "1/2";
			case 3:
				return "1/3";
			case 4:
				return "1/4";
			default:
				return "2";
		}
	}
	public void SetGearedDownFromDisplay(string gearedUpStr) 
	{
		switch(gearedUpStr) {
			case "4":
				gearedDown = -4;
				break;
			case "3":
				gearedDown = -3;
				break;
			case "2":
				gearedDown = -2;
				break;
			case "1/2":
				gearedDown = 2;
				break;
			case "1/3":
				gearedDown = 3;
				break;
			case "1/4":
				gearedDown = 4;
				break;
			default:
				gearedDown = -2;
				break;
		}
	}

}

public class EncoderAnalyzeInstant 
{
	public List<double> displ;
	public List<double> speed;
	public List<double> accel;
	public List<double> force;
	public List<double> power;

	public int graphWidth;
	
	private Rx1y2 usr;
	private Rx1y2 plt;
		
	private double pxPlotArea;
	private double msPlotArea;
	
	//last calculated values on last range of msa and msb
	public double displAverageLast;
	public double displMaxLast;
	public double speedAverageLast;
	public double speedMaxLast;
	public double accelAverageLast;
	public double accelMaxLast;
	public double forceAverageLast;
	public double forceMaxLast;
	public double powerAverageLast;
	public double powerMaxLast;

	public EncoderAnalyzeInstant() {
		displ = new List<double>(); 
		speed = new List<double>(); 
		accel = new List<double>(); 
		force = new List<double>(); 
		power = new List<double>();
		
		graphWidth = 0;
		pxPlotArea = 0;
		msPlotArea = 0;
	}

	//file has a first line with headers
	//2nd.... full data
	public void ReadArrayFile(string filename)
	{
		List<string> lines = Util.ReadFileAsStringList(filename);
		if(lines == null)
			return;
		if(lines.Count <= 1) //return if there's only the header
			return;

		bool headerLine = true;
		foreach(string l in lines) {
				if(headerLine) {
					headerLine = false;
					continue;
				}

			string [] lsplit = l.Split(new char[] {','});
			displ.Add(Convert.ToDouble(Util.ChangeDecimalSeparator(lsplit[1])));
			speed.Add(Convert.ToDouble(Util.ChangeDecimalSeparator(lsplit[2])));
			accel.Add(Convert.ToDouble(Util.ChangeDecimalSeparator(lsplit[3])));
			force.Add(Convert.ToDouble(Util.ChangeDecimalSeparator(lsplit[4])));
			power.Add(Convert.ToDouble(Util.ChangeDecimalSeparator(lsplit[5])));
		}
	}
	
	public void ReadGraphParams(string filename)
	{
		List<string> lines = Util.ReadFileAsStringList(filename);
		if(lines == null)
			return;
		if(lines.Count < 3)
			return;

		graphWidth = Convert.ToInt32(lines[0]);
		usr = new Rx1y2(lines[1]);
		plt = new Rx1y2(lines[2]);

		// calculate the pixels in plot area
		pxPlotArea = graphWidth * (plt.x2 - plt.x1);

		//calculate the ms in plot area
		msPlotArea = usr.x2 - usr.x1;
	}
	
	//gets an instant value
	public double GetParam(string param, int ms) 
	{
		ms --; //converts from starting at 1 (graph) to starting at 0 (data)

		if(ms > displ.Count)
			return -1;

		else {
			if(param == "displ")
				return displ[ms];
			else if(param == "speed")
				return speed[ms];
			else if(param == "accel")
				return accel[ms];
			else if(param == "force")
				return force[ms];
			else if(param == "power")
				return power[ms];
			else
				return -2;
		}
	}
	
	//calculates from a range
	public bool CalculateRangeParams(int msa, int msb)
	{
		msa --; //converts from starting at 1 (graph) to starting at 0 (data)
		msb --; //converts from starting at 1 (graph) to starting at 0 (data)
		
		//if msb < msa invert them
		if(msb < msa) {
			int temp = msa;
			msa = msb;
			msb = temp;
		}

		if(msa > displ.Count || msb > displ.Count)
			return false;

		getAverageAndMax(displ, msa, msb, out displAverageLast, out displMaxLast);
		getAverageAndMax(speed, msa, msb, out speedAverageLast, out speedMaxLast);
		getAverageAndMax(accel, msa, msb, out accelAverageLast, out accelMaxLast);
		getAverageAndMax(force, msa, msb, out forceAverageLast, out forceMaxLast);
		getAverageAndMax(power, msa, msb, out powerAverageLast, out powerMaxLast);
		
		return true;
	}
	private void getAverageAndMax(List<double> dlist, int ini, int end, out double listAVG, out double listMAX) {
		if(ini == end) {
			listAVG = dlist[ini];
			listMAX = dlist[ini];
		}

		double sum = 0;
		double max = - 1000000;
		for(int i = ini; i <= end; i ++) {
			sum += dlist[i];
			if(dlist[i] > max)
				max = dlist[i];
		}

		listAVG = sum / (end - ini + 1); //+1 because count starts at 0
		listMAX = max;
	}


	public int GetVerticalLinePosition(int ms) 
	{
		//this can be called on expose event before calculating needed parameters
		if(graphWidth == 0 || pxPlotArea == 0 || msPlotArea == 0)
			return 0;

		// rule of three
		double px = (ms - usr.x1) * pxPlotArea / msPlotArea;

		// fix margin
		px = px + plt.x1 * graphWidth;

		return Convert.ToInt32(px);
	}
	
	public void ExportToCSV(int msa, int msb, string selectedFileName, string sepString) 
	{
		//if msb < msa invert them
		if(msb < msa) {
			int temp = msa;
			msa = msb;
			msb = temp;
		}

		//this overwrites if needed
		TextWriter writer = File.CreateText(selectedFileName);

		string sep = " ";
		if (sepString == "COMMA")
			sep = ";";
		else
			sep = ",";

		string header = 
			"" + sep +
			Catalog.GetString("Time") + sep + 
			Catalog.GetString("Displacement") + sep +
			Catalog.GetString("Speed") + sep +
			Catalog.GetString("Acceleration") + sep +
			Catalog.GetString("Force") + sep +
			Catalog.GetString("Power");
			
		//write header
		writer.WriteLine(header);

		//write statistics
		writer.WriteLine(
				Catalog.GetString("Difference") + sep +
				(msb-msa).ToString() + sep +
				Util.DoubleToCSV( (GetParam("displ",msb) - GetParam("displ",msa)), sepString ) + sep +
				Util.DoubleToCSV( (GetParam("speed",msb) - GetParam("speed",msa)), sepString ) + sep +
				Util.DoubleToCSV( (GetParam("accel",msb) - GetParam("accel",msa)), sepString ) + sep +
				Util.DoubleToCSV( (GetParam("force",msb) - GetParam("force",msa)), sepString ) + sep +
				Util.DoubleToCSV( (GetParam("power",msb) - GetParam("power",msa)), sepString ) );
		
		//done here because GetParam does the same again, and if we put it in the top of this method, it will be done two times
		msa --; //converts from starting at 1 (graph) to starting at 0 (data)
		msb --; //converts from starting at 1 (graph) to starting at 0 (data)
		
		writer.WriteLine(
				Catalog.GetString("Average") + sep +
				"" + sep +
				Util.DoubleToCSV(displAverageLast, sepString) + sep +
				Util.DoubleToCSV(speedAverageLast, sepString) + sep +
				Util.DoubleToCSV(accelAverageLast, sepString) + sep +
				Util.DoubleToCSV(forceAverageLast, sepString) + sep +
				Util.DoubleToCSV(powerAverageLast, sepString) );
		
		writer.WriteLine(
				Catalog.GetString("Maximum") + sep +
				"" + sep +
				Util.DoubleToCSV(displMaxLast, sepString) + sep +
				Util.DoubleToCSV(speedMaxLast, sepString) + sep +
				Util.DoubleToCSV(accelMaxLast, sepString) + sep +
				Util.DoubleToCSV(forceMaxLast, sepString) + sep +
				Util.DoubleToCSV(powerMaxLast, sepString) );

		//blank line
		writer.WriteLine();

		//write header
		writer.WriteLine(header);

		//write data
		for(int i = msa; i <= msb; i ++)
			writer.WriteLine(
					"" + sep +
					(i+1).ToString() + sep +
					Util.DoubleToCSV(displ[i], sepString) + sep +
					Util.DoubleToCSV(speed[i], sepString) + sep +
					Util.DoubleToCSV(accel[i], sepString) + sep +
					Util.DoubleToCSV(force[i], sepString) + sep +
					Util.DoubleToCSV(power[i], sepString) );

		writer.Flush();
		writer.Close();
		((IDisposable)writer).Dispose();
	}

	public void PrintDebug() {
		LogB.Information("Printing speed");
		foreach(double s in speed)
			LogB.Debug(s.ToString());
	}
}

//for objects coming from R that have "x1 x2 y1 y2" like usr or par
public class Rx1y2 
{
	public double x1;
	public double x2;
	public double y1;
	public double y2;

	public Rx1y2 (string s) {
		string [] sFull = s.Split(new char[] {' '});
		x1 = Convert.ToDouble(Util.ChangeDecimalSeparator(sFull[0]));
		x2 = Convert.ToDouble(Util.ChangeDecimalSeparator(sFull[1]));
		y1 = Convert.ToDouble(Util.ChangeDecimalSeparator(sFull[2]));
		y2 = Convert.ToDouble(Util.ChangeDecimalSeparator(sFull[3]));
	}
}
