namespace AutoCompleteCombobox
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.autoCompleteCombobox1 = new WanJi.Common.Forms.Controls.AutoCompleteCombobox();
            this.SuspendLayout();
            // 
            // autoCompleteCombobox1
            // 
            this.autoCompleteCombobox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.autoCompleteCombobox1.DropDownHeight = 140;
            this.autoCompleteCombobox1.EnableFuzzyMatch = false;
            this.autoCompleteCombobox1.FormattingEnabled = true;
            this.autoCompleteCombobox1.IntegralHeight = false;
            this.autoCompleteCombobox1.Location = new System.Drawing.Point(145, 85);
            this.autoCompleteCombobox1.Name = "autoCompleteCombobox1";
            this.autoCompleteCombobox1.Size = new System.Drawing.Size(121, 22);
            this.autoCompleteCombobox1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(403, 262);
            this.Controls.Add(this.autoCompleteCombobox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private WanJi.Common.Forms.Controls.AutoCompleteCombobox autoCompleteCombobox1;
    }
}

