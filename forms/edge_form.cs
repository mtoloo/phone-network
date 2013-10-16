using System;
using System.Windows.Forms;
using Model;

namespace Simban
{
	public class EdgeForm: Form
	{
		Edge fedge;

		TextBox capacityText;
		TextBox distanceText;

		public Edge edge {
			get {return this.fedge;}
			set {
				this.fedge = value;
				this.Text = "Edge";
				this.capacityText.Text = edge.capacity.ToString();
				this.distanceText.Text = edge.distance.ToString();
			}
		}

		public EdgeForm (Edge edge)
		{
			this.Width = 250;
			this.Height = 180;
			this.SetAutoSizeMode(System.Windows.Forms.AutoSizeMode.GrowAndShrink);

			Label capacityLabel = new Label();
			capacityLabel.Text = "Capacity:";
			capacityLabel.Left = 10;
			capacityLabel.Top = 10;
			this.capacityText = new TextBox();
			this.capacityText.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
			this.capacityText.Top = 10;
			this.capacityText.Left = 65;

			Label distanceLabel = new Label();
			distanceLabel.Text = "Distance:";
			distanceLabel.Left = 10;
			distanceLabel.Top = 35;
			this.distanceText = new TextBox();
			this.distanceText.Top = 35;
			this.distanceText.Left = 65;
			this.distanceText.Anchor = AnchorStyles.Left | AnchorStyles.Top;

			this.edge = edge;

			Button okButton = new Button();
			okButton.Text = "OK";
			okButton.Click += OkBtnOnClick;
			okButton.Top = 100;
			okButton.Left= 10;
			okButton.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
			Button cancelButton = new Button();
			cancelButton.Text = "Cancel";
			cancelButton.Click += CancelBtnOnClick;
			cancelButton.Top = 100;
			cancelButton.Left= 100;
			okButton.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
			Controls.Add(capacityText);
			Controls.Add(capacityLabel);
			Controls.Add(distanceText);
			Controls.Add(distanceLabel);
			Controls.Add(okButton);
			Controls.Add(cancelButton);
		}

		void OkBtnOnClick (object sender, EventArgs e)
		{
			this.fedge.capacity = int.Parse(this.capacityText.Text);
			this.fedge.distance = double.Parse(this.distanceText.Text);
			this.DialogResult = System.Windows.Forms.DialogResult.OK;
		}
		void CancelBtnOnClick (object sender, EventArgs e)
		{
			this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		}

	}
}

