using System;
using System.Windows.Forms;
using Model;

namespace Simban
{
	public class NodeForm: Form
	{
		Node fnode;

		TextBox nameText {
			get;
			set;
		}

		TextBox inputCapacityText {
			get;
			set;
		}

		TextBox outputCapacityText {
			get;
			set;
		}
		public Node node {
			get {return this.fnode;}
			set {
				this.fnode = value;
				this.Text = fnode.name;
				this.nameText.Text = fnode.name;
				this.inputCapacityText.Text = fnode.inputCapacity.ToString();
				this.outputCapacityText.Text = fnode.outputCapacity.ToString();
			}
		}

		public NodeForm (Node node)
		{
			this.Width = 250;
			this.Height = 180;
			this.SetAutoSizeMode(System.Windows.Forms.AutoSizeMode.GrowAndShrink);

			Label nameLabel = new Label();
			nameLabel.Text = "Name:";
			nameLabel.Left = 10;
			nameLabel.Top = 10;
			this.nameText = new TextBox();
			this.nameText.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
			this.nameText.Top = 10;
			this.nameText.Left = 50;

			Label inputCapacityLabel = new Label();
			inputCapacityLabel.Text = "Input capacity:";
			inputCapacityLabel.Left = 10;
			inputCapacityLabel.Top = 35;
			this.inputCapacityText = new TextBox();
			this.inputCapacityText.Top = 35;
			this.inputCapacityText.Left = 100;
			this.inputCapacityText.Anchor = AnchorStyles.Left | AnchorStyles.Top;

			Label outputCapacityLabel = new Label();
			outputCapacityLabel.Text = "Output capacity:";
			outputCapacityLabel.Left = 10;
			outputCapacityLabel.Top = 60;
			this.outputCapacityText = new TextBox();
			this.outputCapacityText.Top = 60;
			this.outputCapacityText.Left = 100;
			this.outputCapacityText.Anchor = AnchorStyles.Left | AnchorStyles.Top;

			this.node = node;

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
			Controls.Add(nameText);
			Controls.Add(nameLabel);
			Controls.Add(inputCapacityText);
			Controls.Add(inputCapacityLabel);
			Controls.Add(outputCapacityText);
			Controls.Add(outputCapacityLabel);
			Controls.Add(okButton);
			Controls.Add(cancelButton);
		}

		void OkBtnOnClick (object sender, EventArgs e)
		{
			this.fnode.name = this.nameText.Text;
			this.fnode.inputCapacity = int.Parse(this.inputCapacityText.Text);
			this.fnode.outputCapacity = int.Parse(this.outputCapacityText.Text);
			this.DialogResult = System.Windows.Forms.DialogResult.OK;
		}
		void CancelBtnOnClick (object sender, EventArgs e)
		{
			this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		}

	}
}

