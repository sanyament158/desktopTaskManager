using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TaskManager.Infrastucture.OfficeDocument
{
    public class WordDocumentReport
    {
        // ======================== РАБОТА С ЗАДАЧАМИ ========================

        public MemoryStream CreateWordDocumentFromTasks(List<Model.Task> tasks)
        {
            var stream = new MemoryStream();

            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document))
            {
                // Добавляем основную часть документа
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();

                // Создаем тело документа
                Body body = mainPart.Document.AppendChild(new Body());

                // Добавляем настройки страницы (альбомная ориентация)
                AddPageSettings(mainPart);

                // Добавляем стили документа
                AddDocumentStyles(mainPart);

                // Добавляем заголовок документа
                AddHeading(body, "Отчет по задачам", 1);
                AddEmptyParagraph(body);

                // Добавляем информацию о количестве задач
                AddParagraph(body, $"Всего задач: {tasks.Count}", false);
                AddEmptyParagraph(body);

                // Создаем таблицу
                Table table = CreateTasksTable(tasks);
                body.AppendChild(table);

                // Добавляем информацию о дате генерации
                AddEmptyParagraph(body);
                AddParagraph(body, $"Документ создан: {DateTime.Now:dd.MM.yyyy HH:mm:ss}", false);

                mainPart.Document.Save();
            }

            stream.Position = 0;
            return stream;
        }

        public void SaveWordDocument(string filePath, List<Model.Task> tasks)
        {
            using (var stream = CreateWordDocumentFromTasks(tasks))
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    stream.CopyTo(fileStream);
                }
            }
        }

        // ======================== РАБОТА С ПОЛЬЗОВАТЕЛЯМИ ========================

        public MemoryStream CreateWordDocumentFromUsers(List<Model.User> users)
        {
            var stream = new MemoryStream();

            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document))
            {
                // Добавляем основную часть документа
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();

                // Создаем тело документа
                Body body = mainPart.Document.AppendChild(new Body());

                // Добавляем настройки страницы (альбомная ориентация)
                AddPageSettings(mainPart);

                // Добавляем стили документа
                AddDocumentStyles(mainPart);

                // Добавляем заголовок документа
                AddHeading(body, "Отчет по пользователям", 1);
                AddEmptyParagraph(body);

                // Добавляем информацию о количестве пользователей
                AddParagraph(body, $"Всего пользователей: {users.Count}", false);
                AddEmptyParagraph(body);

                // Создаем таблицу
                Table table = CreateUsersTable(users);
                body.AppendChild(table);

                // Добавляем информацию о дате генерации
                AddEmptyParagraph(body);
                AddParagraph(body, $"Документ создан: {DateTime.Now:dd.MM.yyyy HH:mm:ss}", false);

                mainPart.Document.Save();
            }

            stream.Position = 0;
            return stream;
        }

        public void SaveUserDocument(string filePath, List<Model.User> users)
        {
            using (var stream = CreateWordDocumentFromUsers(users))
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    stream.CopyTo(fileStream);
                }
            }
        }

        // ======================== ПРИВАТНЫЕ МЕТОДЫ ДЛЯ ЗАДАЧ ========================

        private void AddPageSettings(MainDocumentPart mainPart)
        {
            SectionProperties sectionProps = new SectionProperties();

            PageSize pageSize = new PageSize()
            {
                Width = 16838,
                Height = 11906,
                Orient = PageOrientationValues.Landscape
            };

            PageMargin pageMargin = new PageMargin()
            {
                Top = 567,
                Bottom = 567,
                Left = 567,
                Right = 567,
                Header = 708,
                Footer = 708,
                Gutter = 0
            };

            sectionProps.Append(pageSize);
            sectionProps.Append(pageMargin);

            mainPart.Document.Body ??= mainPart.Document.AppendChild(new Body());
            mainPart.Document.Body.AppendChild(sectionProps);
        }

        private void AddDocumentStyles(MainDocumentPart mainPart)
        {
            StyleDefinitionsPart stylePart = mainPart.AddNewPart<StyleDefinitionsPart>();
            Styles styles = new Styles();

            // Стиль для обычного текста
            Style normalStyle = new Style()
            {
                Type = StyleValues.Paragraph,
                StyleId = "Normal",
                Default = true
            };
            StyleName normalName = new StyleName() { Val = "Normal" };
            normalStyle.Append(normalName);

            StyleRunProperties normalRunProps = new StyleRunProperties();
            normalRunProps.Append(new FontSize() { Val = "20" });
            normalRunProps.Append(new RunFonts() { Ascii = "Segoe UI", HighAnsi = "Segoe UI" });
            normalStyle.Append(normalRunProps);
            styles.Append(normalStyle);

            // Стиль для заголовка 1
            Style heading1Style = new Style()
            {
                Type = StyleValues.Paragraph,
                StyleId = "Heading1"
            };
            StyleName heading1Name = new StyleName() { Val = "heading 1" };
            BasedOn basedOn1 = new BasedOn() { Val = "Normal" };
            heading1Style.Append(heading1Name);
            heading1Style.Append(basedOn1);

            StyleRunProperties heading1RunProps = new StyleRunProperties();
            heading1RunProps.Append(new Bold());
            heading1RunProps.Append(new FontSize() { Val = "32" });
            heading1RunProps.Append(new RunFonts() { Ascii = "Segoe UI", HighAnsi = "Segoe UI" });
            heading1RunProps.Append(new Color() { Val = "2E75B6" });
            heading1Style.Append(heading1RunProps);
            styles.Append(heading1Style);

            // Стиль для заголовка таблицы
            Style tableHeaderStyle = new Style()
            {
                Type = StyleValues.Paragraph,
                StyleId = "TableHeader"
            };
            StyleRunProperties tableHeaderRunProps = new StyleRunProperties();
            tableHeaderRunProps.Append(new Bold());
            tableHeaderRunProps.Append(new FontSize() { Val = "20" });
            tableHeaderRunProps.Append(new RunFonts() { Ascii = "Segoe UI", HighAnsi = "Segoe UI" });
            tableHeaderRunProps.Append(new Color() { Val = "FFFFFF" });
            tableHeaderStyle.Append(tableHeaderRunProps);
            styles.Append(tableHeaderStyle);

            stylePart.Styles = styles;
        }

        private Table CreateTasksTable(List<Model.Task> tasks)
        {
            Table table = new Table();
            TableProperties tblProp = new TableProperties();

            TableWidth tableWidth = new TableWidth() { Width = "100", Type = TableWidthUnitValues.Pct };

            TableBorders borders = new TableBorders(
                new TopBorder() { Val = BorderValues.Single, Size = 4, Color = "2E75B6" },
                new BottomBorder() { Val = BorderValues.Single, Size = 4, Color = "2E75B6" },
                new LeftBorder() { Val = BorderValues.Single, Size = 2, Color = "2E75B6" },
                new RightBorder() { Val = BorderValues.Single, Size = 2, Color = "2E75B6" },
                new InsideHorizontalBorder() { Val = BorderValues.Single, Size = 2, Color = "AAAAAA" },
                new InsideVerticalBorder() { Val = BorderValues.Single, Size = 2, Color = "AAAAAA" }
            );

            TableCellMarginDefault cellMargin = new TableCellMarginDefault();
            cellMargin.Append(new TopMargin() { Width = "50", Type = TableWidthUnitValues.Dxa });
            cellMargin.Append(new BottomMargin() { Width = "50", Type = TableWidthUnitValues.Dxa });
            cellMargin.Append(new LeftMargin() { Width = "80", Type = TableWidthUnitValues.Dxa });
            cellMargin.Append(new RightMargin() { Width = "80", Type = TableWidthUnitValues.Dxa });

            tblProp.Append(tableWidth);
            tblProp.Append(borders);
            tblProp.Append(cellMargin);
            table.AppendChild(tblProp);

            TableGrid tableGrid = new TableGrid();
            int[] columnWidths = { 3500, 1600, 2400, 2400, 1800, 1800 };

            foreach (int width in columnWidths)
            {
                tableGrid.Append(new GridColumn() { Width = width.ToString() });
            }
            table.Append(tableGrid);

            // Заголовок таблицы
            TableRow headerRow = CreateTableHeaderRow(new[] { "Название", "Статус", "Владелец", "Категория", "Дата начала", "Дедлайн" });
            table.Append(headerRow);

            // Данные
            int rowNumber = 0;
            foreach (Model.Task task in tasks)
            {
                TableRow dataRow = new TableRow();

                if (rowNumber % 2 == 1)
                {
                    TableRowProperties rowProps = new TableRowProperties();
                    TableRowHeight rowHeight = new TableRowHeight() { Val = 350 };
                    rowProps.Append(rowHeight);
                    dataRow.Append(rowProps);
                }

                dataRow.Append(CreateCell(task.Title ?? "", rowNumber % 2 == 1, JustificationValues.Left));
                dataRow.Append(CreateCell(task.Status?.Name ?? "Не указан", rowNumber % 2 == 1, JustificationValues.Center));

                string ownerName = task.Owner != null ? $"{task.Owner.Lname}" : "Не назначен";
                dataRow.Append(CreateCell(ownerName, rowNumber % 2 == 1, JustificationValues.Left));

                dataRow.Append(CreateCell(task.Scope?.Name ?? "Не указана", rowNumber % 2 == 1, JustificationValues.Center));
                dataRow.Append(CreateCell(task.Since.ToString("dd.MM.yyyy"), rowNumber % 2 == 1, JustificationValues.Center));
                dataRow.Append(CreateCell(task.Deadline.ToString("dd.MM.yyyy"), rowNumber % 2 == 1, JustificationValues.Center));

                table.Append(dataRow);
                rowNumber++;
            }

            return table;
        }

        // ======================== ПРИВАТНЫЕ МЕТОДЫ ДЛЯ ПОЛЬЗОВАТЕЛЕЙ ========================

        // ======================== ПРИВАТНЫЕ МЕТОДЫ ДЛЯ ПОЛЬЗОВАТЕЛЕЙ ========================

        private Table CreateUsersTable(List<Model.User> users)
        {
            Table table = new Table();
            TableProperties tblProp = new TableProperties();

            TableWidth tableWidth = new TableWidth() { Width = "100", Type = TableWidthUnitValues.Pct };

            TableBorders borders = new TableBorders(
                new TopBorder() { Val = BorderValues.Single, Size = 4, Color = "2E75B6" },
                new BottomBorder() { Val = BorderValues.Single, Size = 4, Color = "2E75B6" },
                new LeftBorder() { Val = BorderValues.Single, Size = 2, Color = "2E75B6" },
                new RightBorder() { Val = BorderValues.Single, Size = 2, Color = "2E75B6" },
                new InsideHorizontalBorder() { Val = BorderValues.Single, Size = 2, Color = "AAAAAA" },
                new InsideVerticalBorder() { Val = BorderValues.Single, Size = 2, Color = "AAAAAA" }
            );

            TableCellMarginDefault cellMargin = new TableCellMarginDefault();
            cellMargin.Append(new TopMargin() { Width = "50", Type = TableWidthUnitValues.Dxa });
            cellMargin.Append(new BottomMargin() { Width = "50", Type = TableWidthUnitValues.Dxa });
            cellMargin.Append(new LeftMargin() { Width = "80", Type = TableWidthUnitValues.Dxa });
            cellMargin.Append(new RightMargin() { Width = "80", Type = TableWidthUnitValues.Dxa });

            tblProp.Append(tableWidth);
            tblProp.Append(borders);
            tblProp.Append(cellMargin);
            table.AppendChild(tblProp);

            // Настройка ширины колонок для отчета по пользователям
            TableGrid tableGrid = new TableGrid();
            int[] columnWidths = { 3000, 3000, 2500 }; // Lname, Role, Scopes

            foreach (int width in columnWidths)
            {
                tableGrid.Append(new GridColumn() { Width = width.ToString() });
            }
            table.Append(tableGrid);

            // Заголовок таблицы
            TableRow headerRow = CreateTableHeaderRow(new[] { "Фамилия", "Роль", "Области ответственности" });
            table.Append(headerRow);

            // Данные пользователей
            int rowNumber = 0;
            foreach (Model.User user in users)
            {
                TableRow dataRow = new TableRow();

                if (rowNumber % 2 == 1)
                {
                    TableRowProperties rowProps = new TableRowProperties();
                    TableRowHeight rowHeight = new TableRowHeight() { Val = 350 };
                    rowProps.Append(rowHeight);
                    dataRow.Append(rowProps);
                }

                // Фамилия (Lname)
                string lastName = user.Lname ?? "Не указана";
                dataRow.Append(CreateCell(lastName, rowNumber % 2 == 1, JustificationValues.Left));

                // Роль с преобразованием
                string roleName = TranslateRole(user.Role?.Name);
                dataRow.Append(CreateCell(roleName, rowNumber % 2 == 1, JustificationValues.Left));

                // Области ответственности (Scopes)
                string scopesText = FormatScopes(user.Scopes);
                dataRow.Append(CreateCell(scopesText, rowNumber % 2 == 1, JustificationValues.Left));

                table.Append(dataRow);
                rowNumber++;
            }

            return table;
        }

        /// <summary>
        /// Преобразует английское название роли в русское
        /// </summary>
        private string TranslateRole(string? roleName)
        {
            if (string.IsNullOrEmpty(roleName))
                return "Не указана";

            return roleName.ToLower() switch
            {
                "admin" => "Администратор",
                "user" => "Сотрудник",
                _ => roleName // Если роль не распознана, возвращаем как есть
            };
        }

        private string FormatScopes(List<Model.Category> scopes)
        {
            if (scopes == null || scopes.Count == 0)
                return "Не указаны";

            return string.Join(", ", scopes.Select(s => s?.Name ?? "Без названия"));
        }
        // ======================== ОБЩИЕ ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ========================

        private TableRow CreateTableHeaderRow(string[] headers)
        {
            TableRow headerRow = new TableRow();

            TableRowProperties headerRowProps = new TableRowProperties();
            TableRowHeight headerHeight = new TableRowHeight() { Val = 400 };
            headerRowProps.Append(headerHeight);
            headerRow.Append(headerRowProps);

            foreach (string header in headers)
            {
                TableCell cell = new TableCell();

                TableCellProperties cellProps = new TableCellProperties();
                Shading shading = new Shading() { Val = ShadingPatternValues.Clear, Color = "Auto", Fill = "2E75B6" };
                cellProps.Append(shading);

                Paragraph paragraph = new Paragraph();
                ParagraphProperties paraProps = new ParagraphProperties();
                Justification justification = new Justification() { Val = JustificationValues.Center };
                paraProps.Append(justification);
                paragraph.Append(paraProps);

                Run run = new Run();
                RunProperties runProps = new RunProperties();
                runProps.Append(new Bold());
                runProps.Append(new FontSize() { Val = "20" });
                runProps.Append(new RunFonts() { Ascii = "Segoe UI", HighAnsi = "Segoe UI" });
                runProps.Append(new Color() { Val = "FFFFFF" });
                run.Append(runProps);
                run.Append(new Text(header));
                paragraph.Append(run);

                cell.Append(cellProps);
                cell.Append(paragraph);
                headerRow.Append(cell);
            }

            return headerRow;
        }

        private TableCell CreateCell(string text, bool isAlternateRow, JustificationValues alignment)
        {
            TableCell cell = new TableCell();

            TableCellProperties cellProps = new TableCellProperties();
            cellProps.Append(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center });

            if (isAlternateRow)
            {
                cellProps.Append(new Shading() { Val = ShadingPatternValues.Clear, Color = "Auto", Fill = "F2F2F2" });
            }

            cell.Append(cellProps);

            Paragraph para = new Paragraph();
            para.Append(new ParagraphProperties(new Justification() { Val = alignment }));

            Run run = new Run();
            RunProperties runProps = new RunProperties();
            runProps.Append(new FontSize() { Val = "18" });
            runProps.Append(new RunFonts() { Ascii = "Segoe UI", HighAnsi = "Segoe UI" });

            run.Append(runProps);
            run.Append(new Text(text));
            para.Append(run);
            cell.Append(para);

            return cell;
        }

        private void AddHeading(Body body, string text, int level)
        {
            Paragraph para = new Paragraph();
            ParagraphProperties paraProps = new ParagraphProperties();
            SpacingBetweenLines spacing = new SpacingBetweenLines() { After = "200" };
            paraProps.Append(spacing);
            para.Append(paraProps);

            Run run = new Run();
            RunProperties runProps = new RunProperties();
            runProps.Append(new Bold());

            if (level == 1)
            {
                runProps.Append(new FontSize() { Val = "32" });
                runProps.Append(new Color() { Val = "2E75B6" });
            }
            else
            {
                runProps.Append(new FontSize() { Val = "24" });
                runProps.Append(new Color() { Val = "404040" });
            }

            runProps.Append(new RunFonts() { Ascii = "Segoe UI", HighAnsi = "Segoe UI" });
            run.Append(runProps);
            run.Append(new Text(text));
            para.Append(run);
            body.Append(para);
        }

        private void AddParagraph(Body body, string text, bool isBold)
        {
            Paragraph para = new Paragraph();
            ParagraphProperties paraProps = new ParagraphProperties();
            SpacingBetweenLines spacing = new SpacingBetweenLines() { After = "100" };
            paraProps.Append(spacing);
            para.Append(paraProps);

            Run run = new Run();
            RunProperties runProps = new RunProperties();
            runProps.Append(new FontSize() { Val = "20" });
            runProps.Append(new RunFonts() { Ascii = "Segoe UI", HighAnsi = "Segoe UI" });

            if (isBold)
            {
                runProps.Append(new Bold());
            }

            run.Append(runProps);
            run.Append(new Text(text));
            para.Append(run);
            body.Append(para);
        }

        private void AddEmptyParagraph(Body body)
        {
            Paragraph para = new Paragraph();
            ParagraphProperties paraProps = new ParagraphProperties();
            SpacingBetweenLines spacing = new SpacingBetweenLines() { After = "0" };
            paraProps.Append(spacing);
            para.Append(paraProps);
            body.Append(para);
        }
    }
}