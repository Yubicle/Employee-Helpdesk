using HelpdeskViewModels;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace HelpdeskWebsite.Reports
{
    public class CallReport
    {
        public async void GenerateReport(string rootpath)
        {
            CallViewModel vm = new();
            List<CallViewModel> calls = await vm.GetAll();

            PageSize pg = PageSize.A4.Rotate();
            var helvetica = PdfFontFactory.CreateFont(StandardFontFamilies.HELVETICA);

            PdfWriter writer = new(rootpath + "/pdfs/callreport.pdf",
                                new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfDocument pdf = new(writer);
            Document document = new(pdf, pg); 
            document.Add(new Image(ImageDataFactory.Create(rootpath + "/img/helpdesk.png"))
                                                                    .ScaleToFit(150, 100)
                                                                    .SetHorizontalAlignment(HorizontalAlignment.CENTER));

            document.Add(new Paragraph("Current Calls").SetFont(helvetica)
                                                                    .SetFontSize(24)
                                                                    .SetBold()
                                                                    .SetTextAlignment(TextAlignment.CENTER));
            document.Add(new Paragraph(""));
            document.Add(new Paragraph(""));
            Table table = new(6);
            table
                .SetWidth(UnitValue.CreatePercentValue(85)) // roughly 50%
                .SetTextAlignment(TextAlignment.CENTER)
                .SetRelativePosition(0, 0, 0, 0)
                .SetHorizontalAlignment(HorizontalAlignment.CENTER);

            table.AddCell(new Cell().Add(new Paragraph("Opened")
                .SetFontSize(16)
                .SetBold()
                .SetPaddingLeft(20)
                .SetTextAlignment(TextAlignment.LEFT))
                .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("Last Name")
                .SetFontSize(16)
                .SetBold()
                .SetPaddingLeft(20)
                .SetTextAlignment(TextAlignment.LEFT))
                .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("Tech")
                .SetFontSize(16)
                .SetBold()
                .SetPaddingLeft(20)
                .SetTextAlignment(TextAlignment.LEFT))
                .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("Problem")
                .SetBold()
                .SetFontSize(16)
                .SetPaddingLeft(20)
                .SetTextAlignment(TextAlignment.LEFT))
                .SetBorder(Border.NO_BORDER));

            table.AddCell(new Cell().Add(new Paragraph("Status")
                .SetBold()
                .SetFontSize(16)
                .SetPaddingLeft(20)
                .SetTextAlignment(TextAlignment.LEFT))
                .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("Closed")
                .SetBold()
                .SetFontSize(16)
                .SetPaddingLeft(20)
                .SetTextAlignment(TextAlignment.LEFT))
                .SetBorder(Border.NO_BORDER));


            // Print the list of Calls
            foreach (var call in calls)
            {
                table.AddCell(new Cell(1,1).Add(new Paragraph(call.DateOpened.ToShortDateString())
                    .SetFontSize(12)
                    .SetPaddingLeft(20)
                    .SetPaddingRight(20)
                    .SetTextAlignment(TextAlignment.LEFT))
                    .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(call.EmployeeName)
                    .SetFontSize(12)
                    .SetPaddingLeft(20)
                    .SetTextAlignment(TextAlignment.LEFT))
                    .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(call.TechName)
                    .SetFontSize(12)
                    .SetPaddingLeft(20)
                    .SetTextAlignment(TextAlignment.LEFT))
                    .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(call.ProblemDescription)
                    .SetFontSize(12)
                    .SetPaddingLeft(20)
                    .SetTextAlignment(TextAlignment.LEFT))
                    .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(call.OpenStatus ? "Open" : "Closed")
                    .SetFontSize(12)
                    .SetPaddingLeft(20)
                    .SetTextAlignment(TextAlignment.LEFT))
                    .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(call.OpenStatus ? "-" : call.DateClosed?.ToShortDateString())
                    .SetFontSize(12)
                    .SetPaddingLeft(20)
                    .SetPaddingRight(20)
                    .SetTextAlignment(TextAlignment.LEFT))
                    .SetBorder(Border.NO_BORDER));
            }



            document.Add(table);

            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("Call report written on - " + DateTime.Now)
                                                                   .SetFontSize(6)
                                                                   .SetTextAlignment(TextAlignment.CENTER));
            document.Close();
        }
    }
}
