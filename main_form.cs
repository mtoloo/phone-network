using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using PhoneNetwork;
using System.Collections.Generic;
using Model;

public class MainForm : Form
{
	private Timer initializeTmr;
	private Database database;
	private StatusBar status;
	private Map map;
	private Nodes nodes;
	private Edges edges;

	public MainForm ()
	{
		CreateView ();
	}

	static public void Main ()
	{
		Application.Run (new MainForm ());
	}

	void CreateView ()
	{
		Text = "Phone-Network";
		Size = new Size (700, 500);
		this.StartPosition = FormStartPosition.CenterScreen;

		this.initializeTmr = new Timer ();
		this.initializeTmr.Interval = 100;
		this.initializeTmr.Tick += new EventHandler (onInitialize);
		this.initializeTmr.Enabled = true;

		ToolBar mainToolBar = new ToolBar ();
//		mainToolBar.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
		mainToolBar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		mainToolBar.Divider = true;
		mainToolBar.DropDownArrows = true;
		mainToolBar.ShowToolTips = true;
		mainToolBar.Size = new System.Drawing.Size (400, 25);
		mainToolBar.TabIndex = 0;
		mainToolBar.Wrappable = false;

		for (int i = 1 ; i < Map.MapModeTitles.Length; i++) {
			ToolBarButton button = new ToolBarButton();
			button.Tag = (MapMode) i;
			button.Text = Map.MapModeTitles[i];
			mainToolBar.Buttons.Add(button);
		}
		mainToolBar.ButtonClick += new ToolBarButtonClickEventHandler(this.toolbarButtonClick);

		this.status = new StatusBar();
		status.Text = "Working...";

		Controls.Add(status);
		Controls.Add(mainToolBar);
	}

	void onInitialize (object sender, EventArgs e)
	{
		this.initializeTmr.Stop ();
		status.Text = "Database...";
		this.database = new Database ("phone-network.sqlite");
		this.database.CreateOrOpen ();
		this.nodes = new Nodes (this.database, true);
		this.edges = new Edges (this.database, true, this.nodes);
		this.map = new Map (this.database, this.nodes, this.edges);
		if (this.map.mapModel.Count () == 0) {
			MapData mapData = new MapData(0, "Main", "map.jpg", 0, 0, 100, 100);
			this.map.mapModel.InsertAndGetId (mapData);
		}
		this.map.SetBounds (10, 50, 600, 400);
		this.LoadMap();
		Controls.Add (map);

		status.Text = "Data...";
		this.nodes.LoadAll();
		this.edges.LoadAll(this.nodes);
		status.Text = "Ready";
	}


//	void databaseCreateVersion1 ()
//	{
//		string sql = @"CREATE TABLE nodes(id INTEGER PRIMARY KEY AUTOINCREMENT,
//name VARCHAR, left INTEGER, top INTEGER);
//CREATE TABLE edges(id INTEGER PRIMARY KEY AUTOINCREMENT, node1_id INTEGER,
//node2_id INTEGER, distance int, 
// FOREIGN KEY(node1_id) REFERENCES nodes(id) ON DELETE CASCADE,
// FOREIGN KEY(node2_id) REFERENCES nodes(id) ON DELETE CASCADE);
//CREATE TABLE maps(id INTEGER PRIMARY KEY AUTOINCREMENT,
//name VARCHAR, file VARCHAR, left INTEGER, top INTEGER, width INTEGER, height INTEGER);
//";
//		this.database.executeScriptContent(sql);
//		this.database.versionAdd("1.0");
//	}
//

	void LoadMap ()
	{
		this.map.Load ("map.jpg");
	}

	void toolbarButtonClick (object sender, ToolBarButtonClickEventArgs e)
	{
		this.map.mapMode = (MapMode) e.Button.Tag;
	}
}