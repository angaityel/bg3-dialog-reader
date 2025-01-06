namespace bg3dialogreader
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            groupBoxLoadDB = new GroupBox();
            comboBoxLanguageSelect = new ComboBox();
            labelDBInfo = new Label();
            labelParse = new Label();
            labelLanguage = new Label();
            labelSelect = new Label();
            buttonCreateDB = new Button();
            buttonOpen = new Button();
            groupBoxExtractHtml = new GroupBox();
            linkLabelSettings = new LinkLabel();
            checkBoxExportLSJ = new CheckBox();
            labelHTML = new Label();
            buttonExtractHTML = new Button();
            groupBoxExtractDE2 = new GroupBox();
            labelDE2 = new Label();
            buttonExtractDE2 = new Button();
            richTextBoxLog = new RichTextBox();
            groupBoxDialogViewer = new GroupBox();
            panelSettings = new Panel();
            groupBoxDevNotesStyle = new GroupBox();
            radioButtonDevNotesSuperscript = new RadioButton();
            pictureBox1 = new PictureBox();
            radioButtonDevNotesInLine = new RadioButton();
            labelCurentFilePath = new Label();
            labelCurrentFileText = new Label();
            listViewDialog = new ListView();
            columnHeaderHandle = new ColumnHeader();
            columnHeaderDialog = new ColumnHeader();
            columnHeaderAudioFile = new ColumnHeader();
            contextMenuStripDialogList = new ContextMenuStrip(components);
            exportAudioToolStripMenuItem = new ToolStripMenuItem();
            copyHandleToolStripMenuItem = new ToolStripMenuItem();
            copyDialogLineToolStripMenuItem = new ToolStripMenuItem();
            copyAudioFilenameToolStripMenuItem = new ToolStripMenuItem();
            treeViewDialog = new TreeView();
            contextMenuStripDialogTree = new ContextMenuStrip(components);
            exportHTMLDialogToolStripMenuItem = new ToolStripMenuItem();
            exportDOS2DialogToolStripMenuItem = new ToolStripMenuItem();
            exportLSJDialogToolStripMenuItem = new ToolStripMenuItem();
            buttonLoadTree = new Button();
            labelExportedFiles = new Label();
            labelExportedFilesCounter = new Label();
            linkLabelNotFoundVGM = new LinkLabel();
            groupBoxLoadDB.SuspendLayout();
            groupBoxExtractHtml.SuspendLayout();
            groupBoxExtractDE2.SuspendLayout();
            groupBoxDialogViewer.SuspendLayout();
            panelSettings.SuspendLayout();
            groupBoxDevNotesStyle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            contextMenuStripDialogList.SuspendLayout();
            contextMenuStripDialogTree.SuspendLayout();
            SuspendLayout();
            // 
            // groupBoxLoadDB
            // 
            groupBoxLoadDB.Controls.Add(comboBoxLanguageSelect);
            groupBoxLoadDB.Controls.Add(labelDBInfo);
            groupBoxLoadDB.Controls.Add(labelParse);
            groupBoxLoadDB.Controls.Add(labelLanguage);
            groupBoxLoadDB.Controls.Add(labelSelect);
            groupBoxLoadDB.Controls.Add(buttonCreateDB);
            groupBoxLoadDB.Controls.Add(buttonOpen);
            groupBoxLoadDB.Location = new Point(4, 12);
            groupBoxLoadDB.Name = "groupBoxLoadDB";
            groupBoxLoadDB.Size = new Size(242, 184);
            groupBoxLoadDB.TabIndex = 0;
            groupBoxLoadDB.TabStop = false;
            groupBoxLoadDB.Text = "Open";
            // 
            // comboBoxLanguageSelect
            // 
            comboBoxLanguageSelect.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxLanguageSelect.Enabled = false;
            comboBoxLanguageSelect.FormattingEnabled = true;
            comboBoxLanguageSelect.Location = new Point(3, 85);
            comboBoxLanguageSelect.Name = "comboBoxLanguageSelect";
            comboBoxLanguageSelect.Size = new Size(228, 23);
            comboBoxLanguageSelect.TabIndex = 6;
            // 
            // labelDBInfo
            // 
            labelDBInfo.AutoSize = true;
            labelDBInfo.Location = new Point(6, 167);
            labelDBInfo.Name = "labelDBInfo";
            labelDBInfo.Size = new Size(38, 15);
            labelDBInfo.TabIndex = 5;
            labelDBInfo.Text = "label3";
            labelDBInfo.Visible = false;
            // 
            // labelParse
            // 
            labelParse.AutoSize = true;
            labelParse.Location = new Point(6, 109);
            labelParse.Name = "labelParse";
            labelParse.Size = new Size(139, 15);
            labelParse.TabIndex = 4;
            labelParse.Text = "3. Parse tags to database:";
            // 
            // labelLanguage
            // 
            labelLanguage.AutoSize = true;
            labelLanguage.Location = new Point(6, 69);
            labelLanguage.Name = "labelLanguage";
            labelLanguage.Size = new Size(105, 15);
            labelLanguage.TabIndex = 3;
            labelLanguage.Text = "2. Select language:";
            // 
            // labelSelect
            // 
            labelSelect.AutoSize = true;
            labelSelect.Location = new Point(6, 16);
            labelSelect.Name = "labelSelect";
            labelSelect.Size = new Size(214, 15);
            labelSelect.TabIndex = 3;
            labelSelect.Text = "1. Select any *.pak file from Data folder:";
            // 
            // buttonCreateDB
            // 
            buttonCreateDB.Enabled = false;
            buttonCreateDB.Location = new Point(3, 125);
            buttonCreateDB.Name = "buttonCreateDB";
            buttonCreateDB.Size = new Size(228, 34);
            buttonCreateDB.TabIndex = 0;
            buttonCreateDB.Text = "Create DB";
            buttonCreateDB.UseVisualStyleBackColor = true;
            buttonCreateDB.Click += buttonCreateDB_Click;
            // 
            // buttonOpen
            // 
            buttonOpen.Location = new Point(3, 32);
            buttonOpen.Name = "buttonOpen";
            buttonOpen.Size = new Size(228, 34);
            buttonOpen.TabIndex = 0;
            buttonOpen.Text = "Open";
            buttonOpen.UseVisualStyleBackColor = true;
            buttonOpen.Click += buttonOpen_Click;
            // 
            // groupBoxExtractHtml
            // 
            groupBoxExtractHtml.Controls.Add(linkLabelSettings);
            groupBoxExtractHtml.Controls.Add(checkBoxExportLSJ);
            groupBoxExtractHtml.Controls.Add(labelHTML);
            groupBoxExtractHtml.Controls.Add(buttonExtractHTML);
            groupBoxExtractHtml.Location = new Point(4, 202);
            groupBoxExtractHtml.Name = "groupBoxExtractHtml";
            groupBoxExtractHtml.Size = new Size(242, 100);
            groupBoxExtractHtml.TabIndex = 1;
            groupBoxExtractHtml.TabStop = false;
            groupBoxExtractHtml.Text = "Extract formatted html";
            // 
            // linkLabelSettings
            // 
            linkLabelSettings.AutoSize = true;
            linkLabelSettings.Location = new Point(140, 0);
            linkLabelSettings.Name = "linkLabelSettings";
            linkLabelSettings.Size = new Size(49, 15);
            linkLabelSettings.TabIndex = 3;
            linkLabelSettings.TabStop = true;
            linkLabelSettings.Text = "Settings";
            linkLabelSettings.LinkClicked += linkLabelSettings_LinkClicked;
            // 
            // checkBoxExportLSJ
            // 
            checkBoxExportLSJ.AutoSize = true;
            checkBoxExportLSJ.Enabled = false;
            checkBoxExportLSJ.Location = new Point(6, 72);
            checkBoxExportLSJ.Name = "checkBoxExportLSJ";
            checkBoxExportLSJ.Size = new Size(218, 19);
            checkBoxExportLSJ.TabIndex = 2;
            checkBoxExportLSJ.Text = "Also export .lsj dialog files (optional)";
            checkBoxExportLSJ.UseVisualStyleBackColor = true;
            // 
            // labelHTML
            // 
            labelHTML.AutoSize = true;
            labelHTML.Location = new Point(6, 16);
            labelHTML.Name = "labelHTML";
            labelHTML.Size = new Size(100, 15);
            labelHTML.TabIndex = 1;
            labelHTML.Text = "Export all dialogs:";
            // 
            // buttonExtractHTML
            // 
            buttonExtractHTML.Enabled = false;
            buttonExtractHTML.Location = new Point(3, 32);
            buttonExtractHTML.Name = "buttonExtractHTML";
            buttonExtractHTML.Size = new Size(228, 34);
            buttonExtractHTML.TabIndex = 0;
            buttonExtractHTML.Text = "Export all dialogs to html";
            buttonExtractHTML.UseVisualStyleBackColor = true;
            buttonExtractHTML.Click += buttonExtractHTML_Click;
            // 
            // groupBoxExtractDE2
            // 
            groupBoxExtractDE2.Controls.Add(labelDE2);
            groupBoxExtractDE2.Controls.Add(buttonExtractDE2);
            groupBoxExtractDE2.Location = new Point(4, 308);
            groupBoxExtractDE2.Name = "groupBoxExtractDE2";
            groupBoxExtractDE2.Size = new Size(239, 84);
            groupBoxExtractDE2.TabIndex = 2;
            groupBoxExtractDE2.TabStop = false;
            groupBoxExtractDE2.Text = "Divinity Engine 2";
            // 
            // labelDE2
            // 
            labelDE2.AutoSize = true;
            labelDE2.Location = new Point(6, 16);
            labelDE2.Name = "labelDE2";
            labelDE2.Size = new Size(213, 15);
            labelDE2.TabIndex = 1;
            labelDE2.Text = "Convert all dialogs to Divinity Engine 2:";
            // 
            // buttonExtractDE2
            // 
            buttonExtractDE2.Enabled = false;
            buttonExtractDE2.Location = new Point(3, 32);
            buttonExtractDE2.Name = "buttonExtractDE2";
            buttonExtractDE2.Size = new Size(228, 34);
            buttonExtractDE2.TabIndex = 0;
            buttonExtractDE2.Text = "Convert";
            buttonExtractDE2.UseVisualStyleBackColor = true;
            buttonExtractDE2.Click += buttonExtractDE2_Click;
            // 
            // richTextBoxLog
            // 
            richTextBoxLog.Location = new Point(4, 398);
            richTextBoxLog.Name = "richTextBoxLog";
            richTextBoxLog.ReadOnly = true;
            richTextBoxLog.Size = new Size(237, 108);
            richTextBoxLog.TabIndex = 3;
            richTextBoxLog.Text = "";
            // 
            // groupBoxDialogViewer
            // 
            groupBoxDialogViewer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxDialogViewer.Controls.Add(panelSettings);
            groupBoxDialogViewer.Controls.Add(labelCurentFilePath);
            groupBoxDialogViewer.Controls.Add(labelCurrentFileText);
            groupBoxDialogViewer.Controls.Add(listViewDialog);
            groupBoxDialogViewer.Controls.Add(treeViewDialog);
            groupBoxDialogViewer.Controls.Add(buttonLoadTree);
            groupBoxDialogViewer.Location = new Point(252, 12);
            groupBoxDialogViewer.Name = "groupBoxDialogViewer";
            groupBoxDialogViewer.Size = new Size(886, 494);
            groupBoxDialogViewer.TabIndex = 4;
            groupBoxDialogViewer.TabStop = false;
            groupBoxDialogViewer.Text = "Audio player and dialog viewer (without formatting)";
            // 
            // panelSettings
            // 
            panelSettings.BackColor = SystemColors.Control;
            panelSettings.Controls.Add(groupBoxDevNotesStyle);
            panelSettings.Location = new Point(6, 167);
            panelSettings.Name = "panelSettings";
            panelSettings.Size = new Size(404, 160);
            panelSettings.TabIndex = 8;
            panelSettings.Visible = false;
            // 
            // groupBoxDevNotesStyle
            // 
            groupBoxDevNotesStyle.Controls.Add(radioButtonDevNotesSuperscript);
            groupBoxDevNotesStyle.Controls.Add(pictureBox1);
            groupBoxDevNotesStyle.Controls.Add(radioButtonDevNotesInLine);
            groupBoxDevNotesStyle.Location = new Point(15, 15);
            groupBoxDevNotesStyle.Name = "groupBoxDevNotesStyle";
            groupBoxDevNotesStyle.Size = new Size(374, 142);
            groupBoxDevNotesStyle.TabIndex = 5;
            groupBoxDevNotesStyle.TabStop = false;
            groupBoxDevNotesStyle.Text = "Devnotes style";
            // 
            // radioButtonDevNotesSuperscript
            // 
            radioButtonDevNotesSuperscript.AutoSize = true;
            radioButtonDevNotesSuperscript.Checked = true;
            radioButtonDevNotesSuperscript.Location = new Point(6, 22);
            radioButtonDevNotesSuperscript.Name = "radioButtonDevNotesSuperscript";
            radioButtonDevNotesSuperscript.Size = new Size(84, 19);
            radioButtonDevNotesSuperscript.TabIndex = 1;
            radioButtonDevNotesSuperscript.TabStop = true;
            radioButtonDevNotesSuperscript.Text = "Superscript";
            radioButtonDevNotesSuperscript.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.dolly;
            pictureBox1.Location = new Point(6, 80);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(360, 53);
            pictureBox1.TabIndex = 4;
            pictureBox1.TabStop = false;
            // 
            // radioButtonDevNotesInLine
            // 
            radioButtonDevNotesInLine.AutoSize = true;
            radioButtonDevNotesInLine.Location = new Point(6, 47);
            radioButtonDevNotesInLine.Name = "radioButtonDevNotesInLine";
            radioButtonDevNotesInLine.Size = new Size(54, 19);
            radioButtonDevNotesInLine.TabIndex = 2;
            radioButtonDevNotesInLine.Text = "Inline";
            radioButtonDevNotesInLine.UseVisualStyleBackColor = true;
            // 
            // labelCurentFilePath
            // 
            labelCurentFilePath.AutoSize = true;
            labelCurentFilePath.Location = new Point(240, 16);
            labelCurentFilePath.Name = "labelCurentFilePath";
            labelCurentFilePath.Size = new Size(38, 15);
            labelCurentFilePath.TabIndex = 4;
            labelCurentFilePath.Text = "label2";
            labelCurentFilePath.Visible = false;
            // 
            // labelCurrentFileText
            // 
            labelCurrentFileText.AutoSize = true;
            labelCurrentFileText.Location = new Point(174, 16);
            labelCurrentFileText.Name = "labelCurrentFileText";
            labelCurrentFileText.Size = new Size(69, 15);
            labelCurrentFileText.TabIndex = 3;
            labelCurrentFileText.Text = "Current file:";
            // 
            // listViewDialog
            // 
            listViewDialog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listViewDialog.Columns.AddRange(new ColumnHeader[] { columnHeaderHandle, columnHeaderDialog, columnHeaderAudioFile });
            listViewDialog.ContextMenuStrip = contextMenuStripDialogList;
            listViewDialog.FullRowSelect = true;
            listViewDialog.Location = new Point(340, 32);
            listViewDialog.Name = "listViewDialog";
            listViewDialog.Size = new Size(540, 456);
            listViewDialog.TabIndex = 2;
            listViewDialog.UseCompatibleStateImageBehavior = false;
            listViewDialog.View = View.Details;
            listViewDialog.DoubleClick += listViewDialog_DoubleClick;
            listViewDialog.MouseClick += listViewDialog_MouseClick;
            listViewDialog.MouseDown += listViewDialog_MouseDown;
            // 
            // columnHeaderHandle
            // 
            columnHeaderHandle.Text = "Handle";
            columnHeaderHandle.Width = 125;
            // 
            // columnHeaderDialog
            // 
            columnHeaderDialog.Text = "Dialog";
            columnHeaderDialog.Width = 300;
            // 
            // columnHeaderAudioFile
            // 
            columnHeaderAudioFile.Text = "Audio File";
            columnHeaderAudioFile.Width = 150;
            // 
            // contextMenuStripDialogList
            // 
            contextMenuStripDialogList.Items.AddRange(new ToolStripItem[] { exportAudioToolStripMenuItem, copyHandleToolStripMenuItem, copyDialogLineToolStripMenuItem, copyAudioFilenameToolStripMenuItem });
            contextMenuStripDialogList.Name = "contextMenuStripDialogList";
            contextMenuStripDialogList.Size = new Size(185, 92);
            // 
            // exportAudioToolStripMenuItem
            // 
            exportAudioToolStripMenuItem.Name = "exportAudioToolStripMenuItem";
            exportAudioToolStripMenuItem.Size = new Size(184, 22);
            exportAudioToolStripMenuItem.Text = "Export audio";
            exportAudioToolStripMenuItem.Click += exportAudioToolStripMenuItem_Click;
            // 
            // copyHandleToolStripMenuItem
            // 
            copyHandleToolStripMenuItem.Name = "copyHandleToolStripMenuItem";
            copyHandleToolStripMenuItem.Size = new Size(184, 22);
            copyHandleToolStripMenuItem.Text = "Copy handle";
            copyHandleToolStripMenuItem.Click += copyHandleToolStripMenuItem_Click;
            // 
            // copyDialogLineToolStripMenuItem
            // 
            copyDialogLineToolStripMenuItem.Name = "copyDialogLineToolStripMenuItem";
            copyDialogLineToolStripMenuItem.Size = new Size(184, 22);
            copyDialogLineToolStripMenuItem.Text = "Copy dialog line";
            copyDialogLineToolStripMenuItem.Click += copyDialogLineToolStripMenuItem_Click;
            // 
            // copyAudioFilenameToolStripMenuItem
            // 
            copyAudioFilenameToolStripMenuItem.Name = "copyAudioFilenameToolStripMenuItem";
            copyAudioFilenameToolStripMenuItem.Size = new Size(184, 22);
            copyAudioFilenameToolStripMenuItem.Text = "Copy audio filename";
            copyAudioFilenameToolStripMenuItem.Click += copyAudioFilenameToolStripMenuItem_Click;
            // 
            // treeViewDialog
            // 
            treeViewDialog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            treeViewDialog.ContextMenuStrip = contextMenuStripDialogTree;
            treeViewDialog.Location = new Point(6, 72);
            treeViewDialog.Name = "treeViewDialog";
            treeViewDialog.Size = new Size(325, 416);
            treeViewDialog.TabIndex = 1;
            treeViewDialog.NodeMouseClick += treeViewDialog_NodeMouseClick;
            treeViewDialog.MouseDoubleClick += treeViewDialog_MouseDoubleClick;
            treeViewDialog.MouseDown += treeViewDialog_MouseDown;
            // 
            // contextMenuStripDialogTree
            // 
            contextMenuStripDialogTree.Items.AddRange(new ToolStripItem[] { exportHTMLDialogToolStripMenuItem, exportDOS2DialogToolStripMenuItem, exportLSJDialogToolStripMenuItem });
            contextMenuStripDialogTree.Name = "contextMenuStripDialogTree";
            contextMenuStripDialogTree.Size = new Size(180, 70);
            // 
            // exportHTMLDialogToolStripMenuItem
            // 
            exportHTMLDialogToolStripMenuItem.Name = "exportHTMLDialogToolStripMenuItem";
            exportHTMLDialogToolStripMenuItem.Size = new Size(179, 22);
            exportHTMLDialogToolStripMenuItem.Text = "Export HTML dialog";
            exportHTMLDialogToolStripMenuItem.Click += exportHTMLDialogToolStripMenuItem_Click;
            // 
            // exportDOS2DialogToolStripMenuItem
            // 
            exportDOS2DialogToolStripMenuItem.Name = "exportDOS2DialogToolStripMenuItem";
            exportDOS2DialogToolStripMenuItem.Size = new Size(179, 22);
            exportDOS2DialogToolStripMenuItem.Text = "Export DOS2 dialog";
            exportDOS2DialogToolStripMenuItem.Click += exportDOS2DialogToolStripMenuItem_Click;
            // 
            // exportLSJDialogToolStripMenuItem
            // 
            exportLSJDialogToolStripMenuItem.Name = "exportLSJDialogToolStripMenuItem";
            exportLSJDialogToolStripMenuItem.Size = new Size(179, 22);
            exportLSJDialogToolStripMenuItem.Text = "Export LSJ dialog";
            exportLSJDialogToolStripMenuItem.Click += exportLSJDialogToolStripMenuItem_Click;
            // 
            // buttonLoadTree
            // 
            buttonLoadTree.Enabled = false;
            buttonLoadTree.Location = new Point(6, 32);
            buttonLoadTree.Name = "buttonLoadTree";
            buttonLoadTree.Size = new Size(325, 34);
            buttonLoadTree.TabIndex = 0;
            buttonLoadTree.Text = "Load dialogs tree";
            buttonLoadTree.UseVisualStyleBackColor = true;
            buttonLoadTree.Click += buttonLoadTree_Click;
            // 
            // labelExportedFiles
            // 
            labelExportedFiles.AutoSize = true;
            labelExportedFiles.Location = new Point(10, 509);
            labelExportedFiles.Name = "labelExportedFiles";
            labelExportedFiles.Size = new Size(83, 15);
            labelExportedFiles.TabIndex = 5;
            labelExportedFiles.Text = "Files exported:";
            labelExportedFiles.Visible = false;
            // 
            // labelExportedFilesCounter
            // 
            labelExportedFilesCounter.AutoSize = true;
            labelExportedFilesCounter.Location = new Point(93, 509);
            labelExportedFilesCounter.Name = "labelExportedFilesCounter";
            labelExportedFilesCounter.Size = new Size(13, 15);
            labelExportedFilesCounter.TabIndex = 6;
            labelExportedFilesCounter.Text = "0";
            labelExportedFilesCounter.Visible = false;
            // 
            // linkLabelNotFoundVGM
            // 
            linkLabelNotFoundVGM.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            linkLabelNotFoundVGM.AutoSize = true;
            linkLabelNotFoundVGM.LinkArea = new LinkArea(71, 19);
            linkLabelNotFoundVGM.Location = new Point(258, 509);
            linkLabelNotFoundVGM.Name = "linkLabelNotFoundVGM";
            linkLabelNotFoundVGM.Size = new Size(726, 21);
            linkLabelNotFoundVGM.TabIndex = 7;
            linkLabelNotFoundVGM.TabStop = true;
            linkLabelNotFoundVGM.Text = "vgmstream-cli.exe not found. For playing and converting audio download vgmstream-win64.zip and unzip files into current directory";
            linkLabelNotFoundVGM.UseCompatibleTextRendering = true;
            linkLabelNotFoundVGM.Visible = false;
            linkLabelNotFoundVGM.LinkClicked += linkLabelNotFoundVGM_LinkClicked;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1150, 531);
            Controls.Add(linkLabelNotFoundVGM);
            Controls.Add(labelExportedFilesCounter);
            Controls.Add(labelExportedFiles);
            Controls.Add(groupBoxDialogViewer);
            Controls.Add(richTextBoxLog);
            Controls.Add(groupBoxExtractDE2);
            Controls.Add(groupBoxExtractHtml);
            Controls.Add(groupBoxLoadDB);
            Name = "Form1";
            Text = "BG3 Dialog Reader 1.2.2";
            groupBoxLoadDB.ResumeLayout(false);
            groupBoxLoadDB.PerformLayout();
            groupBoxExtractHtml.ResumeLayout(false);
            groupBoxExtractHtml.PerformLayout();
            groupBoxExtractDE2.ResumeLayout(false);
            groupBoxExtractDE2.PerformLayout();
            groupBoxDialogViewer.ResumeLayout(false);
            groupBoxDialogViewer.PerformLayout();
            panelSettings.ResumeLayout(false);
            groupBoxDevNotesStyle.ResumeLayout(false);
            groupBoxDevNotesStyle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            contextMenuStripDialogList.ResumeLayout(false);
            contextMenuStripDialogTree.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox groupBoxLoadDB;
        private GroupBox groupBoxExtractHtml;
        private GroupBox groupBoxExtractDE2;
        private RichTextBox richTextBoxLog;
        private GroupBox groupBoxDialogViewer;
        private Button buttonCreateDB;
        private Button buttonOpen;
        private Button buttonExtractHTML;
        private Button buttonExtractDE2;
        private Button buttonLoadTree;
        private TreeView treeViewDialog;
        private ListView listViewDialog;
        private Label labelSelect;
        private ColumnHeader columnHeaderHandle;
        private ColumnHeader columnHeaderDialog;
        private ColumnHeader columnHeaderAudioFile;
        private ComboBox comboBoxLanguageSelect;
        private Label labelDBInfo;
        private Label labelParse;
        private Label labelLanguage;
        private Label labelHTML;
        private Label labelDE2;
        private CheckBox checkBoxExportLSJ;
        private Label labelCurentFilePath;
        private Label labelCurrentFileText;
        private Label labelExportedFiles;
        private Label labelExportedFilesCounter;
        private LinkLabel linkLabelNotFoundVGM;
        private ContextMenuStrip contextMenuStripDialogTree;
        private ContextMenuStrip contextMenuStripDialogList;
        private ToolStripMenuItem exportAudioToolStripMenuItem;
        private ToolStripMenuItem copyHandleToolStripMenuItem;
        private ToolStripMenuItem copyDialogLineToolStripMenuItem;
        private ToolStripMenuItem copyAudioFilenameToolStripMenuItem;
        private ToolStripMenuItem exportHTMLDialogToolStripMenuItem;
        private ToolStripMenuItem exportDOS2DialogToolStripMenuItem;
        private ToolStripMenuItem exportLSJDialogToolStripMenuItem;
        private LinkLabel linkLabelSettings;
        private Panel panelSettings;
        private RadioButton radioButtonDevNotesInLine;
        private RadioButton radioButtonDevNotesSuperscript;
        private PictureBox pictureBox1;
        private GroupBox groupBoxDevNotesStyle;
    }
}
