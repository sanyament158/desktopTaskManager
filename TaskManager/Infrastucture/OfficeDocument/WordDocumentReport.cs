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

        /// <summary>
        /// Сохраняет документ Word в файл
        /// </summary>
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

        /// <summary>
        /// Добавляет настройки страницы (альбомная ориентация и поля)
        /// </summary>
        private void AddPageSettings(MainDocumentPart mainPart)
        {
            // Настройки страницы
            SectionProperties sectionProps = new SectionProperties();

            // Размер страницы A4 в альбомной ориентации
            PageSize pageSize = new PageSize()
            {
                Width = 16838,  // 297mm
                Height = 11906,  // 210mm
                Orient = PageOrientationValues.Landscape
            };

            // Уменьшенные поля страницы
            PageMargin pageMargin = new PageMargin()
            {
                Top = 567,       // 0.75 см
                Bottom = 567,    // 0.75 см
                Left = 567,      // 0.75 см
                Right = 567,     // 0.75 см
                Header = 708,
                Footer = 708,
                Gutter = 0
            };

            sectionProps.Append(pageSize);
            sectionProps.Append(pageMargin);

            // Добавляем секцию в документ
            mainPart.Document.Body ??= mainPart.Document.AppendChild(new Body());
            mainPart.Document.Body.AppendChild(sectionProps);
        }

        /// <summary>
        /// Добавляет стили документа (шрифты, цвета)
        /// </summary>
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
            normalRunProps.Append(new FontSize() { Val = "20" }); // 10pt (уменьшен)
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
            heading1RunProps.Append(new FontSize() { Val = "32" }); // 16pt (уменьшен)
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
            tableHeaderRunProps.Append(new FontSize() { Val = "20" }); // 10pt
            tableHeaderRunProps.Append(new RunFonts() { Ascii = "Segoe UI", HighAnsi = "Segoe UI" });
            tableHeaderRunProps.Append(new Color() { Val = "FFFFFF" });
            tableHeaderStyle.Append(tableHeaderRunProps);

            styles.Append(tableHeaderStyle);

            stylePart.Styles = styles;
        }

        /// <summary>
        /// Создает таблицу со списком задач
        /// </summary>
        private Table CreateTasksTable(List<Model.Task> tasks)
        {
            Table table = new Table();

            // Настройка свойств таблицы
            TableProperties tblProp = new TableProperties();

            // Ширина таблицы - 100% от ширины страницы
            TableWidth tableWidth = new TableWidth() { Width = "100", Type = TableWidthUnitValues.Pct };

            // Границы таблицы
            TableBorders borders = new TableBorders(
                new TopBorder() { Val = BorderValues.Single, Size = 4, Color = "2E75B6" },
                new BottomBorder() { Val = BorderValues.Single, Size = 4, Color = "2E75B6" },
                new LeftBorder() { Val = BorderValues.Single, Size = 2, Color = "2E75B6" },
                new RightBorder() { Val = BorderValues.Single, Size = 2, Color = "2E75B6" },
                new InsideHorizontalBorder() { Val = BorderValues.Single, Size = 2, Color = "AAAAAA" },
                new InsideVerticalBorder() { Val = BorderValues.Single, Size = 2, Color = "AAAAAA" }
            );

            // Уменьшенные отступы в ячейках
            TableCellMarginDefault cellMargin = new TableCellMarginDefault();
            cellMargin.Append(new TopMargin() { Width = "50", Type = TableWidthUnitValues.Dxa });
            cellMargin.Append(new BottomMargin() { Width = "50", Type = TableWidthUnitValues.Dxa });
            cellMargin.Append(new LeftMargin() { Width = "80", Type = TableWidthUnitValues.Dxa });
            cellMargin.Append(new RightMargin() { Width = "80", Type = TableWidthUnitValues.Dxa });

            tblProp.Append(tableWidth);
            tblProp.Append(borders);
            tblProp.Append(cellMargin);
            table.AppendChild(tblProp);

            // Увеличенная ширина колонок для альбомной ориентации
            TableGrid tableGrid = new TableGrid();
            int[] columnWidths = { 3500, 1600, 2400, 2400, 1800, 1800 }; // Увеличенные ширины в Dxa

            foreach (int width in columnWidths)
            {
                tableGrid.Append(new GridColumn() { Width = width.ToString() });
            }
            table.Append(tableGrid);

            // Создаем заголовок таблицы с фоном
            TableRow headerRow = new TableRow();

            // Свойства строки заголовка
            TableRowProperties headerRowProps = new TableRowProperties();
            TableRowHeight headerHeight = new TableRowHeight() { Val = 400 };
            headerRowProps.Append(headerHeight);
            headerRow.Append(headerRowProps);

            // Заголовки
            string[] headers = { "Название", "Статус", "Владелец", "Категория", "Дата начала", "Дедлайн" };

            foreach (string header in headers)
            {
                TableCell cell = new TableCell();

                // Фон заголовка
                TableCellProperties cellProps = new TableCellProperties();
                Shading shading = new Shading() { Val = ShadingPatternValues.Clear, Color = "Auto", Fill = "2E75B6" };
                cellProps.Append(shading);

                // Выравнивание по центру
                Paragraph paragraph = new Paragraph();
                ParagraphProperties paraProps = new ParagraphProperties();
                Justification justification = new Justification() { Val = JustificationValues.Center };
                paraProps.Append(justification);
                paragraph.Append(paraProps);

                Run run = new Run();
                RunProperties runProps = new RunProperties();
                runProps.Append(new Bold());
                runProps.Append(new FontSize() { Val = "20" }); // Уменьшен шрифт
                runProps.Append(new RunFonts() { Ascii = "Segoe UI", HighAnsi = "Segoe UI" });
                runProps.Append(new Color() { Val = "FFFFFF" });
                run.Append(runProps);
                run.Append(new Text(header));
                paragraph.Append(run);

                cell.Append(cellProps);
                cell.Append(paragraph);
                headerRow.Append(cell);
            }

            table.Append(headerRow);

            // Заполняем таблицу данными
            int rowNumber = 0;
            foreach (Model.Task task in tasks)
            {
                TableRow dataRow = new TableRow();

                // Чередование цветов строк
                if (rowNumber % 2 == 1)
                {
                    TableRowProperties rowProps = new TableRowProperties();
                    TableRowHeight rowHeight = new TableRowHeight() { Val = 350 };
                    rowProps.Append(rowHeight);
                    dataRow.Append(rowProps);
                }

                // Название
                dataRow.Append(CreateCell(task.Title ?? "", rowNumber % 2 == 1, JustificationValues.Left));

                // Статус
                dataRow.Append(CreateCell(task.Status?.Name ?? "Не указан", rowNumber % 2 == 1, JustificationValues.Center));

                // Владелец
                string ownerName = task.Owner != null ? $"{task.Owner.Lname}" : "Не назначен";
                dataRow.Append(CreateCell(ownerName, rowNumber % 2 == 1, JustificationValues.Left));

                // Категория
                dataRow.Append(CreateCell(task.Scope?.Name ?? "Не указана", rowNumber % 2 == 1, JustificationValues.Center));

                // Дата начала
                dataRow.Append(CreateCell(task.Since.ToString("dd.MM.yyyy"), rowNumber % 2 == 1, JustificationValues.Center));

                // Дедлайн
                dataRow.Append(CreateCell(task.Deadline.ToString("dd.MM.yyyy"), rowNumber % 2 == 1, JustificationValues.Center));

                table.Append(dataRow);
                rowNumber++;
            }

            return table;
        }

        /// <summary>
        /// Создает ячейку таблицы с текстом
        /// </summary>
        private TableCell CreateCell(string text, bool isAlternateRow, JustificationValues alignment)
        {
            TableCell cell = new TableCell();

            // Свойства ячейки
            TableCellProperties cellProps = new TableCellProperties();

            // Вертикальное выравнивание по центру
            cellProps.Append(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center });

            // Фон для четных строк
            if (isAlternateRow)
            {
                cellProps.Append(new Shading() { Val = ShadingPatternValues.Clear, Color = "Auto", Fill = "F2F2F2" });
            }

            cell.Append(cellProps);

            // Параграф с выравниванием
            Paragraph para = new Paragraph();
            para.Append(new ParagraphProperties(new Justification() { Val = alignment }));

            // Текст с уменьшенным шрифтом
            Run run = new Run();
            RunProperties runProps = new RunProperties();
            runProps.Append(new FontSize() { Val = "18" }); // 9pt (еще меньше)
            runProps.Append(new RunFonts() { Ascii = "Segoe UI", HighAnsi = "Segoe UI" });

            run.Append(runProps);
            run.Append(new Text(text));
            para.Append(run);
            cell.Append(para);

            return cell;
        }

        /// <summary>
        /// Добавляет заголовок в документ
        /// </summary>
        private void AddHeading(Body body, string text, int level)
        {
            Paragraph para = new Paragraph();
            ParagraphProperties paraProps = new ParagraphProperties();

            // Отступ после заголовка
            SpacingBetweenLines spacing = new SpacingBetweenLines() { After = "200" };
            paraProps.Append(spacing);
            para.Append(paraProps);

            Run run = new Run();
            RunProperties runProps = new RunProperties();
            runProps.Append(new Bold());

            if (level == 1)
            {
                runProps.Append(new FontSize() { Val = "32" }); // 16pt
                runProps.Append(new Color() { Val = "2E75B6" });
            }
            else
            {
                runProps.Append(new FontSize() { Val = "24" }); // 12pt
                runProps.Append(new Color() { Val = "404040" });
            }

            runProps.Append(new RunFonts() { Ascii = "Segoe UI", HighAnsi = "Segoe UI" });
            run.Append(runProps);
            run.Append(new Text(text));
            para.Append(run);
            body.Append(para);
        }

        /// <summary>
        /// Добавляет обычный параграф
        /// </summary>
        private void AddParagraph(Body body, string text, bool isBold)
        {
            Paragraph para = new Paragraph();
            ParagraphProperties paraProps = new ParagraphProperties();
            SpacingBetweenLines spacing = new SpacingBetweenLines() { After = "100" };
            paraProps.Append(spacing);
            para.Append(paraProps);

            Run run = new Run();
            RunProperties runProps = new RunProperties();
            runProps.Append(new FontSize() { Val = "20" }); // 10pt
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

        /// <summary>
        /// Добавляет пустой параграф
        /// </summary>
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