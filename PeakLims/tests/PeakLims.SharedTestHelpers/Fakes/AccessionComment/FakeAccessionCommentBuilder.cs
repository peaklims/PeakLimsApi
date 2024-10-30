namespace PeakLims.SharedTestHelpers.Fakes.AccessionComment;

using Accession;
using Bogus;
using Domain.Accessions;
using PeakLims.Domain.AccessionComments;

public class FakeAccessionCommentBuilder 
{
    private static readonly Faker Faker = new Faker();
    private Accession _accession = null;
    private string _comment;
    private string _userIdentifier = Faker.Random.Guid().ToString();

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

    public FakeAccessionCommentBuilder WithUserIdentifier(string userIdentifier)
    {
        _userIdentifier = userIdentifier;
        return this;
    }
    
    public AccessionComment Build()
    {
        if (_accession == null)
            WithMockAccession();
        
        var faker = new Faker();
        var comment = _comment ?? faker.Lorem.Sentence();
        var accessionComment = AccessionComment.Create(_accession, comment, _userIdentifier);
        return accessionComment;
    }
}