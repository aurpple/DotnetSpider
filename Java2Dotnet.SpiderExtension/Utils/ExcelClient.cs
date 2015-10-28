//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using DocumentFormat.OpenXml.Packaging;
//using DocumentFormat.OpenXml.Spreadsheet;
//using Java2Dotnet.Spider.Lib;

//namespace Java2Dotnet.Spider.Extension.Utils
//{
//	public class ExcelVision
//	{
//		public static string Postfix97 = "XLS";

//		public static string Postfix03 = "XLSX";
//	}

//	//http://simpleooxml.codeplex.com/
//	//http://www.pin5i.com/showtopic-21817.html
//	//http://blog.csdn.net/lbj147123/article/details/6603942
//	public class OpenXmlSdkExporter
//	{
//		private static string[] Level = {"A", "B", "C", "D", "E", "F", "G",
//	"H", "I", "G", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T",
//	"U", "V", "W", "X", "Y", "Z" };

//		public static List<DataTable> Import(string path)
//		{
//			List<DataTable> tables = new List<DataTable>();

//			if (path.EndsWith(ExcelVision.POSTFIX_SVN))
//				return tables;

//			using (MemoryStream stream = SpreadsheetReader.StreamFromFile(path))
//			{
//				using (SpreadsheetDocument doc = SpreadsheetDocument.Open(stream, true))
//				{
//					foreach (Sheet sheet in doc.WorkbookPart.Workbook.Descendants<Sheet>())
//					{
//						DataTable table = new DataTable(sheet.Name.Value);

//						WorksheetPart worksheet = (WorksheetPart)doc.WorkbookPart.GetPartById(sheet.Id);

//						List<string> columnsNames = new List<string>();

//						foreach (Row row in worksheet.Worksheet.Descendants<Row>())
//						{
//							foreach (Cell cell in row)
//							{
//								string columnName = Regex.Match(cell.CellReference.Value, "[a-zA-Z]+").Value;

//								if (!columnsNames.Contains(columnName))
//								{
//									columnsNames.Add(columnName);
//								}

//							}
//						}

//						columnsNames.Sort(CompareColumn);

//						foreach (string columnName in columnsNames)
//						{
//							table.Columns.Add(columnName);
//						}

//						foreach (Row row in worksheet.Worksheet.Descendants<Row>())
//						{
//							DataRow tableRow = table.NewRow();
//							table.Rows.Add(tableRow);

//							foreach (Cell cell in row)
//							{
//								string columnName = Regex.Match(cell.CellReference.Value, "[a-zA-Z]+").Value;
//								tableRow[columnName] = GetValue(cell, doc.WorkbookPart.SharedStringTablePart);
//							}
//						}

//						if (table.Rows.Count <= 0)
//							continue;
//						if (table.Columns.Count <= 0)
//							continue;

//						tables.Add(table);
//					}
//				}
//			}

//			return tables;
//		}

//		public static String GetValue(Cell cell, SharedStringTablePart stringTablePart)
//		{

//			if (cell.ChildElements.Count == 0)

//				return null;

//			//get cell value

//			String value = cell.CellValue.InnerText;

//			//Look up real value from shared string table

//			if ((cell.DataType != null) && (cell.DataType == CellValues.SharedString))

//				value = stringTablePart.SharedStringTable

//				.ChildElements[Int32.Parse(value)]

//				.InnerText;

//			return value;

//		}


//		public static void Export(string path, List<DataTable> tables)
//		{
//			using (MemoryStream stream = SpreadsheetReader.Create())
//			{
//				using (SpreadsheetDocument doc = SpreadsheetDocument.Open(stream, true))
//				{
//					SpreadsheetWriter.RemoveWorksheet(doc, "Sheet1");
//					SpreadsheetWriter.RemoveWorksheet(doc, "Sheet2");
//					SpreadsheetWriter.RemoveWorksheet(doc, "Sheet3");

//					foreach (DataTable table in tables)
//					{
//						WorksheetPart sheet = SpreadsheetWriter.InsertWorksheet(doc, table.TableName);
//						WorksheetWriter writer = new WorksheetWriter(doc, sheet);

//						SpreadsheetStyle style = SpreadsheetStyle.GetDefault(doc);

//						foreach (DataRow row in table.Rows)
//						{
//							for (int i = 0; i < table.Columns.Count; i++)
//							{
//								string columnName = SpreadsheetReader.GetColumnName("A", i);
//								string location = columnName + (table.Rows.IndexOf(row) + 1);
//								writer.PasteText(location, row[i].ToString(), style);
//							}
//						}

//						writer.Save();
//					}
//					SpreadsheetWriter.StreamToFile(path, stream);//保存到文件中
//				}
//			}
//		}

//		private static int CompareColumn(string x, string y)
//		{
//			int xIndex = Letter_to_num(x);
//			int yIndex = Letter_to_num(y);
//			return xIndex.CompareTo(yIndex);
//		}

//		/// <summary>
//		/// 数字26进制，转换成字母，用递归算法
//		/// </summary>
//		/// <param name="value"></param>
//		/// <returns></returns>
//		private static string Num_to_letter(int value)
//		{
//			//此处判断输入的是否是正确的数字，略（正在表达式判断）
//			int remainder = value % 26;
//			//remainder = (remainder == 0) ? 26 : remainder;
//			int front = (value - remainder) / 26;
//			if (front < 26)
//			{
//				return Level[front - 1] + Level[remainder];
//			}
//			else
//			{
//				return Num_to_letter(front) + Level[remainder];
//			}
//			//return "";
//		}

//		/// <summary>
//		/// 26进制字母转换成数字
//		/// </summary>
//		/// <param name="letter"></param>
//		/// <returns></returns>
//		private static int Letter_to_num(string str)
//		{
//			//此处判断是否是由A-Z字母组成的字符串，略（正在表达式片段）
//			char[] letter = str.ToCharArray(); //拆分字符串
//			int reNum = 0;
//			int power = 1; //用于次方算值
//			int times = 1;  //最高位需要加1
//			int num = letter.Length;//得到字符串个数
//									//得到最后一个字母的尾数值
//			reNum += Char_num(letter[num - 1]);
//			//得到除最后一个字母的所以值,多于两位才执行这个函数
//			if (num >= 2)
//			{
//				for (int i = num - 1; i > 0; i--)
//				{
//					power = 1;//致1，用于下一次循环使用次方计算
//					for (int j = 0; j < i; j++)           //幂，j次方，应该有函数
//					{
//						power *= 26;
//					}
//					reNum += (power * (Char_num(letter[num - i - 1]) + times));  //最高位需要加1，中间位数不需要加一
//					times = 0;
//				}
//			}
//			//Console.WriteLine(letter.Length);
//			return reNum;
//		}

//		/// <summary>
//		/// 输入字符得到相应的数字，这是最笨的方法，还可用ASIICK编码；
//		/// </summary>
//		/// <param name="ch"></param>
//		/// <returns></returns>
//		private static int Char_num(char ch)
//		{
//			switch (ch)
//			{
//				case 'A':
//					return 0;
//				case 'B':
//					return 1;
//				case 'C':
//					return 2;
//				case 'D':
//					return 3;
//				case 'E':
//					return 4;
//				case 'F':
//					return 5;
//				case 'G':
//					return 6;
//				case 'H':
//					return 7;
//				case 'I':
//					return 8;
//				case 'J':
//					return 9;
//				case 'K':
//					return 10;
//				case 'L':
//					return 11;
//				case 'M':
//					return 12;
//				case 'N':
//					return 13;
//				case 'O':
//					return 14;
//				case 'P':
//					return 15;
//				case 'Q':
//					return 16;
//				case 'R':
//					return 17;
//				case 'S':
//					return 18;
//				case 'T':
//					return 19;
//				case 'U':
//					return 20;
//				case 'V':
//					return 21;
//				case 'W':
//					return 22;
//				case 'X':
//					return 23;
//				case 'Y':
//					return 24;
//				case 'Z':
//					return 25;
//			}
//			return -1;
//		}
//	}
//}
