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
		public string TurkceKarakter(string text)
		{

			text = text.Replace("İ", "\u0130");

			text = text.Replace("ı", "\u0131");

			text = text.Replace("Ş", "\u015e");

			text = text.Replace("ş", "\u015f");

			text = text.Replace("Ğ", "\u011e");

			text = text.Replace("ğ", "\u011f");

			text = text.Replace("Ö", "\u00d6");

			text = text.Replace("ö", "\u00f6");

			text = text.Replace("ç", "\u00e7");

			text = text.Replace("Ç", "\u00c7");

			text = text.Replace("ü", "\u00fc");

			text = text.Replace("Ü", "\u00dc");

			return text;
		}
		void MainFormLoad(object sender, EventArgs e)
		{
			
			//Yeni bir sütun daha  oluştur
			sütun = new DataColumn("numara");
			sütun.DataType = Type.GetType("System.Int32");
			//Sütunlara ekle
			dt.Columns.Add(sütun);
			
			sütun = new DataColumn("calisma tip");
			sütun.DataType = Type.GetType("System.String");
			//Sütunlara ekle
			dt.Columns.Add(sütun);
			
			//Yeni bir sütun daha  oluştur
			sütun = new DataColumn("miktar");
			sütun.DataType = Type.GetType("System.Double");
			//Sütunlara ekle
			dt.Columns.Add(sütun);
						//Yeni bir sütun daha  oluştur
			sütun = new DataColumn("birim");
			sütun.DataType = Type.GetType("System.String");
			//Sütunlara ekle
			dt.Columns.Add(sütun);
						//Yeni bir sütun daha  oluştur
			sütun = new DataColumn("fiyat");
			sütun.DataType = Type.GetType("System.Int32");
			//Sütunlara ekle
			dt.Columns.Add(sütun);
									//Yeni bir sütun daha  oluştur
			sütun = new DataColumn("total");
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
		//	int ucret=Convert.ToInt32(textBox1.Text);
			//	komut.Connection=baglan;
			komut.CommandText="insert into isi(ad,bTarih,ucret) values('"+c+"','"+tarih+"',0)";
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
			int ucret=0;
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
			String miktar=(textBox2.Text);
			//		komut.Connection=baglan;
			komut.CommandText="insert into miktar(iNo,cMiktar,cNo) values("+anahtar+",'"+miktar+"',"+ci+")";
			//		komut.Connection=baglan;
			l=komut.ExecuteNonQuery();
			if(l==1)MessageBox.Show("başarılı","mesaj", MessageBoxButtons.OK,MessageBoxIcon.None);
			komut.Cancel();
			komut.CommandText="select max(ID) as bas from miktar where iNo="+anahtar+"";
			rd=komut.ExecuteReader();
			while(rd.Read())
				bas=Convert.ToInt32(rd["bas"]);
			rd.Close();
			string birim="";
			string bfiyat="";
			komut.CommandText="select birim,bfiyat from calisma where cNo="+ci+"";
			rd=komut.ExecuteReader();
			while(rd.Read())
			{
				birim=rd["birim"].ToString();
				bfiyat=rd["bfiyat"].ToString();
			}
			rd.Close();
			komut.Cancel();
			dataGridView1.Rows.Add(bas.ToString(),comboBox2.SelectedItem.ToString(),Convert.ToDouble(textBox2.Text),birim,bfiyat);
			komut.CommandText="select ucret from isi where iNo="+anahtar+"";
			rd=komut.ExecuteReader();
			while(rd.Read())
			ucret=Convert.ToInt32(rd["ucret"]);
			rd.Close();
			double degeri=Convert.ToDouble(textBox2.Text);
			degeri=(int)(ucret+Convert.ToDouble(textBox2.Text)*Convert.ToInt32(bfiyat));
			komut.CommandText="update isi set ucret="+degeri+" where iNo="+anahtar+"";
			//		komut.Connection=baglan;
			l=komut.ExecuteNonQuery();
			label10.Text=degeri.ToString()+" TL";
			komut.Cancel();
			
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
						dataGridView2.Rows.Add(lp[j].ToString(),rd["bTarih"].ToString(),rd["ucret"].ToString());
					}
					rd.Close();
					
					komut.CommandText="select *from tahsil where iNo="+lp[j]+"";
					rd=komut.ExecuteReader();
					while(rd.Read())
					{
						dataGridView2.Rows.Add(lp[j].ToString(),rd["aTarih"].ToString(),"-"+(rd["kBakiye"].ToString()));
					}
					rd.Close();
					
					komut.CommandText="select m.cMiktar as mik,c.cAd as ad,c.birim as birim,c.bfiyat as fiyat " +
						"from calisma c,miktar m " +
						"where m.cNo=c.cNo and m.iNo="+lp[j]+"";
					rd=komut.ExecuteReader();
					while(rd.Read())
					{
						satır = dt.NewRow();
						satır["numara"] = Convert.ToInt32(lp[j]);
						satır["calisma tip"] = rd["ad"].ToString();
						satır["miktar"] = Convert.ToDouble(rd["mik"]);
						satır["birim"] = rd["birim"].ToString();
						satır["fiyat"] = Convert.ToInt32(rd["fiyat"]);
						satır["total"]=Convert.ToInt32(rd["fiyat"])*Convert.ToDouble(rd["mik"]);
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
				dataGridView2.Rows.Add(deger.ToString(),rd["bTarih"].ToString(),rd["ucret"].ToString());
			}
			rd.Close();
			
			komut.CommandText="select *from tahsil where iNo="+deger+"";
			rd=komut.ExecuteReader();
			while(rd.Read())
			{
				dataGridView2.Rows.Add(deger.ToString(),rd["aTarih"].ToString(),"-"+(rd["kBakiye"].ToString()));
			}
			rd.Close();
			kac=0;
					komut.CommandText="select m.cMiktar as mik,c.cAd as ad,c.birim as birim,c.bfiyat as fiyat " +
						"from calisma c,miktar m " +
						"where m.cNo=c.cNo and m.iNo="+deger+"";
					rd=komut.ExecuteReader();
					while(rd.Read())
					{
						satır = dt.NewRow();
						satır["numara"] = Convert.ToInt32(deger);
						satır["calisma tip"] = rd["ad"].ToString();
						satır["miktar"] = Convert.ToDouble(rd["mik"]);
						satır["birim"] = rd["birim"].ToString();
						satır["fiyat"] = Convert.ToInt32(rd["fiyat"]);
						satır["total"]=Convert.ToInt32(rd["fiyat"])*Convert.ToDouble(rd["mik"]);
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
				dataGridView2.Rows.Add(deger.ToString(),dateTimePicker2.Text,"-"+maas[0].ToString());
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
			Document document = new iTextSharp.text.Document();
			PdfWriter.GetInstance(document, new FileStream("cari.pdf", FileMode.Create));

			BaseFont arial = BaseFont.CreateFont("C:\\windows\\fonts\\arial.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
			Font font = new Font(arial, 12);
			if (document.IsOpen() == false)
			{

				document.Open();
				//PdfPTable table = new PdfPTable(dataGridView2.Columns.Count);
				document.Add(new Paragraph(TurkceKarakter("             "+comboBox1.Text.ToString()+" Hesabı Cari Ekstresi     ") ,font));
				document.Add(new Paragraph(TurkceKarakter("                        Tarih                        Bakiye            ") ,font));
			/*		for (int j = 1; j < dataGridView2.Columns.Count; j++)
				{
					table.AddCell(new Phrase(dataGridView2.Columns[j].HeaderText));
				}
				table.HeaderRows = 1;*/
				
				for (int i = 0; i < dataGridView2.Rows.Count; i++)
				{
					if(Convert.ToInt32(dataGridView2[2, i].Value)>0)
					document.Add(new Paragraph(TurkceKarakter("            "+dataGridView2[1, i].Value.ToString()+"            "+dataGridView2[2, i].Value.ToString()),font));
					else
					document.Add(new Paragraph(TurkceKarakter("            "+dataGridView2[1, i].Value.ToString()+"            "+dataGridView2[2, i].Value.ToString()+" alındı"),font));	
					document.Add(new Paragraph("  ",font));
					PdfPTable table = new PdfPTable(5);
					
					for (int l = 0; l < kac; l++)
					{
						
						int a=Convert.ToInt32(dataGridView2[0,i].Value);
						if(a==Convert.ToInt32(dataGrid1[l,0]) && Convert.ToInt32(dataGridView2[2,i].Value)>0)
						{
							
							for (int k =1; k < dataGrid1.VisibleColumnCount; k++)
							{
								if (dataGrid1[l,k].ToString() != null)
								{
									table.AddCell(new Phrase(dataGrid1[l,k].ToString()));
								}
							}
							
						}

					}document.Add(table);
					
				}
				
				

				
				
				document.Close();
				System.Diagnostics.Process.Start("cari.pdf");
			}
			
		}
		
		void ComboBox3SelectedIndexChanged(object sender, EventArgs e)
		{
			
		}
	}
}
