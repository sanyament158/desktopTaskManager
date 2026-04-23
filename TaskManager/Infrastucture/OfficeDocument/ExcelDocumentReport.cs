using System;
using System.Collections.Generic;
using System.Text;
using ClosedXML.Excel;
using TaskManager.Infrastucture.Network;

namespace TaskManager.Infrastucture.OfficeDocument
{
    public class ExcelDocumentReport
    {
        public List<Model.User> UsersDict {  get; set; }
        public List<Model.Task> TasksDict {  get; set; }

        // make document object methods
        public XLWorkbook ExportTasks(List<Model.Task> tasks)
        {
            // Создаём новую книгу
            var workbook = new XLWorkbook();

            // Добавляем лист с названием "Задачи"
            var worksheet = workbook.Worksheets.Add("Задачи");

            // Заголовки столбцов (человекочитаемые)
            string[] headers = { "Название", "Исполнитель", "Статус", "Категория", "Начало", "Срок сдачи", "Задачу создал"};

            // Записываем заголовки в первую строку
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            // Стилизуем заголовки
            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.SetBold();
            headerRow.Style.Font.SetFontColor(XLColor.White);
            headerRow.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#2F5597"));
            headerRow.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            headerRow.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

            // Заполняем данными
            int currentRow = 2;
            foreach (var task in tasks)
            {
                worksheet.Cell(currentRow, 1).Value = task.Title ?? "";
                worksheet.Cell(currentRow, 2).Value = GetTakedUserLname(task) ?? "Не назначен"; 
                worksheet.Cell(currentRow, 3).Value = task.Status?.Name ?? "Неизвестно";
                worksheet.Cell(currentRow, 4).Value = task.Scope?.Name ?? "Без категории";
                worksheet.Cell(currentRow, 5).Value = task.Since.ToString("dd.MM.yyyy");
                worksheet.Cell(currentRow, 6).Value = task.Deadline.ToString("dd.MM.yyyy");
                worksheet.Cell(currentRow, 7).Value = task.Owner.Lname;

                // Раскрашиваем строки в зависимости от статуса или просрочки
                ApplyRowStyling(worksheet, currentRow, task);

                currentRow++;
            }

            // Применяем автофильтр к заголовкам
            worksheet.RangeUsed().SetAutoFilter();

            // Автоматически подгоняем ширину столбцов
            worksheet.Columns().AdjustToContents();

            // Дополнительное форматирование для столбца с описанием (делаем его чуть шире)
            worksheet.Column(2).Width = 40;

            // Добавляем итоговую строку с подсчётом задач
            AddSummaryRow(worksheet, currentRow, tasks.Count);

            return workbook;
        }

        /// <summary>
        /// Применяет стили к строке в зависимости от статуса и просрочки
        /// </summary>
        private string GetTakedUserLname(Model.Task task)
        {
            string takedUserLname = UsersDict.Where(u => u.Id == task.IdUserTaked).First().Lname ?? "Не назначен";
            return takedUserLname;

        }
        private void ApplyRowStyling(IXLWorksheet worksheet, int rowNum, Model.Task task)
        {
            var row = worksheet.Row(rowNum);

            // Проверяем просрочку
            bool isOverdue = task.Deadline < DateTime.Now.Date &&
                            (task.Status?.Name?.ToLower() != "завершено" && task.Status?.Name?.ToLower() != "closed");

            if (isOverdue)
            {
                // Просроченные задачи - красный фон
                row.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#FFCCCC"));
                row.Style.Font.SetFontColor(XLColor.FromHtml("#9C0000"));
            }
            else if (task.Status?.Name?.ToLower() == "завершено" || task.Status?.Name?.ToLower() == "closed")
            {
                // Завершённые задачи - зелёный фон
                row.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#C6EFCE"));
                row.Style.Font.SetFontColor(XLColor.FromHtml("#006100"));
            }
            else if (task.IdUserTaked != null && task.IdUserTaked > 0)
            {
                // Задачи в работе - голубоватый фон
                row.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#DDEBF7"));
            }

            // Добавляем границы для всех ячеек в строке
            row.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            row.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
        }

        /// <summary>
        /// Добавляет итоговую строку с подсчётами
        /// </summary>
        private void AddSummaryRow(IXLWorksheet worksheet, int lastRow, int totalCount)
        {
            int summaryRow = lastRow + 1;

            worksheet.Cell(summaryRow, 1).Value = "ИТОГО:";
            worksheet.Cell(summaryRow, 1).Style.Font.SetBold();
            worksheet.Cell(summaryRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

            worksheet.Cell(summaryRow, 2).Value = $"Всего задач: {totalCount}";
            worksheet.Cell(summaryRow, 2).Style.Font.SetBold();

            // Объединяем ячейки для красоты (необязательно)
            worksheet.Range(summaryRow, 3, summaryRow, 8).Merge();
            worksheet.Cell(summaryRow, 3).Value = "Данные актуальны на " + DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            worksheet.Cell(summaryRow, 3).Style.Font.SetItalic();
            worksheet.Cell(summaryRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

            // Фон для итоговой строки
            worksheet.Row(summaryRow).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#F2F2F2"));
        }

        /// <summary>
        /// Альтернативный метод: экспорт и сразу сохранение в файл
        /// </summary>
        public XLWorkbook ExportUsersByScope(string scope, List<Model.User> users)
        {
            // Создаём книгу
            var workbook = new XLWorkbook();

            // Название листа (обрезаем, если слишком длинное)
            string sheetName = $"Сотрудники, ответственные за {scope}";
            if (sheetName.Length > 31) sheetName = sheetName.Substring(0, 31);
            var worksheet = workbook.Worksheets.Add(sheetName);

            // ========== ЗАГОЛОВОК НАД ТАБЛИЦЕЙ (строка 1) ==========
            string title = $"Сотрудники, ответственные за {scope}";
            worksheet.Range("A1:B1").Merge();  // A1 до B1 (2 столбца)
            worksheet.Cell("A1").Value = title;
            worksheet.Cell("A1").Style.Font.SetBold();
            worksheet.Cell("A1").Style.Font.SetFontSize(14);
            worksheet.Cell("A1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            // ========== ЗАГОЛОВКИ СТОЛБЦОВ (строка 2) ==========
            string[] headers = { "Фамилия", "Категории" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(2, i + 1).Value = headers[i];
            }

            // Стилизация заголовков столбцов (строка 2)
            var headerRow = worksheet.Row(2);
            headerRow.Style.Font.SetBold();
            headerRow.Style.Font.SetFontColor(XLColor.White);
            headerRow.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#2F5597"));
            headerRow.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            headerRow.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

            // ========== ЗАПОЛНЕНИЕ ДАННЫМИ (начиная с 3 строки) ==========
            int currentRow = 3;
            foreach (var user in users)
            {
                // Формируем строку со всеми категориями пользователя
                string allScopes = user.Scopes != null
                    ? string.Join(", ", user.Scopes.Select(s => s.Name))
                    : "Нет категорий";

                worksheet.Cell(currentRow, 1).Value = user.Lname ?? "—";
                worksheet.Cell(currentRow, 2).Value = allScopes;

                currentRow++;
            }

            // Если пользователей не найдено — выводим сообщение
            if (users.Count == 0)
            {
                worksheet.Cell(3, 1).Value = $"Сотрудники, ответственные за '{scope}', не найдены";
                worksheet.Range(3, 1, 3, 2).Merge();
                worksheet.Cell(3, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(3, 1).Style.Font.SetItalic();
                worksheet.Cell(3, 1).Style.Font.SetFontColor(XLColor.Gray);
            }

            // ========== НАСТРОЙКА ВНЕШНЕГО ВИДА ==========
            worksheet.RangeUsed()?.SetAutoFilter();
            worksheet.Columns().AdjustToContents();
            worksheet.Column(2).Width = 40; // Столбец с категориями пошире

            // Итоговая строка
            AddSummaryRow(worksheet, currentRow, users.Count, scope);

            return workbook;
        }

        private void AddSummaryRow(IXLWorksheet worksheet, int lastRow, int totalCount, string scope)
        {
            int summaryRow = lastRow + 1;

            worksheet.Cell(summaryRow, 1).Value = "ИТОГО:";
            worksheet.Cell(summaryRow, 1).Style.Font.SetBold();
            worksheet.Cell(summaryRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

            worksheet.Cell(summaryRow, 2).Value = $"Всего сотрудников: {totalCount} | Категория: {scope} | Отчёт сформирован: {DateTime.Now:dd.MM.yyyy HH:mm}";
            worksheet.Cell(summaryRow, 2).Style.Font.SetBold();
            worksheet.Cell(summaryRow, 2).Style.Font.SetItalic();

            worksheet.Row(summaryRow).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#F2F2F2"));
        }
    }
}
