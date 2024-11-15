using MySql.Data;
using MySql.Data.MySqlClient;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace SOLIDWinFormsPDF
{
    public partial class Form1 : Form
    {
        private readonly IDateBase _datebase;
        private readonly IPDFReader _pDFReader;

        public Form1(string connectionString, IPDFReader pdfReader) 
        {
            _datebase = new MySql(connectionString);
            _pDFReader = pdfReader;
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string search = textBox1.Text;

            if(search != null && int.TryParse(search, out int numberPDF)) 
            {
                Result(numberPDF);
            }
            else 
            {
                textBox2.Clear();
            }
        }

        private void Result(int numberPDF) 
        {
            byte[] filePDF = _datebase.GetFile(numberPDF);

            if (filePDF != null) 
            {
                string textFile = _pDFReader.Read(filePDF);
                textBox2.Text = textFile;
            }
            else 
            {
                textBox2.Text = "Not Found";
            }
        }
    }


    public interface IPDFReader 
    {
        string Read(byte[] filePDF);
    }

    public class PRead : IPDFReader 
    {
        public string Read(byte[] filePDF) 
        {
            StringBuilder text = new StringBuilder();

            try
            {
                using(PdfReader reader = new PdfReader(filePDF)) 
                {
                    for (int i = 1; i <= reader.NumberOfPages; i++) 
                    {
                        text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                    }
                }
            }
            catch (Exception ex)
            {
                text.AppendLine($"Error: {ex}");
            }

            return text.ToString();
        }
    }

    // База данных
    public interface IDateBase 
    {
        byte[] GetFile(int numberPDF);
    }

    public class MySql : IDateBase 
    {
        private readonly string _connectionString;

        public MySql(string connectionString) 
        {
            _connectionString = connectionString;
        }

        public byte[] GetFile(int numberPDF) 
        {
            using(MySqlConnection connection = new MySqlConnection(_connectionString)) 
            {
                connection.Open();
                string query = "SELECT Path FROM info WHERE Name = @Number";

                using (MySqlCommand command = new MySqlCommand(query, connection)) 
                {
                    command.Parameters.AddWithValue("@Number", numberPDF);
                    using(MySqlDataReader reader = command.ExecuteReader()) 
                    {
                        if (reader.HasRows) 
                        {
                            reader.Read();
                            return (byte[])reader["Path"];
                        }
                    }
                }
            }
            return null;
        }
    }
}
