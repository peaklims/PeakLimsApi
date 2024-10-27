using PeakLims.Domain.Containers;
using PeakLims.Domain.Containers.Models;
using PeakLims.Domain.SampleTypes;

namespace PeakLims.Domain.SampleTypes;

using Ardalis.SmartEnum;
using Containers;
using Containers.Models;
using Exceptions;

public class SampleType : ValueObject
{
    private SampleTypeEnum _sampleType;
    public string Value
    {
        get => _sampleType.Name;
        private set
        {
            if (!SampleTypeEnum.TryFromName(value, true, out var parsed))
                throw new ValidationException($"Invalid sample type. Please use one of the following: {string.Join(", ", ListNames())}");

            _sampleType = parsed;
        }
    }
    
    public SampleType(string value) => Value = value;
    public SampleType(SampleTypeEnum value) => Value = value.Name;

    public static SampleType Of(string value) => new SampleType(value);
    public static implicit operator string(SampleType value) => value.Value;
    public static List<string> ListNames() => SampleTypeEnum.List.Select(x => x.Name).ToList();

    public static SampleType WholeBlood() => new SampleType(SampleTypeEnum.WholeBlood.Name);
    public static SampleType SalivaBuccal() => new SampleType(SampleTypeEnum.SalivaBuccal.Name);
    public static SampleType ExtractedDna() => new SampleType(SampleTypeEnum.ExtractedDna.Name);
    public static SampleType CulturedCells() => new SampleType(SampleTypeEnum.CulturedCells.Name);
    public static SampleType Tissue() => new SampleType(SampleTypeEnum.Tissue.Name);
    public static SampleType Data() => new SampleType(SampleTypeEnum.Data.Name);
    
    public List<Container> GetDefaultContainers(Guid organizationId) => _sampleType.GetDefaultContainers(organizationId);

    protected SampleType() { } // EF Core
}

public abstract class SampleTypeEnum(string name, int value) : SmartEnum<SampleTypeEnum>(name, value)
{
    public static readonly SampleTypeEnum WholeBlood = new WholeBloodType();
    public static readonly SampleTypeEnum SalivaBuccal = new SalivaBuccalType();
    public static readonly SampleTypeEnum ExtractedDna = new ExtractedDnaType();
    public static readonly SampleTypeEnum CulturedCells = new CulturedCellsType();
    public static readonly SampleTypeEnum Tissue = new TissueType();
    public static readonly SampleTypeEnum Data = new DataType();

    public abstract List<Container> GetDefaultContainers(Guid organizationId);
    
    private class WholeBloodType : SampleTypeEnum
    {
        public WholeBloodType() : base("Whole Blood", 0)
        {
        }
        
        // EDTA Tubes (Purple Top): Contains EDTA as an anticoagulant, commonly used for hematology tests and DNA/RNA extraction.
        // Sodium Heparin Tubes (Green Top): Contains heparin as an anticoagulant, often used for flow cytometry and cell culture work.
        // Serum Separator Tubes (SST, Gold or Red-Gray Top): No anticoagulant, contains a gel separator for serum collection after clotting. Used for biochemistry and immunology assays.
        // Vacutainer Tubes: General-purpose tubes for blood collection; tube choice depends on downstream applications.
        public override List<Container> GetDefaultContainers(Guid organizationId)
        {
            return
            [
                Container.Create(new ContainerForCreation
                {
                    UsedFor = SampleType.WholeBlood().Value,
                    Type = "EDTA Tubes (Purple Top)",
                    OrganizationId = organizationId
                }),

                Container.Create(new ContainerForCreation
                {
                    UsedFor = SampleType.WholeBlood().Value,
                    Type = "Sodium Heparin Tubes (Green Top)",
                    OrganizationId = organizationId
                }),

                Container.Create(new ContainerForCreation
                {
                    UsedFor = SampleType.WholeBlood().Value,
                    Type = "Serum Separator Tubes (SST, Gold or Red-Gray Top)",
                    OrganizationId = organizationId
                }),

                Container.Create(new ContainerForCreation
                {
                    UsedFor = SampleType.WholeBlood().Value,
                    Type = "Vacutainer Tubes",
                    OrganizationId = organizationId
                })
            ];
        }
    }

    private class SalivaBuccalType : SampleTypeEnum
    {
        public SalivaBuccalType() : base("Saliva/Buccal", 1)
        {
        }
        public override List<Container> GetDefaultContainers(Guid organizationId)
        {
            return
            [
                Container.Create(new ContainerForCreation
                {
                    UsedFor = SampleType.SalivaBuccal().Value,
                    Type = "Saliva Collection Tubes",
                    OrganizationId = organizationId
                }),

                Container.Create(new ContainerForCreation
                {
                    UsedFor = SampleType.SalivaBuccal().Value,
                    Type = "Buccal Swabs",
                    OrganizationId = organizationId
                }),

                Container.Create(new ContainerForCreation
                {
                    UsedFor = SampleType.SalivaBuccal().Value,
                    Type = "Oragene DNA Saliva Collection Kits",
                    OrganizationId = organizationId
                })
            ];
        }
    }

    private class ExtractedDnaType : SampleTypeEnum
    {
        public ExtractedDnaType() : base("Extracted DNA", 2)
        {
        }
        // Microcentrifuge Tubes: 1.5 mL or 2.0 mL tubes commonly used for storing DNA samples.
        // PCR Tubes: For small volumes, especially when storing aliquots for PCR reactions.
        // Cryovials: Used for long-term storage of extracted DNA at -20°C or -80°C.
        // 96-Well Plates: For high-throughput sample storage, often sealed with adhesive covers or plate sealers for DNA samples.
            
        public override List<Container> GetDefaultContainers(Guid organizationId)
        {
            return
            [
                Container.Create(new ContainerForCreation
                {
                    UsedFor = SampleType.ExtractedDna().Value,
                    Type = "Microcentrifuge Tubes",
                    OrganizationId = organizationId
                }),

                Container.Create(new ContainerForCreation
                {
                    UsedFor = SampleType.ExtractedDna().Value,
                    Type = "PCR Tubes",
                    OrganizationId = organizationId
                }),

                Container.Create(new ContainerForCreation
                {
                    UsedFor = SampleType.ExtractedDna().Value,
                    Type = "Cryovials",
                    OrganizationId = organizationId
                }),

                Container.Create(new ContainerForCreation
                {
                    UsedFor = SampleType.ExtractedDna().Value,
                    Type = "96-Well Plates",
                    OrganizationId = organizationId
                })
            ];
        }
    }

    private class CulturedCellsType : SampleTypeEnum
    {
        public CulturedCellsType() : base("Cultured Cells", 3)
        {
        }
        //Tissue Culture Flasks: T25, T75, T150 flasks for adherent or suspension cells.
        // Petri Dishes: For culturing cells, typically in sizes like 35 mm or 60 mm.
        // Centrifuge Tubes: 15 mL or 50 mL tubes for cell pellets after trypsinization or harvesting.
        // Cryovials: For freezing and long-term storage of cultured cells at -80°C or in liquid nitrogen.
            
        public override List<Container> GetDefaultContainers(Guid organizationId)
        {
            return
            [
                Container.Create(new ContainerForCreation
                {
                    UsedFor = SampleType.CulturedCells().Value,
                    Type = "Tissue Culture Flasks",
                    OrganizationId = organizationId
                }),

                Container.Create(new ContainerForCreation
                {
                    UsedFor = SampleType.CulturedCells().Value,
                    Type = "Petri Dishes",
                    OrganizationId = organizationId
                }),

                Container.Create(new ContainerForCreation
                {
                    UsedFor = SampleType.CulturedCells().Value,
                    Type = "Centrifuge Tubes",
                    OrganizationId = organizationId
                }),

                Container.Create(new ContainerForCreation
                {
                    UsedFor = SampleType.CulturedCells().Value,
                    Type = "Cryovials",
                    OrganizationId = organizationId
                })
            ];
        }
    }

    private class TissueType : SampleTypeEnum
    {
        public TissueType() : base("Tissue", 4)
        {
        }
        
        //Formalin-Filled Containers: For preservation and fixation of tissue samples before histology.
        // Sterile Plastic Tubes or Containers: Used for fresh or snap-frozen tissue samples for molecular analyses.
        // Cryovials or Cryotubes: For storing frozen tissue samples in liquid nitrogen or at -80°C for long-term preservation.
        
        public override List<Container> GetDefaultContainers(Guid organizationId)
        {
            return
            [
                Container.Create(new ContainerForCreation
                {
                    UsedFor = SampleType.Tissue().Value,
                    Type = "Formalin-Filled Containers",
                    OrganizationId = organizationId
                }),

                Container.Create(new ContainerForCreation
                {
                    UsedFor = SampleType.Tissue().Value,
                    Type = "Sterile Plastic Tubes or Containers",
                    OrganizationId = organizationId
                }),

                Container.Create(new ContainerForCreation
                {
                    UsedFor = SampleType.Tissue().Value,
                    Type = "Cryovials or Cryotubes",
                    OrganizationId = organizationId
                })
            ];
        }
    }

    private class DataType : SampleTypeEnum
    {
        public DataType() : base("Data", 100)
        {
        }
        
        public override List<Container> GetDefaultContainers(Guid organizationId)
        {
            return 
                [
                    Container.Create(new ContainerForCreation
                    {
                        UsedFor = SampleType.Data().Value,
                        Type = "USB Drive",
                        OrganizationId = organizationId
                    }),
                    Container.Create(new ContainerForCreation
                    {
                        UsedFor = SampleType.Data().Value,
                        Type = "Hard Drive",
                        OrganizationId = organizationId
                    }),
                    Container.Create(new ContainerForCreation
                    {
                        UsedFor = SampleType.Data().Value,
                        Type = "Cloud Storage",
                        OrganizationId = organizationId
                    }),
                    Container.Create(new ContainerForCreation
                    {
                        UsedFor = SampleType.Data().Value,
                        Type = "SFTP",
                        OrganizationId = organizationId
                    }),
                    Container.Create(new ContainerForCreation
                    {
                        UsedFor = SampleType.Data().Value,
                        Type = "NAS",
                        OrganizationId = organizationId
                    })
                ];
        }
    }
}