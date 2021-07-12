using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector.View
{
	public class TextBoxView : IView, IDocumentListener
	{
		private IDocument _model;

		private RichTextBox _textBox;

		internal RichTextBox TextBox => _textBox;

		public IDocument Document
		{
			get
			{
				return _model;
			}
			set
			{
				_model = value;
				OnModelChanged();
			}
		}

		public bool ReadOnly
		{
			get
			{
				return _textBox.ReadOnly;
			}
			set
			{
				_textBox.ReadOnly = value;
			}
		}

		public bool WordWrap
		{
			get
			{
				return _textBox.WordWrap;
			}
			set
			{
				_textBox.WordWrap = value;
			}
		}

		public string Text
		{
			get
			{
				return _textBox.Text;
			}
			set
			{
				_textBox.Text = value;
			}
		}

		public Font Font
		{
			get
			{
				return _textBox.Font;
			}
			set
			{
				_textBox.Font = value;
			}
		}

		public Color BackColor
		{
			get
			{
				return _textBox.BackColor;
			}
			set
			{
				_textBox.BackColor = value;
			}
		}

		public ContextMenuStrip ContextMenuStrip
		{
			get
			{
				return _textBox.ContextMenuStrip;
			}
			set
			{
				_textBox.ContextMenuStrip = value;
			}
		}

		public int SelectionStart
		{
			get
			{
				return _textBox.SelectionStart;
			}
			set
			{
				_textBox.SelectionStart = value;
			}
		}

		public int SelectionLength
		{
			get
			{
				return _textBox.SelectionLength;
			}
			set
			{
				_textBox.SelectionLength = value;
			}
		}

		public event EventHandler Click;

		private void OnModelChanged()
		{
			Draw();
		}

		public TextBoxView(RichTextBox textBox)
		{
			if (textBox == null)
			{
				throw new ArgumentNullException("textBox");
			}
			_textBox = textBox;
			_textBox.Click += _textBox_Click;
		}

		private void _textBox_Click(object sender, EventArgs e)
		{
			if (this.Click != null)
			{
				this.Click(this, e);
			}
		}

		public void SelectAll()
		{
			_textBox.SelectAll();
		}

		public void Copy()
		{
			_textBox.Copy();
		}

		public int Find(string str, int start, RichTextBoxFinds options)
		{
			if (_textBox != null)
			{
				return _textBox.Find(str, start, options);
			}
			throw new NotImplementedException();
		}

		public void Clear()
		{
			_textBox.DeselectAll();
			_textBox.Clear();
		}

		public void Draw()
		{
			Clear();
			if (Document != null)
			{
				DrawNoHighlight();
			}
		}

		private void DrawNoHighlight()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ILine item in Document.Lines())
			{
				AddLine(stringBuilder, item);
			}
			_textBox.Text = stringBuilder.ToString();
		}

		private void AddLine(StringBuilder sb, ILine line)
		{
			foreach (ISegment item in line)
			{
				sb.Append(item.Text);
			}
		}

		private void AddLine(RichTextBox tb, ILine line)
		{
			foreach (ISegment item in line)
			{
				tb.AppendText(item.Text);
			}
		}

		public void CommitUpdate()
		{
		}
	}
}
