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
        // ================= TASKS =================

        public MemoryStream CreateWordDocumentFromTasks(List<Model.Task> tasks)
        {
            var stream = new MemoryStream();

            using (var wordDocument = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document))
            {
                var mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                var body = mainPart.Document.AppendChild(new Body());

                AddPageSettings(mainPart);
                AddDocumentStyles(mainPart);

                AddHeading(body, "Отчет по задачам", 1);
                AddEmptyParagraph(body);
                AddParagraph(body, $"Всего задач: {tasks.Count}", false);
                AddEmptyParagraph(body);

                body.Append(CreateTasksTable(tasks));

                AddFooter(body);

                mainPart.Document.Save();
            }

            stream.Position = 0;
            return stream;
        }

        // ================= USERS =================

        public MemoryStream CreateWordDocumentFromUsers(List<Model.User> users)
        {
            var stream = new MemoryStream();

            using (var wordDocument = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document))
            {
                var mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                var body = mainPart.Document.AppendChild(new Body());

                AddPageSettings(mainPart);
                AddDocumentStyles(mainPart);

                AddHeading(body, "Отчет по сотрудникам", 1);
                AddEmptyParagraph(body);
                AddParagraph(body, $"Всего сотрудников: {users.Count}", false);
                AddEmptyParagraph(body);

                body.Append(CreateUsersTable(users));

                AddFooter(body);

                mainPart.Document.Save();
            }

            stream.Position = 0;
            return stream;
        }

        // ================= TABLE: TASKS =================

        private Table CreateTasksTable(List<Model.Task> tasks)
        {
            var table = CreateBaseTable();

            var headers = new[] { "Название", "Статус", "Категория", "Начало", "Срок", "Создал" };
            table.Append(CreateTableHeaderRow(headers));

            int i = 0;

            foreach (var t in tasks)
            {
                var row = new TableRow();

                row.Append(CreateCell(t.Title ?? "", i % 2 == 1, JustificationValues.Left));
                row.Append(CreateCell(t.Status?.Name ?? "", i % 2 == 1, JustificationValues.Center));
                row.Append(CreateCell(t.Scope?.Name ?? "", i % 2 == 1, JustificationValues.Center));
                row.Append(CreateCell(t.Since.ToString("dd.MM.yyyy"), i % 2 == 1, JustificationValues.Center));
                row.Append(CreateCell(t.Deadline.ToString("dd.MM.yyyy"), i % 2 == 1, JustificationValues.Center));
                row.Append(CreateCell(t.Owner?.Lname ?? "", i % 2 == 1, JustificationValues.Left));

                table.Append(row);
                i++;
            }

            ApplyTableBordersOnlyUsed(table);

            return table;
        }

        // ================= TABLE: USERS (без Role) =================

        private Table CreateUsersTable(List<Model.User> users)
        {
            var table = CreateBaseTable();

            var headers = new[] { "Фамилия", "Области ответственности" };
            table.Append(CreateTableHeaderRow(headers));

            int i = 0;

            foreach (var u in users)
            {
                var row = new TableRow();

                var scopes = (u.Scopes != null && u.Scopes.Any())
                    ? string.Join(", ", u.Scopes.Select(s => s.Name))
                    : "Не указаны";

                row.Append(CreateCell(u.Lname ?? "", i % 2 == 1, JustificationValues.Left));
                row.Append(CreateCell(scopes, i % 2 == 1, JustificationValues.Left));

                table.Append(row);
                i++;
            }

            ApplyTableBordersOnlyUsed(table);

            return table;
        }

        // ================= FOOTER =================

        private void AddFooter(Body body)
        {
            AddEmptyParagraph(body);

            AddParagraph(body, $"Документ создан: {DateTime.Now:dd.MM.yyyy HH:mm:ss}", false);

            AddEmptyParagraph(body);

            AddParagraph(body, "Подпись: __________________________", false);
            AddParagraph(body, "Фамилия Инициалы: __________________", false);
        }

        // ================= TABLE BASE =================

        private Table CreateBaseTable()
        {
            var table = new Table();

            var props = new TableProperties(
                new TableWidth { Width = "100", Type = TableWidthUnitValues.Pct }
            );

            table.AppendChild(props);
            return table;
        }

        private void ApplyTableBordersOnlyUsed(Table table)
        {
            var props = table.GetFirstChild<TableProperties>();

            props.Append(new TableBorders(
                new TopBorder { Val = BorderValues.Single, Size = 4, Color = "2E75B6" },
                new BottomBorder { Val = BorderValues.Single, Size = 4, Color = "2E75B6" },
                new LeftBorder { Val = BorderValues.Single, Size = 2, Color = "2E75B6" },
                new RightBorder { Val = BorderValues.Single, Size = 2, Color = "2E75B6" },
                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 2, Color = "AAAAAA" },
                new InsideVerticalBorder { Val = BorderValues.Single, Size = 2, Color = "AAAAAA" }
            ));
        }

        // ================= CELLS =================

        private TableRow CreateTableHeaderRow(string[] headers)
        {
            var row = new TableRow();

            foreach (var h in headers)
            {
                var cell = new TableCell();

                cell.Append(new TableCellProperties(
                    new Shading { Fill = "2E75B6" }
                ));

                var p = new Paragraph(
                    new Run(
                        new RunProperties(
                            new Bold(),
                            new Color { Val = "FFFFFF" },
                            new FontSize { Val = "20" },
                            new RunFonts { Ascii = "Segoe UI" }
                        ),
                        new Text(h)
                    )
                );

                cell.Append(p);
                row.Append(cell);
            }

            return row;
        }

        private TableCell CreateCell(string text, bool alt, JustificationValues align)
        {
            var cell = new TableCell();

            var props = new TableCellProperties();

            if (alt)
                props.Append(new Shading { Fill = "F2F2F2" });

            cell.Append(props);

            var p = new Paragraph(
                new ParagraphProperties(new Justification { Val = align }),
                new Run(
                    new RunProperties(
                        new FontSize { Val = "18" },
                        new RunFonts { Ascii = "Segoe UI" }
                    ),
                    new Text(text)
                )
            );

            cell.Append(p);
            return cell;
        }

        // ================= BASIC DOC =================

        private void AddHeading(Body body, string text, int level)
        {
            body.Append(new Paragraph(
                new Run(
                    new RunProperties(
                        new Bold(),
                        new FontSize { Val = level == 1 ? "32" : "24" },
                        new Color { Val = "2E75B6" },
                        new RunFonts { Ascii = "Segoe UI" }
                    ),
                    new Text(text)
                )
            ));
        }

        private void AddParagraph(Body body, string text, bool bold)
        {
            body.Append(new Paragraph(
                new Run(
                    new RunProperties(
                        new FontSize { Val = "20" },
                        new RunFonts { Ascii = "Segoe UI" }
                    ),
                    new Text(text)
                )
            ));
        }

        private void AddEmptyParagraph(Body body)
        {
            body.Append(new Paragraph());
        }

        private void AddPageSettings(MainDocumentPart mainPart)
        {
            var section = new SectionProperties(
                new PageSize
                {
                    Width = 16838,
                    Height = 11906,
                    Orient = PageOrientationValues.Landscape
                }
            );

            mainPart.Document.Body ??= mainPart.Document.AppendChild(new Body());
            mainPart.Document.Body.Append(section);
        }

        private void AddDocumentStyles(MainDocumentPart mainPart)
        {
            var part = mainPart.AddNewPart<StyleDefinitionsPart>();
            part.Styles = new Styles();
        }
    }
}