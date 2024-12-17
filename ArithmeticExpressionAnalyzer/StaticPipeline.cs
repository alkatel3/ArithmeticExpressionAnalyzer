using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArithmeticExpressionAnalyzer
{
    public static class StaticPipeline
    {
        public static readonly int LayersCount = 3;

        public static List<PipelineNode> EvaluateTree(Node root)
        {
            var pipelineRoot = new PipelineNode(root);
            //pipelineRoot.DisplayNode();
            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine();
            // Отримуємо вузли за рівнями
            var levels = PipelineNode.GetNodesByLevels(pipelineRoot);
            var groupedlevels = new List<List<PipelineNode>>();

            foreach (var level in levels)
            {
                groupedlevels.AddRange(GetLevels(level));
            }

            return loadStaticPipeline(groupedlevels);
        }

        private static List<PipelineNode> loadStaticPipeline(List<List<PipelineNode>> leaves)
        {
            var res = new List<PipelineNode>();

            for(var i = 0; i< leaves.Count; i++)
            {
                for(var j = 0; j < leaves[i].Count; j++)
                {
                    var temp = leaves[i][j];

                    if (res.Count == 0)
                    {
                        temp.InputPutRow = 1;
                    }
                    else
                    {
                        var lastLoaded = res.Last();
                        var leftOut = -1;
                        var rightOut = -1;
                        var lastOut = -1;

                        if (temp.Left is not null)
                        {
                            if (!temp.Left.IsLoaded)
                                throw new Exception();

                            leftOut = temp.Left.OutPutRow+1;
                        }

                        if (temp.Right is not null)
                        {
                            if (!temp.Right.IsLoaded)
                                throw new Exception();

                            rightOut = temp.Right.OutPutRow+1;
                        }

                        if(lastLoaded.Value == temp.Value && lastLoaded.Level == temp.Level)
                            lastOut = lastLoaded.InputPutRow + temp.Duration;
                        else
                            lastOut = lastLoaded.OutPutRow - 1;

                        temp.InputPutRow = new List<int> { leftOut, rightOut, lastOut }.Max();
                    }

                    temp.IsLoaded = true;
                    temp.OutPutRow = temp.InputPutRow + temp.Duration * LayersCount + 1;
                    res.Add(temp);
                }
            }

            return res;
        }

        private static List<List<PipelineNode>> GetLevels(List<PipelineNode> level)
        {
            var operations = new List<string> { "*", "/", "-", "+" };
            var res = new List<List<PipelineNode>>();

            foreach (var operation in operations)
            {
                if (level.Any(l => l.Value == operation))
                {
                    var temp = level.Where(l => l.Value == operation).ToList();
                    res.Add(temp);
                }
            }

            return res;
        }

        internal static void Display(List<PipelineNode> PipelineNodes, int startCol = 0, bool rewriteExcel = false)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                // Створення листа
                var worksheet = package.Workbook.Worksheets.Add("Table");

                // Заповнення заголовків
                worksheet.Cells[1, startCol+1].Value = "T";
                worksheet.Cells[1, startCol + 2].Value = "Input";
                for (int i = 1; i <= LayersCount; i++)
                {
                    worksheet.Cells[1, startCol + 2 + i].Value = "S" + i;
                }
                worksheet.Cells[1, startCol + LayersCount + 3].Value = "Output";

                // Форматування заголовків
                using (var range = worksheet.Cells[1, 1, 1, LayersCount + 3])
                {
                    range.Style.Font.Bold = true;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                // Додавання даних
                string[,] res = GetMatrixOfPipelineNodes(PipelineNodes);

                for(var i =0; i<res.GetLength(0); i++)
                {
                    worksheet.Cells[i + 2, startCol + 1].Value = i + 1;
                    worksheet.Cells[i + 2, startCol + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    for (var j = 0; j < res.GetLength(1); j++)
                    {
                        var cell = worksheet.Cells[i + 2, startCol + j + 2];

                        if(j == 0 || j == res.GetLength(1)-1)
                        {
                            cell.Value = res[i, j];
                            continue;
                        }

                        if (!string.IsNullOrEmpty(res[i, j]))
                        {
                            cell.Value = res[i, j];
                            cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen); // Салатовий колір
                        }

                        cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }
                }

                // Автоматичне налаштування ширини колонок
                worksheet.Cells.AutoFitColumns();

                try
                {
                    // Збереження файлу
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Table.xlsx");
                    File.WriteAllBytes(filePath, package.GetAsByteArray());
                    Console.WriteLine("Дані збережені в таблицю, відкрийте її");
                }
                catch
                {
                    Console.WriteLine("Помилка при збереженні таблиці! Закрийте таблицю та спробуйте ще раз");
                }
            }
        }

        private static string[,] GetMatrixOfPipelineNodes(List<PipelineNode> pipelineNodes)
        {
            var res = new string[pipelineNodes.Last().OutPutRow, LayersCount + 2];

            for (var a = 0; a < pipelineNodes.Count; a++)
            {
                var item = pipelineNodes[a];
                res[item.InputPutRow-1, 0] = $"[{a}] =>";
                for (int i = 1; i <= LayersCount; i++)
                {
                    for (var j = 1; j <= item.Duration; j++)
                    {
                        res[item.InputPutRow + j + (i - 1) * item.Duration-1, i] = item.Value + $" [{a}]";
                    }
                }
                res[item.OutPutRow-1, LayersCount + 1] = "=>" + $" [{a}]";
            }

            return res;
        }
    }

}
