#define TRACE
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Tools.DocumentReflector.Properties;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector.View
{
	internal class ReflectorControl : UserControl
	{
		private TextBoxView _xmlView;

		private TextBoxView _codeView;

		private AsyncOperation _asyncOp;

		private Thread _reflectThread;

		private object _lockObj = new object();

		private object _reflectingObject;

		private IContainer components;

		private SplitContainer splitContainer1;

		public override Font Font
		{
			get
			{
				return base.Font;
			}
			set
			{
				_xmlView.Font = value;
				_codeView.Font = value;
				base.Font = value;
			}
		}

		public event AsyncCompletedEventHandler ReflectCompleted;

		public ReflectorControl()
		{
			InitializeComponent();
			InitializeViews();
			Dock = DockStyle.Fill;
			_asyncOp = AsyncOperationManager.CreateOperation(null);
		}

		private void InitializeViews()
		{
			RichTextBox tbXml = new RichTextBox();
			tbXml.Dock = DockStyle.Fill;
			tbXml.ReadOnly = true;
			tbXml.WordWrap = false;
			tbXml.DetectUrls = false;
			tbXml.HideSelection = false;
			ContextMenuStrip cmXml = new ContextMenuStrip();
			cmXml.Opening += delegate(object sender, CancelEventArgs e)
			{
				AddXmlContextMenuItems(cmXml, tbXml, e);
			};
			tbXml.ContextMenuStrip = cmXml;
			splitContainer1.Panel1.Controls.Add(tbXml);
			RichTextBox tbCode = new RichTextBox();
			tbCode.Dock = DockStyle.Fill;
			tbCode.ReadOnly = true;
			tbCode.WordWrap = false;
			tbCode.BackColor = Color.LightYellow;
			tbCode.DetectUrls = false;
			tbCode.HideSelection = false;
			ContextMenuStrip cmCode = new ContextMenuStrip();
			cmCode.Opening += delegate(object sender, CancelEventArgs e)
			{
				AddCodeContextMenuItems(cmCode, tbCode, e);
			};
			tbCode.ContextMenuStrip = cmCode;
			splitContainer1.Panel2.Controls.Add(tbCode);
			splitContainer1.Panel1MinSize = 64;
			splitContainer1.Panel2MinSize = 64;
			splitContainer1.SplitterMoved += delegate
			{
				tbXml.Refresh();
			};
			_xmlView = new TextBoxView(tbXml);
			_codeView = new TextBoxView(tbCode);
		}

		public void ReflectAsync(object openXmlObject)
		{
			if (openXmlObject != _reflectingObject)
			{
				lock (_lockObj)
				{
					_reflectingObject = openXmlObject;
				}
				CancelReflectThread();
				_xmlView.Text = string.Empty;
				_codeView.Text = string.Empty;
				_reflectThread = new Thread(ReflectOnThread);
				_reflectThread.Start(openXmlObject);
				ShowXml(openXmlObject);
			}
		}

		public void CancelReflecting()
		{
			if (CancelReflectThread())
			{
				OnReflectCompleted(cancel: true, null, null);
			}
		}

		private bool CancelReflectThread()
		{
			if (_reflectThread != null && _reflectThread.IsAlive)
			{
				lock (_lockObj)
				{
					_reflectThread.Abort();
				}
				return true;
			}
			return false;
		}

		private void ShowXml(object openXmlObject)
		{
			OpenXmlElement openXmlElement = openXmlObject as OpenXmlElement;
			if (openXmlElement != null)
			{
				ElementXmlDocument model = new ElementXmlDocument(openXmlElement);
				ShowDocument(_xmlView, model);
				splitContainer1.Panel1Collapsed = false;
			}
			else
			{
				splitContainer1.Panel1Collapsed = true;
			}
		}

		private void ReflectOnThread(object openXmlObject)
		{
			IDocument code = null;
			try
			{
				if (openXmlObject is OpenXmlPackage)
				{
					code = Reflect(openXmlObject as OpenXmlPackage);
				}
				else if (openXmlObject is OpenXmlPart)
				{
					code = Reflect(openXmlObject as OpenXmlPart);
				}
				else if (openXmlObject is OpenXmlElement)
				{
					code = Reflect(openXmlObject as OpenXmlElement);
				}
				else if (openXmlObject is DataPart)
				{
					code = Reflect(openXmlObject as DataPart);
				}
				_asyncOp.Post(delegate(object state)
				{
					OnReflectCompleted(cancel: false, code, state);
				}, null);
			}
			catch (ThreadAbortException)
			{
				Trace.WriteLine("Reflection thread is aborted.");
			}
			catch (Exception ex3)
			{
				Exception ex = ex3;
				_asyncOp.Post(delegate(object state)
				{
					OnReflectFailed(ex, state);
				}, null);
			}
		}

		private void OnReflectCompleted(bool cancel, IDocument code, object state)
		{
			lock (_lockObj)
			{
				_reflectingObject = null;
			}
			if (!cancel)
			{
				ShowDocument(_codeView, code);
			}
			if (this.ReflectCompleted != null)
			{
				this.ReflectCompleted(this, new AsyncCompletedEventArgs(null, cancel, state));
			}
		}

		private void OnReflectFailed(Exception error, object state)
		{
			if (this.ReflectCompleted != null)
			{
				this.ReflectCompleted(this, new AsyncCompletedEventArgs(error, cancelled: false, state));
			}
		}

		private IDocument Reflect(OpenXmlPackage package)
		{
			FullCodeReflector fullCodeReflector = new FullCodeReflector();
			ICodeModel codeDom = fullCodeReflector.Reflect(package);
			return new CodeDocument(codeDom);
		}

		private IDocument Reflect(OpenXmlPart part)
		{
			FullCodeReflector fullCodeReflector = new FullCodeReflector();
			ICodeModel codeDom = fullCodeReflector.Reflect(part);
			return new CodeDocument(codeDom);
		}

		private IDocument Reflect(OpenXmlElement element)
		{
			FullCodeReflector fullCodeReflector = new FullCodeReflector();
			ICodeModel codeDom = fullCodeReflector.Reflect(element);
			return new CodeDocument(codeDom);
		}

		private IDocument Reflect(DataPart part)
		{
			FullCodeReflector fullCodeReflector = new FullCodeReflector();
			ICodeModel codeDom = fullCodeReflector.Reflect(part);
			return new CodeDocument(codeDom);
		}

		private void ShowDocument(TextBoxView view, IDocument model)
		{
			lock (_lockObj)
			{
				view.Document = model;
				view.TextBox.SuspendLayout();
				view.Font = null;
				view.Font = Font;
				view.TextBox.ResumeLayout();
				model?.Listeners.Add(view);
			}
		}

		private void AddXmlContextMenuItems(ContextMenuStrip cms, RichTextBox tbXml, CancelEventArgs e)
		{
			cms.Items.Clear();
			if (string.IsNullOrEmpty(tbXml.Text))
			{
				e.Cancel = true;
				return;
			}
			ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(DocumentFormat.OpenXml.Tools.DocumentReflector.Properties.Resources.CopySelectionMenuItemHeader, null, OnCopySelection);
			if (tbXml.SelectionLength == 0)
			{
				toolStripMenuItem.Enabled = false;
			}
			cms.Items.Add(toolStripMenuItem);
			cms.Items.Add(new ToolStripMenuItem(DocumentFormat.OpenXml.Tools.DocumentReflector.Properties.Resources.FindMenuItemHeader, null, OnFind));
			e.Cancel = false;
		}

		private void AddCodeContextMenuItems(ContextMenuStrip cms, RichTextBox tbCode, CancelEventArgs e)
		{
			cms.Items.Clear();
			if (string.IsNullOrEmpty(tbCode.Text))
			{
				e.Cancel = true;
				return;
			}
			ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(DocumentFormat.OpenXml.Tools.DocumentReflector.Properties.Resources.CopyAllCodeMenuItemHeader, null, OnCopyAllCode);
			if (string.IsNullOrEmpty(tbCode.Text))
			{
				toolStripMenuItem.Enabled = false;
			}
			ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem(DocumentFormat.OpenXml.Tools.DocumentReflector.Properties.Resources.CopySelectionMenuItemHeader, null, OnCopySelection);
			if (tbCode.SelectionLength == 0)
			{
				toolStripMenuItem2.Enabled = false;
			}
			cms.Items.Add(toolStripMenuItem);
			cms.Items.Add(toolStripMenuItem2);
			cms.Items.Add(new ToolStripMenuItem(DocumentFormat.OpenXml.Tools.DocumentReflector.Properties.Resources.FindMenuItemHeader, null, OnFind));
			e.Cancel = false;
		}

		private void OnCopyAllCode(object sender, EventArgs e)
		{
			ToolStripItem toolStripItem = sender as ToolStripItem;
			ContextMenuStrip contextMenuStrip = toolStripItem.Owner as ContextMenuStrip;
			RichTextBox richTextBox = contextMenuStrip.SourceControl as RichTextBox;
			richTextBox.SelectAll();
			richTextBox.Copy();
			richTextBox.Focus();
		}

		private void OnCopySelection(object sender, EventArgs e)
		{
			ToolStripItem toolStripItem = sender as ToolStripItem;
			ContextMenuStrip contextMenuStrip = toolStripItem.Owner as ContextMenuStrip;
			RichTextBox richTextBox = contextMenuStrip.SourceControl as RichTextBox;
			richTextBox.Copy();
		}

		private void OnFind(object sender, EventArgs e)
		{
			ToolStripItem toolStripItem = sender as ToolStripItem;
			ContextMenuStrip contextMenuStrip = toolStripItem.Owner as ContextMenuStrip;
			RichTextBox richTextBox = contextMenuStrip.SourceControl as RichTextBox;
			string title = ((richTextBox == _xmlView.TextBox) ? DocumentFormat.OpenXml.Tools.DocumentReflector.Properties.Resources.FindInXmlTitle : DocumentFormat.OpenXml.Tools.DocumentReflector.Properties.Resources.FindInCodeTitle);
			FindInRichTextBox(title, richTextBox);
		}

		private static void FindInRichTextBox(string title, RichTextBox rtb)
		{
			if (rtb.Text.Length == 0)
			{
				return;
			}
			IFindService service = ToolSingleton.Instance.Services.GetService<IFindService>();
			FindMethod findMethod = delegate(FindOption option)
			{
				RichTextBoxFinds richTextBoxFinds = RichTextBoxFinds.None;
				if (option.MatchCase)
				{
					richTextBoxFinds |= RichTextBoxFinds.MatchCase;
				}
				if (option.IsUp)
				{
					richTextBoxFinds |= RichTextBoxFinds.Reverse;
					int selectionStart = rtb.SelectionStart;
					int selectionLength = rtb.SelectionLength;
					int num = rtb.Find(option.Keyword, 0, rtb.SelectionStart, richTextBoxFinds);
					if (num > selectionStart)
					{
						rtb.SelectionStart = selectionStart;
						rtb.SelectionLength = selectionLength;
						return false;
					}
					return num >= 0;
				}
				int num2 = rtb.SelectionStart + rtb.SelectionLength;
				int num3 = rtb.Text.Length - 1;
				int selectionStart2 = rtb.SelectionStart;
				int selectionLength2 = rtb.SelectionLength;
				if (num2 > num3)
				{
					return false;
				}
				int num4 = rtb.Find(option.Keyword, num2, num3, richTextBoxFinds);
				if (num4 < num2)
				{
					rtb.SelectionStart = selectionStart2;
					rtb.SelectionLength = selectionLength2;
					return false;
				}
				return true;
			};
			service.InvokeFind("DocumentReflectorWindow", findMethod, title);
		}

		public void InvokeFind()
		{
			RichTextBox textBox = _codeView.TextBox;
			if (_xmlView.TextBox.Focused)
			{
				textBox = _xmlView.TextBox;
			}
			string title = ((textBox == _xmlView.TextBox) ? DocumentFormat.OpenXml.Tools.DocumentReflector.Properties.Resources.FindInXmlTitle : DocumentFormat.OpenXml.Tools.DocumentReflector.Properties.Resources.FindInCodeTitle);
			FindInRichTextBox(title, textBox);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				CancelReflecting();
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			splitContainer1 = new System.Windows.Forms.SplitContainer();
			splitContainer1.SuspendLayout();
			SuspendLayout();
			splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			splitContainer1.Location = new System.Drawing.Point(0, 0);
			splitContainer1.Name = "splitContainer1";
			splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			splitContainer1.Size = new System.Drawing.Size(150, 150);
			splitContainer1.TabIndex = 0;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(splitContainer1);
			base.Name = "ReflectorControl";
			splitContainer1.ResumeLayout(false);
			ResumeLayout(false);
		}
	}
}
