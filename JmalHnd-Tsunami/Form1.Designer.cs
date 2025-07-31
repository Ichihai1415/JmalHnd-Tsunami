namespace JmalHnd_Tsunami
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            GetTimer = new System.Windows.Forms.Timer(components);
            CMS = new System.Windows.Forms.ContextMenuStrip(components);
            TSMGetnow = new System.Windows.Forms.ToolStripMenuItem();
            TSMOnlyVTSE41 = new System.Windows.Forms.ToolStripMenuItem();
            TSM_ReleaseSite = new System.Windows.Forms.ToolStripMenuItem();
            CMS.SuspendLayout();
            SuspendLayout();
            // 
            // GetTimer
            // 
            GetTimer.Enabled = true;
            GetTimer.Interval = 300000;
            GetTimer.Tick += GetTimer_Tick;
            // 
            // CMS
            // 
            CMS.ImageScalingSize = new System.Drawing.Size(20, 20);
            CMS.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { TSMGetnow, TSMOnlyVTSE41, TSM_ReleaseSite });
            CMS.Name = "CMS";
            CMS.Size = new System.Drawing.Size(181, 92);
            // 
            // TSMGetnow
            // 
            TSMGetnow.Name = "TSMGetnow";
            TSMGetnow.Size = new System.Drawing.Size(213, 22);
            TSMGetnow.Text = "今すぐ取得する";
            TSMGetnow.Click += TSMGetnow_Click;
            // 
            // TSMOnlyVTSE41
            // 
            TSMOnlyVTSE41.Name = "TSMOnlyVTSE41";
            TSMOnlyVTSE41.Size = new System.Drawing.Size(180, 22);
            TSMOnlyVTSE41.Text = "*VTSE41のみモード";
            TSMOnlyVTSE41.Click += TSMGetnowOnlyWarn_Click;
            // 
            // TSM_ReleaseSite
            // 
            TSM_ReleaseSite.Name = "TSM_ReleaseSite";
            TSM_ReleaseSite.Size = new System.Drawing.Size(180, 22);
            TSM_ReleaseSite.Text = "配布ページを開く";
            TSM_ReleaseSite.Click += TSM_ReleaseSite_Click;
            // 
            // Form1
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            ClientSize = new System.Drawing.Size(1280, 720);
            ContextMenuStrip = CMS;
            Margin = new System.Windows.Forms.Padding(2);
            Name = "Form1";
            Text = "JmalHnd-Tsunami";
            Load += Form1_Load;
            CMS.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer GetTimer;
        private System.Windows.Forms.ContextMenuStrip CMS;
        private System.Windows.Forms.ToolStripMenuItem TSM_ReleaseSite;
        private System.Windows.Forms.ToolStripMenuItem TSMGetnow;
        private System.Windows.Forms.ToolStripMenuItem TSMOnlyVTSE41;
    }
}

