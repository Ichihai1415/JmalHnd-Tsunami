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
            this.components = new System.ComponentModel.Container();
            this.GetTimer = new System.Windows.Forms.Timer(this.components);
            this.CMS = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.TSM_LongFeed = new System.Windows.Forms.ToolStripMenuItem();
            this.TSM_ReleaseSite = new System.Windows.Forms.ToolStripMenuItem();
            this.CMS.SuspendLayout();
            this.SuspendLayout();
            // 
            // GetTimer
            // 
            this.GetTimer.Enabled = true;
            this.GetTimer.Interval = 300000;
            this.GetTimer.Tick += new System.EventHandler(this.GetTimer_Tick);
            // 
            // CMS
            // 
            this.CMS.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.CMS.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSM_LongFeed,
            this.TSM_ReleaseSite});
            this.CMS.Name = "CMS";
            this.CMS.Size = new System.Drawing.Size(225, 52);
            // 
            // TSM_LongFeed
            // 
            this.TSM_LongFeed.Name = "TSM_LongFeed";
            this.TSM_LongFeed.Size = new System.Drawing.Size(224, 24);
            this.TSM_LongFeed.Text = "長期フィードから取得する";
            this.TSM_LongFeed.Click += new System.EventHandler(this.TSM_LongFeed_Click);
            // 
            // TSM_ReleaseSite
            // 
            this.TSM_ReleaseSite.Name = "TSM_ReleaseSite";
            this.TSM_ReleaseSite.Size = new System.Drawing.Size(224, 24);
            this.TSM_ReleaseSite.Text = "配布サイト";
            this.TSM_ReleaseSite.Click += new System.EventHandler(this.TSM_ReleaseSite_Click);
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(1280, 720);
            this.ContextMenuStrip = this.CMS;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "JmalHnd-Tsunami";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.CMS.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer GetTimer;
        private System.Windows.Forms.ContextMenuStrip CMS;
        private System.Windows.Forms.ToolStripMenuItem TSM_LongFeed;
        private System.Windows.Forms.ToolStripMenuItem TSM_ReleaseSite;
    }
}

