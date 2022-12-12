namespace DiagramControl
{
	partial class PanningControl
	{
		/// <summary> 
		/// 필수 디자이너 변수입니다.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// 사용 중인 모든 리소스를 정리합니다.
		/// </summary>
		/// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region 구성 요소 디자이너에서 생성한 코드

		/// <summary> 
		/// 디자이너 지원에 필요한 메서드입니다. 
		/// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
		/// </summary>
		private void InitializeComponent()
		{
			this.openGLControl1 = new SharpGL.OpenGLControl();
			((System.ComponentModel.ISupportInitialize)(this.openGLControl1)).BeginInit();
			this.SuspendLayout();
			// 
			// openGLControl1
			// 
			this.openGLControl1.AutoSize = true;
			this.openGLControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.openGLControl1.DrawFPS = false;
			this.openGLControl1.FrameRate = 28;
			this.openGLControl1.Location = new System.Drawing.Point(0, 0);
			this.openGLControl1.Margin = new System.Windows.Forms.Padding(14, 12, 14, 12);
			this.openGLControl1.Name = "openGLControl1";
			this.openGLControl1.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL2_1;
			this.openGLControl1.RenderContextType = SharpGL.RenderContextType.FBO;
			this.openGLControl1.RenderTrigger = SharpGL.RenderTrigger.Manual;
			this.openGLControl1.Size = new System.Drawing.Size(1000, 1000);
			this.openGLControl1.TabIndex = 0;
			this.openGLControl1.OpenGLDraw += new SharpGL.RenderEventHandler(this.OnOpenGLDraw);
			this.openGLControl1.Resized += new System.EventHandler(this.OnGLControlResized);
			this.openGLControl1.Load += new System.EventHandler(this.OnGLControlLoad);
			this.openGLControl1.DoubleClick += new System.EventHandler(this.openGLControl1_DoubleClick);
			this.openGLControl1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUp);
			this.openGLControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
			this.openGLControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);
			this.openGLControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
			this.openGLControl1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.OnMouseWheel);
			// 
			// PanningControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.Color.Red;
			this.Controls.Add(this.openGLControl1);
			this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.Name = "PanningControl";
			this.Size = new System.Drawing.Size(1000, 1000);
			((System.ComponentModel.ISupportInitialize)(this.openGLControl1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private SharpGL.OpenGLControl openGLControl1;
	}
}
