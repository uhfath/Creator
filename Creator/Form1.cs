using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Creator
{
	public partial class Form1 : Form
	{
		private string destinationDirectory;
		private string[] subDirectories = Enumerable.Empty<string>().ToArray();

		public Form1()
		{
			InitializeComponent();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			var result = folderBrowserDialog1.ShowDialog();
			if (result == DialogResult.OK)
			{
				textBox2.Text = folderBrowserDialog1.SelectedPath;
			}
		}

		private void button3_Click(object sender, EventArgs e)
		{
			textBox1.Clear();
		}

		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
			var worker = sender as BackgroundWorker;
			for (var i = 0; i < subDirectories.Length && !worker.CancellationPending; i++)
			{
				var destination = Path.Combine(destinationDirectory, subDirectories[i]);

				try
				{
					Directory.CreateDirectory(destination);
					worker.ReportProgress((int)((decimal)(i + 1) / (decimal)subDirectories.Length * 100.0M));
				}
				catch (Exception ex)
				{
					var result = MessageBox.Show($"Ошибка при создании папки. Прервать процесс?{Environment.NewLine}{ex.Message}", "Ошибка создания", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
					if (result == DialogResult.Yes)
					{
						worker.CancelAsync();
						break;
					}
				}
			}
		}

		private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			progressBar1.Value = e.ProgressPercentage;
		}

		private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			MessageBox.Show("Создание папок завершён", "Процесс завершён", MessageBoxButtons.OK, MessageBoxIcon.Information);
			button2.Text = "Начать";
			progressBar1.Value = 0;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (!backgroundWorker1.IsBusy)
			{
				if (!Directory.Exists(textBox2.Text))
				{
					MessageBox.Show("Некорректный путь к папке назначения!", "Ошибка папки назначения", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else
				{
					button2.Text = "Отменить";

					destinationDirectory = textBox2.Text;
					subDirectories = textBox1.Lines
						.Select(l => l.Trim())
						.Where(l => l.Length > 0)
						.OrderBy(l => l).ToArray();

					backgroundWorker1.RunWorkerAsync();
				}
			}
			else
			{
				backgroundWorker1.CancelAsync();
			}
		}
	}
}
