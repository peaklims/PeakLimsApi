namespace PeakLims.SharedTestHelpers.Fakes.Files;

using System.Text;
using Bogus;
using Microsoft.AspNetCore.Http;

public class FakeFileBuilder
{
    private static readonly Faker Faker = new Faker();
    private string _content = string.Empty;
    private string _name = "defaultName";
    private string _fileName = $"{Faker.Lorem.Word()}.txt";
    private string _contentType = "text/plain";

    public FakeFileBuilder WithContent(string content)
    {
        _content = content;
        return this;
    }

    public FakeFileBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public FakeFileBuilder WithFileName(string fileName)
    {
        _fileName = fileName;
        return this;
    }

    public FakeFileBuilder WithContentType(string contentType)
    {
        _contentType = contentType;
        return this;
    }

    public IFormFile Build()
    {
        return new FakeFormFile(_content, _name, _fileName, _contentType);
    }
}


public class FakeFormFile : IFormFile
{
    private readonly Stream _stream;
    private readonly string _name;
    private readonly string _fileName;
    private readonly string _contentType;

    public FakeFormFile(string content, string name, string fileName, string contentType = "text/plain")
    {
        _stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        _name = name;
        _fileName = fileName;
        _contentType = contentType;
    }

    public string ContentType => _contentType;

    public string ContentDisposition => throw new NotImplementedException();

    public IHeaderDictionary Headers => throw new NotImplementedException();

    public long Length => _stream.Length;

    public string Name => _name;

    public string FileName => _fileName;

    public void CopyTo(Stream target)
    {
        _stream.CopyTo(target);
    }

    public async Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
    {
        await _stream.CopyToAsync(target, cancellationToken);
    }

    public Stream OpenReadStream()
    {
        return _stream;
    }
}
