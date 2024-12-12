namespace PeakLims.Domain.RelationshipTypes;

using Ardalis.SmartEnum;
using PeakLims.Exceptions;

public class RelationshipType : ValueObject
{
    private RelationshipTypeEnum _relationshipType;
    public string Value
    {
        get => _relationshipType.Name;
        private set
        {
            if (!RelationshipTypeEnum.TryFromName(value, true, out var parsed))
                throw new ValidationException($"Invalid relationship type. Please use one of the following: {string.Join(", ", ListNames())}");

            _relationshipType = parsed;
        }
    }
    
    public RelationshipType(string value) => Value = value;

    public static RelationshipType Of(string value) => new RelationshipType(value);
    public static List<string> ListNames() => RelationshipTypeEnum.List.Select(x => x.Name).ToList();

    public static RelationshipType Son() => new RelationshipType(RelationshipTypeEnum.Son.Name);
    public static RelationshipType Daughter() => new RelationshipType(RelationshipTypeEnum.Daughter.Name);
    public static RelationshipType Father() => new RelationshipType(RelationshipTypeEnum.Father.Name);
    public static RelationshipType Mother() => new RelationshipType(RelationshipTypeEnum.Mother.Name);
    public static RelationshipType Brother() => new RelationshipType(RelationshipTypeEnum.Brother.Name);
    public static RelationshipType Sister() => new RelationshipType(RelationshipTypeEnum.Sister.Name);
    public static RelationshipType HalfBrother() => new RelationshipType(RelationshipTypeEnum.HalfBrother.Name);
    public static RelationshipType HalfSister() => new RelationshipType(RelationshipTypeEnum.HalfSister.Name);
    public static RelationshipType MaternalHalfBrother() => new RelationshipType(RelationshipTypeEnum.MaternalHalfBrother.Name);
    public static RelationshipType MaternalHalfSister() => new RelationshipType(RelationshipTypeEnum.MaternalHalfSister.Name);
    public static RelationshipType PaternalHalfBrother() => new RelationshipType(RelationshipTypeEnum.PaternalHalfBrother.Name);
    public static RelationshipType PaternalHalfSister() => new RelationshipType(RelationshipTypeEnum.PaternalHalfSister.Name);
    public static RelationshipType Grandfather() => new RelationshipType(RelationshipTypeEnum.Grandfather.Name);
    public static RelationshipType Grandmother() => new RelationshipType(RelationshipTypeEnum.Grandmother.Name);
    public static RelationshipType MaternalGrandFather() => new RelationshipType(RelationshipTypeEnum.MaternalGrandFather.Name);
    public static RelationshipType MaternalGrandMother() => new RelationshipType(RelationshipTypeEnum.MaternalGrandMother.Name);
    public static RelationshipType PaternalGrandFather() => new RelationshipType(RelationshipTypeEnum.PaternalGrandFather.Name);
    public static RelationshipType PaternalGrandMother() => new RelationshipType(RelationshipTypeEnum.PaternalGrandMother.Name);
    public static RelationshipType Uncle() => new RelationshipType(RelationshipTypeEnum.Uncle.Name);
    public static RelationshipType Aunt() => new RelationshipType(RelationshipTypeEnum.Aunt.Name);
    public static RelationshipType MaternalUncle() => new RelationshipType(RelationshipTypeEnum.MaternalUncle.Name);
    public static RelationshipType MaternalAunt() => new RelationshipType(RelationshipTypeEnum.MaternalAunt.Name);
    public static RelationshipType PaternalUncle() => new RelationshipType(RelationshipTypeEnum.PaternalUncle.Name);
    public static RelationshipType PaternalAunt() => new RelationshipType(RelationshipTypeEnum.PaternalAunt.Name);
    public static RelationshipType Cousin() => new RelationshipType(RelationshipTypeEnum.Cousin.Name);
    public static RelationshipType MaternalCousin() => new RelationshipType(RelationshipTypeEnum.MaternalCousin.Name);
    public static RelationshipType PaternalCousin() => new RelationshipType(RelationshipTypeEnum.PaternalCousin.Name);
    public static RelationshipType Nephew() => new RelationshipType(RelationshipTypeEnum.Nephew.Name);
    public static RelationshipType Niece() => new RelationshipType(RelationshipTypeEnum.Niece.Name);


    protected RelationshipType() { } // EF Core
    private abstract class RelationshipTypeEnum(string name, int value) : SmartEnum<RelationshipTypeEnum>(name, value)
    {
        public static readonly RelationshipTypeEnum Son = new SonType();
        public static readonly RelationshipTypeEnum Daughter = new DaughterType();
        public static readonly RelationshipTypeEnum Father = new FatherType();
        public static readonly RelationshipTypeEnum Mother = new MotherType();
        public static readonly RelationshipTypeEnum Brother = new BrotherType();
        public static readonly RelationshipTypeEnum Sister = new SisterType();
        public static readonly RelationshipTypeEnum HalfBrother = new HalfBrotherType();
        public static readonly RelationshipTypeEnum HalfSister = new HalfSisterType();
        public static readonly RelationshipTypeEnum MaternalHalfBrother = new HalfBrotherMaternalType();
        public static readonly RelationshipTypeEnum MaternalHalfSister = new HalfSisterMaternalType();
        public static readonly RelationshipTypeEnum PaternalHalfBrother = new HalfBrotherPaternalType();
        public static readonly RelationshipTypeEnum PaternalHalfSister = new HalfSisterPaternalType();
        public static readonly RelationshipTypeEnum Grandfather = new GrandfatherType();
        public static readonly RelationshipTypeEnum Grandmother = new GrandmotherType();
        public static readonly RelationshipTypeEnum MaternalGrandFather = new GrandfatherMaternalType();
        public static readonly RelationshipTypeEnum MaternalGrandMother = new GrandmotherMaternalType();
        public static readonly RelationshipTypeEnum PaternalGrandFather = new GrandfatherPaternalType();
        public static readonly RelationshipTypeEnum PaternalGrandMother = new GrandmotherPaternalType();
        public static readonly RelationshipTypeEnum Uncle = new UncleType();
        public static readonly RelationshipTypeEnum Aunt = new AuntType();
        public static readonly RelationshipTypeEnum MaternalUncle = new UncleMaternalType();
        public static readonly RelationshipTypeEnum MaternalAunt = new AuntMaternalType();
        public static readonly RelationshipTypeEnum PaternalUncle = new UnclePaternalType();
        public static readonly RelationshipTypeEnum PaternalAunt = new AuntPaternalType();
        public static readonly RelationshipTypeEnum Cousin = new CousinType();
        public static readonly RelationshipTypeEnum MaternalCousin = new CousinMaternalType();
        public static readonly RelationshipTypeEnum PaternalCousin = new CousinPaternalType();
        public static readonly RelationshipTypeEnum Nephew = new NephewType();
        public static readonly RelationshipTypeEnum Niece = new NieceType();

        const string ChildRole = "Child";
        const string ParentRole = "Parent";
        const string SiblingRole = "Sibling";
        const string GrandparentRole = "Grandparent";
        const string AuntUncleRole = "Aunt/Uncle";
        const string CousinRole = "Cousin";
        const string NephewNieceRole = "Nephew/Niece";

        public abstract string GetRole();

        private class SonType() : RelationshipTypeEnum("Son", 0)
        {
            public override string GetRole() => ChildRole;
        }

        private class DaughterType() : RelationshipTypeEnum("Daughter", 1)
        {
            public override string GetRole() => ChildRole;
        }

        private class FatherType() : RelationshipTypeEnum("Father", 2)
        {
            public override string GetRole() => ParentRole;
        }

        private class MotherType() : RelationshipTypeEnum("Mother", 3)
        {
            public override string GetRole() => ParentRole;
        }

        private class BrotherType() : RelationshipTypeEnum("Brother", 4)
        {
            public override string GetRole() => SiblingRole;
        }

        private class SisterType() : RelationshipTypeEnum("Sister", 5)
        {
            public override string GetRole() => SiblingRole;
        }

        private class HalfBrotherType() : RelationshipTypeEnum("Half Brother", 6)
        {
            public override string GetRole() => SiblingRole;
        }

        private class HalfSisterType() : RelationshipTypeEnum("Half Sister", 7)
        {
            public override string GetRole() => SiblingRole;
        }

        private class HalfBrotherMaternalType() : RelationshipTypeEnum("Half Brother (Maternal)", 8)
        {
            public override string GetRole() => SiblingRole;
        }

        private class HalfSisterMaternalType() : RelationshipTypeEnum("Half Sister (Maternal)", 9)
        {
            public override string GetRole() => SiblingRole;
        }

        private class HalfBrotherPaternalType() : RelationshipTypeEnum("Half Brother (Paternal)", 10)
        {
            public override string GetRole() => SiblingRole;
        }

        private class HalfSisterPaternalType() : RelationshipTypeEnum("Half Sister (Paternal)", 11)
        {
            public override string GetRole() => SiblingRole;
        }

        private class GrandfatherType() : RelationshipTypeEnum("Grandfather", 12)
        {
            public override string GetRole() => GrandparentRole;
        }

        private class GrandmotherType() : RelationshipTypeEnum("Grandmother", 13)
        {
            public override string GetRole() => GrandparentRole;
        }

        private class GrandfatherMaternalType() : RelationshipTypeEnum("Grandfather (Maternal)", 14)
        {
            public override string GetRole() => GrandparentRole;
        }

        private class GrandmotherMaternalType() : RelationshipTypeEnum("Grandmother (Maternal)", 15)
        {
            public override string GetRole() => GrandparentRole;
        }

        private class GrandfatherPaternalType() : RelationshipTypeEnum("Grandfather (Paternal)", 16)
        {
            public override string GetRole() => GrandparentRole;
        }

        private class GrandmotherPaternalType() : RelationshipTypeEnum("Grandmother (Paternal)", 17)
        {
            public override string GetRole() => GrandparentRole;
        }

        private class UncleType() : RelationshipTypeEnum("Uncle", 18)
        {
            public override string GetRole() => AuntUncleRole;
        }

        private class AuntType() : RelationshipTypeEnum("Aunt", 19)
        {
            public override string GetRole() => AuntUncleRole;
        }

        private class UncleMaternalType() : RelationshipTypeEnum("Uncle (Maternal)", 20)
        {
            public override string GetRole() => AuntUncleRole;
        }

        private class AuntMaternalType() : RelationshipTypeEnum("Aunt (Maternal)", 21)
        {
            public override string GetRole() => AuntUncleRole;
        }

        private class UnclePaternalType() : RelationshipTypeEnum("Uncle (Paternal)", 22)
        {
            public override string GetRole() => AuntUncleRole;
        }

        private class AuntPaternalType() : RelationshipTypeEnum("Aunt (Paternal)", 23)
        {
            public override string GetRole() => AuntUncleRole;
        }

        private class CousinType() : RelationshipTypeEnum("Cousin", 24)
        {
            public override string GetRole() => CousinRole;
        }

        private class CousinMaternalType() : RelationshipTypeEnum("Cousin (Maternal)", 25)
        {
            public override string GetRole() => CousinRole;
        }

        private class CousinPaternalType() : RelationshipTypeEnum("Cousin (Paternal)", 26)
        {
            public override string GetRole() => CousinRole;
        }

        private class NephewType() : RelationshipTypeEnum("Nephew", 27)
        {
            public override string GetRole() => NephewNieceRole;
        }

        private class NieceType() : RelationshipTypeEnum("Niece", 28)
        {
            public override string GetRole() => NephewNieceRole;
        }
    }

}