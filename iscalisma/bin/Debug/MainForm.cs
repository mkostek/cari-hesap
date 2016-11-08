/*
 * Created by SharpDevelop.
 * User: Mert
 * Date: 13.09.2015
 * Time: 12:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Windows.Forms;

using iTextSharp.text;
using iTextSharp.text.pdf;

namespace iscalisma
{

	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		int kac=0;
		int l=0;
		int anahtar=0;
		OleDbConnection baglan;
		OleDbCommand komut=new OleDbCommand();
		OleDbDataReader rd;
		DataTable dt = new DataTable();
   		DataColumn sütun;
   		DataRow satır ;
   		int deger;
   		int tarih;
		void MainFormLoad(object sender, EventArgs e)
		{
			
			//Yeni bir sütun daha  oluştur
			sütun = new DataColumn("calisma tip");
			sütun.DataType = Type.GetType("System.String");
			//Sütunlara ekle
			dt.Columns.Add(sütun);
			
			//Yeni bir sütun daha  oluştur
			sütun = new DataColumn("miktar");
			sütun.DataType = Type.GetType("System.Int32");
			//Sütunlara ekle
			dt.Columns.Add(sütun);
			
			//dataGrid kontrolünde oluşturduğumuz tabloyu göster
			dataGrid1.DataSource = dt;
			
			
			DateTime t=DateTime.Today;
			tarih=t.Month;
			tarih-=2;
			MessageBox.Show(tarih.ToString());
			int[] gt={0,0};
			string[] yazılacak=new string[150];
			int hu=0;
			int hy=0;
			string ad;
			baglan= new OleDbConnection("provider=Microsoft.jet.oledb.4.0; Data Source=is.mdb;");
			baglan.Open();
			komut.Connection=baglan;
			komut.CommandType=CommandType.Text;
			komut.CommandText="select distinct(ad) as ad from isi";
		   	rd=komut.ExecuteReader();
		   	while(rd.Read())
		   	{
		   		comboBox3.Items.Add(rd["ad"].ToString());
		   		//comboBox3.ValueMember=rd["i"]
		   		//comboBox1.Items.Add(rd["ad"].ToString());
		   		yazılacak[hu++]=rd["ad"].ToString();
		   	}
		   	rd.Close();
			
		   	
		   	//
		   	
		   	
		   	for(int i=0;i<hu;i++)
		   	{
		   		gt=new int[]{0,0};
		   		hy=0;
		   		komut.CommandText = string.Format("SELECT ucret FROM isi WHERE isi.ad='{0}' ", yazılacak[i]);
		   		rd=komut.ExecuteReader();
		   	//	komut.ResetCommandTimeout();
		   		while(rd.Read())
		   		{
		   			gt[0] +=(int)rd["ucret"];
		   		}
				rd.Close();
				
				komut.CommandText = string.Format("SELECT tahsil.kBakiye as deger  FROM tahsil,isi WHERE isi.iNo=tahsil.iNo and isi.ad='{0}'", yazılacak[i]);
		   		rd=komut.ExecuteReader();
		   	//	komut.ResetCommandTimeout();
		   		while(rd.Read())
		   		{
		   			gt[1] += (int)rd["deger"];
		   		}
				rd.Close();
				
				if(gt[0]>gt[1])
		   		{
		   			comboBox1.Items.Add(yazılacak[i]);
		   		}
		   	}		   		
		   	
		   	
		   	//
		   komut.CommandText="select cAd from calisma";
		   	rd=komut.ExecuteReader();
		   	while(rd.Read())
		   		comboBox2.Items.Add( rd["cAd"].ToString());
		   	rd.Close();
		   	komut.Cancel();
		   	
		   	
		}

		void ListBox1SelectedIndexChanged(object sender, EventArgs e)
		{
	
		}
		void Button1Click(object sender, EventArgs e)
		{
			dataGridView1.Rows.Clear();
			string c=comboBox3.Text;
			string tarih=dateTimePicker1.Text;
			int ucret=Convert.ToInt32(textBox1.Text);
		//	komut.Connection=baglan;
			komut.CommandText="insert into isi(ad,bTarih,ucret) values('"+c+"','"+tarih+"',"+ucret+")";
		//	komut.Connection=baglan;
			l=komut.ExecuteNonQuery();
			if(l==1)MessageBox.Show("kayıt başarı ile eklendi...\n sarfiyatları ekleyiniz","mesaj", MessageBoxButtons.OK,MessageBoxIcon.None);
			komut.Cancel();
			komut.CommandText="select max(iNo) as ds from isi ";
			rd=komut.ExecuteReader();
			while(rd.Read())
				anahtar=Convert.ToInt32(rd["ds"]);
			rd.Close();
			komut.CommandText="insert into tahsil(iNo,aTarih,kBakiye) values("+anahtar+",'"+tarih+"',0)";
		}
		void Button2Click(object sender, EventArgs e)
		{
			l=0;
			int bas=0;
			int ci=0;
			//listBox1.Items.Add("")
			komut.CommandText="select cNo from calisma where cAd='"+comboBox2.SelectedItem+"'";
//			komut.Connection=baglan;
			rd=komut.ExecuteReader();
			while(rd.Read())
				ci=Convert.ToInt32(rd["cNo"]);
			rd.Close();
			komut.Cancel();
			int miktar=Convert.ToInt32(textBox2.Text);
	//		komut.Connection=baglan;
			komut.CommandText="insert into miktar(iNo,cMiktar,cNo) values("+anahtar+","+miktar+","+ci+")";
	//		komut.Connection=baglan;
			l=komut.ExecuteNonQuery();
			if(l==1)MessageBox.Show("başarılı","mesaj", MessageBoxButtons.OK,MessageBoxIcon.None);
			komut.Cancel();
			komut.CommandText="select max(ID) as bas from miktar where iNo="+anahtar+"";
			rd=komut.ExecuteReader();
			while(rd.Read())
				bas=Convert.ToInt32(rd["bas"]);
			rd.Close();
			komut.Cancel();
			dataGridView1.Rows.Add(bas.ToString(),comboBox2.SelectedItem.ToString(),Convert.ToInt32(textBox2.Text));
		}
		void DataGridView1CellClick(object sender, DataGridViewCellEventArgs e)
		{
			int deger;
			int numara;
			 if (e.ColumnIndex == 0)
   			 {// hangi kolona göre işlem yapacaksak onun index i ile karşılaştırıyoruz
			 	deger=Convert.ToInt32(dataGridView1.CurrentCell.Value);
			 	object a=MessageBox.Show(deger+" nolu kaydı siliyorsunuz...","Uyarı",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
			 	if(Convert.ToInt16(a)==6)
			 	{
			 		dataGridView1.Rows.RemoveAt((e.RowIndex));
			// 		komut.Connection=baglan;
			 		komut.CommandText="delete from miktar where ID="+deger+"";
			 		//komut.ExecuteReader();
			 		if(1==komut.ExecuteNonQuery())
			 		{
			 			MessageBox.Show(deger+" nolu kayıt başarı ile silindi...","mesaj",MessageBoxButtons.OK,MessageBoxIcon.Information);
			 		}
			 		komut.Cancel();
			 	}
			 }
		}		
	
	

		void ComboBox1SelectedIndexChanged(object sender, EventArgs e)
		{
			dt.Clear();
			dataGridView2.Rows.Clear();
				int i=0;
			// disable once SuggestUseVarKeywordEvident
			int[] k={0,0};
			// disable once ConvertToConstant.Local
			int[] lp=new int[50];
			comboBox4.Text=" ";
			comboBox4.Items.Clear();
			komut.CommandText = string.Format("select iNo from isi where ad like '%{0}%' order by bTarih asc", comboBox1.Text);
			komut.Connection=baglan;
			rd=komut.ExecuteReader();
			
			while(rd.Read())
			{
				lp[i]=(int)(rd["iNo"]);
				i+=1;
			}
			rd.Close();
			kac=0;
			for(int j=0;j<i;j++)
			{
				k=new int[]{0,0};
				komut.CommandText="select ucret from isi where iNo="+lp[j]+"";
				rd=komut.ExecuteReader();
				while(rd.Read())
				{
					k[0] = (int)rd["ucret"];
				}
				rd.Close();
				komut.CommandText="select kBakiye as e from tahsil where iNo="+lp[j]+"";
				rd=komut.ExecuteReader();
				while(rd.Read())
				{
					k[1]+= (int)rd["e"];

				}
				rd.Close();
				if(k[0]>k[1])
				{
					comboBox4.Items.Add(lp[j].ToString());
					
				//	dt.Clear();
					
					//deger=Convert.ToInt32(comboBox4.SelectedItem);
					komut.CommandText="select *from isi where iNo="+lp[j]+"";
					rd=komut.ExecuteReader();
					while(rd.Read())
					{
						dataGridView2.Rows.Add(rd["bTarih"].ToString(),rd["ucret"].ToString());
					}
					rd.Close();
					
					komut.CommandText="select *from tahsil where iNo="+lp[j]+"";
					rd=komut.ExecuteReader();
					while(rd.Read())
					{
						dataGridView2.Rows.Add(rd["aTarih"].ToString(),"-"+(rd["kBakiye"].ToString()));
					}
					rd.Close();
					
					komut.CommandText="select m.cMiktar as mik,c.cAd as ad " +
						"from calisma c,miktar m " +
						"where m.cNo=c.cNo and m.iNo="+lp[j]+"";
					rd=komut.ExecuteReader();
					while(rd.Read())
					{
						satır = dt.NewRow();
						satır["calisma tip"] = rd["ad"].ToString();
						satır["miktar"] = Convert.ToInt32(rd["mik"]);
						//Veri tablomuza kontrolüne ekle
						dt.Rows.Add(satır);
						kac++;
					}
					rd.Close();
				}
			}
			
			
		}
		void GroupBox4Enter(object sender, EventArgs e)
		{
	
		}



		void ComboBox4SelectedIndexChanged(object sender, EventArgs e)
		{
//			for(int sil=1;sil<	dataGridView2.Rows.Count+1 ;sil++)
//			{
//				dataGridView2.Rows.RemoveAt(sil);
//			}
			dataGridView2.Rows.Clear();
			dt.Clear();
			
			deger=Convert.ToInt32(comboBox4.SelectedItem);
			komut.CommandText="select *from isi where iNo="+deger+"";
			rd=komut.ExecuteReader();
			while(rd.Read())
			{
				dataGridView2.Rows.Add(rd["bTarih"].ToString(),rd["ucret"].ToString());
			}
			rd.Close();
			
			komut.CommandText="select *from tahsil where iNo="+deger+"";
			rd=komut.ExecuteReader();
			while(rd.Read())
			{
				dataGridView2.Rows.Add(rd["aTarih"].ToString(),"-"+(rd["kBakiye"].ToString()));
			}
			rd.Close();
			kac=0;
			komut.CommandText="select m.cMiktar as mik,c.cAd as ad " +
				"from calisma c,miktar m " +
				"where m.cNo=c.cNo and m.iNo="+deger+"";
			rd=komut.ExecuteReader();
			while(rd.Read())
			{
			satır = dt.NewRow();
			satır["calisma tip"] = rd["ad"].ToString();
			satır["miktar"] = Convert.ToInt32(rd["mik"]);
			//Veri tablomuza kontrolüne ekle
			dt.Rows.Add(satır);
			kac++;
			}
			rd.Close();
		}
		void ComboBox1Leave(object sender, EventArgs e)
		{
					

			
		}
		void Button4Click(object sender, EventArgs e)
		{
			int h=0;
			int[] maas={0,0,0};
			maas[0]=Convert.ToInt32(textBox3.Text);
			komut.CommandText="select *from isi where iNo="+deger+"";
			rd=komut.ExecuteReader();
			while(rd.Read())
			{
				maas[1]=Convert.ToInt32(rd["ucret"]);
			}
			rd.Close();
			komut.CommandText="select kBakiye from tahsil where iNo="+deger+"";
			rd=komut.ExecuteReader();
			while(rd.Read())
			{
				maas[2]+=(int)rd["kBakiye"];
			}
			rd.Close();
			if(maas[0]<=maas[1]&&(maas[2]+maas[0])<=maas[1])
			{
				dataGridView2.Rows.Add(dateTimePicker2.Text,"-"+maas[0].ToString());
				komut.CommandText="insert into tahsil(iNo,aTarih,kBakiye)" +
					"values("+deger+",'"+dateTimePicker2.Text+"',"+maas[0]+")";
				if(1==komut.ExecuteNonQuery())
				{
					MessageBox.Show("kayıt başarı ile işlendi...","başarılı",MessageBoxButtons.OK,MessageBoxIcon.Information);
				}
			}
			else 
			{
				MessageBox.Show("Ücretin üzerinde tahsilat","hata",MessageBoxButtons.AbortRetryIgnore,MessageBoxIcon.Error);
			}
			
			
			
		

		}
		void TakipClick(object sender, EventArgs e)
		{
			comboBox1.Items.Clear();
			comboBox3.Items.Clear();
			int[] gt={0,0};
			string[] yazılacak=new string[150];
			int hu=0;
			int hy=0;
			komut.CommandText="select distinct(ad) as ad from isi";
		   	rd=komut.ExecuteReader();
		   	while(rd.Read())
		   	{
		   		comboBox3.Items.Add(rd["ad"].ToString());
		   		//comboBox1.Items.Add(rd["ad"].ToString());
		   		yazılacak[hu++]=rd["ad"].ToString();
		   	}
		   	rd.Close();
			
		   	
		   	//
		   	
		   	
		   	for(int i=0;i<hu;i++)
		   	{
		   		gt=new int[]{0,0};
		   		hy=0;
		   		komut.CommandText = string.Format("SELECT ucret as deger  FROM isi WHERE isi.ad='{0}'", yazılacak[i]);
		   		rd=komut.ExecuteReader();
		   	//	komut.ResetCommandTimeout();
		   		while(rd.Read())
		   		{
		   			gt[0] += (int)rd["deger"];
		   		}
				rd.Close();
				
				komut.CommandText = string.Format("SELECT tahsil.kBakiye as deger  FROM tahsil,isi WHERE isi.iNo=tahsil.iNo and isi.ad='{0}'", yazılacak[i]);
		   		rd=komut.ExecuteReader();
		   	//	komut.ResetCommandTimeout();
		   		while(rd.Read())
		   		{
		   			gt[1] += (int)rd["deger"];
		   		}
				rd.Close();
				
				if(gt[0]-gt[1]>0)
		   		{
		   			comboBox1.Items.Add(yazılacak[i]);
		   		}
		   	}		   		
		}


		
		void Button3Click(object sender, EventArgs e)
		{
			Document document = new Document(PageSize.A4, 10, 10, 50, 10);
			Random r=new Random();
			int x=r.Next(0,100);
			PdfWriter pw = PdfWriter.GetInstance(document, new FileStream(x.ToString()+".pdf", FileMode.Create));
			

			
			document.Open();
			PdfPTable table = new PdfPTable(dataGridView2.Columns.Count);
			for (int j = 0; j < dataGridView2.Columns.Count; j++)
			{
				table.AddCell(new Phrase(dataGridView2.Columns[j].HeaderText));
			}
			table.HeaderRows = 1;
			for (int i = 0; i < dataGridView2.Rows.Count; i++)
			{
				for (int k = 0; k < dataGridView2.Columns.Count; k++)
				{
					if (dataGridView2[k, i].Value != null)
					{
						table.AddCell(new Phrase(dataGridView2[k, i].Value.ToString()));
					}
				}
			}
			document.Add(table);
			Paragraph p1=new Paragraph("                   " +
			                           "                   ");
			document.Add(p1);
			table = new PdfPTable(2);
			//table.HeaderRows = 0;
			table.AddCell(new Phrase("calismaTip"));
			table.AddCell(new Phrase("miktar"));
			for (int i = 0; i < kac; i++)
			{
				for (int k =0; k < dataGrid1.VisibleColumnCount; k++)
				{
					if (dataGrid1[i,k].ToString() != null)
					{
						table.AddCell(new Phrase(dataGrid1[i,k].ToString()));
					}
				}
			}
			document.Add(table);
			
			document.Close();
			pw.Close();
			System.Diagnostics.Process.Start(x+".pdf");
		}
	}
}
