namespace PeakLims.SharedTestHelpers.Fakes.AccessionComment;

using Accession;
using Bogus;
using Domain.Accessions;
using PeakLims.Domain.AccessionComments;
using PeakLims.Domain.AccessionComments.Models;

public class FakeAccessionCommentBuilder 
{
    private Accession _accession = null;
    private string _comment;

    public FakeAccessionCommentBuilder WithComment(string comment)
    {
        _comment = comment;
        return this;
    }

    public FakeAccessionCommentBuilder WithAccession(Accession accession)
    {
        _accession = accession;
        return this;
    }

    public FakeAccessionCommentBuilder WithMockAccession()
    {
        _accession = new FakeAccessionBuilder().Build();
        return this;
    }
    
    public AccessionComment Build()
    {
        if (_accession == null)
            WithMockAccession();
        
        var faker = new Faker();
        var comment = _comment ?? faker.Lorem.Sentence();
        var accessionComment = AccessionComment.Create(_accession, comment);
        return accessionComment;
    }
}