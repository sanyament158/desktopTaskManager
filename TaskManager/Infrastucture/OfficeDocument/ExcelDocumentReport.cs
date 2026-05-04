using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using TaskManager.Model;

namespace TaskManager.Infrastructure.OfficeDocument
{
    public class ExcelDocumentReport
    {
        public XLWorkbook BuildTasksReport(List<Model.Task> tasks)
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Задачи");

            string[] headers =
            {
                "Название",
                "Статус",
                "Категория",
                "Начало",
                "Срок",
                "Создал"
            };

            // ===== HEADER =====
            for (int i = 0; i < headers.Length; i++)
                ws.Cell(1, i + 1).Value = headers[i];

            var headerRow = ws.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#2F5597");
            headerRow.Style.Font.FontColor = XLColor.White;
            headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            int row = 2;

            // ===== DATA =====
            foreach (var t in tasks)
            {
                ws.Cell(row, 1).Value = t.Title;
                ws.Cell(row, 2).Value = t.Status?.Name;
                ws.Cell(row, 3).Value = t.Scope?.Name;
                ws.Cell(row, 4).Value = t.Since.ToString("dd.MM.yyyy");
                ws.Cell(row, 5).Value = t.Deadline.ToString("dd.MM.yyyy");
                ws.Cell(row, 6).Value = t.Owner?.Lname;

                ApplyRowStyle(ws, row, t);

                row++;
            }

            // ===== USED RANGE BORDER (ВАЖНО) =====
            var usedRange = ws.RangeUsed();
            if (usedRange != null)
            {
                usedRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                usedRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            }

            ws.Columns().AdjustToContents();

            // ===== FOOTER =====
            AddFooter(ws, row + 2);

            return wb;
        }

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

        private void AddFooter(IXLWorksheet ws, int startRow)
        {
            ws.Cell(startRow, 1).Value = $"Документ создан: {DateTime.Now:dd.MM.yyyy HH:mm}";
            ws.Range(startRow, 1, startRow, 3).Merge();
            ws.Cell(startRow, 1).Style.Font.Italic = true;

            // подпись линия
            ws.Cell(startRow + 2, 1).Value = "Подпись:";
            ws.Cell(startRow + 2, 2).Value = "__________________________";

            // ФИО линия
            ws.Cell(startRow + 3, 1).Value = "Фамилия И.О.:";
            ws.Cell(startRow + 3, 2).Value = "__________________________";

            ws.Range(startRow + 2, 1, startRow + 3, 2)
                .Style.Font.FontSize = 11;
        }

        // ===== USERS REPORT =====
        public XLWorkbook BuildUsersReport(List<User> users)
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Сотрудники");

            string[] headers =
            {
        "Фамилия",
        "Зоны ответственности"
    };

            // ===== HEADER =====
            for (int i = 0; i < headers.Length; i++)
                ws.Cell(1, i + 1).Value = headers[i];

            var header = ws.Row(1);
            header.Style.Font.Bold = true;
            header.Style.Fill.BackgroundColor = XLColor.FromHtml("#2F5597");
            header.Style.Font.FontColor = XLColor.White;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            int row = 2;

            // ===== DATA =====
            foreach (var u in users)
            {
                ws.Cell(row, 1).Value = u.Lname ?? "—";

                var scopes = u.Scopes != null && u.Scopes.Any()
                    ? string.Join(", ", u.Scopes.Select(s => s.Name))
                    : "Нет зон";

                ws.Cell(row, 2).Value = scopes;

                ApplyUserRowStyle(ws, row);

                row++;
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

            AddFooter(ws, row + 2);

            return wb;
        }

        private void ApplyUserRowStyle(IXLWorksheet ws, int row)
        {
            var r = ws.Row(row);

            r.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            r.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        }
    }
}