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
 *  Copyright (C) 2016-2017   Xavier de Blas <xaviblas@gmail.com> 
 */

using System;
using System.Collections.Generic; //List<T>
using Gdk;
using Gtk;
using Mono.Unix;

public class ChronopicRegisterWindowTypes
{
	public string SerialNumber;
	public string Port;
	public bool Unknown;
	public bool Contacts;
	public bool Encoder;

	public ChronopicRegisterWindowTypes (string serialNumber, string port, bool unknown, bool contacts, bool encoder)
	{
		this.SerialNumber = serialNumber;
		this.Port = port;
		this.Unknown = unknown;
		this.Contacts = contacts;
		this.Encoder = encoder;
	}

	public ChronopicRegisterWindowTypes (ChronopicRegisterPort crp)
	{
		this.SerialNumber = crp.SerialNumber;
		this.Port = crp.Port;

		Unknown = false;
		Contacts = false;
		Encoder = false;

		if(crp.Type == ChronopicRegisterPort.Types.UNKNOWN)
			Unknown = true;
		else if(crp.Type == ChronopicRegisterPort.Types.CONTACTS)
			Contacts = true;
		else
			Encoder = true;
	}
}


public class ChronopicRegisterWindow
{
	Gtk.Window chronopic_register_win;
	Gtk.VBox vbox_main;
	public Gtk.Button FakeButtonCloseSerialPort;

	public ChronopicRegisterWindow(Gtk.Window app1, List<ChronopicRegisterPort> list)
	{
		createWindow(app1);
		//put an icon to window
		UtilGtk.IconWindow(chronopic_register_win);

		createVBoxMain();
		createContent(list);
		createButtons();

		chronopic_register_win.ShowAll();
	}


	private void createWindow(Gtk.Window app1)
	{
		chronopic_register_win = new Gtk.Window (Catalog.GetString("Chronopic window"));
		chronopic_register_win.AllowGrow = false;
		chronopic_register_win.Modal = true;
		chronopic_register_win.TransientFor = app1;

		chronopic_register_win.DeleteEvent += on_delete_event;

		/* Sets the border width of the window. */
		chronopic_register_win.BorderWidth= 20;
	}

	private void createVBoxMain()
	{
		vbox_main = new Gtk.VBox(false, 20);
		chronopic_register_win.Add(vbox_main);
	}


	Gtk.TreeView treeview;
	Gtk.ListStore listStoreAll; //stores the chronopics that have assigned a serial port: They are plugged in.

	//based on: ~/informatica/progs_meus/mono/treemodel.cs
	private void createContent(List<ChronopicRegisterPort> list)
	{
		treeview = new Gtk.TreeView();

		// Create column , cell renderer and add the cell to the serialN column
		Gtk.TreeViewColumn serialNCol = new Gtk.TreeViewColumn ();
		serialNCol.Title = " " + Catalog.GetString("Serial Number") + " ";
		Gtk.CellRendererText serialNCell = new Gtk.CellRendererText ();
		serialNCol.PackStart (serialNCell, true);

		// Create column , cell renderer and add the cell to the port column
		Gtk.TreeViewColumn portCol = new Gtk.TreeViewColumn ();
		portCol.Title = " Port ";
		Gtk.CellRendererText portCell = new Gtk.CellRendererText ();
		portCol.PackStart (portCell, true);


		//-- cell renderer toggles

		Gtk.TreeViewColumn unknownCol = new Gtk.TreeViewColumn ();
		unknownCol.Title = " " + Catalog.GetString("Not configured") + " ";
		Gtk.CellRendererToggle unknownCell = new Gtk.CellRendererToggle ();
		unknownCell.Activatable = true;
		unknownCell.Radio = true; 	//draw as radiobutton
		unknownCell.Toggled += new Gtk.ToggledHandler (unknownToggled);
		unknownCol.PackStart (unknownCell, true);

		Gtk.TreeViewColumn contactsCol = new Gtk.TreeViewColumn ();
		contactsCol.Title = " " + Catalog.GetString("Jumps/Races") + " ";
		Gtk.CellRendererToggle contactsCell = new Gtk.CellRendererToggle ();
		contactsCell.Activatable = true;
		contactsCell.Radio = true; 	//draw as radiobutton
		contactsCell.Toggled += new Gtk.ToggledHandler (contactsToggled);
		contactsCol.PackStart (contactsCell, true);

		Gtk.TreeViewColumn encoderCol = new Gtk.TreeViewColumn ();
		encoderCol.Title = " " + Catalog.GetString("Encoder") + " ";
		Gtk.CellRendererToggle encoderCell = new Gtk.CellRendererToggle ();
		encoderCell.Activatable = true;
		encoderCell.Radio = true; 	//draw as radiobutton
		encoderCell.Toggled += new Gtk.ToggledHandler (encoderToggled);
		encoderCol.PackStart (encoderCell, true);

		//-- end of cell renderer toggles


		listStoreAll = new Gtk.ListStore (typeof (ChronopicRegisterWindowTypes));

		int connectedCount = 0;
		int unknownCount = 0;
		foreach(ChronopicRegisterPort crp in list)
		{
			if(crp.Port != "")
			{
				listStoreAll.AppendValues(new ChronopicRegisterWindowTypes(crp));
				connectedCount ++;

				if(crp.Type == ChronopicRegisterPort.Types.UNKNOWN)
					unknownCount ++;
			}
		}

		serialNCol.SetCellDataFunc (serialNCell, new Gtk.TreeCellDataFunc (RenderSerialN));
		portCol.SetCellDataFunc (portCell, new Gtk.TreeCellDataFunc (RenderPort));
		unknownCol.SetCellDataFunc (unknownCell, new Gtk.TreeCellDataFunc (RenderUnknown));
		contactsCol.SetCellDataFunc (contactsCell, new Gtk.TreeCellDataFunc (RenderContacts));
		encoderCol.SetCellDataFunc (encoderCell, new Gtk.TreeCellDataFunc (RenderEncoder));

		treeview.Model = listStoreAll;

		// Add the columns to the TreeView
		treeview.AppendColumn (serialNCol);
		treeview.AppendColumn (portCol);
		treeview.AppendColumn (unknownCol);
		treeview.AppendColumn (contactsCol);
		treeview.AppendColumn (encoderCol);

		Gtk.HBox hbox = new Gtk.HBox(false, 12);
	
		//create image
		Pixbuf pixbuf = new Pixbuf (null, Util.GetImagePath(false) + Constants.FileNameChronopic);
		Gtk.Image image = new Gtk.Image();
		image.Pixbuf = pixbuf;
		hbox.Add(image);

		//create label
		Gtk.Label label = new Gtk.Label();
		label.Text = writeLabel(connectedCount, unknownCount);
		hbox.Add(label);

		Gtk.VBox vboxTV = new Gtk.VBox(false, 12);
		vboxTV.Add(hbox);

		if(connectedCount > 0)
			vboxTV.Add(treeview);

		vbox_main.Add(vboxTV);
	}

	private string writeLabel(int chronopicsCount, int unknownCount)
	{
		if(chronopicsCount > 0)
		{
			string str = string.Format(Catalog.GetPluralString(
						"Found 1 Chronopic.",
						"Found {0} Chronopics.",
						chronopicsCount),
					chronopicsCount);

			if(unknownCount > 0)
			{
				str += "\n\n";
				str += Catalog.GetString("Please, mark Chronopic/s as") +
					"\n- " +
					Catalog.GetString("Jumps") + "/" + Catalog.GetString("Races") + " " + Catalog.GetString("or") +
					"\n- " + Catalog.GetString("Encoder");
			}
			return str;
		}

		return Catalog.GetString("Chronopic/s not found") + "\n\n" + Catalog.GetString("Connect and reopen this window.");
	}

	private void RenderSerialN (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
	{
		ChronopicRegisterWindowTypes crwt = (ChronopicRegisterWindowTypes) model.GetValue (iter, 0);
		(cell as Gtk.CellRendererText).Text = crwt.SerialNumber;
	}

	private void RenderPort (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
	{
		ChronopicRegisterWindowTypes crwt = (ChronopicRegisterWindowTypes) model.GetValue (iter, 0);
		(cell as Gtk.CellRendererText).Text = crwt.Port;
	}

	private void RenderUnknown (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
	{
		ChronopicRegisterWindowTypes crwt = (ChronopicRegisterWindowTypes) model.GetValue (iter, 0);
		(cell as Gtk.CellRendererToggle).Active = crwt.Unknown;
	}

	private void RenderContacts (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
	{
		ChronopicRegisterWindowTypes crwt = (ChronopicRegisterWindowTypes) model.GetValue (iter, 0);
		(cell as Gtk.CellRendererToggle).Active = crwt.Contacts;
	}

	private void RenderEncoder (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
	{
		ChronopicRegisterWindowTypes crwt = (ChronopicRegisterWindowTypes) model.GetValue (iter, 0);
		(cell as Gtk.CellRendererToggle).Active = crwt.Encoder;
	}



	private void unknownToggled (object sender, Gtk.ToggledArgs args)
	{
		Gtk.TreeIter iter;
		listStoreAll.GetIter (out iter, new Gtk.TreePath (args.Path));

		ChronopicRegisterWindowTypes crwt = (ChronopicRegisterWindowTypes) listStoreAll.GetValue (iter, 0);

		if(! crwt.Unknown) {
			crwt.Unknown = true;
			crwt.Contacts = false;
			crwt.Encoder = false;
		}

		//store on SQL
		SqliteChronopicRegister.Update(false,
				new ChronopicRegisterPort(crwt.SerialNumber, ChronopicRegisterPort.Types.UNKNOWN),
				ChronopicRegisterPort.Types.UNKNOWN);
	}

	private void contactsToggled (object sender, Gtk.ToggledArgs args)
	{
		Gtk.TreeIter iter;
		listStoreAll.GetIter (out iter, new Gtk.TreePath (args.Path));

		ChronopicRegisterWindowTypes crwt = (ChronopicRegisterWindowTypes) listStoreAll.GetValue (iter, 0);

		if(! crwt.Contacts) {
			crwt.Unknown = false;
			crwt.Contacts = true;
			crwt.Encoder = false;
		}

		//store on SQL
		SqliteChronopicRegister.Update(false,
				new ChronopicRegisterPort(crwt.SerialNumber, ChronopicRegisterPort.Types.CONTACTS),
				ChronopicRegisterPort.Types.CONTACTS);
	}

	private void encoderToggled (object sender, Gtk.ToggledArgs args)
	{
		Gtk.TreeIter iter;
		listStoreAll.GetIter (out iter, new Gtk.TreePath (args.Path));

		ChronopicRegisterWindowTypes crwt = (ChronopicRegisterWindowTypes) listStoreAll.GetValue (iter, 0);

		if(! crwt.Encoder) {
			crwt.Unknown = false;
			crwt.Contacts = false;
			crwt.Encoder = true;
		}

		//store on SQL
		SqliteChronopicRegister.Update(false,
				new ChronopicRegisterPort(crwt.SerialNumber, ChronopicRegisterPort.Types.ENCODER),
				ChronopicRegisterPort.Types.ENCODER);
	}

	private void createButtons()
	{
		FakeButtonCloseSerialPort = new Gtk.Button();
		Gtk.Button button_close_serial_port = new Gtk.Button("Close serial port (debug)");
		button_close_serial_port.Clicked += new EventHandler(on_button_close_serial_port_clicked);

		Gtk.Button button_OSX_readme = new Gtk.Button("MacOSX Readme");
		button_OSX_readme.Clicked += new EventHandler(on_button_OSX_readme_clicked);

		//---- button close start --->
		Gtk.Button button_close = new Gtk.Button("Close Window");
		button_close.Clicked += new EventHandler(on_button_close_clicked);

		Gtk.AccelGroup ag = new Gtk.AccelGroup (); //button can be called clicking Escape key
		chronopic_register_win.AddAccelGroup (ag);

		button_close.AddAccelerator
			("activate", ag, new Gtk.AccelKey
			 (Gdk.Key.Escape, Gdk.ModifierType.None,
			  Gtk.AccelFlags.Visible));
		//<---- button close end

		//add buttons to containers
		Gtk.HButtonBox hbox = new Gtk.HButtonBox ();
		//hbox.Add(button_close_serial_port);

		if( UtilAll.GetOSEnum() == UtilAll.OperatingSystems.MACOSX)
			hbox.Add(button_OSX_readme);

		hbox.Add(button_close);

		vbox_main.Add(hbox);
	}
	
	private void on_button_close_serial_port_clicked(object o, EventArgs args)
	{
		//try first to see if a sp is opened on a cp but that /ttyusbserial does not exists
		FakeButtonCloseSerialPort.Click();
	}

	private void on_button_OSX_readme_clicked(object o, EventArgs args)
	{
		new DialogMessage(Constants.MessageTypes.INFO,
				Catalog.GetString("There is a known problem with MacOSX:") + "\n" +
				Catalog.GetString("If Chronopic is disconnected after jumps or runs execution, that port will be blocked until restart of machine") + "\n\n" +
				Catalog.GetString("We are working on a solution.")
				);
	}

	private void on_button_close_clicked(object o, EventArgs args)
	{
		chronopic_register_win.Hide();
		chronopic_register_win = null;
	}

	private void on_delete_event (object o, DeleteEventArgs args)
	{
		LogB.Information("calling on_delete_event");

		args.RetVal = true;

		on_button_close_clicked(new object(), new EventArgs());
	}
}

