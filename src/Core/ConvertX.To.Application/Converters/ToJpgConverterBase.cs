using System.IO.Compression;
using System.Text;
using ConvertX.To.Application.Domain;
using ConvertX.To.Application.Exceptions;
using ConvertX.To.Application.Interfaces;
using ConvertX.To.Domain.Options;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace ConvertX.To.Application.Converters;

public abstract class ToJpgConverterBase : MsGraphDriveItemConverterBase, IDisposable
{
    private readonly ConverterFactory _converterFactory;
    private readonly ILoggerAdapter<IConverter> _logger;
    private List<InMemoryFile> _convertedJpgStreams = new();

    private List<InMemoryFile> _splitPdfStreams = new();

    public ToJpgConverterBase(ConverterFactory converterFactory, string sourceFormat, string targetFormat,
        ILoggerAdapter<IConverter> logger, IMsGraphFileConversionService msGraphFileConversionService) : base(
        sourceFormat, targetFormat, logger, msGraphFileConversionService)
    {
        _converterFactory = converterFactory;
        _logger = logger;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public override async Task<(string, Stream)> ConvertAsync(Stream source, ConversionOptions conversionOptions)
    {
        if (!conversionOptions.ToJpgOptions.SplitIfPossible) return await base.ConvertAsync(source, conversionOptions);

        IConverter? toPdfConverter;

        try
        {
            toPdfConverter = _converterFactory.Create(_sourceFormat, "pdf");
        }
        catch (UnsupportedConversionException _)
        {
            _logger.LogTrace(
                "{option} was requested but there is no {sourceFormat} to {targetFormat} converter to use as an intermediary (proceeding with non-split conversion)",
                nameof(conversionOptions.ToJpgOptions.SplitIfPossible), _sourceFormat, "pdf");
            return await base.ConvertAsync(source, conversionOptions);
        }

        var newConversionOptions = new ConversionOptions
        {
            ToJpgOptions = new ToJpgOptions
            {
                SplitIfPossible = false
            }
        };

        var (_, pdfStream) = await toPdfConverter.ConvertAsync(source, newConversionOptions);

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var sourcePdfDocument = PdfReader.Open(pdfStream, PdfDocumentOpenMode.Import);

        if (sourcePdfDocument.PageCount == 1)
        {
            source.Position = 0;
            return await base.ConvertAsync(source, conversionOptions);
        }

        _splitPdfStreams = SplitPdf(sourcePdfDocument);

        _convertedJpgStreams = await ConvertSplitPdfsToIndividualJpgs(_splitPdfStreams, newConversionOptions);

        var zippedJpgs = await ZipFilesAsync(_convertedJpgStreams);

        DisposeTemporaryStreams();

        return ("zip", zippedJpgs);
    }

    private async Task<List<InMemoryFile>> ConvertSplitPdfsToIndividualJpgs(List<InMemoryFile> splitPdfStreams,
        ConversionOptions newConversionOptions)
    {
        var convertedPdfStreams = new List<InMemoryFile>();

        var pdfToJpgConverter = _converterFactory.Create("pdf", "jpg");

        foreach (var splitPdfStream in splitPdfStreams)
        {
            var convertedPdfStream =
                await ConvertPdfStreamFileToJpgStreamFile(newConversionOptions, splitPdfStream, pdfToJpgConverter);
            convertedPdfStreams.Add(convertedPdfStream);
        }
        
        _logger.LogTrace("Converted {SplitPdfStreamsCount} PDFs to {ConvertedPdfStreamsCount} JPGs",
            splitPdfStreams.Count, convertedPdfStreams.Count);

        return convertedPdfStreams;
    }

    private async Task<InMemoryFile> ConvertPdfStreamFileToJpgStreamFile(ConversionOptions newConversionOptions,
        InMemoryFile splitPdfInMemory, IConverter pdfToJpgConverter)
    {
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(splitPdfInMemory.Name);

        var (_, jpgStream) = await pdfToJpgConverter.ConvertAsync(splitPdfInMemory.Stream, newConversionOptions);

        var convertedPdfStream = new InMemoryFile
        {
            Name = $"{fileNameWithoutExtension}.jpg",
            Stream = jpgStream
        };
        
        _logger.LogTrace("Converted {SplitPdfInMemoryName} to {ConvertedPdfStreamName}", splitPdfInMemory.Name,
            convertedPdfStream.Name);
        
        return convertedPdfStream;
    }

    private List<InMemoryFile> SplitPdf(PdfDocument sourcePdfDocument)
    {
        var splitPdfStreams = new List<InMemoryFile>();

        for (var pageIndex = 0; pageIndex < sourcePdfDocument.PageCount; pageIndex++)
        {
            var currentPageNumber = pageIndex + 1;

            var splitPdfFile = SplitPdfFile(sourcePdfDocument, pageIndex, currentPageNumber);

            splitPdfStreams.Add(splitPdfFile);
        }
        
        _logger.LogTrace("Split {SourcePdfDocumentPageCount} pages into {SplitPdfStreamsCount} PDFs",
            sourcePdfDocument.PageCount, splitPdfStreams.Count);

        return splitPdfStreams;
    }

    private static InMemoryFile SplitPdfFile(PdfDocument sourcePdfDocument, int pageIndex, int currentPageNumber)
    {
        var outputPdfDocument = new PdfDocument();

        outputPdfDocument.AddPage(sourcePdfDocument.Pages[pageIndex]);

        var fileName = $"{currentPageNumber}_of_{sourcePdfDocument.PageCount}.pdf";

        var splitPdfFile = new InMemoryFile
        {
            Name = fileName,
            Stream = new MemoryStream()
        };

        outputPdfDocument.Save(splitPdfFile.Stream);
        return splitPdfFile;
    }

    private async Task<Stream> ZipFilesAsync(List<InMemoryFile> files)
    {
        var zipStream = new MemoryStream();

        using (var zip = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
        {
            foreach (var file in files)
            {
                var entry = zip.CreateEntry(file.Name);
                await using var entryStream = entry.Open();
                await file.Stream.CopyToAsync(entryStream);
            }
        }

        zipStream.Position = 0;
        
        _logger.LogTrace("Zipped {Count} files into a single zip file", files.Count);
        return zipStream;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;

        DisposeTemporaryStreams();

        _splitPdfStreams = new List<InMemoryFile>();
        _convertedJpgStreams = new List<InMemoryFile>();
    }

    private void DisposeTemporaryStreams()
    {
        foreach (var splitPdfStream in _splitPdfStreams) splitPdfStream.Stream.Dispose();

        foreach (var convertedJpgStream in _convertedJpgStreams) convertedJpgStream.Stream.Dispose();
    }
}