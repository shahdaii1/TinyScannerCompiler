using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TinyScanner
{
    public class ScannerForm : Form
    {
        private TextBox txtSource = null!;
        private Button btnScan = null!;
        private Button btnSample = null!;
        private Button btnClear = null!;
        private DataGridView dgvTokens = null!;
        private DataGridView dgvErrors = null!;
        private Label lblTokenCount = null!;
        private Label lblErrorCount = null!;
        private Label lblLineCount = null!;

        public ScannerForm()
        {
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "Tiny Language Scanner";
            this.Size = new Size(1000, 700);
            this.MinimumSize = new Size(800, 600);
            this.Font = new Font("Segoe UI", 9.5f);
            this.BackColor = Color.FromArgb(240, 240, 245);

            Label lblSrc = new Label { Text = "Source Code:", Left = 10, Top = 10, AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };

            txtSource = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Both,
                Font = new Font("Consolas", 10f),
                Left = 10,
                Top = 30,
                Width = 960,
                Height = 160,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                WordWrap = false,
                AcceptsReturn = true,
                AcceptsTab = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            btnScan = new Button { Text = "Scan", Left = 10, Top = 200, Width = 90, Height = 32, BackColor = Color.FromArgb(59, 130, 246), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), Cursor = Cursors.Hand };
            btnScan.FlatAppearance.BorderSize = 0;

            btnSample = new Button { Text = "Load Sample", Left = 110, Top = 200, Width = 110, Height = 32, BackColor = Color.FromArgb(100, 100, 120), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnSample.FlatAppearance.BorderSize = 0;

            btnClear = new Button { Text = "Clear", Left = 230, Top = 200, Width = 90, Height = 32, BackColor = Color.FromArgb(160, 60, 60), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnClear.FlatAppearance.BorderSize = 0;

            btnScan.Click += (s, e) => RunScan();
            btnSample.Click += (s, e) => LoadSample();

            btnClear.Click += (s, e) => ClearAll();
            Button btnParse = new Button
            {
                Text = "Parse",
                Left = 330,
                Top = 200,
                Width = 90,
                Height = 32,
                BackColor = Color.FromArgb(34, 197, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            btnParse.FlatAppearance.BorderSize = 0;
            btnParse.Click += (s, e) => RunParse();
            lblTokenCount = new Label { Text = "Tokens: 0", Left = 400, Top = 207, AutoSize = true, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = Color.FromArgb(59, 130, 246) };
            lblErrorCount = new Label { Text = "Errors: 0", Left = 510, Top = 207, AutoSize = true, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = Color.FromArgb(34, 197, 94) };
            lblLineCount = new Label { Text = "Lines: 0", Left = 620, Top = 207, AutoSize = true, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = Color.FromArgb(120, 120, 140) };

            Label lblTok = new Label { Text = "Token Table:", Left = 10, Top = 245, AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
            Label lblErr = new Label { Text = "Errors:", Left = 600, Top = 245, AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };

            dgvTokens = MakeGrid(new[] { "#", "Lexeme", "Token Class", "Category" }, new[] { 40, 150, 160, 0 });
            dgvTokens.Left = 10; dgvTokens.Top = 265; dgvTokens.Width = 580; dgvTokens.Height = 350;
            dgvTokens.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;

            dgvErrors = MakeGrid(new[] { "Line", "Lexeme", "Error" }, new[] { 50, 100, 0 });
            dgvErrors.Left = 600; dgvErrors.Top = 265; dgvErrors.Width = 370; dgvErrors.Height = 350;
            dgvErrors.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

            this.Controls.AddRange(new Control[] { lblSrc, txtSource, btnScan, btnSample, btnClear, btnParse, lblTokenCount, lblErrorCount, lblLineCount, lblTok, dgvTokens, lblErr, dgvErrors });

            this.Resize += (s, e) =>
            {
                txtSource.Width = this.ClientSize.Width - 20;
                dgvTokens.Height = this.ClientSize.Height - dgvTokens.Top - 20;
                dgvErrors.Width = this.ClientSize.Width - dgvErrors.Left - 10;
                dgvErrors.Height = this.ClientSize.Height - dgvErrors.Top - 20;
            };
        }

        private DataGridView MakeGrid(string[] headers, int[] widths)
        {
            var dgv = new DataGridView
            {
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToResizeRows = false,
                BorderStyle = BorderStyle.None,
                BackgroundColor = Color.White,
                GridColor = Color.FromArgb(220, 220, 225),
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None
            };
            dgv.DefaultCellStyle.Font = new Font("Consolas", 9.5f);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 66, 150);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 230, 240);
            dgv.ColumnHeadersHeight = 30;
            dgv.RowTemplate.Height = 26;

            for (int i = 0; i < headers.Length; i++)
            {
                var col = new DataGridViewTextBoxColumn { HeaderText = headers[i] };
                if (widths[i] == 0) col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                else col.Width = widths[i];
                dgv.Columns.Add(col);
            }
            return dgv;
        }

        private void RunScan()
        {
            string src = txtSource.Text;
            if (string.IsNullOrWhiteSpace(src)) return;

            var scanner = new Scanner();
            scanner.StartScanning(src);
            int lineCount = src.Split('\n').Length;

            lblTokenCount.Text = $"Tokens: {scanner.Tokens.Count}";
            lblErrorCount.Text = $"Errors: {scanner.Errors.Count}";
            lblErrorCount.ForeColor = scanner.Errors.Count > 0 ? Color.FromArgb(220, 50, 50) : Color.FromArgb(34, 197, 94);
            lblLineCount.Text = $"Lines: {lineCount}";

            dgvTokens.Rows.Clear();
            for (int i = 0; i < scanner.Tokens.Count; i++)
            {
                var tok = scanner.Tokens[i];
                string cat = GetCategory(tok.token_type.ToString());
                int row = dgvTokens.Rows.Add((i + 1).ToString(), tok.lex, tok.token_type.ToString(), cat);
                dgvTokens.Rows[row].DefaultCellStyle.ForeColor = CategoryColor(cat);
            }

            dgvErrors.Rows.Clear();
            foreach (var err in scanner.Errors)
            {
                int row = dgvErrors.Rows.Add(err.line.ToString(), err.lex, err.message);
                dgvErrors.Rows[row].DefaultCellStyle.ForeColor = Color.FromArgb(153, 27, 27);
                dgvErrors.Rows[row].DefaultCellStyle.BackColor = Color.FromArgb(254, 242, 242);
            }
        }

        private string GetCategory(string tc)
        {
            var kw = new HashSet<string> { "Int", "Float", "String", "Read", "Write", "Repeat", "Until", "If", "ElseIf", "Else", "Then", "Return", "Endl", "Main", "ReservedWord" };
            if (kw.Contains(tc)) return "Keyword";
            if (tc == "Identifier") return "Identifier";
            if (tc == "Constant") return "Number";
            if (tc == "StringConstant") return "String";
            return "Operator";
        }

        private Color CategoryColor(string cat)
        {
            switch (cat)
            {
                case "Keyword": return Color.FromArgb(37, 99, 235);
                case "Identifier": return Color.FromArgb(21, 128, 61);
                case "Number": return Color.FromArgb(180, 83, 9);
                case "String": return Color.FromArgb(109, 40, 217);
                default: return Color.FromArgb(60, 60, 80);
            }
        }

        private void LoadSample()
        {
            txtSource.Text =
@"/* computes factorial */
int main()
{
  int x;
  read x;
  if x > 0 then
    int fact := 1;
    repeat
      fact := fact * x;
      x := x - 1;
    until x = 0
    write fact;
  end
  return 0;
}";
            RunScan();
        }

        private void ClearAll()
        {
            txtSource.Clear();
            dgvTokens.Rows.Clear();
            dgvErrors.Rows.Clear();
            lblTokenCount.Text = "Tokens: 0";
            lblErrorCount.Text = "Errors: 0";
            lblErrorCount.ForeColor = Color.FromArgb(34, 197, 94);
            lblLineCount.Text = "Lines: 0";
        }

        private void RunParse()
        {
            if(string.IsNullOrWhiteSpace(txtSource.Text)) return;

            var scanner = new Scanner();
            scanner.StartScanning(txtSource.Text);

            if (scanner.Errors.Count > 0)
            {
                MessageBox.Show("Fix scan errors first!", "Error");
                return;
            }

            var parser = new Parser();
            var root = parser.StartParsing(scanner.Tokens);

            Form treeForm = new Form
            {
                Text = "Parse Tree",
                Size = new Size(500, 600)
            };

            TreeView tv = new TreeView { Dock = DockStyle.Fill };
            var treeNode = Parser.PrintParseTree(root);
            if (treeNode != null)
                tv.Nodes.Add(treeNode);
            tv.ExpandAll();

            treeForm.Controls.Add(tv);
            treeForm.Show();
        }
    }
    }   
////////////