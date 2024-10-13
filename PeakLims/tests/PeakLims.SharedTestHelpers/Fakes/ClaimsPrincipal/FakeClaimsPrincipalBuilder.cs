namespace PeakLims.SharedTestHelpers.Fakes.ClaimsPrincipal;

using System.Security.Claims;
using AutoBogus;
using Utilities;

public class FakeClaimsPrincipalBuilder
    {
        private readonly List<Claim> _claims;

        public FakeClaimsPrincipalBuilder()
        {
            var faker = new AutoFaker<DefaultClaims>()
                .RuleFor(dc => dc.UserId, f => f.Random.Guid().ToString())
                .RuleFor(dc => dc.Email, f => f.Internet.Email())
                .RuleFor(dc => dc.FirstName, f => f.Name.FirstName())
                .RuleFor(dc => dc.LastName, f => f.Name.LastName())
                .RuleFor(dc => dc.Username, f => f.Internet.UserName())
                .RuleFor(dc => dc.ClientId, f => f.Random.AlphaNumeric(10))
                .RuleFor(dc => dc.OrganizationId, TestingConsts.DefaultTestingOrganizationId);

            var defaultClaims = faker.Generate();

            _claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, defaultClaims.UserId),
                new Claim(ClaimTypes.Email, defaultClaims.Email),
                new Claim(ClaimTypes.GivenName, defaultClaims.FirstName),
                new Claim(ClaimTypes.Surname, defaultClaims.LastName),
                new Claim("preferred_username", defaultClaims.Username),
                new Claim("client_id", defaultClaims.ClientId),
                new Claim("organization_id", defaultClaims.OrganizationId.ToString())
            };
        }

        public FakeClaimsPrincipalBuilder WithUserId(string userId)
        {
            RemoveClaim(ClaimTypes.NameIdentifier);
            _claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
            return this;
        }

        public FakeClaimsPrincipalBuilder WithEmail(string email)
        {
            RemoveClaim(ClaimTypes.Email);
            _claims.Add(new Claim(ClaimTypes.Email, email));
            return this;
        }

        public FakeClaimsPrincipalBuilder WithFirstName(string firstName)
        {
            RemoveClaim(ClaimTypes.GivenName);
            _claims.Add(new Claim(ClaimTypes.GivenName, firstName));
            return this;
        }

        public FakeClaimsPrincipalBuilder WithLastName(string lastName)
        {
            RemoveClaim(ClaimTypes.Surname);
            _claims.Add(new Claim(ClaimTypes.Surname, lastName));
            return this;
        }

        public FakeClaimsPrincipalBuilder WithUsername(string username)
        {
            RemoveClaim("preferred_username");
            _claims.Add(new Claim("preferred_username", username));
            return this;
        }

        public FakeClaimsPrincipalBuilder WithClientId(string clientId)
        {
            RemoveClaim("client_id");
            _claims.Add(new Claim("client_id", clientId));
            return this;
        }

        public FakeClaimsPrincipalBuilder WithOrganizationId(Guid organizationId)
        {
            RemoveClaim("organization_id");
            _claims.Add(new Claim("organization_id", organizationId.ToString()));
            return this;
        }

        public FakeClaimsPrincipalBuilder WithClaim(string type, string value)
        {
            RemoveClaim(type);
            _claims.Add(new Claim(type, value));
            return this;
        }

        public ClaimsPrincipal Build()
        {
            var identity = new ClaimsIdentity(_claims, "TestAuthType");
            return new ClaimsPrincipal(identity);
        }

        private void RemoveClaim(string claimType)
        {
            _claims.RemoveAll(c => c.Type == claimType);
        }
        
        private class DefaultClaims
        {
            public string UserId { get; set; }
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Username { get; set; }
            public string ClientId { get; set; }
            public Guid OrganizationId { get; set; }
        }
    }