using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using TaskManager.Model;

namespace TaskManager.Infrastructure.OfficeDocument
{
    public class ExcelDocumentReport
    {
        // ================= TASKS REPORT =================

        public XLWorkbook BuildTasksReport(List<Model.Task> tasks)
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Задачи");

            int currentRow = 1;

            // ===== MAIN HEADER =====
            AddMainHeader(ws, ref currentRow);

            string[] headers =
            {
                "Название",
                "Статус",
                "Категория",
                "Начало",
                "Срок",
                "Создал"
            };

            // ===== TABLE HEADER =====
            for (int i = 0; i < headers.Length; i++)
                ws.Cell(currentRow, i + 1).Value = headers[i];

            var headerRow = ws.Row(currentRow);

            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#2F5597");
            headerRow.Style.Font.FontColor = XLColor.White;
            headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRow.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            currentRow++;

            // ===== DATA =====
            foreach (var t in tasks)
            {
                ws.Cell(currentRow, 1).Value = t.Title;
                ws.Cell(currentRow, 2).Value = t.Status?.Name;
                ws.Cell(currentRow, 3).Value = t.Scope?.Name;
                ws.Cell(currentRow, 4).Value = t.Since.ToString("dd.MM.yyyy");
                ws.Cell(currentRow, 5).Value = t.Deadline.ToString("dd.MM.yyyy");
                ws.Cell(currentRow, 6).Value = t.Owner?.Lname;

                ApplyRowStyle(ws, currentRow, t);

                currentRow++;
            }

            // ===== BORDER ONLY USED RANGE =====
            var usedRange = ws.RangeUsed();

            if (usedRange != null)
            {
                usedRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                usedRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            }

            ws.Columns().AdjustToContents();

            // ===== FOOTER =====
            AddFooter(ws, currentRow + 2);

            return wb;
        }

        // ================= USERS REPORT =================

        public XLWorkbook BuildUsersReport(List<User> users)
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Сотрудники");

            int currentRow = 1;

            // ===== MAIN HEADER =====
            AddMainHeader(ws, ref currentRow);

            string[] headers =
            {
                "Фамилия",
                "Зоны ответственности"
            };

            // ===== TABLE HEADER =====
            for (int i = 0; i < headers.Length; i++)
                ws.Cell(currentRow, i + 1).Value = headers[i];

            var header = ws.Row(currentRow);

            header.Style.Font.Bold = true;
            header.Style.Fill.BackgroundColor = XLColor.FromHtml("#2F5597");
            header.Style.Font.FontColor = XLColor.White;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            header.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            currentRow++;

            // ===== DATA =====
            foreach (var u in users)
            {
                ws.Cell(currentRow, 1).Value = u.Lname ?? "—";

                var scopes = u.Scopes != null && u.Scopes.Any()
                    ? string.Join(", ", u.Scopes.Select(s => s.Name))
                    : "Нет зон";

                ws.Cell(currentRow, 2).Value = scopes;

                ApplyUserRowStyle(ws, currentRow);

                currentRow++;
            }

            // ===== BORDER ONLY USED RANGE =====
            var usedRange = ws.RangeUsed();

            if (usedRange != null)
            {
                usedRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                usedRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            }

            ws.Columns().AdjustToContents();
            ws.Column(2).Width = 40;

            // ===== FOOTER =====
            AddFooter(ws, currentRow + 2);

            return wb;
        }

        // ================= MAIN HEADER =================

        private void AddMainHeader(IXLWorksheet ws, ref int currentRow)
        {
            // Цех Печенька
            ws.Cell(currentRow, 1).Value = "OOO \"Печенька\" ";

            ws.Range(currentRow, 1, currentRow, 6).Merge();

            var titleCell = ws.Cell(currentRow, 1);

            titleCell.Style.Font.Bold = true;
            titleCell.Style.Font.FontSize = 20;
            titleCell.Style.Font.FontColor = XLColor.FromHtml("#2F5597");
            titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            currentRow++;

            // ИНН
            ws.Cell(currentRow, 1).Value = "ИНН 190005211";

            ws.Range(currentRow, 1, currentRow, 6).Merge();

            var innCell = ws.Cell(currentRow, 1);

            innCell.Style.Font.FontSize = 12;
            innCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            currentRow += 2;
        }

        // ================= TASK ROW STYLE =================

        private void ApplyRowStyle(IXLWorksheet ws, int row, Model.Task t)
        {
            var r = ws.Row(row);

            bool overdue = t.Deadline < DateTime.Now &&
                           t.Status?.Name?.ToLower() != "завершено";

            if (overdue)
                r.Style.Fill.BackgroundColor = XLColor.OrangeRed;
            else if (t.Status?.Name?.ToLower() == "завершено")
                r.Style.Fill.BackgroundColor = XLColor.LightGreen;
            else if (t.IdUserTaked > 0)
                r.Style.Fill.BackgroundColor = XLColor.LightBlue;
        }

        // ================= USER ROW STYLE =================

        private void ApplyUserRowStyle(IXLWorksheet ws, int row)
        {
            var r = ws.Row(row);

            r.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            r.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        }

        // ================= FOOTER =================

        private void AddFooter(IXLWorksheet ws, int startRow)
        {
            ws.Cell(startRow, 1).Value =
                $"Документ создан: {DateTime.Now:dd.MM.yyyy HH:mm}";

            ws.Range(startRow, 1, startRow, 3).Merge();

            ws.Cell(startRow, 1).Style.Font.Italic = true;

            // ===== ПОДПИСЬ =====
            ws.Cell(startRow + 2, 1).Value = "Подпись:";
            ws.Cell(startRow + 2, 2).Value = "__________________________";

            // ===== ФИО =====
            ws.Cell(startRow + 3, 1).Value = "Фамилия И.О.:";
            ws.Cell(startRow + 3, 2).Value = "__________________________";

            ws.Range(startRow + 2, 1, startRow + 3, 2)
                .Style.Font.FontSize = 11;
        }
    }
}